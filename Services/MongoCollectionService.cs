using MongoDB.Driver;

namespace MyApi.Services
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
