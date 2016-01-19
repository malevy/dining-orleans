using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using dpo_interfaces;

namespace dining_philosophers_orleans
{
    public class Philosopher
    {
        private static readonly RNGCryptoServiceProvider _RandomProvider = new RNGCryptoServiceProvider();
        private readonly int _id;
        private readonly IChopstick _leftChopstick;
        private readonly IChopstick _rightChopstick;

        public Philosopher(int id, IChopstick leftChopstick, IChopstick rightChopstick)
        {
            _id = id;
            _leftChopstick = leftChopstick;
            _rightChopstick = rightChopstick;
        }

        public async void EatThink(int allowedBites)
        {
            int bitesRemaining = allowedBites;
            while (bitesRemaining > 0)
            {
                Console.WriteLine("Philosopher {0} is thinking", this._id);
                Thread.Sleep(GetRandomWaitTime());

                if (!(await _leftChopstick.Take(this._id))) continue;
                if (!(await _rightChopstick.Take(this._id)))
                {
                    await _leftChopstick.Return(this._id);
                    continue;
                }

                Console.WriteLine("Philosopher {0} is eating (nom nom nom)", this._id);
                bitesRemaining--;
                Thread.Sleep(GetRandomWaitTime());

                await Task.WhenAll(_leftChopstick.Return(this._id), _rightChopstick.Return(this._id)) ;
            }

            Console.WriteLine("Philosopher {0} is full", this._id);
        }

        private static int GetRandomWaitTime()
        {
            byte[] components = new byte[2];
            _RandomProvider.GetBytes(components);
            var asInt = (int) BitConverter.ToInt16(components, 0);
            if (asInt < 0) asInt = -asInt;
            return asInt;
        }
    }
}