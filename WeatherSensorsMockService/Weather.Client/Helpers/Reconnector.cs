using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Weather.Client.Helpers
{
    /// <summary>
    /// Simple reconnection service
    /// </summary>
    public static class Reconnector
    {
        /// <summary>
        /// Default retry amount
        /// </summary>
        public const int DefaultRetryCount = 3;

        /// <summary>
        /// Default delay before retry (sec)
        /// </summary>
        public const int DefaultRetryDelay = 10;

        /// <summary>
        /// Reconnection processor
        /// </summary>
        /// <typeparam name="T"> Return type </typeparam>
        /// <param name="func"> Executed function </param>
        /// <param name="logger"> Logger </param>
        /// <param name="retryCount"> Retry amount </param>
        /// <param name="retryDelay"> Delay before retry (sec) </param>
        /// <returns> Func return </returns>
        public static async Task<T> DoWithRetryAsync<T>(
          Func<Task<T>> func,
          ILogger logger,
          int retryCount = DefaultRetryCount,
          int retryDelay = DefaultRetryDelay)
        {
            for (int i = 0; i < retryCount + 1; ++i)
            {
                try
                {
                    await func();
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "No connection with a GRPC service. Reconnection...");
                    System.Threading.Thread.Sleep(retryDelay * 1000);
                }
            }
            return default;
        }

    }
}