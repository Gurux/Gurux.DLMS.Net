using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gurux.DLMS.Objects
{
    public interface IGXDLMSBase
    {
        /// <summary>
        /// Returns amount of attributes.
        /// </summary>
        /// <returns>Count of attributes.</returns>
        int GetAttributeCount();

        /// <summary>
        /// Returns amount of methods.
        /// </summary>
        /// <returns></returns>
        int GetMethodCount();

        /// <summary>
        /// Returns value of given attribute.
        /// </summary>
        /// <param name="index">Attribute index</param>
        /// <returns>Value of the attribute index.</returns>
        Object GetValue(int index, out DataType type, byte[] parameters);
        
        /// <summary>
        /// Set value of given attribute.
        /// </summary>
        /// <param name="index">Attribute index</param>
        /// <param name="value">Value of the attribute index.</param>
        void SetValue(int index, Object value);

        /// <summary>
        /// Invokes method.
        /// </summary>
        /// <param name="index">Method index.</param>
        void Invoke(int index, Object parameters);
    }
}
