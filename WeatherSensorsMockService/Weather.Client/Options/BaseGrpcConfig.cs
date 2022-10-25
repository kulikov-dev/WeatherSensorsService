using System;

namespace Weather.Client.Options
{
    /// <summary>
    /// GRPC configuration
    /// </summary>
    public class BaseGrpcConfig
    {
        /// <summary>
        /// Path
        /// </summary>
        public string Url { get; set; } = null;

        /// <summary>
        /// Max sending message size (bytes)
        /// </summary>
        public int SendMessageSize { get; set; } = 1024;

        /// <summary>
        /// Max recieving message size (bytes)
        /// </summary>
        public int ReceiveMessageSize { get; set; } = 1024;

        /// <summary>
        /// Get config section path
        /// </summary>
        /// <returns> Configuration </returns>
        /// <exception cref="ArgumentNullException"> Not initialized GRPC config </exception>
        public virtual string GetSectionPath()
        {
            throw new ArgumentNullException("You have to initialize config first.");
        }
    }
}
