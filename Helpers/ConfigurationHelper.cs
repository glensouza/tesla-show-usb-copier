namespace TeslaLightShow.Helpers;

public class ConfigurationHelper
{
    private readonly AppSettings appSettings;

    public ConfigurationHelper(IConfiguration config)
    {
        this.appSettings = new AppSettings
        {
            SourceFolder = config.GetValue<string>("SourceFolder"),
            DestinationDrive = config.GetValue<string>("DestinationDrive"),
            FormatDrive = config.GetValue<bool>("FormatDrive"), // format every drive?
            ConvertWav = config.GetValue<bool>("ConvertWav") // prefer WAV file over MP3?
        };
    }

    public void ResetConfigFile()
    {
        string newAppSettings = this.appSettings.ToJson();
        File.WriteAllText("appsettings.json", newAppSettings);
    }

    public AppSettings GetValues()
    {
        return this.appSettings;
    }

    public AppSettings SetConvertWav(bool convertWav)
    {
        this.appSettings.ConvertWav = convertWav;
        return this.appSettings;
    }

    public AppSettings SetDestinationFolder(string destinationDrive)
    {
        this.appSettings.DestinationDrive = destinationDrive;
        return this.appSettings;
    }

    public AppSettings SetFormatDrive(bool formatDrive)
    {
        this.appSettings.FormatDrive = formatDrive;
        return this.appSettings;
    }

    public AppSettings SetSourceFolder(string sourceFolder)
    {
        this.appSettings.SourceFolder = sourceFolder;
        return this.appSettings;
    }
}
