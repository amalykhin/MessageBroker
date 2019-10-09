using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessageBroker
{
    public interface INewsRepository
    {
        Task<IEnumerable<NewsStory>> GetAllNews();
        Task<IEnumerable<NewsStory>> GetNewsByTag(string tag);
        Task<IEnumerable<NewsStory>> GetLatestNews(string tags);

        Task AddStory(NewsStory item);
        
        Task<bool> RemoveStory(string id);
        Task<bool> RemoveAllNotes();
    }
}