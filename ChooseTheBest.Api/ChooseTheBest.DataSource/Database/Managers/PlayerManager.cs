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
	public interface IPlayerManager : IManager<PlayerEntity>
	{
		Task<PlayerEntity> FindByLogin(string login);
	}

	public class PlayerManager : Manager<PlayerEntity>, IPlayerManager
	{
		public PlayerManager(IDatabaseConnectionHolder databaseConnectionHolder, string? collectionName = null) : base(databaseConnectionHolder, collectionName)
		{
		}

		public async Task<PlayerEntity> FindByLogin(string login)
		{
			return await Collection.Find(new BsonDocument("Login", login)).FirstOrDefaultAsync();
		}
	}
}
