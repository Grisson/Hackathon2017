using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmController.lib.Data;

namespace ArmController.lib
{
    internal class TestAgent
    {
        public Guid Id;
        //public PosePosition FirstTouchPosition;
        //public TouchResponse FirstTouchPoint;

        public PosePosition CurrentPosition { get; set; }

        public TestAgent() : this(Guid.NewGuid())
        {

        }

        public TestAgent(string id) : this(Guid.Parse(id))
        {
        }

        public TestAgent(Guid id)
        {
            Id = id;
            CurrentPosition = PosePosition.InitializePosition();
        }

        public string[] ToCommond(List<TestStep> steps)
        {
            var idealPosition = CurrentPosition.Clone();
            var result = new List<string>();
            foreach(var step in steps)
            {
                var tmp = this.ToCommond(step, idealPosition);
                if ((tmp != null) && (tmp.Length > 0))
                {
                    result.AddRange(tmp);
                }
            }

            return result.ToArray();
        }


        public string[] ToCommond(TestStep step, PosePosition idealPosition)
        {
            // logic
            return new[] {"G91 G0 X0.1 Y0.1"};
        }
    }
}
