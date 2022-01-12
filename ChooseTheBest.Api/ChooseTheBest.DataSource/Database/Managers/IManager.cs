using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ChooseTheBest.DataSource.Database.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ChooseTheBest.DataSource.Database.Managers
{
	public interface IManager<T> where T : IHasId
	{
		Task<T?> FindById(string id);
		Task<T[]> FindByIds(string[] ids);
		Task Create(T entity);
		Task Update(T entity);
		Task<bool> Delete(string id);
		Task<bool> Delete(T entity);
	}

	public class Manager<T> : IManager<T> where T : IHasId
	{
		public Manager(IDatabaseConnectionHolder databaseConnectionHolder, string? collectionName = null)
		{
			_collectionName = collectionName;
			_database = databaseConnectionHolder.GetDatabase();
		}

		protected IMongoCollection<T> Collection => _database.GetCollection<T>(_collectionName ?? GetType().FullName);
		
		public async Task<T?> FindById(string id)
		{
			return await Collection
				.Find(new BsonDocument("_id", new ObjectId(id)))
				.FirstOrDefaultAsync();
		}

		public async Task<T[]> FindByIds(string[] ids)
		{
			var filterDef = new FilterDefinitionBuilder<T>();
			var filter = filterDef.In(x => x.Id, ids);

			var result = await Collection
				.Find(filter)
				.ToListAsync();

			return result.ToArray();
		}

		public async Task Create(T entity)
		{
			await Collection.InsertOneAsync(entity);
		}

		public async Task Update(T entity)
		{
			await Collection
				.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(entity.Id)), 
				entity, new ReplaceOptions { IsUpsert = true });
		}

		public async Task<bool> Delete(string id)
		{
			var result = await Collection
				.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));

			return result.DeletedCount >= 1;
		}

		public async Task<bool> Delete(T entity)
		{
			var result = await Collection
				.DeleteOneAsync(new BsonDocument("_id", new ObjectId(entity.Id)));

			return result.DeletedCount >= 1;
		}

		private readonly string? _collectionName;
		private readonly IMongoDatabase _database;
	}
}
