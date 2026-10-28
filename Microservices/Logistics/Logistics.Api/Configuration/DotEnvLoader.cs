using System;
using System.IO;

namespace Logistics.Api.Configuration
{
    public static class DotEnvLoader
    {
        public static void Load()
        {
            var envPath = FindEnvFile();
            if (envPath == null || !File.Exists(envPath))
            {
                return;
            }

            foreach (var line in File.ReadAllLines(envPath))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                {
                    continue;
                }

                var separatorIndex = trimmed.IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                var key = trimmed.Substring(0, separatorIndex).Trim();
                var value = trimmed.Substring(separatorIndex + 1).Trim().Trim('"', '\'');

                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
                {
                    Environment.SetEnvironmentVariable(key, value);
                }
            }
        }

        private static string? FindEnvFile()
        {
            // 1. Search upwards from Current Directory
            var current = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (current != null)
            {
                var candidate = Path.Combine(current.FullName, ".env");
                if (File.Exists(candidate))
                {
                    return candidate;
                }
                current = current.Parent;
            }

            // 2. Search upwards from BaseDirectory (bin/Debug/...)
            var baseDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (baseDir != null)
            {
                var candidate = Path.Combine(baseDir.FullName, ".env");
                if (File.Exists(candidate))
                {
                    return candidate;
                }
                baseDir = baseDir.Parent;
            }

            return null;
        }
    }
}
