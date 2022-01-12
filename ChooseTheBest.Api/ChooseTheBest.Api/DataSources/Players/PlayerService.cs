using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ChooseTheBest.Api.DataSources.Players.Exceptions;
using ChooseTheBest.Api.Utils;
using ChooseTheBest.DataSource.Database.Managers;
using ChooseTheBest.DataSource.Database.Models;
using ChooseTheBest.Model.Game.Players;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ChooseTheBest.Api.DataSources.Players
{
	public interface IPlayerService
	{
		Task<PlayerInfo> CreatePlayer();
		Task<PlayerInfo> GetPlayer(string id);
		Task<PlayerInfo[]> GetPlayers(string[] ids);

		Task RegisterPlayer(string id, string login, string password);
		Task DeletePlayer(string id);

		Task<PlayerInfo> LoginPlayer(string login, string password);
	}

	public class PlayerService : IPlayerService
	{
		public PlayerService([NotNull] IPlayerManager playerManager, [NotNull] IPasswordHasher passwordHasher)
		{
			_playerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
			_passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
		}

		public async Task<PlayerInfo> CreatePlayer()
		{
			var playerEntity = new PlayerEntity();
			await _playerManager.Create(playerEntity);

			return new PlayerInfo()
			{
				PlayerId = playerEntity.Id,
			};
		}

		public async Task<PlayerInfo> GetPlayer(string id)
		{
			var player = await _playerManager.FindById(id);
			if (player == null)
				throw new PlayerNotFoundException();

			return new PlayerInfo()
			{
				PlayerAvatarBase64 = player.AvatarBase64,
				PlayerId = player.Id,
				PlayerName = player.DisplayName,
			};
		}

		public async Task<PlayerInfo[]> GetPlayers(string[] ids)
		{
			var players = await _playerManager.FindByIds(ids);
			if (players.Length != ids.Length)
				throw new PlayerNotFoundException();

			return players.Select(player => new PlayerInfo
			{
				PlayerAvatarBase64 = player.AvatarBase64,
				PlayerId = player.Id,
				PlayerName = player.DisplayName,
			}).ToArray();
		}

		public async Task RegisterPlayer(string id, string login, string password)
		{
			var player = await _playerManager.FindById(id);
			if (player == null)
				throw new PlayerNotFoundException();

			var loginPlayer = await _playerManager.FindByLogin(login);
			if (loginPlayer.Id != id)
				throw new PlayerLoginExistException();

			player.Login = login;
			(player.PasswordHash, player.PasswordSalt) = _passwordHasher.HashPassword(password);
		}

		public async Task DeletePlayer(string id)
		{
			await _playerManager.Delete(id);
		}

		public async Task<PlayerInfo> LoginPlayer(string login, string password)
		{
			var player = await _playerManager.FindByLogin(login);
			if (player == null)
				throw new PlayerNotFoundException();

			if (player.PasswordHash == _passwordHasher.HashPassword(password, player.PasswordSalt)) 
				return new PlayerInfo()
				{
					PlayerId = player.Id,
					PlayerAvatarBase64 = player.AvatarBase64,
					PlayerName = player.DisplayName,
				};

			throw new PlayerWrongPasswordException();
		}
		
		private readonly IPlayerManager _playerManager;
		private readonly IPasswordHasher _passwordHasher;
	}
}
