using NAudio.Wave;

namespace TeslaLightShow.Helpers;

public static class AudioHelper
{
    public static string ConvertMp3ToWav(string mp3File)
    {
        string wavFile = Path.ChangeExtension(mp3File, ".wav");
        Mp3FileReader reader = new(mp3File);
        WaveFileWriter.CreateWaveFile(wavFile, reader);
        return wavFile;
    }
}
