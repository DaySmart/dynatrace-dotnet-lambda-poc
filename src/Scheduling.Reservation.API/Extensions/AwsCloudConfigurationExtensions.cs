using Amazon.SecretsManager;
using Microsoft.Extensions.Options;
using Scheduling.Reservation.Models;

namespace Scheduling.Reservation.Extensions;

public static class AwsCloudConfigurationExtenzsions
{
    public static void AddAwsSpecificDependencies(this IServiceCollection services)
    {
        Console.WriteLine($"enter -> {nameof(AddAwsSpecificDependencies)}");

        services.AddSingleton<IAmazonSecretsManager>(provider =>
        {
            var logger = provider.GetService<ILogger<Program>>();
            logger.LogDebug($"enter -> services.AddSingleton<IAmazonSecretsManager>");

            logger.LogDebug("acquiring IOptions<AwsConfigurationSettings>");
            var configuration = provider.GetService<IOptions<AwsConfigurationSettings>>()?.Value;
            logger.LogDebug("IOptions<AwsConfigurationSettings> acquired");
            try
            {
                var config = new AmazonSecretsManagerConfig { ServiceURL = configuration?.SecretManagerEndpoint };

                logger.LogDebug($"Initializing AmazonSecretsManagerClient for {config}");
                var secretsManager = new AmazonSecretsManagerClient(config);
                logger.LogDebug("AmazonSecretsManagerClient initialized.");

                logger.LogDebug("Returning AmazonSecretsManagerClient");
                return secretsManager;
            }
            catch (Exception exception)
            {
                logger.LogError("Unable to initialize AmazonSecretsManagerClient -- fucking thanks amazon.", exception);
                throw;
            }

        });
    }
}
