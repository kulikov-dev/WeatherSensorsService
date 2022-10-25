using System;

namespace Weather.Data
{
    /// <summary>
    /// Sample information
    /// </summary>
    public class SensorSample
    {
        /// <summary>
        ///  Event identifier
        /// </summary>
        public long EventId { get; set; }

        /// <summary>
        /// Sensor info and current measures
        /// </summary>
        public SensorInfo SensorInfo { get; set; }

        /// <summary>
        /// Date of measurment
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
