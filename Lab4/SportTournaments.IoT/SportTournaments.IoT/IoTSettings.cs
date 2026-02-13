using System.Text.Json;

public sealed class IoTSettings
{
    public string ApiBaseUrl { get; set; } = "http://localhost:7050";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string TournamentId { get; set; } = "";

    public int WinPoints { get; set; } = 3;
    public int DrawPoints { get; set; } = 1;
    public int LosePoints { get; set; } = 0;

    public int MaxScore { get; set; } = 99;
    public int MaxGoalDiff { get; set; } = 20;

    public int TimeoutSec { get; set; } = 10;
    public int MaxRetries { get; set; } = 2;

    public bool OfflineQueueEnabled { get; set; } = true;
    public string QueueFilePath { get; set; } = "queue.json";

    public static IoTSettings Load(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Config not found: {path}");

        var json = File.ReadAllText(path);
        var settings = JsonSerializer.Deserialize<IoTSettings>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (settings is null)
            throw new InvalidOperationException("Failed to parse settings.");

        if (string.IsNullOrWhiteSpace(settings.ApiBaseUrl))
            throw new InvalidOperationException("ApiBaseUrl is required.");

        return settings;
    }
}
