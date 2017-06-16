using System;
using System.Threading;

namespace Hamsa.Common
{
    public class CodeEngine<T> : ICodeEngine where T : IExecutableCode, new()
    {
        private volatile bool _shouldStop;
        private IExecutableCode instance;

        public void Run()
        {
            instance = new T();

            instance.Setup();

            new Thread(() =>
            {
                while (!_shouldStop)
                {
                    instance.Loop();
                }

                Console.WriteLine("Worker thread: terminating gracefully.");
                instance = null;
            }).Start();
        }

        public void Stop()
        {
            _shouldStop = true;
        }

        #region Dispose

        protected bool m_Disposed = false;

        public void Dispose()
        {
            // If this function is being called the user wants to release the
            // resources. lets call the Dispose which will do this for us.
            Dispose(true);

            // Now since we have done the cleanup already there is nothing left
            // for the Finalizer to do. So lets tell the GC not to call it later.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing == true)
            {
                //someone want the deterministic release of all resources
                //Let us release all the managed resources
                //ReleaseManagedResources();
                if(instance != null)
                {
                    instance.Dispose();
                }
            }
            else
            {
                // Do nothing, no one asked a dispose, the object went out of
                // scope and finalized is called so lets next round of GC 
                // release these resources
            }

            // Release the unmanaged resource in any case as they will not be 
            // released by GC
            //CleanUp();
        }

        ~CodeEngine()
        {
            // The object went out of scope and finalized is called
            // Lets call dispose in to release unmanaged resources 
            // the managed resources will anyways be released when GC 
            // runs the next time.
            Dispose(false);
        }

        #endregion
    }
}
