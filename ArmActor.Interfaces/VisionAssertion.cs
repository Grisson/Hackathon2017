using System.Collections.Generic;
using System.Drawing;

namespace ArmActor
{
    public class VisionAssertion
    {
        public Rectangle CropArea { get; set; }
        public string TargetText { get; set; }
    }

    public class VisionDataModel
    {
        public Dictionary<string, VisionAssertion> AssertionDictionary { get; set; }

        public string CurrentAssertionName { get; set; }


    }

    public class VisionAssertionResult
    {
        public bool IsPassed { get; set; }

        public Rectangle[] Regions { get; set; }
    }
}
