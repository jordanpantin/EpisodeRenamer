using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace RenameTvFile.Helper;

public static class TvdbHelper
{
    public static async Task<string> LoginAsync(HttpClient httpClient, string apikey)
    {
        string url = "https://api4.thetvdb.com/v4/login";
        var requestContent = new StringContent("{\"apikey\":\"" + apikey + "\"}", Encoding.UTF8, "application/json");

        try
        {
            var response = await httpClient.PostAsync(url, requestContent);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Login failed: {response.StatusCode}");
                return null;
            }

            string json = await response.Content.ReadAsStringAsync();
            JsonNode root = JsonNode.Parse(json);
            return root["data"]["token"].ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            return null;
        }
    }

    public static async Task<Dictionary<(string Title, string Year), string>> FetchAllEpisodesAsync(HttpClient httpClient, string seriesId)
    {
        var map = new Dictionary<(string Title, string Year), string>();
        int page = 0;
        bool hasMore = true;

        while (hasMore)
        {
            string url = $"https://api4.thetvdb.com/v4/series/{seriesId}/episodes/default?page={page}";

            try
            {
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Fetch episodes failed: {response.StatusCode}");
                    break;
                }

                string json = await response.Content.ReadAsStringAsync();
                JsonNode root = JsonNode.Parse(json);

                var episodes = root["data"]["episodes"]?.AsArray();

                if (episodes == null || episodes.Count == 0)
                {
                    hasMore = false;
                    break;
                }

                foreach (var ep in episodes)
                {
                    string epTitle = ep["name"]?.ToString();
                    string epYear = ep["year"]?.ToString();
                    string epNum = ep["number"]?.ToString();
                    string season = ep["seasonNumber"]?.ToString();

                    if (!string.IsNullOrEmpty(epTitle) && !string.IsNullOrEmpty(epNum) && !string.IsNullOrEmpty(season))
                    {
                        map[(epTitle, season)] = epNum;
                    }
                }

                page++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fetch error: {ex.Message}");
                hasMore = false;
            }
        }

        return map;
    }
}
