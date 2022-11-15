using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using Scheduling.Domain.Entities;
using Scheduling.Infrastructure.Utils;
using Scheduling.Reservation.Models;

namespace Scheduling.Reservation.Extensions;

public static class RepositoryBootstrapExtensions
{
    private const string PRODUCT_GROUP_CLAIM_NAME = @"ds.product-group";
    // private const string PATH_TO_CA_FILE = "certs/rds-combined-ca-bundle.pem";

    public static void AddRepositoryServices(this IServiceCollection services)
    {
        Console.WriteLine($"enter -> {nameof(AddRepositoryServices)}");

        // Setup database client
        services.AddSingleton<IMongoClient>(provider =>
        {
            var logger = provider.GetService<ILogger<Program>>();
            logger.LogDebug($"enter -> services.AddSingleton<IMongoClient>");

            try
            {
                logger.LogDebug("acquiring databaseConnectionSettings");
                var databaseConnectionSettings = provider.GetService<IOptions<ReservationDatabaseSettings>>()?.Value;

                logger.LogDebug("acquiring connectionString");
                var connectionString = databaseConnectionSettings?.ConnectionString ?? "";
                var obscuredConnectionString = Regex.Replace(connectionString, "(mongodb://[\\w_]+\\:)(.*?)@(.*?)", $"$1***HIDDEN***@");
                logger.LogDebug($"connectionString: {obscuredConnectionString}");
                logger.LogDebug($"initializing MongoClientSettings from: {obscuredConnectionString}");
                var settings = MongoClientSettings.FromConnectionString(connectionString);

                logger.LogDebug("MongoClientSettings initialized");

                logger.LogDebug("Applying additional settings -- AllowInsecureTls, ClusterConfigurator (query logging)");

                // settings.SslSettings.ClientCertificates = certificateCollection;
                settings.AllowInsecureTls = true; // TEMPORARY
                settings.ClusterConfigurator = cb => {
                    cb.Subscribe<CommandStartedEvent>(e => {
                        logger.LogDebug($"{e.CommandName} - {e.Command.ToJson()}");
                    });
                };

                logger.LogDebug("Additional settings applied -- AllowInsecureTls, ClusterConfigurator (query logging)");

                logger.LogDebug("initializing MongoClient");

                var client = new MongoClient(settings);
                logger.LogDebug("Client initialized");

                return client;
            }
            catch (Exception exception)
            {
                var appException = new ApplicationException("Unable to initialize MongoClient", exception);
                logger.LogError(appException.Message, appException);
                throw appException;
            }
        });

        // Setup collection resolver
        services.AddScoped<IMongoCollection<ReservationEntity>>(provider =>
        {
            var logger = provider.GetService<ILogger<Program>>();
            logger.LogDebug($"enter -> services.AddScoped<IMongoCollection<Models.Reservation>>");

            var context = provider.GetService<IHttpContextAccessor>()?.HttpContext;
            logger.LogDebug($"retrieved context");
            
            var productGroup = context?.User.FindFirst(PRODUCT_GROUP_CLAIM_NAME)?.Value ?? "test";
            logger.LogDebug($"productGroup: {productGroup}");
            
            var databaseConnectionSettings = provider.GetService<IOptions<ReservationDatabaseSettings>>()?.Value;
            logger.LogDebug($"retrieved databaseConnectionSettings");
            
            var collectionName = $"{databaseConnectionSettings?.CollectionName}_{productGroup}";
            logger.LogDebug($"collectionName: {collectionName}");
            var mongoDatabase = provider.GetService<IMongoClient>()?.GetDatabase(databaseConnectionSettings?.DatabaseName);
            
            logger.LogDebug($"acquired database");
            var exists = mongoDatabase?.ListCollectionNames().ToList().Any(name => name == collectionName) ?? false;
            logger.LogDebug($"{collectionName} exists? {exists}");
            
            var collection = (exists 
                                 ? mongoDatabase?.GetCollection<ReservationEntity>(collectionName) 
                                 : CollectionManager.CreateReservationCollection(mongoDatabase, collectionName)) ??
                             throw new InvalidOperationException("Where my database collection?! :sadface:");

            logger.LogDebug($"returning collection {collectionName}");
            
            return collection;
        });
    }
}
