using System;
using System.Collections.Generic;

namespace MessageBroker
{
    public class Pipe
    {
        protected readonly Queue<NewsStory> storyQueue = new Queue<NewsStory>();

        public delegate void PipeEventHandler(NewsStory story);
        public event PipeEventHandler OnStoryAdded;

        public virtual void AddStory(NewsStory story)
        {
            storyQueue.Enqueue(story);
            OnStoryAdded?.Invoke(story);
        }
        public NewsStory GetStory() => storyQueue.Dequeue();
    }

    public class Channel : Pipe
    {
        private Dictionary<int, Pipe> _subscriberQueues = new Dictionary<int, Pipe>();
        private INewsRepository _repo;
        
        public string Name { get; set; }

        public Channel(string name, INewsRepository repo)
        {
            Name = name;
            _repo = repo;
        }

        public override void AddStory(NewsStory story)
        {
            foreach (var subQ in _subscriberQueues.Values)
            {
                subQ.AddStory(story);
            }
        }

        public void Subscribe(int clientId)
        {
            _subscriberQueues[clientId] = new Pipe();
            var news = _repo.GetLatestNews(Name).Result;
            foreach (var newsStory in news)
            {
                _subscriberQueues[clientId].AddStory(newsStory);
            }
        }

        public void Unsubscribe(int subId)
        {
            _subscriberQueues.Remove(subId);
        }

        public NewsStory GetStory(int subId) => _subscriberQueues[subId].GetStory();
    }
}