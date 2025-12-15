using SecurityDriven.Core;
using System.Threading.Tasks;

namespace TechChallenge.ChaosMonkey;

public abstract class BaseChaosMonkey : IChaosMonkey
{
    private readonly ChausChance _chaosChance;
    private readonly CryptoRandom _cryptoRandom;

    protected BaseChaosMonkey(ChausChance chaosChance, CryptoRandom cryptoRandom)
    {
        _chaosChance = chaosChance;
        _cryptoRandom = cryptoRandom;
    }

    public ValueTask UnleashChaos() =>
        ShouldUnleashChaos()
            ? DoChaos()
            : ValueTask.CompletedTask;

    protected abstract ValueTask DoChaos();

    protected virtual bool ShouldUnleashChaos()
    {
        double randomValue = _cryptoRandom.NextDouble();
        bool shouldUnleashChaos = randomValue < _chaosChance.Probability;

        return shouldUnleashChaos;
    }
}