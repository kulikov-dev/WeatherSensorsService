using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Weather.BL
{
    /// <summary>
    /// Sensors data storage
    /// </summary>
    public class SensorsStorage : ISensorsStorage
    {
        /// <summary>
        /// Sensor data
        /// </summary>
        /// <remarks> Key - identifier, value - samples </remarks>
        private readonly ConcurrentDictionary<long, SensorLog> _log = new();

        /// <summary>
        /// Add new sample
        /// </summary>
        /// <param name="info"> Sample info </param>
        /// <param name="aggregationDuration"> Aggregation duration </param>
        public void AddSample(SensorSample @info, int aggregationDuration)
        {
            _log.AddOrUpdate(@info.SensorInfo.Id, new SensorLog(@info), (k, l) => { l.AddSample(@info, aggregationDuration); return l; });
        }

        /// <inheritdoc/>
        public List<AggregatedSensorSample> GetAverageLogBySensor(long sensorId)
        {
            if (_log.ContainsKey(sensorId))
            {
                return _log[sensorId].GetAverageLog();
            }

            return new List<AggregatedSensorSample>();
        }

        /// <inheritdoc/>
        public List<SensorSample> GetFullLog()
        {
            return _log.Values.SelectMany(item => item.GetFullLog()).ToList();
        }

        /// <inheritdoc/>
        public List<SensorSample> GetFullLogBySensor(long sensorId)
        {
            if (_log.ContainsKey(sensorId))
            {
                return _log[sensorId].GetFullLog();
            }

            return new List<SensorSample>();
        }

        /// <inheritdoc/>
        public AggregatedSensorSample GetAverageLogBySensor(long sensorId, DateTime startDate, int duration)
        {
            if (_log.ContainsKey(sensorId))
            {
                return _log[sensorId].GetAverageLog(startDate, duration);
            }

            return new AggregatedSensorSample(startDate, new List<SensorSample>());
        }
    }
}