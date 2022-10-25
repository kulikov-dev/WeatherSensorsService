using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Weather.Emulator.Mocks;

namespace Weather.Emulator.Controllers
{
    /// <summary>
    /// Sensors controller
    /// </summary>
    [Route("sensors")]
    public class SensorsController : ControllerBase
    {
        /// <summary>
        /// List of available sensors
        /// </summary>
        private readonly List<Data.SensorInfo> _sensors;

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="sensors"> List of available sensors </param>
        public SensorsController(IOptions<List<Data.SensorInfo>> sensors)
        {
            _sensors = sensors.Value;
        }

        /// <summary>
        /// Get current sensors sample
        /// </summary>
        /// <returns> List of current samples </returns>
        /// <remarks>//url/sensors/</remarks>
        [HttpGet]
        public async Task<ActionResult<List<EventResponse>>> GetSensorsInfo()
        {
            var rand = new Random(1042);
            var list = new List<EventResponse>(_sensors.Count);

            for (int i = 0; i < _sensors.Count; i++)
            {
                Data.SensorInfo item = _sensors[i];

                WeatherMock.Generate(ref item);

                var itemResponse = new EventResponse()
                {
                    EventId = rand.Next(10000),
                    SensorInfo = new SensorInfo() { Id = item.Id, Name = item.Name, SensorType = (SensorTypeEnum)(int)item.SensorType },
                    Temperature = item.Temperature,
                    Humidity = item.Humidity,
                    Co2 = item.CO2,
                    CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
                };

                list.Add(itemResponse);
            }

            return Ok(list);
        }
    }
}
