namespace Scheduling.Reservation.Models;

public class AwsConfigurationSettings
{
    public const string SettingsKey = @"AWS";
    public string DatabaseSecretArn { get; set; }
    public bool Enabled { get; set; }
    public string SecretManagerEndpoint { get; set; }
}
