using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS
{    
    /// <summary>
    /// OBIS Code class is used to find out default descrition to OBIS Code.
    /// </summary>
    class GXStandardObisCode
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXStandardObisCode()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXStandardObisCode(string[] obis, string desc, string interfaces, string dataType)
        {
            this.OBIS = new string[6];
            if (obis != null)
            {
                Array.Copy(obis, this.OBIS, 6);
            }
            this.Description = desc;
            this.Interfaces = interfaces;
            DataType = dataType;
        }

        /// <summary>
        /// OBIS code.
        /// </summary>
        public string[] OBIS
        {
            get;
            set;
        }

        /// <summary>
        /// OBIS code description.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Interfaces that are using this OBIS code.
        /// </summary>
        public string Interfaces
        {
            get;
            set;
        }

        /// <summary>
        /// Standard data types.
        /// </summary>
        public string DataType
        {
            get;
            set;
        }

        /// <summary>
        /// Convert to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join(".", OBIS) + " " + Description;
        }
    }
}
