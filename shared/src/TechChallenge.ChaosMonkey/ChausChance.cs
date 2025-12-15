using System;

namespace TechChallenge.ChaosMonkey;

public record struct ChausChance
{
    public ChausChance(double probability)
    {
        Probability = probability;
    }

    public static ChausChance FromPercentage(double percentage)
    {
        if (percentage is < 0.0 or > 100.0)
        {
            throw new ArgumentOutOfRangeException(nameof(percentage));
        }

        var chance = new ChausChance(percentage / 100.0);

        return chance;
    }

    public double Probability { get; set; }

    public readonly void Deconstruct(out double Probability)
    {
        Probability = this.Probability;
    }
}