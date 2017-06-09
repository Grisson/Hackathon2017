using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamsa.Common
{
    public interface IExecutableCode
    {
        void Setup();

        void Loop();
    }
}
