using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Internal
{
    /// <summary>
    /// Reserved for internal use.
    /// </summary>
    enum FrameType : byte
    {
        ////////////////////////////////////////////
        // This command is used to set the secondary station in connected mode and reset
        // its sequence number variables.
        SNRM = 0x93, // Set Normal Response Mode (SNRM)
        ////////////////////////////////////////////
        // This response is used to confirm that the secondary received and acted on an SNRM or DISC command.
        UA = 0x73, // Unnumbered Acknowledge (UA)
        ////////////////////////////////////////////
        // This command and response is used to transfer a block of data together with its sequence number.
        // The command also includes the sequence number of the next frame the transmitting station expects to see.
        // This way, it works as an RR. Like RR, it enables transmission of I frames from the opposite side.
        Information = 0x10, // Information (I)

        /// <summary>
        /// AARE frame.
        /// </summary>
        AARE = 0x30,
        ////////////////////////////////////////////
        // This response is used to indicate an error condition. The two most likely error conditions are:
        // Invalid command or Sequence number problem.
        Rejected = 0x97,  // Frame Reject
        ////////////////////////////////////////////
        // This command is used to terminate the connection.
        Disconnect = 0x53,
        ////////////////////////////////////////////
        // This response is used to inform the primary station that the secondary is disconnected.
        DisconnectMode = 0x1F // Disconnected Mode
    }
}
