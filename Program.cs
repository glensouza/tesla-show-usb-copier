using Colorful;
using System.Windows.Forms;
using Console = Colorful.Console;

using IHost host = Host.CreateDefaultBuilder(args).Build();

// Get values from the appsettings.json config file
IConfiguration configuration = host.Services.GetRequiredService<IConfiguration>();
ConfigurationHelper config = new(configuration);
AppSettings appSettings = config.GetValues();

DriveHelper driveHelper = new(appSettings.DestinationDrive ?? string.Empty);
FigletFont font = FigletFont.Load("big.flf");
Figlet figlet = new(font);

ClearConsole();
Console.WriteLine("Current Settings:");
AddConsoleLine();
Console.WriteLine($"  Source Folder: {appSettings.SourceFolder}");
Console.WriteLine($"  Destination Drive: {appSettings.DestinationDrive}");
Console.WriteLine($"  Format Drive? {(appSettings.FormatDrive ?? false ? "yes" : "no")}");
Console.WriteLine($"  Convert MP3 to WAV file? {(appSettings.ConvertWav ?? false ? "yes" : "no")}");
AddConsoleLine();

bool changeSettings = false;
if (!Directory.Exists(appSettings.SourceFolder))
{
    changeSettings = true;
}
else
{
    Console.Write("Would you like to change any of these settings? (y/n) ");
    ConsoleKey changeResponse = 0;
    while (changeResponse != ConsoleKey.Y && changeResponse != ConsoleKey.N)
    {
        changeResponse = Console.ReadKey(true).Key;
    }

    changeSettings = changeResponse == ConsoleKey.Y;
    Console.WriteLine($"{changeResponse.ToString().ToLower()}");
}

if (changeSettings)
{
    bool doneChanging = false;
    while (!doneChanging)
    {
        ClearConsole();
        Console.WriteLine("Current Settings:");
        AddConsoleLine();
        Console.WriteLine($"  1- Source Folder: {appSettings.SourceFolder}");
        Console.WriteLine($"  2- Destination Drive: {appSettings.DestinationDrive}");
        Console.WriteLine($"  3- Format Drive? {(appSettings.FormatDrive ?? false ? "yes" : "no")}");
        Console.WriteLine($"  4- Convert MP3 to WAV file? {(appSettings.ConvertWav ?? false ? "yes" : "no")}");
        if (Directory.Exists(appSettings.SourceFolder))
        {
            Console.WriteLine("  5- Done Changing");
        }

        AddConsoleLine();

        Console.Write("Select Setting to Change: ");
        int changeResponse = 0;
        while (!Enumerable.Range(1,5).Contains(changeResponse))
        {
            char readOption = Console.ReadKey(true).Key.ToString().LastOrDefault();
            _ = int.TryParse(readOption.ToString(), out changeResponse);
        }

        switch (changeResponse)
        {
            case 1:
                Console.WriteLine("1");
                AddConsoleLine();
                Console.WriteLine("Select the new folder location from the dialog");
                AddConsoleLine();

                string folderSelected = string.Empty;
                Thread thread = new(() =>
                {
                    FolderBrowserDialog folderBrowserDialog = new();
                    // ReSharper disable once AccessToModifiedClosure
                    folderBrowserDialog.SelectedPath = appSettings.SourceFolder ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    folderBrowserDialog.Description = "Select a folder";
                    DialogResult result = folderBrowserDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        folderSelected = folderBrowserDialog.SelectedPath;
                    }
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();

                appSettings = config.SetSourceFolder(folderSelected);
                break;
            case 2:
                Console.WriteLine("2");
                AddConsoleLine();

                bool foundDrive = false;
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    if (drive.DriveType == DriveType.Removable && drive.IsReady)
                    {
                        if (!foundDrive)
                        {
                            Console.WriteLine("Available USB drives:");
                        }

                        foundDrive = true;
                        Console.WriteLine($"{drive.Name} {driveHelper.BytesToString(drive.TotalSize)}");
                    }
                }

                if (foundDrive)
                {
                    AddConsoleLine();
                }

                Console.Write("Enter new value for Destination Drive Letter: ");
                char driveLetter = '0';
                while (!char.IsLetter(driveLetter))
                {
                    driveLetter = Console.ReadKey(true).KeyChar;
                }

                appSettings = config.SetDestinationFolder($"{driveLetter.ToString().ToUpper()}:");
                driveHelper.SetNewDestinationDrive(appSettings.DestinationDrive!);
                break;
            case 3:
                Console.WriteLine("3");
                appSettings = config.SetFormatDrive(!appSettings.FormatDrive ?? false);
                break;
            case 4:
                Console.WriteLine("4");
                appSettings = config.SetConvertWav(!appSettings.ConvertWav ?? false);
                break;
            default:
                doneChanging = true;
                break;
        }

        if (doneChanging)
        {
            config.ResetConfigFile();
        }
    }
}

