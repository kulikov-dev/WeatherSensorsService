using System;
using System.Collections.Generic;
using System.Linq;

namespace Weather.Data
{
    /// <summary>
    /// Aggregated sample for the specific period
    /// </summary>
    public sealed class AggregatedSensorSample
    {
        /// <summary>
        /// Start aggregation date
        /// </summary>
        /// <remarks> Aggregates data from this date for next N minutes </remarks>
        public DateTime AggregationDate { get; set; }

        /// <summary>
        /// Average temperature
        /// </summary>
        public double AverageTemperature { get; set; }

        /// <summary>
        /// Average humidity
        /// </summary>
        public double AverageHumidity { get; set; }

        /// <summary>
        /// Min CO2
        /// </summary>
        public double MinCO2 { get; set; }

        /// <summary>
        /// Max CO2
        /// </summary>
        public double MaxCO2 { get; set; }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="aggregationDate"> Start aggregation date </param>
        /// <param name="items"> List of items to aggregate </param>
        public AggregatedSensorSample(DateTime aggregationDate, List<SensorSample> items)
        {
            AggregationDate = aggregationDate;
            AverageTemperature = 0;
            AverageHumidity = 0;

            if (items == null || !items.Any())
            {
                MinCO2 = MaxCO2 = 0;

                return;
            }

            MinCO2 = int.MaxValue;
            MaxCO2 = int.MinValue;

            foreach (var item in items)
            {
                AverageTemperature += item.SensorInfo.Temperature;
                AverageHumidity += item.SensorInfo.Humidity;

                MinCO2 = Math.Min(MinCO2, item.SensorInfo.CO2);
                MaxCO2 = Math.Max(MaxCO2, item.SensorInfo.CO2);
            }

            AverageHumidity /= items.Count;
            AverageTemperature /= items.Count;
        }
    }
}
