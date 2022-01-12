using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ChooseTheBest.DataSource.Database.Managers;
using ChooseTheBest.DataSource.Database.Models;
using MongoDB.Bson.Serialization.Serializers;

namespace ChooseTheBest.Api.DataSources.Sessions
{
	public interface ISessionsService
	{
		Task<(string token, string refreshToken)> CreateToken(string playerId);
		Task<bool> ValidateToken(string playerId, string token);
		Task<string> RefreshToken(string token, string refreshToken);
	}

	public class SessionsService : ISessionsService
	{
		public static readonly TimeSpan DefaultExpirationPeriod = TimeSpan.FromDays(30);

		public SessionsService(ISessionManager sessionManager)
		{
			_sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
		}

		public async Task<(string token, string refreshToken)> CreateToken(string playerId)
		{
			var session = new SessionEntity()
			{
				Expiration = DateTimeOffset.Now.Add(DefaultExpirationPeriod),
				PlayerId = playerId,

				Token = Guid.NewGuid().ToString(),
				RefreshToken = Guid.NewGuid().ToString(),
			};

			await _sessionManager.Create(session);

			return (session.Token, session.RefreshToken);
		}

		public async Task<bool> ValidateToken(string playerId, string token)
		{
			var tokenEntity = await _sessionManager.FindToken(token);

			return tokenEntity.Expiration > DateTimeOffset.Now;
		}

		public async Task<string> RefreshToken(string token, string refreshToken)
		{
			var tokenEntity = await _sessionManager.FindToken(token);
			if (tokenEntity == null)
				throw new TokenNotFoundException();

			tokenEntity.RefreshToken = Guid.NewGuid().ToString();
			tokenEntity.Expiration = DateTimeOffset.Now.Add(DefaultExpirationPeriod);

			await _sessionManager.Update(tokenEntity);

			return tokenEntity.RefreshToken;
		}

		private readonly ISessionManager _sessionManager;
	}

	public class TokenNotFoundException : Exception
	{
	}
}