ClearConsole();
Console.WriteLine($"Locating Tesla light shows in source folder {appSettings.SourceFolder}");
AddConsoleLine();
int showNumber = 0;
List<LightShow> lightShows = new();
foreach (string directory in Directory.GetDirectories(appSettings.SourceFolder!))
{
    string sequence = string.Empty;
    string audio = string.Empty;

    foreach (string file in Directory.GetFiles(directory))
    {
        switch (Path.GetExtension(file))
        {
            case ".fseq":
                sequence = file;
                break;
            case ".wav":
                audio = file;
                break;
            case ".mp3":
                if (string.IsNullOrEmpty(audio))
                {
                    audio = file;
                }

                break;
        }
    }

    string showName = directory.Split("\\").Last();
    showNumber++;
    lightShows.Add(new LightShow
    {
        Number = showNumber,
        ShowName = showName,
        SequenceFile = sequence,
        AudioFile = audio,
        Include = false
    });
}

if (!lightShows.Any())
{
    ClearConsole();
    Console.WriteLine($"No Tesla light shows found in directory {appSettings.SourceFolder}.");
    Console.WriteLine("Push [Enter] to end program.");
    Console.ReadLine();
    return;
}

ConsoleTableHelper table = new();
table.SetHeaders("#", "Show Name", "Sequence", "Audio");
foreach (LightShow lightShow in lightShows)
{
    table.AddRow(lightShow.Number.ToString(), lightShow.ShowName, $"[{(string.IsNullOrEmpty(lightShow.SequenceFile) ? "X" : "\u221A")}]", $"[{(string.IsNullOrEmpty(lightShow.AudioFile) ? "X" : "\u221A")}]");
}

ClearConsole();
Console.WriteLine("Found Tesla light shows:");
AddConsoleLine();
Console.WriteLine(table.ToString());
AddConsoleLine();
int usbDrives = 0;
while (!Enumerable.Range(1, lightShows.Count).Contains(usbDrives))
{
    Console.Write("How many USB drives do you have today? ");
    string readOption = Console.ReadLine();
    _ = int.TryParse(readOption, out usbDrives);
    if (usbDrives > lightShows.Count)
    {
        usbDrives = lightShows.Count;
    }
}

table.SetHeaders("#", "Show Name", "Included");
bool doneSelecting = false;
while (!doneSelecting)
{
    while (lightShows.Count(s => s.Include) < usbDrives)
    {
        ClearConsole();
        Console.WriteLine("Available Tesla light shows:");
        AddConsoleLine();
        table.ClearRows();
        foreach (LightShow lightShow in lightShows.Where(s => !string.IsNullOrEmpty(s.SequenceFile) && !string.IsNullOrEmpty(s.AudioFile)))
        {
            table.AddRow(lightShow.Number.ToString(), lightShow.ShowName, $"[{(lightShow.Include ? "\u221A" : (string.IsNullOrEmpty(lightShow.SequenceFile) || string.IsNullOrEmpty(lightShow.AudioFile) ? "X" : " "))}]");
        }

        Console.WriteLine(table.ToString());
        AddConsoleLine();
        int showSelected = 0;
        while (!Enumerable.Range(1, lightShows.Count).Contains(showSelected))
        {
            Console.Write($"Enter Tesla light show number to include ({lightShows.Count(s => s.Include)}/{usbDrives}): ");
            string readOption = Console.ReadLine();
            _ = int.TryParse(readOption, out showSelected);
            LightShow? selectedShow = lightShows.FirstOrDefault(s => s.Number == showSelected && !string.IsNullOrEmpty(s.SequenceFile) && !string.IsNullOrEmpty(s.AudioFile));
            if (selectedShow != null)
            {
                selectedShow.Include = !selectedShow.Include;
            }
        }
    }

    int includeNumber = 0;
    ClearConsole();
    Console.WriteLine("Tesla light shows selected to be included:");
    AddConsoleLine();
    table.SetHeaders("#", "Show Name");
    table.ClearRows();
    foreach (LightShow lightShow in lightShows.Where(s => s.Include))
    {
        table.AddRow((++includeNumber).ToString(), lightShow.ShowName);
    }

    Console.WriteLine(table.ToString());
    AddConsoleLine();

    Console.Write("Is this list correct? (y/n) ");
    ConsoleKey doneResponse = 0;
    while (doneResponse != ConsoleKey.Y && doneResponse != ConsoleKey.N)
    {
        doneResponse = Console.ReadKey(true).Key;
    }

    Console.WriteLine($"{doneResponse.ToString().ToLower()}");
    doneSelecting = doneResponse == ConsoleKey.Y;
    if (!doneSelecting)
    {
        foreach (LightShow lightShow in lightShows.Where(s => s.Include))
        {
            lightShow.Include = false;
        }
    }
}

