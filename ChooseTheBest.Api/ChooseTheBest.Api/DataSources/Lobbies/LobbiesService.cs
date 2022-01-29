using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChooseTheBest.Api.DataSources.Lobbies.Exceptions;
using ChooseTheBest.Api.DataSources.Players;
using ChooseTheBest.Api.DataSources.Players.Exceptions;
using ChooseTheBest.DataSource.Database.Managers;
using ChooseTheBest.DataSource.Database.Models;
using ChooseTheBest.Model.Game.Lobbies;

namespace ChooseTheBest.Api.DataSources.Lobbies
{
	public interface ILobbiesService
	{
		Task<Lobby> CreateLobby(string playerId, string lobbyName, string password, int maxPlayers, string gameModeType);
		Task<List<LobbyInfo>> GetListOfLobbies(int count, int offset);
		Task UpdateSettings(string lobbyId, LobbySettings lobbySettings);
		Task SetConnectionString(string lobbyId, string connectionString);

		Task<Lobby?> FindLobbyByPlayerId(string playerId);
		Task<Lobby> GetLobby(string lobbyId);
		Task<Lobby> JoinLobby(string lobbyId, string playerId, string password);
		Task<Lobby> LeaveLobby(string lobbyId, string playerId);
		Task RemoveLobby(string lobbyId);
	}

	public class LobbiesService : ILobbiesService
	{
		private readonly IPlayerService _playerService;
		private readonly ILobbyManager _lobbyManager;

		public LobbiesService(IPlayerService playerService, ILobbyManager lobbyManager)
		{
			_playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
			_lobbyManager = lobbyManager ?? throw new ArgumentNullException(nameof(lobbyManager));
		}

		public async Task<Lobby> CreateLobby(string playerId, string lobbyName, string password, int maxPlayers, string gameModeType)
		{
			var lobbyEntity = new LobbyEntity()
			{
				LobbyLeaderId = playerId,
				Name = lobbyName,
				Password = password,
				GameModeType = gameModeType,
				MaxPlayersCount = maxPlayers,
				PlayersIds = new []{ playerId },
				LobbyState = LobbyState.Created,
			};
			var players = await _playerService.GetPlayers(lobbyEntity.PlayersIds);

			await _lobbyManager.Create(lobbyEntity);
			return new Lobby()
			{
				LobbyInfo = GetLobbyInfo(lobbyEntity),
				LobbySettings = GetLobbySettings(lobbyEntity),
				ConnectionString = lobbyEntity.ConnectionString,
				Players = players
			};
		}
		
		public async Task<List<LobbyInfo>> GetListOfLobbies(int count, int offset)
		{
			var lobbies = await _lobbyManager.GetListOfLobbies(count, offset);
			return lobbies.Select(GetLobbyInfo).ToList();
		}

		public async Task UpdateSettings(string lobbyId, LobbySettings lobbySettings)
		{
			var lobby = await _lobbyManager.FindById(lobbyId);
			if (lobby == null)
				throw new LobbyNotFoundException();

			if (lobbySettings.LobbyName != null)
				lobby.Name = lobbySettings.LobbyName;
			if (lobbySettings.LobbyPassword != null)
				lobby.Password = lobbySettings.LobbyPassword;
			if (lobbySettings.MaxPlayersCount != null)
				lobby.MaxPlayersCount = lobbySettings.MaxPlayersCount.Value;
			if (lobbySettings.GameModeType != null)
				lobby.GameModeType = lobbySettings.GameModeType;

			await _lobbyManager.Update(lobby);
		}

		public async Task SetConnectionString(string lobbyId, string connectionString)
		{
			var lobby = await _lobbyManager.FindById(lobbyId);
			if (lobby == null)
				throw new LobbyNotFoundException();

			lobby.ConnectionString = connectionString;
			await _lobbyManager.Update(lobby);
		}

		public async Task<Lobby?> FindLobbyByPlayerId(string playerId)
		{
			var lobby = await _lobbyManager.FindByPlayerId(playerId);
			if (lobby == null)
				return null;

			var players = await _playerService.GetPlayers(lobby.PlayersIds);

			return new Lobby()
			{
				LobbyInfo = GetLobbyInfo(lobby),
				LobbySettings = GetLobbySettings(lobby),
				ConnectionString = lobby.ConnectionString,
				Players = players
			};
		}


		public async Task<Lobby> GetLobby(string lobbyId)
		{
			var lobby = await _lobbyManager.FindById(lobbyId);
			if (lobby == null)
				throw new LobbyNotFoundException();

			var players = await _playerService.GetPlayers(lobby.PlayersIds);
			return new Lobby()
			{
				LobbyInfo = GetLobbyInfo(lobby),
				LobbySettings = GetLobbySettings(lobby),
				ConnectionString = lobby.ConnectionString,
				Players = players
			};
		}

		public async Task<Lobby> JoinLobby(string lobbyId, string playerId, string password)
		{
			var playerLobby = await _lobbyManager.FindByPlayerId(playerId);
			if (playerLobby != null)
				throw new PlayerHasLobbyException();

			var lobby = await _lobbyManager.FindById(lobbyId);
			if (lobby == null)
				throw new LobbyNotFoundException();
			if (lobby.Password != password)
				throw new LobbyWrongPasswordException();

			lobby.PlayersIds = lobby.PlayersIds.Append(playerId).ToArray();
			await _lobbyManager.Update(lobby);

			var players = await _playerService.GetPlayers(lobby.PlayersIds);
			return new Lobby()
			{
				LobbyInfo = GetLobbyInfo(lobby),
				LobbySettings = GetLobbySettings(lobby),
				ConnectionString = lobby.ConnectionString,
				Players = players
			};
		}

		public async Task<Lobby> LeaveLobby(string lobbyId, string playerId)
		{
			var lobby = await _lobbyManager.FindByPlayerId(playerId);
			if (lobby == null || lobby.Id != lobbyId || !lobby.PlayersIds.Contains(playerId))
				throw new PlayerNotInLobbyException();
			
			lobby.PlayersIds = lobby.PlayersIds.Where(x => x != playerId).ToArray();
			await _lobbyManager.Update(lobby);

			var players = await _playerService.GetPlayers(lobby.PlayersIds);
			return new Lobby()
			{
				LobbyInfo = GetLobbyInfo(lobby),
				LobbySettings = GetLobbySettings(lobby),
				ConnectionString = lobby.ConnectionString,
				Players = players
			};
		}

		public async Task RemoveLobby(string lobbyId)
		{
			await _lobbyManager.Delete(lobbyId);
		}

		private static LobbyInfo GetLobbyInfo(LobbyEntity lobbyEntity)
		{
			return new LobbyInfo()
			{
				LobbyId = lobbyEntity.Id,
				LobbyName = lobbyEntity.Name,
				LobbyLeader = lobbyEntity.LobbyLeaderId,
				LobbyState = lobbyEntity.LobbyState,
				GameModeType = lobbyEntity.GameModeType,
				HasPassword = !string.IsNullOrEmpty(lobbyEntity.Password),
				MaxPlayersCount = lobbyEntity.MaxPlayersCount,
				PlayersCount = lobbyEntity.PlayersIds.Length,
			};
		}

		private static LobbySettings GetLobbySettings(LobbyEntity lobby)
		{
			return new LobbySettings()
			{
				LobbyName = lobby.Name,
				LobbyPassword = lobby.Password,
				GameModeType = lobby.GameModeType,
				MaxPlayersCount = lobby.MaxPlayersCount,
			};
		}
	}
}
