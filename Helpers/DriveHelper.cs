namespace TeslaLightShow.Helpers;

public class DriveHelper
{
    private string destinationDrive;

    public DriveHelper(string destinationDrive)
    {
        this.destinationDrive = destinationDrive;
    }

    public string BytesToString(long byteCount)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
        if (byteCount == 0)
        {
            return "0" + suf[0];
        }

        long bytes = Math.Abs(byteCount);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
        string bytesToString = (Math.Sign(byteCount) * num) + suf[place];
        return bytesToString;
    }

    /*
        Deletes all that can be deleted
        Returns false if some files remain in folder
        It deals with
        - Readonly files
        - Deletion delay
        - Locked files
        It doesn't use Directory.Delete because the process is aborted on exception.
    */
    public bool ClearDrive(string pathName = "")
    {
        bool errors = false;
        DirectoryInfo directory = new(string.IsNullOrEmpty(pathName) ? this.destinationDrive : pathName);

        foreach (FileInfo file in directory.EnumerateFiles())
        {
            try
            {
                file.IsReadOnly = false;
                file.Delete();
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.Message);
                errors = true;
            }

            // Wait for the item to disappear (avoid 'dir not empty' error).
            while (file.Exists)
            {
                Thread.Sleep(10);
                file.Refresh();
            }
        }

        foreach (DirectoryInfo subDirectory in directory.EnumerateDirectories())
        {
            try
            {
                this.ClearDrive(subDirectory.FullName);
                subDirectory.Delete();
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.Message);
                errors = true;
            }

            //Wait for the item to disappear (avoid 'dir not empty' error).
            while (subDirectory.Exists)
            {
                Thread.Sleep(10);
                subDirectory.Refresh();
            }
        }

        return !errors;
    }

    public bool EjectDrive()
    {
        // TODO: Implement
        return false;
    }

    public bool FormatUsb(string fileSystem = "FAT32", bool quickFormat = true, int clusterSize = 4096, string label = "TesLight", bool enableCompression = false)
    {
        //verify conditions for the letter format: driveLetter[0] must be letter. driveLetter[1] must be ":" and all the characters mustn't be more than 2
        if (this.destinationDrive.Length != 2 || this.destinationDrive[1] != ':' || !char.IsLetter(this.destinationDrive[0]) || !Directory.Exists(this.destinationDrive))
        {
            return false;
        }

        foreach (string file in Directory.GetFiles(this.destinationDrive))
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

        foreach (string directory in Directory.GetDirectories(this.destinationDrive))
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

        string[] files = Directory.GetFiles(this.destinationDrive);
        string[] directories = Directory.GetDirectories(this.destinationDrive);

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

        ManagementObjectSearcher searcher = new (@"select * from Win32_Volume WHERE DriveLetter = '" + this.destinationDrive + "'");
        foreach (ManagementBaseObject? o in searcher.Get())
        {
            ManagementObject? vi = (ManagementObject)o;
            if (vi == null)
            {
                continue;
            }

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

    public void SetNewDestinationDrive(string newDestinationDrive)
    {
        this.destinationDrive = newDestinationDrive;
    }
}
