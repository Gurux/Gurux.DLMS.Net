using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ICube.XmlPdu;
using Gurux.Common;
using System.Xml.Schema;
using Gurux.DLMS.Enums;
using System.Xml.Linq;

namespace Gurux.DLMS.XmlTester
{
    public partial class Form1 : Form
    {
        XmlSchemaSet schemas = new XmlSchemaSet();
        List<string> files = new List<string>();
        public Form1()
        {
            InitializeComponent();
            schemas.Add("http://www.dlms.com/COSEMpdu", "COSEMpdu.xsd");
            //ParseText(@"C:\Projects\Gurux.Meter.Simulator\iskra hollanti.txt");
//                        files.Add(@"C:\Projects\Gurux.Meter.Simulator\iskra hollanti.txt");
            //files.Add(@"C:\Projects\Gurux.Meter.Simulator\director_log.txt");
//            files.Add(@"C:\Projects\Gurux.Meter.Simulator\Landis+Gyr.txt");
            files.Add(@"C:\Projects\Gurux.Meter.Simulator\translator.txt");
            ParseText(files[0]);
            
        }

        static string Equals(string expected, string actual)
        {
            actual = actual.Replace("ParameterizedAccess", "ParametrizedAccess");
            actual = actual.Replace("UInt32", "DoubleLongUnsigned");
            actual = actual.Replace("UInt16", "LongUnsigned");
            actual = actual.Replace("UInt8", "Unsigned");
            actual = actual.Replace("Int32", "DoubleLong");
            actual = actual.Replace("Int16", "Long");
            actual = actual.Replace("Int8", "Integer");
            actual = actual.Replace("Low", "LOW_SECURITY");
            actual = actual.Replace("\"High\"", "\"HIGH_SECURITY\"");
            actual = actual.Replace("HighGMAC", "HIGH_SECURITY_GMAC");
            actual = actual.Replace("AttributeDescriptorWithSelection", "_AttributeDescriptorWithSelection");
            actual = actual.Replace("CallingAuthentication", "CallingAuthenticationValue");
            actual = actual.Replace("RespondingAuthentication", "RespondingAuthenticationValue");
            actual = actual.Replace("<None />", "<NullData />");
            actual = actual.Replace("<Normal>", "<GetResponsenormal>");
            actual = actual.Replace("</Normal>", "</GetResponsenormal>");
            actual = actual.Replace("<String", "<VisibleString");
            
            int cnt = expected.Length;
            if (actual.Length < cnt)
            {
                cnt = actual.Length;
            }
            for (int pos = 0; pos != actual.Length; ++pos)
            {
                if (expected[pos] != actual[pos])
                {
                    return actual.Substring(pos);
                }
            }
            if (actual.Length != expected.Length)
            {
                return "Data is missing from the end.";
            }
            return expected;
        }

        private void ParseText(string file)
        {
            TraceTB.Clear();
            GXByteBuffer bb = new GXByteBuffer();
            GXDLMSTranslator tr = new GXDLMSTranslator(TranslatorOutputType.StandardXml);
            tr.ShowStringAsHex = true;
            tr.PduOnly = PduOnlyCB.Checked;
            tr.CompletePdu = CompletePDUCB.Checked;
            string str = System.IO.File.ReadAllText(file);
            bb.Set(GXDLMSTranslator.HexToBytes(str));
            StringBuilder sb = new StringBuilder();
            GXByteBuffer pdu = new GXByteBuffer();
            while (tr.FindNextFrame(bb, pdu))
            {
                sb.Append(tr.MessageToXml(bb));
                if (pdu.Size != 0)
                {
                    string actual = tr.PduToXml(pdu);
                    XDocument doc = XDocument.Parse(actual);
                    doc.Validate(schemas, (o, e) =>
                    {
                        throw new Exception(e.Message);
                    });
                }
                /*
                if (pdu.Size != 0 && pdu.Data[0] != 0x81)
                {
                    MemoryStream ms = new MemoryStream(pdu.Array());
                    string expected = XmlPduInterface.PduToXml(ms).ToString();
                    string actual = tr.PduToXml(pdu);
                    string tmp = Equals(expected, actual);
                    if (expected.CompareTo(tmp) != 0)
                    {
                        System.Diagnostics.Debug.WriteLine(expected);
                        System.Diagnostics.Debug.WriteLine(actual);
                        pdu.Position = 0;
                        actual = tr.PduToXml(pdu);
                    }
                    byte[] actualBytes = tr.XmlToPdu(actual);
                    //System.Diagnostics.Debug.WriteLine(GXCommon.ToHex(actualBytes, true));
                }
                 * */
            }
            TraceTB.Text = sb.ToString();
        }

        private void OpenBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Multiselect = true;
                dlg.InitialDirectory = @"C:\Projects\Gurux.Meter.Simulator";
                dlg.Filter = "Text files(*.txt) | *.txt";
                dlg.DefaultExt = ".txt";
                dlg.ValidateNames = true;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    files.Clear();
                    files.AddRange(dlg.FileNames);
                    Update(null, null);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(this, Ex.Message);
            }
        }

        private void Update(object sender, EventArgs e)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            foreach (string it in files)
            {
                ParseText(it);
            }
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }
    }
}
