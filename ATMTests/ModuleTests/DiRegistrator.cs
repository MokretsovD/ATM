using ATM;
using ATM.Card;
using ATM.Cash;
using ATM.HostProcessor;
using ATM.HostProcessor.Mock;
using System;
using Unity;

namespace ATMTests.ModuleTests
{
    public class DiRegistrator : IDisposable
    {
        private static DiRegistrator instance;

        private static object syncRoot = new object();
        private readonly IUnityContainer container;
        protected DiRegistrator()
        {
            container = new UnityContainer();

            container.RegisterType<IATMachine, ATMachine>();
            container.RegisterType<ICashProcessor, CashProcessor>();
            container.RegisterType<ICardProcessor, CardProcessor>();
            container.RegisterType<IHostProcessorService, HostProcessorServiceMock>();
            container.RegisterType<IHistoryManager, HistoryManager>();
        }

        public static DiRegistrator Register()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new DiRegistrator();
                }
            }
            return instance;
        }

        
        public T Resolve<T>()
        {
            return container.Resolve<T>();
        }
        
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    container.Dispose();
                }                
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
