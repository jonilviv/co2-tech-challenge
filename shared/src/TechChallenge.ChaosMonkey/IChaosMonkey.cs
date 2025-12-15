using System.Threading.Tasks;

namespace TechChallenge.ChaosMonkey;

public interface IChaosMonkey
{
    ValueTask UnleashChaos();
}