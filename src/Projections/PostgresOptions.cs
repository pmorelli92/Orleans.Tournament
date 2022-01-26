namespace Orleans.Tournament.Projections
{
    public class PostgresOptions
    {
        public string ConnectionString { get; }

        public PostgresOptions(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}