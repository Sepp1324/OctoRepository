using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public class PhysicalProperties
    {
        /// <summary>
        /// Information about the Hardness (Härte) of a Block
        /// </summary>
        public float Hardness { get; set; }

        /// <summary>
        /// Information about the Density (Dichte) of a Block
        /// </summary>
        public float Density { get; set; }

        /// <summary>
        /// Information about the Granularity (Granularität) of a Block
        /// </summary>
        public float Granularity { get; set; }

        /// <summary>
        /// Information about the FractureToughness (Bruchzähigkeit) of a Block
        /// </summary>
        public float FractureToughness { get; set; }
    }
}
