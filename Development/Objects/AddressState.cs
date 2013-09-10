using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Objects
{
    /// <summary>
    /// Defines whether or not the device has been assigned an address 
    /// since last power up of the device.
    /// </summary>
    public enum AddressState
    {
        /// <summary>
        /// Not assigned an address yet.
        /// </summary>
        None,
        /// <summary>
        /// Assigned an address either by manual setting, or by automated method.
        /// </summary>
        Assigned
    }
}
