using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChooseTheBest.DataSource.Database.Models
{
	public class SessionEntity : IHasId
	{
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public string Token { get; set; }
		public string RefreshToken { get; set; }
		public string PlayerId { get; set; }
		public DateTimeOffset Expiration { get; set; }
	}
}
