using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChooseTheBest.DataSource.Database.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChooseTheBest.DataSource.Database.Managers
{
	public interface ISessionManager : IManager<SessionEntity>
	{
		Task<SessionEntity> FindToken(string token);
	}

	public class SessionManager : Manager<SessionEntity>, ISessionManager
	{
		public SessionManager(IDatabaseConnectionHolder databaseConnectionHolder, string? collectionName = null) : base(databaseConnectionHolder, collectionName)
		{
		}

		public async Task<SessionEntity> FindToken(string token)
		{
			return await Collection.Find(new BsonDocument("Token", token)).FirstOrDefaultAsync();
		}
	}
}
