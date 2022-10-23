namespace Weather.Client.Options
{
    /// <summary>
    /// Service configuratin
    /// </summary>
    public class ServiceConfig
    {
        /// <summary>
        /// Aggregation period (minutes)
        /// </summary>
        public int AggregationIntervalTime { get; set; }

        /// <summary>
        /// Reconnection amount
        /// </summary>
        public int ReconnectionAmount { get; set; }

        /// <summary>
        /// Reconnection timeout (seconds)
        /// </summary>
        public int ReconnectionTimeout { get; set; }
    }
}