using System;
using System.Threading.Tasks;
using dpo_interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Orleans;

namespace dining_philosophers_orleans.Grains
{
    public class Chopstick : Grain, IChopstick
    {
        private const int NO_OWNER = -1;

        private int _owner = NO_OWNER;

        public Task<bool> Take(int philosopher)
        {
            if (_owner != NO_OWNER) return Task.FromResult(false);
            _owner = philosopher;
            return Task.FromResult(true);
        }

        public Task Return(int philosopher)
        {
            if (_owner == NO_OWNER) return TaskDone.Done;
            if (_owner != philosopher) throw new InvalidOperationException("chopstick can only be returned by the owner");

            _owner = NO_OWNER;
            return TaskDone.Done;
        }
    }
}