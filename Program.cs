using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;

namespace LogFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var catalogForScan = @"c:\";
            var pathExportDir = @"c:\ExportedLogs\";
            var pathArchives = $@"{pathExportDir}archives\";
            var pathErrLog = $@"{pathExportDir}log\";
            var filesMasks = new List<string> { ".log" };
                        
            if (!Directory.Exists(pathArchives)) Directory.CreateDirectory(pathArchives);
            if (!Directory.Exists(pathErrLog)) Directory.CreateDirectory(pathErrLog);

            Console.WriteLine($"Scan catalog: \"{catalogForScan}\"");
            Console.WriteLine($"Dest catalog:\"{pathArchives}\"");
            Console.WriteLine($"Log catalog:\"{pathErrLog}\"\n\r");

            ScanSource(catalogForScan);

            Console.WriteLine();
            Console.WriteLine($"Scan catalog: {catalogForScan} ended.");

            void ScanSource(string ds)
            {
                try
                {
                    foreach (string d in Directory.GetDirectories(ds))
                    {
                        try
                        {
                            if (d.EndsWith("log") || d.EndsWith("logs"))
                            {
                                ZipLogDirs(d);
                                Console.Write("d");
                                continue;
                            }
                            else
                            {
                                foreach (string f in Directory.GetFiles(d, "*.*"))
                                {
                                    foreach (var v in filesMasks)
                                    {
                                        if (Path.GetFileName(f).ToLower().EndsWith(v))
                                        {
                                            ZipLogFiles(f);
                                            Console.Write("+");
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            PrintErr(ex.Message);
                        }
                        ScanSource(d);
                    }
                }
                catch { }
            }

            void ZipLogFiles(string fileName)
            {
                using (var zf = new ZipFile())
                {
                    zf.AddFile(fileName);
                    var tmp = $@"{pathArchives}{fileName.Replace("\\", "~").Replace(":", "")}.zip";
                    try
                    {
                        zf.Save(tmp);
                    }
                    catch (Exception ex)
                    {
                        PrintErr(ex.Message);
                    }
                }
            }

            void ZipLogDirs(string dirName)
            {
                using (var zf = new ZipFile())
                {
                    zf.AddDirectory(dirName);
                    var tmp = $@"{pathArchives}{dirName.Replace("\\", "~").Replace(":", "")}.zip";
                    try
                    {
                        zf.Save(tmp);
                    }
                    catch (Exception ex)
                    {
                        PrintErr(ex.Message);
                    }
                }
            }
          
            void PrintErr(string exMsg)
            {
                using (var sw = new StreamWriter($@"{pathErrLog}error.log", true))
                {
                    sw.WriteLine($"{exMsg}");
                }                
            }
        }
    }
}

