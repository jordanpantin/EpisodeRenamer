using RenameTvFile.Helper;

// Keep credentials and local paths outside the source code.
string? apiKey = Environment.GetEnvironmentVariable("TVDB_API_KEY");
string? sourceDir = Environment.GetEnvironmentVariable("RENAME_TV_SOURCE_DIR");
string? seriesId = Environment.GetEnvironmentVariable("RENAME_TV_SERIES_ID");
string? seriesTitle = Environment.GetEnvironmentVariable("RENAME_TV_SERIES_TITLE");
string? recursiveSetting = Environment.GetEnvironmentVariable("RENAME_TV_RECURSIVE");
bool recursive = true;

if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(sourceDir)
    || string.IsNullOrWhiteSpace(seriesId) || string.IsNullOrWhiteSpace(seriesTitle))
{
    Console.Error.WriteLine("Set TVDB_API_KEY, RENAME_TV_SOURCE_DIR, RENAME_TV_SERIES_ID and RENAME_TV_SERIES_TITLE before running.");
    Environment.ExitCode = 1;
    return;
}

if (!string.IsNullOrWhiteSpace(recursiveSetting) && !bool.TryParse(recursiveSetting, out recursive))
{
    Console.Error.WriteLine("RENAME_TV_RECURSIVE must be true or false.");
    Environment.ExitCode = 1;
    return;
}

if (!Directory.Exists(sourceDir))
{
    Console.WriteLine($"Directory not found: {sourceDir}");
    Environment.ExitCode = 1;
    return;
}

using var httpClient = new HttpClient();

// Login to get token
string? token = await TvdbHelper.LoginAsync(httpClient, apiKey);
if (string.IsNullOrEmpty(token))
{
    return;
}
httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

// Fetch all episodes once
Dictionary<(string Title, string Year), string> episodeMap = await TvdbHelper.FetchAllEpisodesAsync(httpClient, seriesId);

// Get all video files
var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
var videoExtensions = new[] { ".mkv", ".mp4", ".avi", ".mpeg" };
var files = Directory.EnumerateFiles(sourceDir, "*.*", searchOption)
    .Where(f => videoExtensions.Contains(Path.GetExtension(f).ToLower()));

foreach (var file in files)
{
    await FileHelper.ProcessFileAsync(file, episodeMap, sourceDir, seriesTitle);
}

Console.WriteLine("Processing completed.");
