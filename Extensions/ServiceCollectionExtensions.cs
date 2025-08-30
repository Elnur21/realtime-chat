using MongoDB.Driver;
using MyApi.Services;
using MyApi.Models;
using System.Reflection;

namespace MyApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMongoCollections(this IServiceCollection services, IMongoClient client, string databaseName)
        {
            var modelTypes = Assembly.GetAssembly(typeof(User))!
                .GetTypes()
                .Where(t => t.IsClass && t.Namespace == "MyApi.Models");

            foreach (var type in modelTypes)
            {
                var serviceType = typeof(MongoCollectionService<>).MakeGenericType(type);
                var instance = Activator.CreateInstance(serviceType, client, databaseName);
                services.AddSingleton(serviceType, instance!);
            }
        }
    }
}
