using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Gurux.Common;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;

namespace Gurux.Common
{
    /// <summary>
    /// Because MTA threading model is used clipboard can't access directly.
    /// </summary>
    public class ClipboardCopy
    {
        /// <summary>
        /// Copies data from items and sub items in the list view to the clipboard.
        /// </summary>
        /// <param name="data">Data to copy.</param>
        static public void CopyDataToClipboard(object data)
        {
            string str = string.Empty;
            if (data is string)
            {
                str = data as string;
            }
            else if (data != null && data.GetType().IsArray)
            {
                foreach (object it in (Array)data)
                {
                    str += it.ToString();
                }
            }
            else
            {
                ListView view = data as ListView;
                if (view != null)
                {
                    if (view.VirtualMode)
                    {
                        StringBuilder sb = new StringBuilder(view.SelectedIndices.Count * 50);
                        foreach (int pos in view.SelectedIndices)
                        {
                            ListViewItem it = view.Items[pos];
                            foreach (ListViewItem.ListViewSubItem sub in it.SubItems)
                            {
                                sb.Append(sub.Text);
                                sb.Append('\t');
                            }
                            sb.Remove(sb.Length - 1, 1);
                            sb.Append(Environment.NewLine);
                        }
                        str = sb.ToString();
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder(view.SelectedIndices.Count * 50);
                        foreach (ListViewItem it in view.SelectedItems)
                        {
                            foreach (ListViewItem.ListViewSubItem sub in it.SubItems)
                            {
                                sb.Append(sub.Text);
                                sb.Append('\t');
                            }
                            sb.Remove(sb.Length - 1, 1);
                            sb.Append(Environment.NewLine);
                        }
                        str = sb.ToString();
                    }
                }                
            }
            if (!string.IsNullOrEmpty(str))
            {
                ClipboardCopy threadHelper = new ClipboardCopy(str);
                ThreadStart ts = new ThreadStart(threadHelper.CopyToClipboard);
                Thread STAThread1 = new Thread(ts);
                STAThread1.SetApartmentState(ApartmentState.STA);
                STAThread1.Start();
                STAThread1.Join();
            }
        }

        private string Data = string.Empty;

        ClipboardCopy(string data)
        {
            Data = data;
        }

        void CopyToClipboard()
        {
            Clipboard.SetDataObject(Data, true);
        }
    }
}
