using System;
using System.Diagnostics.Tracing;

namespace MessageBroker
{
    public class NewsStory {
        public string Tag { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public DateTime PublishedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        /*string IMessage.Content { 
            get {
                return $"{Title} {Body} {Author} {PublishedDate} {ModifiedDate}";
            }
        }*/
    }
}