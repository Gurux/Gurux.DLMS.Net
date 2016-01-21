using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS
{
    class GXServerReply
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXServerReply()
        {
            Data = new GXByteBuffer();
        }

        ///<summary>
        ///Server received data.
        ///</summary>
        public GXByteBuffer Data
        {
            get;
            set;
        }

        ///<summary>
        ///Server reply messages.
        ///</summary>
        public List<byte[][]> ReplyMessages
        {
            get;
            set;
        }

        ///<summary>
        ///Reply message index. 
        ///</summary>
        public int Index   
        {
            get;
            set;
        }
    }
}
