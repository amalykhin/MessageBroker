using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver.Core.Misc;

namespace MessageBroker
{
    public class MessageBroker
    {
        private Pipe head;
        private StoryRouter router;
        private INewsRepository _repo;
        public MessageBroker(INewsRepository repo)
        {
            _repo = repo;
            head = new Pipe();
            router = new StoryRouter();
            var wiretap = new Wiretap
            {
                WiretapPipe = new Pipe()
            };
            new ContentEnricher
            {
                Input = head,
                Output = new Pipe()
            }.Then(wiretap).Then(router);

            //wiretap.Then(router);
            new DbStoreFilter(repo) {
                Input = wiretap.WiretapPipe
            };
        }

        public void Put(NewsStory story)
        {
            EnsureChannelExists(story.Tag);
            head.AddStory(story);
        }
        
        public IEnumerable<NewsStory> Get(string channelName, int subId)
        {
            EnsureChannelExists(channelName);
            Channel channel = router.GetChannel(channelName);
            var stories = new List<NewsStory>();
            try
            {
                while (true)
                {
                    stories.Add(channel.GetStory(subId));
                }
            }
            catch (Exception e)
            {
            }
            
            return stories;
        }

        public void Subscribe(string channel, int clientId)
        {
            EnsureChannelExists(channel);
            router.GetChannel(channel).Subscribe(clientId);
        }

        private void EnsureChannelExists(string channelName)
        {
            if (!router.HasChannel(channelName))
            {
                router.AddChannel(new Channel(channelName, _repo));
            }
        }
    }
}