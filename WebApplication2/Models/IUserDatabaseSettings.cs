namespace WebApplication2.Models
{
    public interface IUserDatabaseSettings
    {
        string NftUsersCollectionName { get; set; }
        string LeaderBoardCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
