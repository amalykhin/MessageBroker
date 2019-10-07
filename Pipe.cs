using System;
using System.Collections.Generic;

namespace MessageBroker
{
    public class Pipe
    {
        protected readonly Queue<NewsStory> storyQueue = new Queue<NewsStory>();

        public delegate void PipeEventHandler(NewsStory story);
        public event PipeEventHandler OnStoryAdded;
        public void AddStory(NewsStory story) => storyQueue.Enqueue(story); 
        public NewsStory GetStory() => storyQueue.Dequeue();
    }

    public class Channel : Pipe
    {
        public string Name { get; set; }

        public Channel(string name)
        {
            Name = name;
        }
    }
}