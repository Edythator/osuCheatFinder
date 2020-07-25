using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace osuAnticheat___2020_07_06
{
    class Variables
    {
        public static WebClient wc = new WebClient();
        public static void ErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ATTENTION] " + message);
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }
        public static void WarningMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("[WARNING] " + message);
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        public static void InfoMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[INFO] " + message);
        }

        /*public static List<string> GetFiles(string path)
        {
            List<string> allFiles = new List<string>();

            foreach (string files in Directory.GetFiles(path))
                allFiles.Add(files);
            foreach (string directory in Directory.GetDirectories(path))
                GetFiles(directory);
            return allFiles;
        }*/
        private static void AddFiles(string path, IList<string> files)
        {
            try
            {
                Directory.GetFiles(path)
                    .ToList()
                    .ForEach(s => files.Add(s));

                Directory.GetDirectories(path)
                    .ToList()
                    .ForEach(s => AddFiles(s, files));
            }
            catch { }
        }

        public static (string name, long fileSize, string hash, short cheatFactor)[] hashes =
        {
                ("osu!fallback AQN Timewarp", 1316864, "66358ea5527ec37954b49828b79fa68e", 10),
                ("Extreme Injector v3", 1981952, "c4394fb4daaf350cdbf5303d812e917e", 2),
                ("aqn!ownage", 10240, "67c3d2b15d3b6f47dceced5b6bd1c0ad", 10),
                ("Old AQN Launcher", 2409984, "bb28ed8b5793b7da59e480646502ebd5", 10),
                ("AutoDisconnector", 353280, "cb3b64f144f4168ec49e9434daa2047e", 8),
                ("BLAC Playcount Increaser", 9169408, "c2b5393afa08b7f29d240bba40866009", 10),
                ("keybosu", 3332096, "545c231372df93c7cd61c1d24bd46913", 10),
                ("nosue replay swag", 84480, "d8f805718c357662790933be4120898a", 10),
                ("osu! client unblock", 9728, "74b63920e9b8b807c105fd4a5c4f929f", 10),
                ("osu! console v1.1", 14336, "643f73c1600aa50375400f580a5a7718", 10),
                ("osu! hack cleaner", 6656, "f923a33b30f82506b2b055689426e56e", 10),
                ("osu! hack launcher", 145408, "2d08ac5dc1ad87db10a34577c3d778c0", 10),
                ("osu! ID finder", 47104, "8fc06e9d6115706f07216018eb8bf580", 6),
                ("osu! key input changer", 67072, "830567c623c200fefedf428023bd1d87", 10),
                ("osu!cracker", 11776, "aea7a719ed4af45a7206732e4191a0eb", 10),
                ("osu!fallback AQN Auto", 1460224, "e37a4f953448af364a9ac8b940d11edb", 10),
                ("osu!fallback AQN Catch", 175616, "5a9e448896a959aa21cefbeda264b7b4", 10),
                ("osu!fallback AQN Maniac v3.48", 1486848, "3afe69640f82d1307b7347771e72d3f9", 10),
                ("osu!fallback AQN Timewarp", 1316864, "66358ea5527ec37954b49828b79fa68e", 10),
                ("osu!ftw", 1287680, "5bf719c1fba65601829a5b18cf277255", 10),
                ("osu!replaybot (AndreySeVen) console", 8192, "2e9c7a8b59e9c3ce651dd737b0d72883", 10),
                ("osu!replaybot (AndreySeVen)", 973824, "7b783120dec35303d16e159c2c95a2fc", 10),
                ("osu!smoked", 110080, "d10d3ea4d87d973903f63a5c2e36beb1", 5),
                ("osu!stealer", 10752, "79c8a8e1ee7c9ef115a4288abd1b51e7", 10),
                ("osu!unblock", 6144, "a7b093de05afaef3f1a3811153e3b68c", 5),
                ("OsuIdentification", 4846592, "9fcc565c4a0c5659e9a561fb9720fa77", 7),
                ("ReplayReader", 88576, "21103142af312b3e555f2f62129c30a0", 10),
                ("ReplaySwag", 16900096, "770982c3dbe29b7d206bb8c3b6de43c8", 10),
                ("Spinbot", 26112, "10084b1fc2828cf8fce69624959525b4", 10),
                ("West Beginning", 4590080, "8780793bd13d80137889d80c5d0bdaaa", 10),
                ("West End 2", 29640, "74f55586d3bf751d75210cc813a1c2fa", 10),
                ("West End", 1820160, "6e81e552b227f6b282c22059a8ca2e33", 10),
                ("Cheat Engine 7.1", 384416, "cff55136c6f7251e51d99b3b2fe17f4f", 2),
                ("AQN Loader", 6571024, "9daf697f7ab7d93c76bddfcb9d6effa4f", 50)
        };

        public static void HashScanning(int cheatFactor, MD5 md5)
        {
            List<string> files = new List<string>();
            AddFiles(@"C:\Users\" + Environment.UserName, files);

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                for (int i = 0; i < hashes.Length; i++)
                {
                    if (fileInfo.Length != hashes[i].fileSize)
                        continue;

                    try
                    {
                        string hash = BitConverter.ToString(md5.ComputeHash(File.ReadAllBytes(file))).Replace("-", "").ToLower();
                        if (hash == hashes[i].hash)
                        {
                            if (hashes[i].cheatFactor >= 10)
                                ErrorMessage("Detected " + hashes[i].name);
                            else
                                WarningMessage("Detected " + hashes[i].name);

                            cheatFactor += hashes[i].cheatFactor;
                        }
                    }
                    catch { }
                }
            }
        }
    }
}
