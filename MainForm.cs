using System;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using LightCom.WinCE;

namespace GPSDetector
{
    public partial class MainForm: Form
    {
        LightCom.Gps.DetectGPS scanner = new LightCom.Gps.DetectGPS ();
        public MainForm ()
        {
            try
            {
                IntPtr hGps = WinMobile5GPSWrapper.GPSOpenDevice (IntPtr.Zero, IntPtr.Zero, null, 0);
                WinMobile5GPSWrapper.GPS_POSITION pos = new WinMobile5GPSWrapper.GPS_POSITION ();
                pos.Init ();
                int result = WinMobile5GPSWrapper.GPSGetPosition (hGps, ref pos, 1000, 0);
                result = WinMobile5GPSWrapper.GPSCloseDevice (hGps);
            }
            catch (Exception e)
            {
                MessageBox.Show (e.ToString ());
            }

            InitializeComponent ();
        }

        private void MainForm_Load (object sender, EventArgs e)
        {
        }

        private void btnStartScan_Click (object sender, EventArgs e)
        {
            if (this.scanner.IsScanning)
            {
                StopScan (50);
            }
            else
            {
                StartScan ();
            }
        }

        private void ClearLog ()
        {
            this.txtLog.Text = string.Empty;
        }

        
        public void ProcessScanMsg (LightCom.Gps.DetectGPS.EventType et, string port, int baudRate)
        {
            this.Invoke (new LightCom.Gps.DetectGPS.GPSDetectorEventHandler (this.ScanMsgProcessor), new Object [] { et, port, baudRate });
        }
        public void ScanMsgProcessor (LightCom.Gps.DetectGPS.EventType et, string port, int baudRate)
        {
            string msg;
            switch (et)
            {
                case LightCom.Gps.DetectGPS.EventType.etAbortRequestReceived:
                    msg = string.Format ("{0} - прерывается", port);
                    break;
                case LightCom.Gps.DetectGPS.EventType.etPortScanStarted:
                    msg = string.Format ("Сканирование {0} начато", port);
                    break;
                case LightCom.Gps.DetectGPS.EventType.etGPSBaudRateFound:
                    msg = string.Format ("Найден GPS на {0}:{1}", port, baudRate);
                    break;
                case LightCom.Gps.DetectGPS.EventType.etPortScanFinished:
                    msg = string.Format ("Сканирование {0} завершено", port);
                    break;
                default:
                    return;
            }

            AddToLog (msg);
        }


        void StopScan (int waitTimeout)
        {
            try
            {
                this.btnStartScan.Enabled = false;
                if (scanner.IsScanning)
                {

                    scanner.SendAbortRequest ();

                    for (int i = 0; i < waitTimeout; ++i)
                    {
                        Application.DoEvents ();
                        Thread.Sleep (waitTimeout * 100);
                        if (!scanner.IsScanning)
                        {
                            break;
                        }
                    }

                    while (scanner.IsScanning)
                    {
                        foreach (Thread thread in scanner.Threads)
                        {
                            try
                            {
                                if (!thread.Join (waitTimeout * 100 / scanner.Threads.Length))
                                {
                                    thread.Abort ();
                                }
                            }
                            catch (Exception)
                            {
                            }
                            Application.DoEvents ();
                        }
                    }
                }
                timer1.Enabled = false;
                this.btnStartScan.Text = "Искать";
            }
            finally
            {
                this.btnStartScan.Enabled = true;
            }
        }

        void StartScan ()
        {
            StopScan (50);
            txtLog.Text = "";
            timer1.Enabled = true;
            scanner.Start (ProcessScanMsg);
            this.btnStartScan.Text = "Стоп";
        }

        private void AddToLog (string msg)
        {
            this.txtLog.Text = this.txtLog.Text + msg + "\r\n";
            this.txtLog.SelectionStart = this.txtLog.Text.Length - 2;
            this.txtLog.ScrollToCaret ();
        }

        private void MainForm_Closing (object sender, CancelEventArgs e)
        {
            StopScan (50);
        }

        private void timer1_Tick (object sender, EventArgs e)
        {
            if (!scanner.IsScanning)
            {
                timer1.Enabled = false;
                this.btnStartScan.Text = "Искать";
                AddToLog ("Сканирование завершено.");                
            }
        }

        void test (List<String> container, int Count)
        {
            DateTime start = DateTime.Now;
            for (int i = 0; i < Count; ++i)
            {
                container.Add (i.ToString());
            }
            DateTime finish = DateTime.Now;
            TimeSpan ts = finish - start;
            this.AddToLog (((int) ts.TotalMilliseconds).ToString() + " ms");
        }

        int count
        {
            get { return Convert.ToInt32 (this.textBox1.Text); }
        }
        private void button1_Click (object sender, EventArgs e)
        {
            this.AddToLog ("Test1");
            List<String> container = new List<string> ();
            test (container, count);
        }

        private void button2_Click (object sender, EventArgs e)
        {
            this.AddToLog ("Test2");
            List<String> container = new List<string> ();
            container.Capacity = count;
            test (container, count);             
        }
    }
}