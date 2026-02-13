using System.Text.Json;

public sealed class OfflineQueue
{
    private readonly string _path;

    public OfflineQueue(string path)
    {
        _path = path;
    }

    public void Enqueue(QueuedResult item)
    {
        var list = LoadAll();
        list.Add(item);
        SaveAll(list);
    }

    public List<QueuedResult> LoadAll()
    {
        if (!File.Exists(_path)) return new List<QueuedResult>();

        var json = File.ReadAllText(_path);
        var list = JsonSerializer.Deserialize<List<QueuedResult>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return list ?? new List<QueuedResult>();
    }

    public void Clear()
    {
        if (File.Exists(_path)) File.Delete(_path);
    }

    private void SaveAll(List<QueuedResult> list)
    {
        var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_path, json);
    }
}

public sealed class QueuedResult
{
    public string TournamentId { get; set; } = "";
    public string MatchId { get; set; } = "";
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public string CreatedAtUtc { get; set; } = DateTime.UtcNow.ToString("O");
}
