# RenameTvFile

.NET 10 console application for renaming and organizing video files using episode data from TheTVDB.

## Configuration

Set the environment variables in the terminal used to run the application:

| Variable | Description |
| --- | --- |
| `TVDB_API_KEY` | TheTVDB API key (required) |
| `RENAME_TV_SOURCE_DIR` | Folder containing the videos (required) |
| `RENAME_TV_SERIES_ID` | Numeric series ID on TheTVDB (required) |
| `RENAME_TV_SERIES_TITLE` | Series name used in the new filenames (required) |
| `RENAME_TV_RECURSIVE` | `true` or `false`; defaults to `true` |

PowerShell example with placeholder values to replace locally:

```powershell
$env:TVDB_API_KEY = '<your-api-key>'
$env:RENAME_TV_SOURCE_DIR = '<video-folder>'
$env:RENAME_TV_SERIES_ID = '<tvdb-series-id>'
$env:RENAME_TV_SERIES_TITLE = '<series-name>'
$env:RENAME_TV_RECURSIVE = 'true'
dotnet run --project RenameTvFIle/RenameTvFile/RenameTvFile.csproj
```

The application reads environment variables directly; it does not load a `.env` file.

## Example: Looney Tunes

To process **Looney Tunes** videos, use TheTVDB series ID `72514`.

Replace the key and folder below with your local values:

```powershell
$env:TVDB_API_KEY = '<your-api-key>'
$env:RENAME_TV_SOURCE_DIR = '<your-Looney-Tunes-video-folder>'
$env:RENAME_TV_SERIES_ID = '72514'
$env:RENAME_TV_SERIES_TITLE = 'Looney Tunes'
$env:RENAME_TV_RECURSIVE = 'true'
dotnet run --project RenameTvFIle/RenameTvFile/RenameTvFile.csproj
```

A file named `Episode Title (1930).mkv` is matched against season `1930` on TheTVDB. If the episode is recognized with number `N`, it is moved into the `1930` subfolder and renamed to `Looney Tunes - S1930E{N padded to two digits} - Episode Title.mkv`. For example, episode number `1` produces `Looney Tunes - S1930E01 - Episode Title.mkv`. If no episode matches, the file keeps its original name and is moved into the year subfolder.

## Supported Format

Files must be named `Episode Title (YYYY).ext` (`.mkv`, `.mp4`, `.avi`, `.mpeg`).
The current implementation uses the year as the TheTVDB season number, so it is suited to series whose seasons are organized by year.
Videos are moved into a `YYYY` subfolder, and recognized episodes are renamed to
`Series Name - SYYYYENN - Episode Title.ext`.

## Build

```powershell
dotnet build RenameTvFIle/RenameTvFIle.slnx
```
