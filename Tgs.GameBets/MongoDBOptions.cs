namespace Tgs.GameBets;

public record MongoDBOptions
{
	public string ConnectionString { get; set; } = "mongodb://localhost";
}
