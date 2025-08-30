using MongoDB.Driver;

namespace RealTimeChat.Services
{
    public class MongoCollectionService<T>
    {
        public IMongoCollection<T> Collection { get; }

        public MongoCollectionService(IMongoClient client, string databaseName)
        {
            var database = client.GetDatabase(databaseName);
            Collection = database.GetCollection<T>(typeof(T).Name);
        }
    }
}
