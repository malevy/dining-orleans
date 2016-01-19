using System.Threading.Tasks;
using Orleans;

namespace dpo_interfaces
{
    public interface IChopstick : IGrainWithIntegerKey
    {
        Task<bool> Take(int philosopher);
        Task Return(int philosopher);
    }
}