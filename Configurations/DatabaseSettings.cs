namespace RentApplication.Configurations;

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = "RentApplicationDB";
    public string CollectionName { get; set; } = "Users";
}
