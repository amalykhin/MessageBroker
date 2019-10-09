using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessageBroker.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    public class SampleDataController : Controller
    {
        private MessageBroker broker;
        private IHttpContextAccessor context;
        private ClientService clientService;
        private INewsRepository repo;

        public SampleDataController(IHttpContextAccessor contextAccessor, 
                                    MessageBroker broker, 
                                    ClientService clientService,
                                    INewsRepository repo)
        {
            this.broker = broker;
            this.context = contextAccessor;
            this.clientService = clientService;
            this.repo = repo;
        }

        private IPAddress GetClientIP() => context.HttpContext?.Connection?.RemoteIpAddress;

        [HttpGet("/subscribe/{channel}")]
        public object Subscribe(string channel)
        {
            var ip = GetClientIP();
            var clientId = clientService.AddClient(ip);
            clientService.AddSubscriber(clientId, channel);

            var latestNews = repo.GetLatestNews(channel).Result;
            return Ok(new
            {
                ClientId = clientId,
                News = latestNews
            });
        }
        
        [HttpGet("/{clientId}/stories")]
        public IEnumerable<NewsStory> GetStories(int clientId)
        {
            var channelNames = clientService.GetSubscribedChannels(clientId);
            return channelNames.SelectMany(name => broker.Get(name));
        }

        [HttpPost("/stories")]
        public IActionResult PostStory([FromBody] NewsStory story)
        {
            Console.WriteLine($"PostStory controller: {story.Title} {story.Author} {story.Tag}");
            broker.Put(story);

            return Ok();
        }
    }
}
