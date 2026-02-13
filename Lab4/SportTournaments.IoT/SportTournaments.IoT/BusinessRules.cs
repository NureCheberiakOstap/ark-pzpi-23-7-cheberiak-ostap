public static class BusinessRules
{
    public static void ValidateScore(int home, int away, IoTSettings s)
    {
        if (home < 0 || away < 0)
            throw new ArgumentException("Score cannot be negative.");

        if (home > s.MaxScore || away > s.MaxScore)
            throw new ArgumentException($"Score too large. MaxScore={s.MaxScore}");

        var diff = Math.Abs(home - away);
        if (diff > s.MaxGoalDiff)
            Console.WriteLine($"[WARN] Big score difference ({diff}). MaxGoalDiff={s.MaxGoalDiff}. Continue carefully.");
    }

    public static (int homePoints, int awayPoints, int goalDiff) ComputePoints(int home, int away, IoTSettings s)
    {
        int homePoints, awayPoints;

        if (home > away)
        {
            homePoints = s.WinPoints;
            awayPoints = s.LosePoints;
        }
        else if (home < away)
        {
            homePoints = s.LosePoints;
            awayPoints = s.WinPoints;
        }
        else
        {
            homePoints = s.DrawPoints;
            awayPoints = s.DrawPoints;
        }

        return (homePoints, awayPoints, home - away);
    }
}
