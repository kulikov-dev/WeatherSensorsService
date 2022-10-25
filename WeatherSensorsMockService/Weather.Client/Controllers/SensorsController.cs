using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Weather.Client.Models;
using Weather.Data;

namespace Weather.Client.Controllers
{
    /// <summary>
    /// Controller of sensors
    /// </summary>
    [Route("sensors")]
    public class SensorsController : Controller
    {
        /// <summary>
        /// Sensors storage
        /// </summary>
        private readonly ISensorsStorage _storage;

        /// <summary>
        /// Sensors subscription storage
        /// </summary>
        private readonly ISubscriptionStorage _subscriptions;

        /// <summary>
        /// GRPC client
        /// </summary>
        private readonly Emulator.Generator.GeneratorClient _generatorClient;

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="storage"> Sensors storage </param>
        /// <param name="generatorClient"> GRPC client </param>
        /// <param name="subscriptions"> Sensors subscription storage </param>
        public SensorsController(ISensorsStorage storage, Emulator.Generator.GeneratorClient generatorClient, ISubscriptionStorage subscriptions)
        {
            _storage = storage;
            _generatorClient = generatorClient;
            _subscriptions = subscriptions;
        }

        /// <summary>
        /// Subscribe to a sensor notifications
        /// </summary>
        /// <param name="sensorId"> Sensor identifier </param>
        /// <returns> Action result </returns>
        /// <remarks>//url/sensors/subscribe/id</remarks>
        [HttpGet("subscribe/{sensorId:long}")]
        public async Task<ActionResult> SubscribeToSensor(long sensorId)
        {
            if (_subscriptions.Contains(sensorId))
            {
                return Ok();
            }

            var request = new Emulator.GetSensorsRequest();
            var events = await _generatorClient.GetSensorsAsync(request);
            var contains = false;

            foreach (var item in events.Result)
            {
                if (item.Id == sensorId)
                {
                    contains = true;

                    break;
                }
            }

            if (events.Result.Count == 0 || !contains)
            {
                return NotFound(sensorId);
            }

            _subscriptions.Subscribe(sensorId);
            return Ok();
        }

        /// <summary>
        /// Subscribe to all sensors notifications
        /// </summary>
        /// <returns> Action result </returns>
        /// <remarks>//url/sensors/subscribe/</remarks>
        [HttpGet("subscribe")]
        public async Task<ActionResult> SubscribeAll()
        {
            var request = new Emulator.GetSensorsRequest();
            var events = await _generatorClient.GetSensorsAsync(request);

            foreach (var item in events.Result)
            {
                _subscriptions.Subscribe(item.Id);
            }

            return Ok();
        }

        /// <summary>
        /// Unsubscribe from a sensor notifications
        /// </summary>
        /// <param name="sensorId"> Sensor identifier </param>
        /// <returns> Action result </returns>
        /// <remarks>//url/sensors/unsubscribe/id</remarks>
        [HttpGet("unsubscribe/{sensorId:long}")]
        public async Task<ActionResult> UnsubscribeFromSensor(long sensorId)
        {
            if (!_subscriptions.Contains(sensorId))
            {
                return NotFound(sensorId);
            }

            await Task.Factory.StartNew(() =>
            {
                _subscriptions.Unsubscribe(sensorId);
            });

            return Ok();
        }

        /// <summary>
        /// Unsubscribe from all notifications
        /// </summary>
        /// <returns> Action result </returns>
        /// <remarks>//url/sensors/unsubscribe/</remarks>
        [HttpGet("unsubscribe")]
        public async Task<ActionResult> UnsubscribeAll()
        {
            await Task.Factory.StartNew(() =>
            {
                _subscriptions.UnsubscribeAll();
            });

            return Ok();
        }

        /// <summary>
        /// Get list of all logs from all sensors
        /// </summary>
        /// <returns> List of sensors samples </returns>
        /// <remarks>//url/sensors/log</remarks>
        [HttpGet("log")]
        public async Task<ActionResult<SensorSample>> GetFullLog()
        {
            var result = await Task.Factory.StartNew(() =>
            {
                return _storage.GetFullLog();
            });

            return Ok(result);
        }

        /// <summary>
        /// Get list of logs from a sensor
        /// </summary>
        /// <param name="sensorId"> Sensor identifier </param>
        /// <returns> List of sensor samples </returns>
        /// <remarks>//url/sensors/log/sensorId</remarks>
        [HttpGet("log/{sensorId:long}")]
        public async Task<ActionResult<SensorSample>> GetFullLogBySensor(long sensorId)
        {
            var result = await Task.Factory.StartNew(() =>
            {
                return _storage.GetFullLogBySensor(sensorId);
            });

            return Ok(result);
        }

        /// <summary>
        /// Get aggregated data from a sensor
        /// </summary>
        /// <param name="sensorId"> Sensor identifier </param>
        /// <returns> List of aggregated samples </returns>
        /// <remarks>//url/sensors/average/sensorId</remarks>
        [HttpGet("average/{sensorId:long}")]
        public async Task<ActionResult<AggregatedSensorSample>> GetAggregatedData(long sensorId)
        {
            var result = await Task.Factory.StartNew(() =>
            {
                return _storage.GetAggregatedLogBySensor(sensorId);
            });

            return Ok(result);
        }

        /// <summary>
        /// Get aggregated data from a sensor for period
        /// </summary>
        /// <param name="sensorId"> Sensor identifier </param>
        /// <param name="startDate"> Start data (UTC) </param>
        /// <param name="duration"> Duration in minutes </param>
        /// <returns> List of aggregated for data samples </returns>
        /// <remarks>//url/sensors/average/sensorId/06-07-2022 19:47/10</remarks>
        [HttpGet("average/{sensorId:long}/{startDate}/{duration:int}")]
        public async Task<ActionResult<AggregatedSensorSample>> GetAggregatedDataByInterval(long sensorId, string startDate, int duration)
        {
            if (!DateTime.TryParse(startDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return ValidationProblem("Wrong data format");
            }

            date = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
            var result = await Task.Factory.StartNew(() =>
            {
                return _storage.GetAggregatedLogBySensor(sensorId, date, duration);
            });

            return Ok(result);
        }
    }
}
