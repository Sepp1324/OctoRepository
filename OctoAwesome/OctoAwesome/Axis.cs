using System;

namespace OctoAwesome
{
    /// <summary>
    /// Flag List for aivailable Axis.
    /// </summary>
    [Flags]
    public enum Axis
    {
        /// <summary>
        /// No Axis
        /// </summary>
        None = 0,

        /// <summary>
        /// X Axis (Eas-West-Axis)
        /// </summary>
        X = 1,

        /// <summary>
        /// Y Axis (North-South-Axis)
        /// </summary>
        Y = 2,

        /// <summary>
        /// Z Axis (Height-Axis)
        /// </summary>
        Z = 4
    }
}