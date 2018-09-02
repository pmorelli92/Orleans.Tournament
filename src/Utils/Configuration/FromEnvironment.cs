using Microsoft.Extensions.Configuration;
using Snaelro.Utils.Extensions;

namespace Snaelro.Utils.Configuration
{
    public class FromEnvironment
    {
        private readonly IConfiguration _configuration;

        private FromEnvironment(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static FromEnvironment Build(IConfiguration configuration)
            => new FromEnvironment(configuration);

        public int SiloPort
            => _configuration.GetEnvVarAsInt("SILO_PORT");

        public int GatewayPort
            => _configuration.GetEnvVarAsInt("GATEWAY_PORT");

        public string ClusterId
            => _configuration.GetEnvVar("CLUSTER_ID");

        public string ServiceId
            => _configuration.GetEnvVar("SERVICE_ID");

        public string PostgresInvariant
            => "Npgsql";

        public string PostgresConnection
            => _configuration.GetEnvVar("POSTGRES_CONNECTION");

        public string BuildVersion
            => _configuration.GetEnvVar("BUILD_VERSION");
    }
}