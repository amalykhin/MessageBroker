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

        public SampleDataController(IHttpContextAccessor contextAccessor, MessageBroker broker, ClientService clientService)
        {
            this.broker = broker;
            this.context = contextAccessor;
            this.clientService = clientService;
        }

        private IPAddress GetClientIP() => context.HttpContext?.Connection?.RemoteIpAddress;

        [HttpGet("/subscribe/{channel}")]
        public IActionResult Subscribe(string channel)
        {
            var ip = GetClientIP();
            var clientId = clientService.AddClient(ip);
            clientService.AddSubscriber(clientId, channel);

            return Ok();
        }
        
        [HttpGet("/{clientId}/stories")]
        public IEnumerable<NewsStory> GetStories(int clientId)
        {
            var channelNames = clientService.GetSubscribedChannels(clientId);
            return channelNames.SelectMany(name => broker.Get(name));
        }

    }
}
