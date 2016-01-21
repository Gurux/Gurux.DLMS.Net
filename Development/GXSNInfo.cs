namespace Gurux.DLMS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Gurux.DLMS.Objects;

    ///<summary>
    ///Server uses this class to find Short Name object and attribute index. 
    ///This class is reserved for internal use.
    ///</summary>
    class GXSNInfo
    {
        /// <summary>
        /// Attribute index.
        /// </summary>
        public virtual int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Is attribute index or action index 
        /// </summary>
        public virtual bool IsAction
        {
            get;
            set;
        }

        /// <summary>
        /// COSEM object.
        /// </summary>
        public virtual GXDLMSObject Item
        {
            get;
            set;
        }
    }
}