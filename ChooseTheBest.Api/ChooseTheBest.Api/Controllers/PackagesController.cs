﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChooseTheBest.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChooseTheBest.Api.Controllers
{
	[ApiController]
	[Route(ApiRoutes.Packages)]
	[Authorize]
	public class PackagesController : Controller
	{
	}
}
