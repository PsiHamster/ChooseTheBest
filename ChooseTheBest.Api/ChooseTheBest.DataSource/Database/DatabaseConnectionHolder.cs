using MongoDB.Driver;

namespace ChooseTheBest.DataSource.Database
{
	public interface IDatabaseConnectionHolder
	{
		IMongoDatabase GetDatabase();
	}

	public class DatabaseConnectionHolder : IDatabaseConnectionHolder
	{
		private readonly IMongoDatabase mongoDatabase;

		public DatabaseConnectionHolder(string connectionString, string databaseName)
		{
			var mongoClient = new MongoClient(connectionString);
			this.mongoDatabase = mongoClient.GetDatabase(databaseName);
		}

		public IMongoDatabase GetDatabase()
		{
			return mongoDatabase;
		}
	}
}
