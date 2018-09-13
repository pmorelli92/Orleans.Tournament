namespace Snaelro.Projections.Options
{
    public class Postgres
    {
        public string ConnectionString { get; }

        public Postgres(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}