using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MessageBroker
{
    public class NewsContext
    {
        private readonly IMongoDatabase _database = null;

        public NewsContext(IOptions<Settings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.Database);
        }

        public IMongoCollection<NewsStory> News => _database.GetCollection<NewsStory>("News");
    }
}