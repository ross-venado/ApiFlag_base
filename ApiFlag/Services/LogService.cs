using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace ApiFlag.Services
{
    public class LogService
    {
        public static void WriteLine(string msj)
        {
            try
            {
                var vrlGuardarArchivosProcesados = ConfigurationManager.AppSettings["WRITE_LOG"].ToString();

                if (vrlGuardarArchivosProcesados != "S") { return; }

                var vrlPathFolderLog = string.Format("{0}/{1}", AppDomain.CurrentDomain.BaseDirectory, "/log");
                if (!Directory.Exists(vrlPathFolderLog))
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(vrlPathFolderLog);
                }

                var vrlFechaActual = DateTime.Now.ToString("yyyyMMdd");
                var vrlNombreArchivo = string.Format("{0}/{1}.txt", vrlPathFolderLog, vrlFechaActual);
                using (StreamWriter w = File.AppendText(vrlNombreArchivo))
                {
                    Log(msj, w);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

        }

        public static void Log(string logMessage, TextWriter w)
        {
            var vrlNow = DateTime.Now.ToString();
            w.WriteLine("{0} : {1}", vrlNow, logMessage);
        }
    }
}