ClearConsole();
if (appSettings.ConvertWav ?? false)
{
    if (lightShows.Any(s => s.Include && s.AudioFile.ToLower().Contains(".mp3")))
    {
        foreach (LightShow lightShow in lightShows.Where(s => s.Include && s.AudioFile.ToLower().Contains(".mp3")))
        {
            lightShow.AudioFile = AudioHelper.ConvertMp3ToWav(lightShow.AudioFile);
        }

        Console.WriteLine("Converted all MP3 files to WAV");
    }
    else
    {
        Console.WriteLine("No show MP3 files needed to be converted to WAV");
    }

    AddConsoleLine();
}

string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
ClearConsole();
Console.WriteLine("Creating PDF");
using PdfHelper pdfHelper = new(dateTime, lightShows.Where(s => s.Include).Select(s => s.ShowName).ToList());
string pdfPath = pdfHelper.CreatePdf();

string destinationFolder = $"{appSettings.DestinationDrive}\\LightShow";
int lightShowNumber = 0;
foreach (LightShow lightShow in lightShows.Where(s => s.Include))
{
    string driveFormat = string.Empty;
    bool foundDrive = false;
    while (!foundDrive)
    {
        foreach (DriveInfo drive in DriveInfo.GetDrives())
        {
            if (drive.DriveType == DriveType.Removable && drive.IsReady)
            {
                if (drive.Name.ToLower().Contains(appSettings.DestinationDrive?.ToLower() ?? string.Empty))
                {
                    foundDrive = true;
                    Console.WriteLine($"Found the drive for TeslaShow: {drive.Name} {driveHelper.BytesToString(drive.TotalSize)}");
                    driveFormat = drive.DriveFormat;
                }
            }
        }

        // Check if drive already used
        if (File.Exists($"{appSettings.DestinationDrive}\\{dateTime}"))
        {
            Console.WriteLine($"This drive has already been used. Insert another drive {appSettings.DestinationDrive} and push [Enter].");
            Console.ReadLine();
            foundDrive = false;
        }
        else if (!foundDrive)
        {
            Console.WriteLine($"Insert drive {appSettings.DestinationDrive} and push [Enter].");
            Console.ReadLine();
        }
    }

    if ((appSettings.FormatDrive ?? false) || !driveFormat.ToLower().Contains("fat"))
    {
        Console.WriteLine($"Formatting drive {appSettings.DestinationDrive}");
        try
        {
            driveHelper.FormatUsb();
        }
        catch (Exception ex)
        {
            if (driveFormat.ToLower().Contains("fat"))
            {
                driveHelper.ClearDrive();
            }
            else
            {
                Console.WriteLine($"Error while formatting drive {appSettings.DestinationDrive}");
                Console.WriteLine("If you format the drive yourself in FAT32 format and try again.");
                throw new Exception($"Error formatting drive {appSettings.DestinationDrive}", ex);
            }
        }
    }
    else
    {
        driveHelper.ClearDrive();
    }

    Directory.CreateDirectory(destinationFolder);
    Console.WriteLine($"Copying Light Show #{++lightShowNumber}: {lightShow.ShowName}");
    await using (File.Create($"{appSettings.DestinationDrive}\\{dateTime}")) { }
    await using (File.Create($"{appSettings.DestinationDrive}\\{lightShow.ShowName}")) { }
    await using (File.Create($"{appSettings.DestinationDrive}\\{lightShowNumber}")) { }
    File.Copy(lightShow.AudioFile, $"{destinationFolder}\\lightshow{Path.GetExtension(lightShow.AudioFile)}");
    File.Copy(lightShow.SequenceFile, $"{destinationFolder}\\lightshow.fseq");
    if (!string.IsNullOrEmpty(appSettings.DestinationDrive))
    {
        File.Copy(pdfPath, $"{appSettings.DestinationDrive}\\{Path.GetFileName(pdfPath)}");
    }

    bool ejected = driveHelper.EjectDrive();
    if (!ejected)
    {
        Console.WriteLine($"Make sure you eject this {appSettings.DestinationDrive} drive properly before removing it from your computer.");
        Console.WriteLine();
    }

    if (lightShow.Number != lightShows.LastOrDefault(s => s.Include)?.Number)
    {
        Console.WriteLine($"Insert the next media on drive {appSettings.DestinationDrive} and push [Enter].");
        Console.ReadLine();
    }
}

ClearConsole();
Console.WriteLine("All done! Enjoy your Tesla light shows...");

void AddConsoleLine()
{
    Console.WriteWithGradient("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *", Color.Yellow, Color.Fuchsia, 14);
    Console.WriteLine();
}

void ClearConsole()
{
    Console.Clear();
    Console.WriteLine(figlet.ToAscii("Tesla Light Show"), ColorTranslator.FromHtml("#8AFFEF"));
    AddConsoleLine();
}
