using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class User
{
    /// <summary>
    /// Id
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    /// <summary>
    /// User FIO
    /// </summary>
    public string Name { get; set; } = String.Empty;
}