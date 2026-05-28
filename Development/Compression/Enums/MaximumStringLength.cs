using System.Xml.Serialization;

namespace Gurux.DLMS.Compression.Enums
{
    /// <summary>
    /// Specifies predefined maximum string lengths that can be used to enforce constraints on string values.
    /// </summary>
    /// <remarks>This enumeration provides a set of commonly used maximum string lengths, such as 46, 78, 142,
    /// and 255 characters. It can be used to standardize string length constraints across an application.</remarks>
    public enum MaximumStringLength : byte
    {
        /// <summary>
        /// The maximum string length is 46 characters.
        /// </summary>
        [XmlEnum("46")]
        Value46 = 46,
        /// <summary>
        /// The maximum string length is 78 characters.
        /// </summary>
        [XmlEnum("78")]
        Value78 = 78,
        /// <summary>
        /// The maximum string length is 142 characters.
        /// </summary>
        [XmlEnum("142")]
        Value142 = 142,
        /// <summary>
        /// The maximum string length is 255 characters.
        /// </summary>
        [XmlEnum("255")]
        Value255 = 255
    }
}
