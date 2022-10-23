using System;
using System.Collections.Generic;

namespace Weather.Data
{
    /// <summary>
    /// Interface for sensors storage
    /// </summary>
    public interface ISensorsStorage
    {
        /// <summary>
        /// Get full list of samples by all sensors
        /// </summary>
        /// <returns> List of samples </returns>
        List<SensorSample> GetFullLog();

        /// <summary>
        /// Get full list of samples by sensor id
        /// </summary>
        /// <param name="sensorId"> Sensor identifier </param>
        /// <returns> List of samples </returns>
        List<SensorSample> GetFullLogBySensor(long sensorId);

        /// <summary>
        /// Get aggregated samples by sensor id
        /// </summary>
        /// <param name="sensorId"> Sensor identifier </param>
        /// <returns> List of aggregated samples </returns>
        List<AggregatedSensorSample> GetAverageLogBySensor(long sensorId);

        /// <summary>
        /// Get aggregated samples by sensor id for specified period
        /// </summary>
        /// <param name="sensorId"> Sensor identifier </param>
        /// <param name="startDate"> Start date </param>
        /// <param name="duration"> Duration in minutes </param>
        /// <returns> List of aggregated samples </returns>
        AggregatedSensorSample GetAverageLogBySensor(long sensorId, DateTime startDate, int duration);

        /// <summary>
        /// Add new sample to the storage
        /// </summary>
        /// <param name="info"> Sample info </param>
        /// <param name="aggregationDuration"> Aggregation period (minutes) </param>
        void AddSample(SensorSample @info, int aggregationDuration);
    }
}