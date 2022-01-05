using System.Drawing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Colorful;
using Console = Colorful.Console;

using IHost host = Host.CreateDefaultBuilder(args).Build();
IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

FigletFont font = FigletFont.Load("big.flf");
Figlet figlet = new (font);

Console.WriteLine(figlet.ToAscii("Tesla Light Show"), ColorTranslator.FromHtml("#8AFFEF"));

// Get values from the appsettings.json config file
string sourceFolder = config.GetValue<string>("SourceFolder");
string destinationDrive = config.GetValue<string>("DestinationDrive");
Console.WriteWithGradient("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *", Color.Yellow, Color.Fuchsia, 14);
Console.WriteLine();
Console.WriteLine($"Source Folder: {sourceFolder}");
Console.WriteLine($"Destination Drive: {destinationDrive}");
Console.WriteWithGradient("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *", Color.Yellow, Color.Fuchsia, 14);
Console.WriteLine();
Console.WriteLine();

if (!Directory.Exists(sourceFolder))
{
    // TODO: would you like to open/edit appsettings.json?
    Console.WriteLine($"Source Folder {sourceFolder} does not exist. Push [Enter] to end program.");
    return;
}

List<LightShow> lightShows = new();
foreach (string directory in Directory.GetDirectories(sourceFolder))
{
    //Console.WriteWithGradient("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *", Color.Yellow, Color.Fuchsia, 14);
    //Console.WriteLine();
    //Console.WriteLine($"Found directory: {directory}");
    string sequence = null;
    string audio = null;

    foreach (string file in Directory.GetFiles(directory))
    {
        switch (Path.GetExtension(file))
        {
            case ".fseq":
                //Console.WriteLine($"Found sequence file: {file}");
                sequence = file;
                break;
            case ".wav":
                //Console.WriteLine($"Found audio file: {file}");
                audio = file;
                break;
            case ".mp3":
                if (string.IsNullOrEmpty(audio))
                {
                    //Console.WriteLine($"Found audio file: {file}");
                    audio = file;
                }
                break;
        }
    }

    string showName = directory.Split("\\").Last();
    if (string.IsNullOrEmpty(sequence))
    {
        Console.WriteLine($"Missing sequence file for show {showName}, skipping...");
        continue;
    }

    if (string.IsNullOrEmpty(audio))
    {
        Console.WriteLine($"Missing audio file for show {showName}, skipping...");
        continue;
    }

    lightShows.Add(new LightShow
    {
        AudioFile = audio,
        Directory = directory,
        SequenceFile = sequence,
        ShowName = showName
    });
}

Console.WriteWithGradient("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *", Color.Yellow, Color.Fuchsia, 14);
Console.WriteLine();
Console.WriteLine();
if (!lightShows.Any())
{
    Console.WriteLine("No directories found. Push [Enter] to end program.");
    Console.ReadLine();
    return;
}

// TODO: show list of available and get list of how many they want to do
// TODO: confirm drive letter

string destinationFolder = $"{destinationDrive}\\LightShow";
foreach (LightShow lightShow in lightShows)
{
    while (!Directory.Exists(destinationDrive))
    {
        // TODO: would you like to open/edit appsettings.json?
        Console.WriteLine($"Insert media on drive {destinationDrive} and push [Enter].");
        Console.ReadLine();
    }

    foreach (string file in Directory.GetFiles(destinationDrive))
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

    if (Directory.Exists(destinationFolder))
    {
        foreach (string file in Directory.GetFiles(destinationFolder))
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
    else
    {
        Directory.CreateDirectory(destinationFolder);
    }

    Console.WriteLine($"Copying Light Show {lightShow.ShowName}");
    await using (File.Create($"{destinationDrive}\\{lightShow.ShowName}")) {}
    File.Copy(lightShow.AudioFile, $"{destinationFolder}\\lightshow.{Path.GetExtension(lightShow.AudioFile)}");
    File.Copy(lightShow.SequenceFile, $"{destinationFolder}\\lightshow.fseq");
    if (!lightShow.Equals(lightShows.Last()))
    {
        Console.WriteLine($"Insert the next media on drive {destinationDrive} and push [Enter].");
        Console.ReadLine();
    }
}

await host.RunAsync();
