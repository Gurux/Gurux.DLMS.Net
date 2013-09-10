using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSScriptAction
    {

        /// <summary>
        /// Defines which action to be applied to the referenced object.
        /// </summary>
        public GXDLMSScriptActionType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Executed object type.
        /// </summary>        
        public ObjectType ObjectType
        {
            get;
            set;
        }

        /// <summary>
        /// Logical name of executed object.
        /// </summary>
        public string LogicalName
        {
            get;
            set;
        }
        /// <summary>
        /// Defines which attribute of the selected object is affected; 
        /// or which specific method is to be executed.
        /// </summary>        
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Parameter is service spesific.
        /// </summary>        
        public Object Parameter
        {
            get;
            set;
        }
    }
}
