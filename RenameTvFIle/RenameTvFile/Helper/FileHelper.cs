using System.Text.RegularExpressions;

namespace RenameTvFile.Helper;

public class FileHelper
{
    public static async Task ProcessFileAsync(string filePath, Dictionary<(string Title, string Year), string> episodeMap, string rootDir, string seriesTitle)
    {
        // Note: method is async to match original signature (no awaits inside but preserved for parity)
        string fileName = Path.GetFileName(filePath);

        Console.WriteLine($"Processing: {fileName}");

        // Extract year from (YYYY)
        var yearMatch = Regex.Match(fileName, @"\((\d{4})\)");
        if (!yearMatch.Success)
        {
            Console.WriteLine("  [SKIP] No year found.");
            return;
        }
        string year = yearMatch.Groups[1].Value;

        // Extract title (remove extension and year)
        string extension = Path.GetExtension(fileName);
        string title = Regex.Replace(fileName, @"\.\w+$", "").Replace($" ({year})", "").Trim();

        Console.WriteLine($"  Title: {title} | Year: {year}");

        // Lookup in map (year is season)
        string episodeNumber = null;
        if (episodeMap.TryGetValue((title, year), out string epNum))
        {
            episodeNumber = epNum.PadLeft(2, '0');
            Console.WriteLine($"  TVDB: S{year}E{episodeNumber}");
        }
        else
        {
            Console.WriteLine("  [TVDB] Episode not found. Using original name.");
        }

        // Sonarr convention: "Series Title - S{season}E{episode} - Episode Title.ext"
        string newName = fileName;
        if (!string.IsNullOrEmpty(episodeNumber))
        {
            newName = $"{seriesTitle} - S{year}E{episodeNumber} - {title}{extension}";
        }

        // Create year folder at root and move
        string yearFolder = Path.Combine(rootDir, year);
        if (!Directory.Exists(yearFolder))
        {
            Directory.CreateDirectory(yearFolder);
        }

        string destPath = Path.Combine(yearFolder, newName);
        if (File.Exists(destPath))
        {
            destPath = Path.Combine(yearFolder, Path.GetFileNameWithoutExtension(newName) + "_CONFLICT" + extension);
        }

        try
        {
            File.Move(filePath, destPath);
            Console.WriteLine($"  Moved to: {year}\\{newName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  [ERROR] Move failed: {ex.Message}");
        }

        Console.WriteLine();
    }
}
