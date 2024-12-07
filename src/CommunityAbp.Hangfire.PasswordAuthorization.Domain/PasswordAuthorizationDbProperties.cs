namespace CommunityAbp.Hangfire.PasswordAuthorization;

public static class PasswordAuthorizationDbProperties
{
    public static string DbTablePrefix { get; set; } = "PasswordAuthorization";

    public static string? DbSchema { get; set; } = null;

    public const string ConnectionStringName = "PasswordAuthorization";
}
