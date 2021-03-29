using Microsoft.Extensions.Configuration;

namespace CustomerFunc.Azure.Storage.Blob.Send.Client
{
    public static class Helper
    {
        public static string GetCofigurationKeyValue(this IConfigurationRoot config, string key)
        {
            var configSection = config.GetSection(key);
            if (configSection.Exists())
            {
                return configSection.Value;
            }
            return null;
        }
    }
}
