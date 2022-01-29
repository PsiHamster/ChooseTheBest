using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChooseTheBest.Api.Controllers;
using ChooseTheBest.Api.DataSources.Lobbies;
using ChooseTheBest.Api.DataSources.Players;
using ChooseTheBest.Api.DataSources.Sessions;
using ChooseTheBest.Api.Utils;
using ChooseTheBest.DataSource.Database;
using ChooseTheBest.DataSource.Database.Managers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.IO;

namespace ChooseTheBest.Api
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IDatabaseConnectionHolder>((provider) =>
				new DatabaseConnectionHolder(ApiSecretSettings.ConnectionString, ApiSecretSettings.DatabaseName));
			services.AddSingleton<IPlayerManager>((provider) =>
				new PlayerManager(provider.GetRequiredService<IDatabaseConnectionHolder>()));
			services.AddSingleton<ISessionManager>((provider) =>
				new SessionManager(provider.GetRequiredService<IDatabaseConnectionHolder>()));
			services.AddSingleton<ILobbyManager>((provider) =>
				new LobbyManager(provider.GetRequiredService<IDatabaseConnectionHolder>()));


			services.AddSingleton<IPlayerService>((provider) =>
				new PlayerService(provider.GetRequiredService<IPlayerManager>(), new PasswordHasher()));
			services.AddSingleton<ISessionsService>((provider) =>
				new SessionsService(provider.GetRequiredService<ISessionManager>()));
			services.AddSingleton<ILobbiesService>((provider) =>
				new LobbiesService(provider.GetRequiredService<IPlayerService>(), provider.GetRequiredService<ILobbyManager>()));

			services.AddAuthentication("OAuth")
				.AddJwtBearer("OAuth", config =>
				{
					config.TokenValidationParameters = new TokenValidationParameters()
					{
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ApiSecretSettings.Secret)),
						ValidIssuer = ApiSecretSettings.Issuer,
						ValidAudience = ApiSecretSettings.Audiance,
					};
				});

			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChooseTheBest.Api", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChooseTheBest.Api v1"));
			}

			app.UseHttpsRedirection();
			app.UseRouting();

			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
			app.UseAuthentication();
			app.UseAuthorization();

			var webSocketOptions = new WebSocketOptions()
			{
				KeepAliveInterval = TimeSpan.FromSeconds(120),
			};

			app.UseWebSockets(webSocketOptions);
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
