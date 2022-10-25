using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Weather.Data
{
    /// <summary>
    /// Sensors data storage
    /// </summary>
    public sealed class SensorsStorage : ISensorsStorage
    {
        /// <summary>
        /// Sensors data
        /// </summary>
        /// <remarks> Key - identifier, value - samples </remarks>
        private readonly ConcurrentDictionary<long, SensorLog> _log = new();

        /// <inheritdoc/>
        public void AddSample(SensorSample @info, int aggregationDuration)
        {
            _log.AddOrUpdate(@info.SensorInfo.Id, new SensorLog(@info), (k, l) => { l.AddSample(@info, aggregationDuration); return l; });
        }

        /// <inheritdoc/>
        public List<AggregatedSensorSample> GetAggregatedLogBySensor(long sensorId)
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
        public AggregatedSensorSample GetAggregatedLogBySensor(long sensorId, DateTime startDate, int duration)
        {
            if (_log.ContainsKey(sensorId))
            {
                return _log[sensorId].GetAverageLog(startDate, duration);
            }

            return new AggregatedSensorSample(startDate, new List<SensorSample>());
        }
    }
}
