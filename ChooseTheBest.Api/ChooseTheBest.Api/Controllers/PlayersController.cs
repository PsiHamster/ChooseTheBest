using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ChooseTheBest.Api.DataSources.Players;
using ChooseTheBest.Api.DataSources.Sessions;
using ChooseTheBest.Model;
using ChooseTheBest.Model.Transport.Player;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace ChooseTheBest.Api.Controllers
{
	public static class ApiSecretSettings
	{
		public static string Audiance = "ChooseTheBest";
		public static string Issuer = "ChooseTheBest";
		public static string Secret = "ldfbop;dfzkgpodfgp";
		public static string ConnectionString = "mongodb://localhost:27017/?readPreference=primary&appname=Api&directConnection=true&ssl=false";
		public static string DatabaseName = "choosethebest";
	}

	[ApiController]
	[Route(ApiRoutes.Players)]
	public class PlayersController : Controller
	{
		private readonly ISessionsService _sessionsService;
		private readonly IPlayerService _playerService;

		public PlayersController(ISessionsService sessionsService, IPlayerService playerService)
		{
			_sessionsService = sessionsService;
			_playerService = playerService;
		}

		[HttpGet(ApiRoutes.Create)]
		public async Task<CreateResponse> Create()
		{
			var player = await _playerService.CreatePlayer();
			var session = await _sessionsService.CreateToken(player.PlayerId);

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, player.PlayerId),
				new Claim(JwtRegisteredClaimNames.Name, player.PlayerName),
			};

			var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiSecretSettings.Secret)), SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(ApiSecretSettings.Issuer,
				ApiSecretSettings.Audiance, 
				claims, 
				DateTime.Now, 
				DateTime.Now.AddDays(30),
				signingCredentials);

			return new CreateResponse()
			{
				AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
				Id = player.PlayerId,
			};
		}

		[HttpPost(ApiRoutes.SetPlayerData)]
		[Authorize]
		public async Task<bool> SetPlayerData([FromBody] SetPlayerDataRequest request)
		{
			// todo: some validations

			var playerId = HttpContext.User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
			var player = await _playerService.GetPlayer(playerId);
			
			player.PlayerName = request.PlayerName;
			player.PlayerAvatarBase64 = request.Base64Avatar;

			await _playerService.UpdatePlayerInfo(player);

			return true;
		}
	}
}
