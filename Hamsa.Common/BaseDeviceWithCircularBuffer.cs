using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamsa.Common
{
    public abstract class BaseDeviceWithCircularBuffer<T, D>: BaseDevice<T> 
    {
        protected CircularBuffer<D> DataBuffer { get; set; }

        public virtual D GetLatestData()
        {
            return DataBuffer.Peek();
        }

        public BaseDeviceWithCircularBuffer(int bufferSize)
        {
            DataBuffer = new CircularBuffer<D>(bufferSize);
        }
    }
}
