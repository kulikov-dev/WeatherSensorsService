using System.Collections.Generic;

namespace Weather.Client.Models
{
    /// <summary>
    /// Storage of subscriptions to sensors
    /// </summary>
    public class SubscriptionStorage : ISubscriptionStorage
    {
        /// <summary>
        /// List of subscribed sensor's identifiers
        /// </summary>
        private readonly HashSet<long> _sensorsToListen = new();

        /// <inheritdoc/>
        public void Subscribe(long sensorId)
        {
            _sensorsToListen.Add(sensorId);
        }

        /// <inheritdoc/>
        public void UnsubscribeAll()
        {
            _sensorsToListen.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(long sensorId)
        {
            return _sensorsToListen.Contains(sensorId);
        }

        /// <inheritdoc/>
        public void Unsubscribe(long sensorId)
        {
            _sensorsToListen.Remove(sensorId);
        }
    }
}
