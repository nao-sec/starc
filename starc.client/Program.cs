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
            Thread.Sleep(1000 * 10);

            // Connect OpenVPN
            Console.WriteLine("[+] Connecting VPN...");
            Directory.SetCurrentDirectory(@"E:\");
            var psi = new ProcessStartInfo()
            {
                FileName = "openvpn.exe",
                Arguments = "vpn.ovpn",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            var ps = Process.Start(psi);

            Thread.Sleep(1000 * 10);

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

            // Wait
            Thread.Sleep(1000 * 10);

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
                var sw = new StreamWriter(@"E:\file_temp.txt", true, Encoding.UTF8);
                sw.WriteLine(output);
                sw.Close();
                sw.Dispose();
                DirectoryCopy(temp_dir, @"E:\temp");
            }

            var downloads_dir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
            Directory.SetCurrentDirectory(downloads_dir);
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
            output = ps.StandardOutput.ReadToEnd();
            is_executable = output.Contains("executable");
            if (is_executable)
            {
                var sw = new StreamWriter(@"E:\file_downloads.txt", true, Encoding.UTF8);
                sw.WriteLine(output);
                sw.Close();
                sw.Dispose();
            }
            DirectoryCopy(downloads_dir, @"E:\downloads");

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

        static void DirectoryCopy(string src, string dest)
        {
            var src_dir = new DirectoryInfo(src);
            var dest_dir = new DirectoryInfo(dest);

            if (dest_dir.Exists == false)
            {
                dest_dir.Create();
                dest_dir.Attributes = src_dir.Attributes;
            }
            foreach (var file_info in src_dir.GetFiles())
            {
                file_info.CopyTo(dest_dir.FullName + @"\" + file_info.Name, true);
            }
            foreach (System.IO.DirectoryInfo directoryInfo in src_dir.GetDirectories())
            {
                DirectoryCopy(directoryInfo.FullName, dest_dir.FullName + @"\" + directoryInfo.Name);
            }
        }
    }
}
