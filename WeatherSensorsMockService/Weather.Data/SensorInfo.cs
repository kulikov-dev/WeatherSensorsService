namespace Weather.Data
{
    /// <summary>
    /// Sensor info
    /// </summary>
    public class SensorInfo
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Sensor type
        /// </summary>
        public SensorTypeEnum SensorType { get; set; }

        /// <summary>
        /// Current temperature (С)
        /// </summary>
        public double Temperature { get; set; } = 0;

        /// <summary>
        /// Current humidity (%)
        /// </summary>
        public double Humidity { get; set; } = 50;

        /// <summary>
        /// CO2 (ppm)
        /// </summary>
        public double CO2 { get; set; } = 400;
    }
}