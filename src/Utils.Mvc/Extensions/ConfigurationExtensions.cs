using System;
using Microsoft.Extensions.Configuration;

namespace Snaelro.Utils.Mvc.Extensions
{
    internal static class ConfigurationExtensions
    {
        internal static string GetEnvVar(this IConfiguration @this, string key)
        {
            var envVar = @this[key];

            if (string.IsNullOrEmpty(envVar))
                throw new ArgumentException($"Configuration missing for EnvVar: {key}");

            return envVar;
        }

        internal static int GetEnvVarAsInt(this IConfiguration @this, string key)
        {
            var envVar = @this[key];

            if (string.IsNullOrEmpty(envVar))
                throw new ArgumentException($"Configuration missing for EnvVar: {key}");

            if (!int.TryParse(envVar, out var result))
                throw new InvalidCastException($"Cannot convert {envVar} to integer");

            return result;
        }
    }
}