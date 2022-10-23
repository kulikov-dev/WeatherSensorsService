namespace Weather.Client.Options
{
    /// <summary>
    /// Weather service GRPC configuration
    /// </summary>
    public class WeatherGrpcConfig : BaseGrpcConfig
    {
        /// <inheritdoc/>
        public override string GetSectionPath()
        {
            return "ExternalApis:WeatherEmulator";
        }
    }
}
