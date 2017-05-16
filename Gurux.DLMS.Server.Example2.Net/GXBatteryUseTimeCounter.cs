using Gurux.DLMS.Objects;
using System;
using System.Threading;

namespace Gurux.DLMS.Server.Example2.Net
{
    /// <summary>
    /// This class is updating Battery Use Time Counter once per second.
    /// </summary>
    class GXBatteryUseTimeCounter
    {
        private GXDLMSRegister target;
        private AutoResetEvent closing;
        private Thread thread;

        public GXBatteryUseTimeCounter(GXDLMSRegister value)
        {
            closing = new AutoResetEvent(false);
            target = value;
        }

        /// <summary>
        /// Start thread.
        /// </summary>
        public void Start()
        {
            thread = new Thread(new ThreadStart(Run));
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// Stop thread.
        /// </summary>
        public void Stop()
        {
            if (thread != null)
            {
                closing.Set();
                thread.Join();
                thread = null;
            }
        }


        void Run()
        {
            UInt16 cnt = 0;
            do
            {
                try
                {
                    //Value is reset.
                    if (target.Value == null)
                    {
                        cnt = 0;
                    }
                    target.Value = ++cnt;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            while (!closing.WaitOne(1000));
        }
    }
}
