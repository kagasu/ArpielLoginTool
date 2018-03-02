using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArpielLoginTool
{
    class Program
    {
        static long GetTimeStamp() => (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

        static async Task<string> Login(string id, string password)
        {
            var client = new HttpClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "txtPortalID", id },
                { "txtPortalPW", password }
            });

            var res = await client.PostAsync("https://customer.gamecom.jp/Login/Login.aspx", content);

            var str = await client.GetStringAsync($"http://arpiel.gamecom.jp/game/start/?_={GetTimeStamp()}");
            return str.Replace("\"", "").Replace("\\", "");
        }

        static string GetWeMadeLauncherPath()
        {
            var sb = new StringBuilder();
            var keyName = (IntPtr.Size == 4) ?
                @"HKEY_LOCAL_MACHINE\SOFTWARE\GamecomStarterApp" :
                @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\GamecomStarterApp";

            sb.Append((string)Registry.GetValue(keyName, "LauncherPath", null));
            sb.Append("\\");
            sb.Append((string)Registry.GetValue(keyName, "LauncherFile", null));

            return sb.ToString();
        }

        static async Task Main(string[] args)
        {
            var launcherArguments = await Login(args[0], args[1]);
            Process.Start(new ProcessStartInfo()
            {
                FileName = GetWeMadeLauncherPath(),
                Arguments = launcherArguments
            });
        }
    }
}
