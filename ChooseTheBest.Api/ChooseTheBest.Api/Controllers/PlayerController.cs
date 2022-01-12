using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChooseTheBest.Model.Transport.Player;
using Microsoft.AspNetCore.Mvc;

namespace ChooseTheBest.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PlayerController : Controller
	{
		[HttpPost]
		public CreateResponse Create()
		{
			return new CreateResponse()
			{
				AccessToken = "",
				Id = ""
			};
		}

		[HttpPost]
		public RegistrationResponse Register([FromBody] RegistrationRequest request)
		{
			return new RegistrationResponse()
			{

			};
		}
	}
}
