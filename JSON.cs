using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace osuAnticheat___2020_07_06
{
    class JSON
    {
        [JsonPropertyName("file_version")]
        public string FileVersion { get; set; }

        [JsonPropertyName("filename")]
        public string FileName { get; set; }

        [JsonPropertyName("file_hash")]
        public string FileMD5 { get; set; }

        [JsonPropertyName("filesize")]
        public string FileSize { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("patch_id")]
        public string PatchID { get; set; }

        [JsonPropertyName("url_full")]
        public string FileURL { get; set; }

        public static void GetBuildProperties(out Dictionary<string, JSON> buildFiles, out string osuBuild)
        {
            osuBuild = "";
            buildFiles = new Dictionary<string, JSON>();
            string[] buildNames = { "stable", "stable40", "beta40", "cuttingedge" };
            string jsonBuffer = "";

            foreach (string build in buildNames)
            {
                string buffer = Variables.wc.DownloadString("https://osu.ppy.sh/web/check-updates.php?action=check&stream=" + build).Replace(@"\", "");
                if (buffer.Contains("invalid"))
                    continue;

                string hash = Regex.Match(buffer, "(e\",\"file_hash\":\".*?\")").Value.Split(':')[1].Replace("\"", "");
                if (hash == Program.osuExeMD5)
                {
                    osuBuild = build;
                    jsonBuffer = buffer;
                    break;
                }
            }

            foreach (Match m in Regex.Matches(jsonBuffer, "({.*?})"))
            {
                JSON deserialized = JsonSerializer.Deserialize<JSON>(m.Value);
                buildFiles.Add(deserialized.FileName, deserialized);
            }
        }
    }
}
