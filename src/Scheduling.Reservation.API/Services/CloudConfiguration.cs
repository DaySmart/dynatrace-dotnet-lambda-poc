using Amazon.SecretsManager;
using Scheduling.Reservation.Interfaces;

namespace Scheduling.Reservation.Services;

public class AwsCloudConfiguration : ICloudConfiguration
{
    private readonly IAmazonSecretsManager _secretsManager;

    public AwsCloudConfiguration(IAmazonSecretsManager secretsManager)
    {
        _secretsManager = secretsManager;
    }

    public string GetConfigurationValue(string key)
    {
        return GetSecretStringValue(key);
    }

    private string GetSecretStringValue(string arn)
    {
        var response = _secretsManager.GetSecretValueAsync(new()
        {
            SecretId = arn
        }).Result;

        return response.SecretString;
    }
}
