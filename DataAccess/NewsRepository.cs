using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MessageBroker
{
    public class NewsRepository : INewsRepository
    {
        private readonly NewsContext _context = null;

        public NewsRepository(IOptions<Settings> settings)
        {
            _context = new NewsContext(settings);
        }
        
        public async Task<IEnumerable<NewsStory>> GetAllNews()
        {
            try
            {
                return await _context.News
                    .Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<IEnumerable<NewsStory>> GetNewsByTag(string tag)
        {
            try
            {
                var query = _context.News.Find(story => story.Tag.Equals(tag));

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<IEnumerable<NewsStory>> GetLatestNews(string tag)
        {
            try
            {
                var query = _context.News.Find(story => story.Tag.Equals(tag)).Limit(5);
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task AddStory(NewsStory item)
        {
            try
            {
                await _context.News.InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> RemoveStory(string id)
        {
            try
            {
                DeleteResult actionResult 
                    = await _context.News.DeleteOneAsync(
                        Builders<NewsStory>.Filter.Eq("Id", id));

                return actionResult.IsAcknowledged 
                       && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public Task<bool> RemoveAllNotes()
        {
            throw new System.NotImplementedException();
        }
    }
}