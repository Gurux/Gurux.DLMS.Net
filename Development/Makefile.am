
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG;TRACE" "-keyfile:Gurux.DLMS.snk"
ASSEMBLY = bin/Debug/Gurux.DLMS.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug/

GURUX_DLMS_DLL_MDB_SOURCE=bin/Debug/Gurux.DLMS.dll.mdb
GURUX_DLMS_DLL_MDB=$(BUILD_DIR)/Gurux.DLMS.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-define:TRACE" "-keyfile:Gurux.DLMS.snk"
ASSEMBLY = bin/Release/Gurux.DLMS.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release/

GURUX_DLMS_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(GURUX_DLMS_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(GURUX_DLMS_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	AssociationResult.cs \
	Authentication.cs \
	DataType.cs \
	GXApplicationContextName.cs \
	GXDLMS.cs \
	GXDLMSException.cs \
	GXDLMSLimits.cs \
	GXDLMSLNSettings.cs \
	GXDLMSSNSettings.cs \
	GXDLMSTag.cs \
	GXDLMSTagCollection.cs \
	GXFCS16.cs \
	GXUserInformation.cs \
	InterfaceType.cs \
	ManufacturerSettings/GXAttribute.cs \
	ManufacturerSettings/GXAttributeCollection.cs \
	ManufacturerSettings/GXAuthentication.cs \
	ManufacturerSettings/GXFileInfo.cs \
	ManufacturerSettings/GXManufacturer.cs \
	ManufacturerSettings/GXManufacturerCollection.cs \
	ManufacturerSettings/GXObisCode.cs \
	ManufacturerSettings/GXObisCodeCollection.cs \
	ManufacturerSettings/GXObisCodeConverter.cs \
	ManufacturerSettings/GXObisValueItem.cs \
	ManufacturerSettings/GXObisValueItemCollection.cs \
	ManufacturerSettings/GXServerAddress.cs \
	ObjectType.cs \
	Properties/AssemblyInfo.cs \
	Properties/Resources.Designer.cs \
	RequestTypes.cs \
	Shared/GXCommon.cs \
	SourceDiagnostic.cs \
	Objects/GXDLMSActionSchedule.cs \
	Objects/GXDLMSActivityCalendar.cs \
	Objects/GXDLMSAssociationLogicalName.cs \
	Objects/GXDLMSAssociationShortName.cs \
	Objects/GXDLMSAutoAnswer.cs \
	Objects/GXDLMSAutoConnect.cs \
	Objects/GXDLMSClock.cs \
	Objects/GXDLMSData.cs \
	Objects/GXDLMSDemandRegister.cs \
	Objects/GXDLMSExtendedRegister.cs \
	Objects/GXDLMSGprsSetup.cs \
	Objects/GXDLMSHdlcSetup.cs \
	Objects/GXDLMSIECOpticalPortSetup.cs \
	Objects/GXDLMSIp4Setup.cs \
	Objects/GXDLMSMacAddressSetup.cs \
	Objects/GXDLMSModemConfiguration.cs \
	Objects/GXDLMSObject.cs \
	Objects/GXDLMSObjectCollection.cs \
	Objects/GXDLMSPppSetup.cs \
	Objects/GXDLMSProfileGeneric.cs \
	Objects/GXDLMSRegister.cs \
	Objects/GXDLMSRegisterActivation.cs \
	Objects/GXDLMSRegisterMonitor.cs \
	Objects/GXDLMSSapAssignment.cs \
	Objects/GXDLMSSchedule.cs \
	Objects/GXDLMSScriptTable.cs \
	Objects/GXDLMSSpecialDaysTable.cs \
	Objects/GXDLMSTcpUdpSetup.cs \
	Internal/GXAPDU.cs \
	Internal/GXCommon.cs \
	Internal/GXFCS16.cs \
	AccessMode.cs \
	Command.cs \
	Unit.cs \
	ManufacturerSettings/GXDateTimeConverter.cs \
	ManufacturerSettings/GXError.cs \
	ManufacturerSettings/GXLogWriter.cs \
	GXDLMSClient.cs \
	GXDLMSServer.cs \
	ValueEventArgs.cs 

DATA_FILES = 

RESOURCES = \
	Properties/Resources.resx,Gurux.DLMS.Properties.Resources.resources 

EXTRAS = \
	Gurux.DLMS.snk \
	gurux.dlms.pc.in 

REFERENCES =  \
	System \
	System.Core \
	System.Xml.Linq \
	System.Data.DataSetExtensions \
	System.Data \
	System.Xml \
	System.Windows.Forms

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

GURUX_DLMS_PC = $(BUILD_DIR)/gurux.dlms.pc

$(eval $(call emit-deploy-wrapper,GURUX_DLMS_PC,gurux.dlms.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
