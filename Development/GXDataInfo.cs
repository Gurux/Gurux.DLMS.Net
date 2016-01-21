using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurux.DLMS.Enums;

namespace Gurux.DLMS
{
    /// <summary>
    /// This class is used in DLMS data parsing. 
    /// </summary>
    class GXDataInfo
    {
        ///<summary>
        /// Last array index. 
        ///</summary>        
        public int Index
        {
            get;
            internal set;
        }

        ///<summary>
        /// Items count in array.
        ///</summary>
        public int Count
        {
            get;
            internal set;
        }

        ///<summary>
        /// Object data type. 
        ///</summary>
        public DataType Type
        {
            get;
            internal set;
        }

        ///<summary>
        ///Is data parsed to the end.
        ///</summary>
        public bool Compleate
        {
            get;
            internal set;
        }

        /// <summary>
        /// Clear settings.
        /// </summary>
        public virtual void Clear()
        {
            Index = 0;
            Count = 0;
            Type = DataType.None;
            Compleate = true;
        }
    }
}
