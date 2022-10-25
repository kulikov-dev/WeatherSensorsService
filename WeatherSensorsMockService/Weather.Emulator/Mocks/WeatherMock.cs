using System;

namespace Weather.Emulator.Mocks
{
    /// <summary>
    /// Mock generator
    /// </summary>
    public static class WeatherMock
    {
        /// <summary>
        /// Setup current samples
        /// </summary>
        /// <param name="sensor"> Sensor </param>
        public static void Generate(ref Data.SensorInfo sensor)
        {
            sensor.Temperature = SmoothRandom(sensor.Temperature, 0, 50, 0.2);
            sensor.Humidity = SmoothRandom(sensor.Humidity, 50, 49, 0.1);
            sensor.CO2 = SmoothRandom(sensor.CO2, 750, 400, sensor.SensorType == Data.SensorTypeEnum.Inside ? 2 : 0.5);
        }

        /// <summary>
        /// Generate weather sample
        /// </summary>
        /// <param name="currentValue"> Current value </param>
        /// <param name="average"> Average value </param>
        /// <param name="oscillation"> Oscillation </param>
        /// <param name="variance"> Variance </param>
        /// <returns> New generated value </returns>
        private static double SmoothRandom(double currentValue, double average, double oscillation, double variance)
        {
            var rand = new Random();
            var maxValue = average + oscillation;
            var minValue = average - oscillation;

            var maxLimit = Math.Min(maxValue, currentValue + variance);
            var minLimit = Math.Max(minValue, currentValue - variance);
            var totalVariance = maxLimit - minLimit;

            var newCurrentValue = Math.Round(minLimit + rand.NextDouble() * totalVariance, 1);

            return newCurrentValue;
        }
    }
}
