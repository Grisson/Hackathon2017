﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Hamsa.REST.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    public partial class VisionAssertionResult
    {
        /// <summary>
        /// Initializes a new instance of the VisionAssertionResult class.
        /// </summary>
        public VisionAssertionResult() { }

        /// <summary>
        /// Initializes a new instance of the VisionAssertionResult class.
        /// </summary>
        public VisionAssertionResult(bool? isPassed = default(bool?), IList<string> regions = default(IList<string>))
        {
            IsPassed = isPassed;
            Regions = regions;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "IsPassed")]
        public bool? IsPassed { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Regions")]
        public IList<string> Regions { get; set; }

    }
}
