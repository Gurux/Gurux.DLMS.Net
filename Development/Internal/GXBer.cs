
namespace Gurux.DLMS.Internal
{
    enum GXBer{
	UniversalClass = 0x00, 
	ApplicationClass = 0x40,
	ContextClass = 0x80,
	PrivateClass = 0xc0,
	Primitive = 0x00,
	Constructed = 0x20,
	BooleanTag = 0x1,
    IntegerTag = 0x2,
    /// <summary>
    /// Bit String, Universal.
    /// </summary>
    BitStringTag = 0x3,
    /// <summary>
    /// Octet string, Universal.
    /// </summary>
    OctetStringTag = 0x4,
    
    NullTag = 0x5,
    /// <summary>
    /// Object identifier, Universal.
    /// </summary>
    ObjectIdentifierTag = 0x6,
    /// <summary>
    /// Object Descriptor.
    /// </summary>
    ObjectDescriptor = 7,
    /// <summary>
    /// External
    /// </summary>
    External = 8,
    /// <summary>
    /// Real (float).
    /// </summary>
    RealTag = 9,
    /// <summary>
    /// Enumerated.
    /// </summary>
	EnumeratedTag = 10,
	/// <summary>
    /// Utf8 String.
	/// </summary>
    Utf8StringTag = 12,
    NumericStringTag = 0x12,
	PrintableStringTag = 0x13,
    TeletexStringTag = 0x14,
	VideotexStringTag = 0x15,
	Ia5StringTag = 0x16,
	GeneralizedTimeTag = 0x18,
	GraphicStringTag = 0x19,
	VisibleStringTag = 0x1A,
	GeneralStringTag = 0x1B,
	UniversalStringTag = 0x1C,
    BmpStringTag = 0x0E
    }
}
