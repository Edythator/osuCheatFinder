using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace osuAnticheat___2020_07_06
{
    class Program
    {
        public static string osuExeMD5 = "";
        static void Main(string[] args)
        {
            int cheatFactor = 0;
            MD5 md5 = MD5.Create();
            string username = "";
            string path = "";
            Stopwatch timer = new Stopwatch();

            Console.WriteLine("Made by Edythator 2020 loul");

            //check if appdata/local/osu! exists
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "osu!")))
            {
                Console.WriteLine("What is your osu! path?");
                path = Console.ReadLine();
            }
            else
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "osu!");

            timer.Start();
            //get username from osu!.user.cfg
            if (Directory.Exists(path))
            {
                osuExeMD5 = BitConverter.ToString(md5.ComputeHash(File.ReadAllBytes(path + @"\osu!.exe"))).Replace("-", "").ToLower();
                foreach (string s in File.ReadAllLines(Path.Combine(path, "osu!." + Environment.UserName + ".cfg")))
                    if (s.StartsWith("Username"))
                    {
                        username = s.Split('=')[1].Substring(1);
                        if (username.Trim() != "")
                            Variables.InfoMessage("Got username " + username);
                        break;
                    }
            }

            else
            {
                Console.WriteLine("The osu! directory path does not exist or could not be accessed. Are you sure that you've entered the right path or that you have sufficient permissions? Exiting...");
                Console.ReadLine();
                return;
            }

            #region osu! build checking
            JSON.GetBuildProperties(out Dictionary<string, JSON> buildFiles, out string osuBuild);

            if (osuBuild == "stable")
                Variables.WarningMessage("Detected osu!fallback b" + buildFiles["osu!.exe"].Timestamp.Split(' ')[0].Replace("-", ""));
            else
                Variables.InfoMessage("Detected osu!" + osuBuild.Replace("40", "") + " b" + buildFiles["osu!.exe"].Timestamp.Split(' ')[0].Replace("-", ""));

            if (buildFiles.Count == 0)
            {
                Console.WriteLine("Your osu! seems to be either out of or date or modified. Please update your osu! client. Exiting...");
                Console.ReadLine();
                return;
            }
            #endregion

            #region osu! component hash checking
            foreach (string file in buildFiles.Keys)
            {
                string currentFile = BitConverter.ToString(md5.ComputeHash(File.ReadAllBytes(Path.Combine(path, file)))).Replace("-", "").ToLower();

                if (currentFile == buildFiles[file].FileMD5)
                    if (file.Contains("osu!auth"))
                        Variables.WarningMessage("Detected osu! anticheat");
                    else 
                        Variables.InfoMessage("Calculated file " + file + " with MD5: " + currentFile);

                else Variables.WarningMessage("File " + file + " does not equate to the correct hash.");
            }
            #endregion

            #region checking known dlls that get loaded in osu! root dir
            foreach (string s in KnownDLLs.DLLs)
                if (File.Exists(Path.Combine(path, s)))
                {
                    Variables.ErrorMessage("Detected " + s + " in the osu! root directory");
                    cheatFactor += 10;
                }
            #endregion

            #region checking for known cheating tools
            Variables.HashScanning(cheatFactor, md5);
            #endregion

            timer.Stop();
            Console.WriteLine("Done. Took " + timer.Elapsed.TotalSeconds + "s.");
            Console.ReadLine();
        }
    }
}
