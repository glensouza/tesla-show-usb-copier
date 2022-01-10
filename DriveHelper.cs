using System.Management;

namespace TeslaLightShow;

public class DriveHelper
{
    public bool FormatUsb(string driveLetter, string fileSystem = "FAT32", bool quickFormat = true, int clusterSize = 4096, string label = "TesLight", bool enableCompression = false)
    {
        //verify conditions for the letter format: driveLetter[0] must be letter. driveLetter[1] must be ":" and all the characters mustn't be more than 2
        if (driveLetter.Length != 2 || driveLetter[1] != ':' || !char.IsLetter(driveLetter[0]) || !Directory.Exists(driveLetter))
        {
            return false;
        }

        //query and format given drive 
        //best option is to use ManagementObjectSearcher

        foreach (string file in Directory.GetFiles(driveLetter))
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        foreach (string directory in Directory.GetDirectories(driveLetter))
        {
            foreach (string file in Directory.GetFiles(directory))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        string[] files = Directory.GetFiles(driveLetter);
        string[] directories = Directory.GetDirectories(driveLetter);

        foreach (string item in files)
        {
            try
            {
                File.Delete(item);
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException) { }
        }

        foreach (string item in directories)
        {
            try
            {
                Directory.Delete(item);
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException) { }
        }

        ManagementObjectSearcher searcher = new (@"select * from Win32_Volume WHERE DriveLetter = '" + driveLetter + "'");
        foreach (ManagementObject vi in searcher.Get())
        {
            try
            {
                bool completed = false;
                ManagementOperationObserver watcher = new();

                watcher.Completed += (sender, args) =>
                {
                    Console.WriteLine("USB format completed " + args.Status);
                    completed = true;
                };
                watcher.Progress += (sender, args) => { Console.WriteLine("USB format in progress " + args.Current); };

                vi.InvokeMethod(watcher, "Format", new object[] { fileSystem, quickFormat, clusterSize, label, enableCompression });

                while (!completed)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return true;
    }
}
