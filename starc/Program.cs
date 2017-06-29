using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace starc
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 1)
            {
                Console.WriteLine("[!] Invalid argument");
                return;
            }

            var vm_name = "starc";
            var snapshot_name = "setuped";
            var shared_folder_name = "share";

            var url = args[0];
            Console.WriteLine("[+] URL: " + url);

            // Set shared folder
            Console.WriteLine("[+] Setting shared folder...");
            var dt = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var drive = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));
            var folder = drive + @"starc.log\" + dt;
            Directory.CreateDirectory(folder);
            var psi = new ProcessStartInfo()
            {
                FileName = "VBoxManage.exe",
                Arguments = "sharedfolder add \"" + vm_name + "\" --name \"" + shared_folder_name + "\" --hostpath \"" + folder + "\" --automount",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            var ps = Process.Start(psi);
            ps.WaitForExit();
            if(ps.ExitCode < 0)
            {
                // Error
                Console.WriteLine("[!] Error, can't set shared folder");
            }

            // Copy OpenVPN config file -> shared file
            // vpn.ovpn, ca.crt, client.crt, client.key
            File.Copy(drive + @"starc.log\config\vpn.ovpn", folder + @"\vpn.ovpn", true);
            File.Copy(drive + @"starc.log\config\ca.crt", folder + @"\ca.crt", true);
            File.Copy(drive + @"starc.log\config\client.crt", folder + @"\client.crt", true);
            File.Copy(drive + @"starc.log\config\client.key", folder + @"\client.key", true);

            // Write URL -> shared file
            var filename = folder + @"\url.txt";
            var sw = new StreamWriter(filename, false, Encoding.UTF8);
            sw.Write(url);
            sw.Close();
            sw.Dispose();

            // Start Guest
            Console.WriteLine("[+] Starting Guest VM...");
            psi = new ProcessStartInfo()
            {
                FileName = "VBoxManage.exe",
                Arguments = "startvm " + vm_name,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            ps = Process.Start(psi);
            ps.WaitForExit();
            if (ps.ExitCode < 0)
            {
                // Error
                Console.WriteLine("[!] Error, can't start Guest VM");
            }

            // Waiting Guest Process
            var end_flag = 0;
            var wait_time = 0;
            while(end_flag < 3)
            {
                // Check VM Status
                psi = new ProcessStartInfo()
                {
                    FileName = "VBoxManage.exe",
                    Arguments = "list runningvms",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                ps = Process.Start(psi);
                ps.WaitForExit();
                var output = ps.StandardOutput.ReadToEnd();
                if(!output.Contains(vm_name))
                {
                    end_flag++;
                }
                Thread.Sleep(1000);
                wait_time++;
                if(wait_time > 10 * 60)
                {
                    end_flag = 3;

                    // shutdown
                    psi = new ProcessStartInfo()
                    {
                        FileName = "VBoxManage.exe",
                        Arguments = "controlvm poweroff",
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };
                    ps = Process.Start(psi);
                    ps.WaitForExit();
                }
            }

            // Wait
            Thread.Sleep(1000);

            // Restore VM
            Console.WriteLine("[+] Restoring VM...");
            psi = new ProcessStartInfo()
            {
                FileName = "VBoxManage.exe",
                Arguments = "snapshot " + vm_name + " restore " + snapshot_name,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            ps = Process.Start(psi);
            ps.WaitForExit();
            if (ps.ExitCode < 0)
            {
                // Error
                Console.WriteLine("[!] Error, can't restore VM");
            }

            // Delete OpenVPN config file
            File.Delete(folder + @"\vpn.ovpn");
            File.Delete(folder + @"\ca.crt");
            File.Delete(folder + @"\client.crt");
            File.Delete(folder + @"\client.key");

            // Commit files
            Console.WriteLine("[+] Committing files...");
            Directory.SetCurrentDirectory(drive + "starc.log");
            psi = new ProcessStartInfo()
            {
                FileName = "git",
                Arguments = "add *",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            ps = Process.Start(psi);
            ps.WaitForExit();
            psi = new ProcessStartInfo()
            {
                FileName = "git",
                Arguments = "commit -a -m \"" + dt + " " + url + "\"",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            ps = Process.Start(psi);
            ps.WaitForExit();
            psi = new ProcessStartInfo()
            {
                FileName = "git",
                Arguments = "push",
                CreateNoWindow = true,
                UseShellExecute = false
            };
            ps = Process.Start(psi);
            ps.WaitForExit();
            ps.WaitForExit();
            if (ps.ExitCode < 0)
            {
                // Error
                Console.WriteLine("[!] Error, can't commit files");
            }

            Console.WriteLine("[!] End all tasks!");
            return;
        }
    }
}
