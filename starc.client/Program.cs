using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace starc.client
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(1000 * 30);

            // Connect OpenVPN
            Console.WriteLine("[+] Connecting VPN...");
            var user_dir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            Directory.SetCurrentDirectory(user_dir + @"\OpenVPN\config");
            var psi = new ProcessStartInfo()
            {
                FileName = "openvpn.exe",
                Arguments = "vpn.ovpn",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            var ps = Process.Start(psi);
            Thread.Sleep(1000 * 30);

            // Start Capture
            Console.WriteLine("[+] Starting packet capture...");
            Directory.SetCurrentDirectory(@"E:\");
            psi = new ProcessStartInfo()
            {
                FileName = "tshark.exe",
                Arguments = @"-i 2 -w traffic.pcap -a duration:300",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            ps = Process.Start(psi);

            // Access by IE
            Console.WriteLine("[+] Accessing by IE...");
            var sr = new StreamReader(@"url.txt", Encoding.UTF8);
            var url = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            Process.Start(url);

            // Wait
            Thread.Sleep(1000 * 60 * 5);

            // Screenshot
            Console.WriteLine("[+] Taking screenshot...");
            var rect = Screen.PrimaryScreen.Bounds;
            var bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
            }
            bmp.Save(@"screenshot.png", ImageFormat.Png);

            // Check exist exec
            Console.WriteLine("[+] Checking exist executable file...");
            var temp_dir = Path.GetTempPath();
            Directory.SetCurrentDirectory(temp_dir);
            psi = new ProcessStartInfo()
            {
                FileName = "file.exe",
                Arguments = "*",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            ps = Process.Start(psi);
            ps.WaitForExit();
            var output = ps.StandardOutput.ReadToEnd();
            var is_executable = output.Contains("executable");
            if(is_executable)
            {
                var sw = new StreamWriter(@"E:\file.txt", true, Encoding.UTF8);
                sw.WriteLine(output);
                sw.Close();
                sw.Dispose();
            }

            // Shutdown
            Console.WriteLine("[!] End all task!");
            psi = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = "/c shutdown /s /t 0",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            ps = Process.Start(psi);

            return;
        }
    }
}
