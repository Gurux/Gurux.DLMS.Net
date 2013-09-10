using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Objects
{
    public enum GXDLMSIp4SetupIpOptionType
    {
        /// <summary>
        /// If this option is present, the device shall be allowed to send security,
        /// compartmentation, handling restrictions and TCC (closed user group)
        /// parameters within its IP Datagrams. The value of the IP-Option-
        /// Length Field must be 11, and the IP-Option-Data shall contain the
        /// value of the Security, Compartments, Handling Restrictions and
        /// Transmission Control Code values, as specified in STD0005 / RFC791.
        /// </summary>
        Security = 0x82,
        /// <summary>
        /// If this option is present, the device shall supply routing information to be
        /// used by the gateways in forwarding the datagram to the destination, and to
        /// record the route information.
        /// The IP-Option-length and IP-Option-Data values are specified in STD0005 / RFC 791.
        /// </summary>
        LooseSourceAndRecordRoute = 0x83,
        /// <summary>
        /// If this option is present, the device shall supply routing information to be
        /// used by the gateways in forwarding the datagram to the destination, and to
        /// record the route information.
        /// The IP-Option-length and IP-Option-Data values are specified in STD0005 / RFC 791.
        /// </summary>
        StrictSourceAndRecordRoute = 0x89,
        /// <summary>
        /// If this option is present, the device shall as well:
        /// send originated IP Datagrams with that option, providing means
        /// to record the route of these Datagrams;
        /// as a router, send routed IP Datagrams with the route option
        /// adjusted according to this option.
        /// The IP-Option-length and IP-Option-Data values are specified in
        /// STD0005 / RFC 791.
        /// </summary>
        RecordRoute = 0x07,
        /// <summary>
        /// If this option is present, the device shall as well:
        /// send originated IP Datagrams with that option, providing means
        /// to time-stamp the datagram in the route to its destination;
        /// as a router, send routed IP Datagrams with the time-stamp option
        /// adjusted according to this option.
        /// The IP-Option-length and IP-Option-Data values are specified in STD0005 / RFC 791.
        /// </summary>
        InternetTimestamp = 0x44
    }
}