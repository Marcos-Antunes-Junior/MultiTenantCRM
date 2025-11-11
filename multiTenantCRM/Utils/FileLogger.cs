using System;
using System.IO;

namespace multiTenantCRM.Utils
{
    public static class FileLogger
    {
        private static readonly string logPath = "tenant_log.txt";

        public static void LogTenantCreation(string tenantName)
        {
            string line = $"{DateTime.Now}: Tenant '{tenantName}' created{Environment.NewLine}";
            File.AppendAllText(logPath, line);
        }

        public static string ReadLogs()
        {
            if (!File.Exists(logPath))
                return "No logs yet.";

            return File.ReadAllText(logPath);
        }
    }
}
