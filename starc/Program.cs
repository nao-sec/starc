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
            var dt = DateTime.Now;
            var dt_str = dt.ToString();
            dt_str = dt_str.Replace(" ", "_").Replace("/", "-").Replace(":", "-");
            var drive = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));
            var folder = drive + @"starc.log\" + dt_str;
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

            // Write URL -> shared file
            var filename = folder + "\\url.txt";
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
            var is_end = false;
            var wait_time = 0;
            while(!is_end)
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
                    is_end = true;
                }
                Thread.Sleep(1000);
                wait_time++;
                if(wait_time > 10 * 60)
                {
                    is_end = true;

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
                Arguments = "commit -a -m \"" + dt_str + " " + url + "\"",
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

            Console.WriteLine("[!] End all task!");
            return;
        }
    }
}
