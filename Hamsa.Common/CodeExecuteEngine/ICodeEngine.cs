using System;

namespace Hamsa.Common
{
    public interface ICodeEngine: IDisposable
    {
        void Run();

        void Stop();
    }
}
