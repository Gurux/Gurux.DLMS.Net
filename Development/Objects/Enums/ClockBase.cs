using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Objects.Enums
{
    public enum ClockBase
    {
        /// <summary>
        /// Not defined
        /// </summary>
        None,
        /// <summary>
        /// Internal Crystal
        /// </summary>
        Crystal,
        /// <summary>
        /// Mains frequency 50 Hz,
        /// </summary>
        Frequency50,
        /// <summary>
        /// Mains frequency 60 Hz,
        /// </summary>
        Frequency60,
        /// <summary>
        /// Global Positioning System.
        /// </summary>
        GPS,
        /// <summary>
        /// Radio controlled.
        /// </summary>
        Radio
    }
}
