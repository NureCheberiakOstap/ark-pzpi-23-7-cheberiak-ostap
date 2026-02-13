using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public sealed class ApiClient
{
    private readonly HttpClient _http;
    private string? _token;

    public ApiClient(string baseUrl, int timeoutSec)
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/"),
            Timeout = TimeSpan.FromSeconds(timeoutSec)
        };
    }

    public void SetToken(string token)
    {
        _token = token;
    }

    private HttpRequestMessage NewRequest(HttpMethod method, string path, object? body = null)
    {
        var req = new HttpRequestMessage(method, path);

        if (!string.IsNullOrWhiteSpace(_token))
            req.Headers.Add("X-Auth-Token", _token);

        if (body != null)
        {
            var json = JsonSerializer.Serialize(body);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return req;
    }

    private static async Task<T> ReadJsonAsync<T>(HttpResponseMessage resp)
    {
        var text = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
            throw new InvalidOperationException($"HTTP {(int)resp.StatusCode}: {text}");

        var obj = JsonSerializer.Deserialize<T>(text, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (obj == null) throw new InvalidOperationException("Failed to parse response.");
        return obj;
    }

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        var req = NewRequest(HttpMethod.Post, "api/auth/login", new LoginRequest { Email = email, Password = password });
        var resp = await _http.SendAsync(req);
        return await ReadJsonAsync<AuthResponse>(resp);
    }

    public async Task<List<TournamentListItem>> GetTournamentsAsync()
    {
        var req = NewRequest(HttpMethod.Get, "api/tournaments");
        var resp = await _http.SendAsync(req);
        return await ReadJsonAsync<List<TournamentListItem>>(resp);
    }

    public async Task<List<MatchListItem>> GetMatchesAsync(string tournamentId)
    {
        var req = NewRequest(HttpMethod.Get, $"api/tournaments/{tournamentId}/matches");
        var resp = await _http.SendAsync(req);
        return await ReadJsonAsync<List<MatchListItem>>(resp);
    }

    public async Task EnterResultAsync(string matchId, int home, int away)
    {
        var body = new CreateMatchResultRequest { HomeScore = home, AwayScore = away };
        var req = NewRequest(HttpMethod.Post, $"api/matches/{matchId}/result", body);
        var resp = await _http.SendAsync(req);

        if (!resp.IsSuccessStatusCode)
        {
            var text = await resp.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"HTTP {(int)resp.StatusCode}: {text}");
        }
    }

    public async Task<List<StandingsRow>> GetStandingsAsync(string tournamentId)
    {
        var req = NewRequest(HttpMethod.Get, $"api/tournaments/{tournamentId}/standings");
        var resp = await _http.SendAsync(req);
        return await ReadJsonAsync<List<StandingsRow>>(resp);
    }
}
