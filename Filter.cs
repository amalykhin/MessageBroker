using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace MessageBroker
{
    public interface IFilter
    {
        Pipe Input { set; }
        Pipe Output { set; }
        void ProcessStory(NewsStory story);
        IFilter Then(IFilter filter, Pipe pipe = null);
    }

    public abstract class Filter : IFilter
    {
        protected Pipe _in;
        protected Pipe _out;
        
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

    public class ContentEnricher : Filter
    {
        public override void ProcessStory(NewsStory s)
        {
            Console.WriteLine("Inside ProcessStory");
            var story = _in.GetStory();
            if (story.PublishedDate == default(DateTime))
            {
                story.PublishedDate = DateTime.Now;
            }

            if (string.IsNullOrEmpty(story.Author))
            {
                story.Author = "Anonymous";
            }
            
            _out.AddStory(story);
        }
    }
    
    public class Wiretap : Filter
    {
        private Pipe _out2;

        public Pipe WiretapPipe
        {
            get => _out2;
            set => _out2 = value;
        }

        public override void ProcessStory(NewsStory s)
        {
            Console.WriteLine("Inside Wiretap");
            var story = _in.GetStory();
            _out.AddStory(story);
            _out2.AddStory(story);
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

        public override void ProcessStory(NewsStory s)
        {
            Console.WriteLine("Inside StoryRouter");
            var story = _in.GetStory();
//            if (!_out.ContainsKey(story.Tag))
//            {
//                AddChannel(story.Tag);
//            }
            _out[story.Tag].AddStory(story);
        }

        //public void AddChannel(string channelName) => AddChannel(new Channel(channelName));
        public void AddChannel(Channel channel) => _out.Add(channel.Name, channel);

        public Channel GetChannel(string name)
        {
            _out.TryGetValue(name, out var channel);
            return channel;
        }

        public bool HasChannel(string name) => _out.ContainsKey(name);
    }

    public class DbStoreFilter : Filter
    {
        private INewsRepository _repo;

        public DbStoreFilter(INewsRepository repo)
        {
            _repo = repo;
        }
        
        public override void ProcessStory(NewsStory story)
        {
            Console.WriteLine("Inside DbStoreFilter");
            Console.WriteLine($"{story.Title} {story.Author} {story.Tag}");
            _repo.AddStory(story);
        }
    }
}