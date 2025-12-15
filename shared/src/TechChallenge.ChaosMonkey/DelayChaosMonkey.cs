using SecurityDriven.Core;
using System;
using System.Threading.Tasks;

namespace TechChallenge.ChaosMonkey;

public class DelayChaosMonkey : BaseChaosMonkey
{
    private readonly TimeSpan _delay;

    public DelayChaosMonkey(TimeSpan delay, ChausChance chaosChance, CryptoRandom cryptoRandom) : base(chaosChance, cryptoRandom)
    {
        _delay = delay;
    }

    protected override async ValueTask DoChaos() => await Task.Delay(_delay).ConfigureAwait(false);
}