using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ChooseTheBest.Api.DataSources.Sessions.Exceptions;
using ChooseTheBest.DataSource.Database.Managers;
using ChooseTheBest.DataSource.Database.Models;
using ChooseTheBest.Model.Game.Session;
using MongoDB.Bson.Serialization.Serializers;

namespace ChooseTheBest.Api.DataSources.Sessions
{
	public interface ISessionsService
	{
		Task<Session> CreateToken(string playerId);
		Task<Session> ValidateToken(string token);
		Task<Session> RefreshToken(string token, string refreshToken);
	}

	public class SessionsService : ISessionsService
	{
		public static readonly TimeSpan DefaultExpirationPeriod = TimeSpan.FromDays(30);

		public SessionsService(ISessionManager sessionManager)
		{
			_sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
		}

		public async Task<Session> CreateToken(string playerId)
		{
			var session = new SessionEntity()
			{
				Expiration = DateTimeOffset.Now.Add(DefaultExpirationPeriod),
				PlayerId = playerId,

				Token = Guid.NewGuid().ToString(),
				RefreshToken = Guid.NewGuid().ToString(),
			};

			await _sessionManager.Create(session);

			return new Session()
			{
				PlayerId = playerId,
				RefreshToken = session.RefreshToken,
				Token = session.Token,
			};
		}
		
		public async Task<Session> ValidateToken(string token)
		{
			var tokenEntity = await _sessionManager.FindToken(token);
			ValidateTokenEntity(tokenEntity);

			return new Session()
			{
				PlayerId = tokenEntity.PlayerId,
				RefreshToken = tokenEntity.RefreshToken,
				Token = tokenEntity.Token,
			};
		}

		public async Task<Session> RefreshToken(string token, string refreshToken)
		{
			var tokenEntity = await _sessionManager.FindToken(token);
			ValidateTokenEntity(tokenEntity);

			tokenEntity.RefreshToken = Guid.NewGuid().ToString();
			tokenEntity.Expiration = DateTimeOffset.Now.Add(DefaultExpirationPeriod);

			await _sessionManager.Update(tokenEntity);

			return new Session()
			{
				PlayerId = tokenEntity.PlayerId,
				RefreshToken = tokenEntity.RefreshToken,
				Token = tokenEntity.Token,
			};
		}

		private static void ValidateTokenEntity(SessionEntity? tokenEntity)
		{
			if (tokenEntity == null)
			{
				throw new TokenNotFoundException();
			}

			if (tokenEntity.Expiration > DateTimeOffset.Now)
			{
				throw new TokenExpiredException();
			}
		}

		private readonly ISessionManager _sessionManager;
	}
}
