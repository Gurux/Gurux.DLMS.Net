
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
    BitStringTag = 0x3,
    OctetStringTag = 0x4,
    NullTag = 0x5,
    ObjectIdentifierTag = 0x6,
    RealTag = 0x9,
	EnumeratedTag = 0xA,
	Utf8StringTag = 0xC,
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
