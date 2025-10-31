using System;

public class BattleLoop
{
    private readonly Pokemon p1;
    private readonly Pokemon p2;
    private readonly int baseDamageP1;
    private readonly int baseDamageP2;

    public BattleLoop(Pokemon first, Pokemon second, int baseDamageFirst, int baseDamageSecond)
    {
        p1 = first;
        p2 = second;
        baseDamageP1 = Math.Max(1, baseDamageFirst);
        baseDamageP2 = Math.Max(1, baseDamageSecond);
    }

    public void Run()
    {
        int turn = 1;
        Console.WriteLine("====Debut du combat====");

        while (!p1.IsKO && !p2.IsKO)
        {
            Console.WriteLine($"\n— Tour {turn} —");

            if (!p1.IsKO && !p2.IsKO)
                p1.Attack(p2, baseDamageP1);

            if (!p1.IsKO && !p2.IsKO)
                p2.Attack(p1, baseDamageP2);

            turn++;
        }

        var winner = p1.IsKO ? p2 : p1;
        Console.WriteLine($"\n=== Vainqueur : {winner.Name} ({winner.Type}) ===");
    }
}