using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChooseTheBest.DataSource.Database.Models
{
	public class LobbyEntity : IHasId
	{
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
		public string Name { get; set; }
		public string LobbyLeaderId { get; set; }
		public string Password { get; set; }
		public int MaxPlayersCount { get; set; }
		public string[] PlayersIds { get; set; }
		public string LobbyState { get; set; }
		public string GameModeType { get; set; }
		public string ConnectionString { get; set; }
		public string PackageName { get; set; }
		public string PackageId { get; set; }
	}
}