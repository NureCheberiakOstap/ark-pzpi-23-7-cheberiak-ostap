namespace SportTournaments.Api.Services;

public static class AuthTokenService
{
    private static readonly Dictionary<string, Guid> _tokens = new();

    public static void Add(string token, Guid userId)
        => _tokens[token] = userId;

    public static bool TryGetUserId(string token, out Guid userId)
        => _tokens.TryGetValue(token, out userId);

    public static void Remove(string token)
        => _tokens.Remove(token);
}
