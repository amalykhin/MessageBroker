using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace MessageBroker
{
    public class MessageBroker
    {
        private Pipe head;
        private StoryRouter router;
        public MessageBroker(INewsRepository repo)
        {
            head = new Pipe();
            router = new StoryRouter();
            var wiretap = new Wiretap
            {
                Input = head,
                Output = new Pipe(),
                WiretapPipe = new Pipe()
            };
            wiretap.Then(router);
            new DbStoreFilter(repo) {
                Input = wiretap.WiretapPipe
            };
        }

        public void Put(NewsStory story) => head.AddStory(story);
        
        public IEnumerable<NewsStory> Get(string channelName)
        {
            Channel channel = router.GetChannel(channelName);
            if (channel == null)
            {
                channel = new Channel(channelName);
                router.AddChannel(channel);
            }

            var stories = new List<NewsStory>();
            try
            {
                while (true)
                {
                    stories.Add(channel.GetStory());
                }
            }
            catch (Exception e)
            {
            }
            
            return stories;
        }
    }
}