using System.Globalization;

static string ReadNonEmpty(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        var s = Console.ReadLine()?.Trim();
        if (!string.IsNullOrWhiteSpace(s)) return s;
    }
}

static int ReadInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        var s = Console.ReadLine()?.Trim();
        if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
            return v;

        Console.WriteLine("Invalid number. Try again.");
    }
}

var settings = IoTSettings.Load("iotsettings.json");
var api = new ApiClient(settings.ApiBaseUrl, settings.TimeoutSec);
var queue = new OfflineQueue(settings.QueueFilePath);

Console.WriteLine("=== Smart Scoreboard (IoT Client) ===");
Console.WriteLine($"API: {settings.ApiBaseUrl}");

AuthResponse auth;
try
{
    auth = await api.LoginAsync(settings.Email, settings.Password);
    api.SetToken(auth.Token);
    Console.WriteLine($"Logged in: {auth.Email} ({auth.Role}), token={auth.Token[..8]}...");
}
catch (Exception ex)
{
    Console.WriteLine($"Login failed: {ex.Message}");
    return;
}

if (settings.OfflineQueueEnabled)
{
    var pending = queue.LoadAll();
    if (pending.Count > 0)
    {
        Console.WriteLine($"[QUEUE] Pending results: {pending.Count}. Try to send now? (y/n)");
        var ans = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (ans == "y" || ans == "yes")
        {
            var sent = 0;
            foreach (var item in pending.ToList())
            {
                try
                {
                    BusinessRules.ValidateScore(item.HomeScore, item.AwayScore, settings);
                    await api.EnterResultAsync(item.MatchId, item.HomeScore, item.AwayScore);
                    sent++;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[QUEUE] Failed to send match={item.MatchId[..8]}... : {e.Message}");
                }
            }

            if (sent == pending.Count)
            {
                queue.Clear();
                Console.WriteLine("[QUEUE] All sent. Queue cleared.");
            }
            else
            {
                Console.WriteLine("[QUEUE] Some items were not sent. Queue kept.");
            }
        }
    }
}

string tournamentId = settings.TournamentId;
if (string.IsNullOrWhiteSpace(tournamentId))
{
    var tournaments = await api.GetTournamentsAsync();
    if (tournaments.Count == 0)
    {
        Console.WriteLine("No tournaments found.");
        return;
    }

    Console.WriteLine("Tournaments:");
    for (int i = 0; i < tournaments.Count; i++)
        Console.WriteLine($"{i + 1}. {tournaments[i].Name}  [{tournaments[i].Status}]  id={tournaments[i].Id}");

    var idx = ReadInt("Choose tournament (number): ") - 1;
    if (idx < 0 || idx >= tournaments.Count)
    {
        Console.WriteLine("Invalid choice.");
        return;
    }
    tournamentId = tournaments[idx].Id;
}

Console.WriteLine($"Selected TournamentId={tournamentId}");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1) List matches");
    Console.WriteLine("2) Enter match result");
    Console.WriteLine("3) Show standings");
    Console.WriteLine("0) Exit");
    var cmd = ReadInt("Command: ");

    if (cmd == 0) break;

    if (cmd == 1)
    {
        var matches = await api.GetMatchesAsync(tournamentId);
        Console.WriteLine($"Matches: {matches.Count}");
        foreach (var m in matches)
        {
            Console.WriteLine($"- id={m.Id} round={m.Round} status={m.Status} home={m.HomeTeamId[..8]} away={m.AwayTeamId[..8]}");
        }
    }
    else if (cmd == 2)
    {
        var matchId = ReadNonEmpty("MatchId (GUID): ");
        var home = ReadInt("HomeScore: ");
        var away = ReadInt("AwayScore: ");

        try
        {
            BusinessRules.ValidateScore(home, away, settings);
            var (hp, ap, gd) = BusinessRules.ComputePoints(home, away, settings);

            Console.WriteLine($"[LOCAL] goalDiff={gd}, points: home={hp}, away={ap}");
            Console.Write("Send to server? (y/n): ");
            var ans = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (ans != "y" && ans != "yes") continue;

            await api.EnterResultAsync(matchId, home, away);
            Console.WriteLine("[OK] Result sent. Match should become 'finished'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");

            if (settings.OfflineQueueEnabled)
            {
                Console.Write("Save to offline queue? (y/n): ");
                var ans = Console.ReadLine()?.Trim().ToLowerInvariant();
                if (ans == "y" || ans == "yes")
                {
                    queue.Enqueue(new QueuedResult
                    {
                        TournamentId = tournamentId,
                        MatchId = matchId,
                        HomeScore = home,
                        AwayScore = away
                    });
                    Console.WriteLine("[QUEUE] Saved.");
                }
            }
        }
    }
    else if (cmd == 3)
    {
        var rows = await api.GetStandingsAsync(tournamentId);
        Console.WriteLine("Standings:");
        foreach (var r in rows)
        {
            Console.WriteLine($"{r.Points,3} pts | {r.TeamName} | P:{r.Played} W:{r.Wins} D:{r.Draws} L:{r.Losses} GD:{r.GoalDiff}");
        }
    }
}
