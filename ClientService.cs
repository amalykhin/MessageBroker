using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MessageBroker
{
    public class ClientService
    {
        private int newClientId;
        private Dictionary<string, ICollection<int>> clientsByChannel 
            = new Dictionary<string, ICollection<int>>();
        private Dictionary<int, IPAddress> addressById = new Dictionary<int, IPAddress>();

        public int AddClient(IPAddress address)
        {
            if (addressById.ContainsValue(address))
            {
                return addressById
                    .Where(kvp => kvp.Value.Equals(address))
                    .Select(kvp => kvp.Key)
                    .First();
            }
            addressById[newClientId] = address;
            return newClientId++;
        }
        public void AddSubscriber(int id, string channel)
        {
            var existingClients = clientsByChannel[channel] ?? new List<int>();
            existingClients.Add(id);
            clientsByChannel[channel] = existingClients;
        }

        public void RemoveSubscriber(int id, string channel)
        {
            if (clientsByChannel[channel] == null)
                return;
            var existingClients = clientsByChannel[channel];
            existingClients.Remove(id);
        }

        public IEnumerable<string> GetSubscribedChannels(int id)
        {
            return from kvp in clientsByChannel
                   where kvp.Value.Contains(id)
                   select kvp.Key;
        }

        public IEnumerable<IPAddress> GetClientAddresses(string channel) =>
            clientsByChannel[channel].Select(clientId => addressById[clientId]);
    }
}