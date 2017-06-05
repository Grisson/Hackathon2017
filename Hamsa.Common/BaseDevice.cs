using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamsa.Common
{
    public abstract class BaseDevice<T> : IDisposable
    {
        protected object Syncroot = new object();

        public bool IsIdel { get; protected set; }

        protected T Device { get; set; }

        public bool Lock()
        {
            if(IsIdel)
            {
                lock (Syncroot)
                {
                    if (IsIdel)
                    {
                        IsIdel = false;
                        return true;
                    }
                }
            }

            return false;
        }


        //public bool Unlock()
        //{
        //    if(!IsIdel)
        //    {
        //        lock(Syncroot)
        //        {

        //        }
        //    }
        //}

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
            }
            else
            {
                // Do nothing, no one asked a dispose, the object went out of
                // scope and finalized is called so lets next round of GC 
                // release these resources
            }

            // Release the unmanaged resource in any case as they will not be 
            // released by GC
            CleanUp();
        }

        public abstract void CleanUp();

        ~BaseDevice()
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
