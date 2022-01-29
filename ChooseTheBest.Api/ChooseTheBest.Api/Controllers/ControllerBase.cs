using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ChooseTheBest.Api.Controllers
{
	public abstract class ControllerBase : Controller
	{
		protected string GetPlayerId()
		{
			var playerId = HttpContext.User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
			return playerId;
		}

		public string GetBaseUrl()
		{
			var request = HttpContext.Request;
			var host = request.Host.ToUriComponent();
			var pathBase = request.PathBase.ToUriComponent();
			return $"{request.Scheme}://{host}{pathBase}";
		}
	}
}
