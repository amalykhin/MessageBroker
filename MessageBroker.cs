using System;
using System.Collections.Generic;

namespace MessageBroker
{
    public class MessageBroker
    {
        private Pipe head;
        private StoryRouter router;
        public MessageBroker()
        {
            head = new Pipe();
            router = new StoryRouter();
            new Wiretap
            {
                Input = head,
                Output = new Pipe(),
                WiretapPipe = new Pipe()
            }.Then(router);
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