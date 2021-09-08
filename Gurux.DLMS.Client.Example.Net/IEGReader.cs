using System;
using System.Collections.Generic;
using Gurux.Serial;
using Gurux.Net;
using System.Threading;
using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using System.Linq;
using System.Text;
using Gurux.DLMS.Reader;

namespace Gurux.DLMS.Client.Example.Net
{
    class IEGReader
    {
        Settings settings = new Settings();
        Reader.GXDLMSReader reader = null;


        /// <summary>
        /// Constructor 
        /// Takes Initial Connection Parameters and Parse it 
        /// </summary>
        /// <param name="args">Connection parameters</param>
        public IEGReader(String[] args, String RefObjectsXmlFile)
        {
            
            ////////////////////////////////////////
            //Handle command line parameters.
            int ret = Settings.GetParameters(args, settings);
            if (ret != 0) { /* Do nothing*/}
            else { Settings_Edited.ShowHelp(); }

            ////////////////////////////////////////
            //Xml file path that contains all the meter COSEM objects.
            settings.outputFile = RefObjectsXmlFile;

            ////////////////////////////////////////
            //Initialize connection settings.
            if (settings.media is GXSerial) { /* Do nothing*/}

            else if (settings.media is GXNet) { /* Do nothing*/}

            else { throw new Exception("Unknown media type."); }
            ////////////////////////////////////////


            reader = new Reader.GXDLMSReader(settings.client, settings.media, settings.trace, settings.invocationCounter);

            try
            {
                settings.media.Open();
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Available ports:");
                Console.WriteLine(string.Join(" ", GXSerial.GetPortNames()));
            }


            //Some meters need a break here.
            Thread.Sleep(1000);


            //Export client and server certificates from the meter.
            if (!string.IsNullOrEmpty(settings.ExportSecuritySetupLN))
            {
                reader.ExportMeterCertificates(settings.ExportSecuritySetupLN);
            }

            //Generate new client and server certificates and import them to the server.
            else if (!string.IsNullOrEmpty(settings.GenerateSecuritySetupLN))
            {
                reader.GenerateCertificates(settings.GenerateSecuritySetupLN);
            }

        }

        public void Voltage()
        {
            
            try
            {
                if (settings.readObjects.Count != 0)
                {
                    bool read = false;
                    if (settings.outputFile != null)
                    {
                        try
                        {
                            settings.client.Objects.Clear();
                            settings.client.Objects.AddRange(GXDLMSObjectCollection.Load(settings.outputFile));
                            read = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            if (System.Diagnostics.Debugger.IsAttached)
                            {
                                Console.ReadKey();
                            }
                        }
                    }
                    reader.InitializeConnection();
                    if (!read)
                    {
                        reader.GetAssociationView(settings.outputFile);
                    }
                    foreach (KeyValuePair<string, int> it in settings.readObjects)
                    {
                        object val = reader.Read(settings.client.Objects.FindByLN(ObjectType.None, it.Key), it.Value);
                        reader.ShowValue(val, it.Value);
                    }
                    if (settings.outputFile != null)
                    {
                        try
                        {
                            settings.client.Objects.Save(settings.outputFile, new GXXmlWriterSettings() { UseMeterTime = true, IgnoreDefaultValues = false });
                        }
                        catch (Exception)
                        {
                            //It's OK if this fails.
                        }
                    }
                }

            }
            catch (GXDLMSException ex)
            {
                Console.WriteLine(ex.Message);
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Console.ReadKey();
                }
            }
            catch (GXDLMSExceptionResponse ex)
            {
                Console.WriteLine(ex.Message);
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Console.ReadKey();
                }
            }
            catch (GXDLMSConfirmedServiceError ex)
            {
                Console.WriteLine(ex.Message);
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.ToString());
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Console.ReadKey();
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Console.WriteLine("Ended. Press any key to continue.");
                    Console.ReadKey();
                }
            }
        }


    }
}
