using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamsa.Azure
{
    public partial class Cognitive
    {
        public string SecretKey { get; private set; }

        public Cognitive(string secretKey)
        {
            SecretKey = secretKey;
        }

        
    }
}
