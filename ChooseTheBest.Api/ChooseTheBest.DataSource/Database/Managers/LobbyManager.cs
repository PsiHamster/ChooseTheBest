using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChooseTheBest.DataSource.Database.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ChooseTheBest.DataSource.Database.Managers
{
	public interface ILobbyManager : IManager<LobbyEntity>
	{
		Task<List<LobbyEntity>> GetListOfLobbies(int count, int offset);
		Task<LobbyEntity?> FindByPlayerId(string playerId);
	}

	public class LobbyManager : Manager<LobbyEntity>, ILobbyManager
	{
		public LobbyManager(IDatabaseConnectionHolder databaseConnectionHolder, string? collectionName = null) : base(databaseConnectionHolder, collectionName)
		{
		}

		public async Task<List<LobbyEntity>> GetListOfLobbies(int count, int offset)
		{
			var filterDef = new FilterDefinitionBuilder<LobbyEntity>();
			var filter = filterDef.Not(new BsonDocumentFilterDefinition<LobbyEntity>(
				new BsonDocument("LobbyState", "Finished")));

			var result = await Collection
				.Find(filter)
				.Skip(offset)
				.Limit(count)
				.ToListAsync();

			return result;
		}

		public async Task<LobbyEntity?> FindByPlayerId(string playerId)
		{
			var filterDef = new FilterDefinitionBuilder<LobbyEntity>();
			var filter = filterDef.AnyIn(
				x => x.PlayersIds,
				new []{ playerId }
			);

			var result = await Collection
				.Find(filter)
				.FirstOrDefaultAsync();

			return result;
		}
	}
}
