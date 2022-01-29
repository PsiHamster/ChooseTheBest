using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChooseTheBest.Api.DataSources.Lobbies;
using ChooseTheBest.Model;
using ChooseTheBest.Model.Game.Game;
using ChooseTheBest.Model.Game.Games;
using ChooseTheBest.Model.Transport.Lobbies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChooseTheBest.Api.Controllers
{
	[ApiController]
	[Route(ApiRoutes.Lobbies)]
	[Authorize]
	public class LobbiesController : ControllerBase
	{
		private readonly ILobbiesService _lobbiesService;

		public LobbiesController(ILobbiesService lobbiesService)
		{
			_lobbiesService = lobbiesService ?? throw new ArgumentNullException(nameof(lobbiesService));
		}

		[HttpGet(ApiRoutes.List)]
		public async Task<LobbyListResponse> GetLobbiesList([FromQuery] string? filter, [FromQuery] int? limit, [FromQuery] int? offset)
		{
			var lobbies = await _lobbiesService.GetListOfLobbies(limit ?? 10000, offset ?? 0);
			return new LobbyListResponse()
			{
				Lobbies = lobbies
			};
		}

		[HttpPost(ApiRoutes.Create)]
		public async Task<ActionResult<LobbyCreateResponse>> Create([FromBody] LobbyCreateRequest lobbyCreateRequest)
		{
			if (string.IsNullOrWhiteSpace(lobbyCreateRequest.LobbyName))
				return BadRequest("Wrong LobbyName");
			if (lobbyCreateRequest.MaxPlayers <= 1)
				return BadRequest("Wrong PlayersCount");
			if (lobbyCreateRequest.GameModeType != GameModeType.Default)
				return BadRequest("Wrong GameModeType");

			var playerId = GetPlayerId();
			var lobby = await _lobbiesService.CreateLobby(playerId, lobbyCreateRequest.LobbyName, lobbyCreateRequest.LobbyPassword,
				lobbyCreateRequest.MaxPlayers, lobbyCreateRequest.GameModeType);

			var connectionString = GetBaseUrl() + "/" + ApiRoutes.Games + "/" + lobby.LobbyInfo.LobbyId;
			await _lobbiesService.SetConnectionString(lobby.LobbyInfo.LobbyId, connectionString);
			lobby.ConnectionString = connectionString;

			return new LobbyCreateResponse()
			{
				Lobby = lobby
			};
		}

		//[HttpGet(ApiRoutes.Settings)]
		//public async Task<LobbySettingsResponse> Settings([FromQuery] string? filter, [FromQuery] int? limit, [FromQuery] int? offset)
		//{

		//}

		[HttpGet(ApiRoutes.Join)]
		public async Task<ActionResult<LobbyJoinResponse>> Join([FromQuery] string lobbyId, [FromQuery] string lobbyPassword)
		{
			var lobby = await _lobbiesService.JoinLobby(lobbyId, GetPlayerId(), lobbyPassword);
			return new LobbyJoinResponse
			{
				Lobby = lobby
			};
		}

		[HttpGet(ApiRoutes.Information)]
		public async Task<ActionResult<LobbyInformationResponse>> Information()
		{
			var lobby = await _lobbiesService.FindLobbyByPlayerId(GetPlayerId());
			if (lobby == null)
				return NotFound("Player not in lobby");
			return new LobbyInformationResponse
			{
				Lobby = lobby
			};
		}

		[HttpGet(ApiRoutes.Leave)]
		public async Task<IActionResult> Leave([FromQuery] string lobbyId)
		{
			var playerId = GetPlayerId();
			var lobby = await _lobbiesService.LeaveLobby(lobbyId, playerId);
			if (lobby.Players.Length == 0)
				await _lobbiesService.RemoveLobby(lobbyId);

			return Ok();
		}
	}
}
