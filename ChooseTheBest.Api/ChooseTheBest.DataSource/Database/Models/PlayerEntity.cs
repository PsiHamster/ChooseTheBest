using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChooseTheBest.DataSource.Database.Models
{
	public class PlayerEntity : IHasId
	{
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
		
		public string Login { get; set; }
		public string PasswordHash { get; set; }
		public string PasswordSalt { get; set; }
		public string DisplayName { get; set; }
		public string AvatarBase64 { get; set; }
	}
}
