using System;
using System.Collections.Generic;
using System.Linq;

namespace Weather.Data
{
    /// <summary>
    /// Sensor samples
    /// </summary>
    public class SensorLog
    {
        /// <summary>
        /// List of all samples
        /// </summary>
        private readonly List<SensorSample> _log = new();

        /// <summary>
        /// List of aggregated samples
        /// </summary>
        private readonly Dictionary<DateTime, AggregatedSensorSample> _aggregationLog = new();

        /// <summary>
        /// Temp storage for items next for aggregation
        /// </summary>
        private readonly List<SensorSample> _itemsToAggregate = new();

        /// <summary>
        /// Last aggregatin date
        /// </summary>
        private DateTime aggregationTime = DateTime.MinValue;

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="sample"> New sample </param>
        public SensorLog(SensorSample sample)
        {
            _log.Add(sample);

            aggregationTime = new DateTime(sample.CreatedAt.Year, sample.CreatedAt.Month, sample.CreatedAt.Day, sample.CreatedAt.Hour, sample.CreatedAt.Minute, 0);

            _itemsToAggregate.Add(sample);
        }

        /// <summary>
        /// Add new sample to the storage
        /// </summary>
        /// <param name="info"> Sample info </param>
        /// <param name="aggregationDuration"> Aggregation period (minutes) </param>
        public void AddSample(SensorSample @info, int aggregationDuration)
        {
            _log.Add(@info);
            if (info.CreatedAt > aggregationTime.AddMinutes(aggregationDuration))
            {
                _aggregationLog.Add(aggregationTime, new AggregatedSensorSample(aggregationTime, _itemsToAggregate));
                _itemsToAggregate.Clear();

                aggregationTime = new DateTime(info.CreatedAt.Year, info.CreatedAt.Month, info.CreatedAt.Day, info.CreatedAt.Hour, info.CreatedAt.Minute, 0);
            }

            _itemsToAggregate.Add(info);
        }

        /// <summary>
        /// Get full list of samples
        /// </summary>
        /// <returns> List of samples </returns>
        public List<SensorSample> GetFullLog()
        {
            return _log;
        }

        /// <summary>
        /// Get aggregated samples
        /// </summary>
        /// <returns> List of aggregated samples </returns>
        public List<AggregatedSensorSample> GetAverageLog()
        {
            return _aggregationLog.Values.ToList();
        }

        /// <summary>
        /// Get aggregated samples for specified period
        /// </summary>
        /// <param name="startDate"> Start date </param>
        /// <param name="duration"> Duration in minutes </param>
        /// <returns> List of aggregated samples </returns>
        public AggregatedSensorSample GetAverageLog(DateTime startDate, int duration)
        {
            var endDate = startDate.AddMinutes(duration);
            var items = _log.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate).ToList();

            return new AggregatedSensorSample(startDate, items);
        }
    }
}
