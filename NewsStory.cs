using System;
using System.Diagnostics.Tracing;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MessageBroker
{
    public class NewsStory {
        [BsonId]
        public ObjectId InternalId { get; set; }
        public string Tag { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        [BsonDateTimeOptions]
        public DateTime PublishedDate { get; set; }
        [BsonDateTimeOptions]
        public DateTime ModifiedDate { get; set; }
        /*string IMessage.Content { 
            get {
                return $"{Title} {Body} {Author} {PublishedDate} {ModifiedDate}";
            }
        }*/
    }
}