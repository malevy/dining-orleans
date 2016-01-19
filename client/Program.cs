using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using dpo_interfaces;
using Orleans;
using Orleans.Runtime.Host;

namespace dining_philosophers_orleans
{
    class Program
    {
        private static SiloHost _host;

        static void Main(string[] args)
        {
            var hostDomain = AppDomain.CreateDomain("simple-host", null, new AppDomainSetup
            {
                AppDomainInitializer = InitSilo,
                AppDomainInitializerArguments = args
            });

            GrainClient.Initialize("ClientConfig.xml");

            const int philosopherCount = 5;

            var chopsticks = Enumerable.Range(0, philosopherCount)
                .Select(n => GrainClient.GrainFactory.GetGrain<IChopstick>(n))
                .ToArray();

            var philosophers = Enumerable.Range(0, philosopherCount)
                .Select(n => new Philosopher(n, chopsticks[n], chopsticks[(n + 1)%philosopherCount]))
                .ToList();

            var cts = new CancellationTokenSource();

            // let them live!!!
            var tasks = philosophers
                .Select(p => Task.Factory.StartNew(() => p.EatThink(4), cts.Token))
                .ToList();

            Console.WriteLine("\nPress enter to stop");
            Console.ReadLine();

            cts.Cancel();

            hostDomain.DoCallBack(ShutdownSilo);
        }

        private static void ShutdownSilo()
        {
            if (null == _host) return;
            _host.StopOrleansSilo();
            _host.Dispose();
            _host = null;
        }

        private static void InitSilo(string[] args)
        {
            _host = new SiloHost(Dns.GetHostName())
            {
                ConfigFileName = "OrleansConfiguration.xml"
            };
            _host.LoadOrleansConfig();
            _host.InitializeOrleansSilo();
            if (_host.StartOrleansSilo())
            {
                Console.WriteLine($"Started host {_host.Name} as a {_host.Type} node");
            }
            else
            {
                Console.WriteLine($"Failed to start host {_host.Name}");
            }
        }
    }
}
