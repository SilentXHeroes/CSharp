using System;
using System.IO;
using System.Windows.Forms;
using Helpers;

namespace FileWriter
{
    public static class WriteLog
    {
        private static bool EnableWrite = false;

        public static void Write(string log)
        {
            try
            {
                if (EnableWrite) Helper.Worker.ReportProgress(5, log.Replace("\r\n", ""));
                File.AppendAllText(Helper.App + @"application\logs\sql.log", log);
            }
            catch(Exception e)
            {
                MessageBox.Show("Une erreur est survenue lors de l'insertion des logs: " + e, "Erreur log", MessageBoxButtons.OK);
            }
        }

        public static void WriteLine(string log)
        {
            Write(log + "\r\n");
        }

        public static void EnableWriteLogs(bool enable)
        {
            EnableWrite = enable;
        }
    }
}
