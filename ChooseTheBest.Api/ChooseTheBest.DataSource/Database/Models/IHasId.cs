using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChooseTheBest.DataSource.Database.Models
{
	public interface IHasId
	{
		[BsonRepresentation(BsonType.ObjectId)]
		string Id { get; set; }
	}
}
