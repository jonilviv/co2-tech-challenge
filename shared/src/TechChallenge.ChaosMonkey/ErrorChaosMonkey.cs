using SecurityDriven.Core;
using System;
using System.Threading.Tasks;

namespace TechChallenge.ChaosMonkey;

public class ErrorChaosMonkey : BaseChaosMonkey
{
    public ErrorChaosMonkey(ChausChance chaosChance, CryptoRandom cryptoRandom) : base(chaosChance, cryptoRandom)
    {
    }

    protected override ValueTask DoChaos() => throw new Exception("Chaos in the Program!");
}