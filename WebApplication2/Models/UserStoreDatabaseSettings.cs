namespace WebApplication2.Models
{
    public class UserStoreDatabaseSettings : IUserDatabaseSettings
    {
        public string NftUsersCollectionName { get ; set ; } =String.Empty;
        public string LeaderBoardCollectionName { get; set; } =String.Empty;  
        public string ConnectionString { get; set; } =String.Empty;
        public string DatabaseName { get; set ; } = String.Empty;
    }
}
