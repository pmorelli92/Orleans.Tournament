using Microsoft.Extensions.Configuration;
using Snaelro.Utils.Mvc.Extensions;

namespace Snaelro.Utils.Mvc.Configuration
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

        public string PubSubStore
            => "PubSubStore";

        public string JwtIssuerKey
            => _configuration.GetEnvVar("JWT_ISSUER_KEY");
    }
}