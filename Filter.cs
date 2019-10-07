using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MessageBroker
{
    public interface IFilter
    {
        Pipe Input { set; }
        Pipe Output { set; }
        void ProcessStory(NewsStory story);
        IFilter Then(IFilter filter, Pipe pipe);
    }

    public abstract class Filter : IFilter
    {
        private Pipe _in;
        private Pipe _out;
        
        public Pipe Input
        {
            set
            {
                _in = value;
                _in.OnStoryAdded += ProcessStory;
            } 
        }
        
        public Pipe Output
        {
            set => _out = value;
            get => _out;
        }

        public abstract void ProcessStory(NewsStory story);
        public IFilter Then(IFilter filter, Pipe pipe = null)
        {
            Pipe p;
            if (pipe == null)
            {
                p = Output ?? new Pipe();
            }
            else
            {
                p = pipe;
            }

            Output = p;
            filter.Input = p;
            
            return filter;
        }
    }

    public class Wiretap : Filter
    {
        private Pipe _out2;

        public Pipe WiretapPipe
        {
            set => _out2 = value;
        }
        
        public Wiretap()
        {
            GetStories();
        }

        public override void ProcessStory(NewsStory story) => _out2.AddStory(story);

        private void SaveStory(NewsStory story)
        {
            throw new NotImplementedException("The persistent storage of news stories is yet to be implemented.");
        }

        private void GetStories()
        {
            throw new NotImplementedException("The persistent storage of news stories is yet to be implemented.");
        }
    }
    
    public class StoryRouter : Filter
    {
        private Dictionary<string, Channel> _out = new Dictionary<string, Channel>();

        public new Channel Output
        {
            set => _out.Add(value.Name, value);
        }

        public override void ProcessStory(NewsStory story)
        {
            if (!_out.ContainsKey(story.Tag))
            {
                AddChannel(story.Tag);
            }
            _out[story.Tag].AddStory(story);
        }

        public void AddChannel(string channelName) => AddChannel(new Channel(channelName));
        public void AddChannel(Channel channel) => _out.Add(channel.Name, channel);
        public Channel GetChannel(string name) => _out[name];
    }
}