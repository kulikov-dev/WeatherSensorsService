namespace Weather.Client.Models
{
    /// <summary>
    /// Interface of subscription to sensors
    /// </summary>
    public interface ISubscriptionStorage
    {
        /// <summary>
        /// Check if has subscription to a sensor
        /// </summary>
        /// <param name="sensorId"> Sensor identifier </param>
        /// <returns> True if contains </returns>
        bool Contains(long sensorId);

        /// <summary>
        /// Subscribe to a sensor
        /// </summary>
        /// <param name="sensorId"> Sensor identifier </param>
        void Subscribe(long sensorId);

        /// <summary>
        /// Unsusbscribe from a sensor
        /// </summary>
        /// <param name="sensorId"> Sensor identifier </param>
        void Unsubscribe(long sensorId);

        /// <summary>
        /// Unsubscribe from all sensors events
        /// </summary>
        void UnsubscribeAll();
    }
}