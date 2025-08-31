using MongoDB.Driver;
using RealTimeChat.Services;
using RealTimeChat.Models;
using RealTimeChat.Settings;
using System.Reflection;

namespace RealTimeChat.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMongoCollections(this IServiceCollection services, IMongoClient client, string databaseName)
        {
            var modelTypes = Assembly.GetAssembly(typeof(User))!
                .GetTypes()
                .Where(t => t.IsClass && t.Namespace == "RealTimeChat.Models");

            foreach (var type in modelTypes)
            {
                var serviceType = typeof(MongoCollectionService<>).MakeGenericType(type);
                var instance = Activator.CreateInstance(serviceType, client, databaseName);
                services.AddSingleton(serviceType, instance!);
            }
        }

        public static void AddGeminiAiService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GeminiSettings>(configuration.GetSection("Gemini"));
            services.AddHttpClient<IGeminiAiService, GeminiAiService>();
        }
    }
}
