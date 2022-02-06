namespace GPSDetector
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose ();
            }
            base.Dispose (disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent ()
        {
            this.btnStartScan = new System.Windows.Forms.Button ();
            this.txtLog = new System.Windows.Forms.TextBox ();
            this.bottomPanel = new System.Windows.Forms.Panel ();
            this.timer1 = new System.Windows.Forms.Timer ();
            this.t1 = new System.Windows.Forms.Button ();
            this.t2 = new System.Windows.Forms.Button ();
            this.textBox1 = new System.Windows.Forms.TextBox ();
            this.bottomPanel.SuspendLayout ();
            this.SuspendLayout ();
            // 
            // btnStartScan
            // 
            this.btnStartScan.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartScan.Location = new System.Drawing.Point (165, 6);
            this.btnStartScan.Name = "btnStartScan";
            this.btnStartScan.Size = new System.Drawing.Size (72, 20);
            this.btnStartScan.TabIndex = 0;
            this.btnStartScan.Text = "Искать";
            this.btnStartScan.Click += new System.EventHandler (this.btnStartScan_Click);
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point (0, 0);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size (240, 265);
            this.txtLog.TabIndex = 1;
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add (this.textBox1);
            this.bottomPanel.Controls.Add (this.t2);
            this.bottomPanel.Controls.Add (this.t1);
            this.bottomPanel.Controls.Add (this.btnStartScan);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point (0, 265);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size (240, 29);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler (this.timer1_Tick);
            // 
            // t1
            // 
            this.t1.Location = new System.Drawing.Point (4, 7);
            this.t1.Name = "t1";
            this.t1.Size = new System.Drawing.Size (25, 20);
            this.t1.TabIndex = 1;
            this.t1.Text = "1";
            this.t1.Click += new System.EventHandler (this.button1_Click);
            // 
            // t2
            // 
            this.t2.Location = new System.Drawing.Point (35, 6);
            this.t2.Name = "t2";
            this.t2.Size = new System.Drawing.Size (26, 20);
            this.t2.TabIndex = 2;
            this.t2.Text = "2";
            this.t2.Click += new System.EventHandler (this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point (68, 7);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size (76, 21);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "10000";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF (96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size (240, 294);
            this.Controls.Add (this.txtLog);
            this.Controls.Add (this.bottomPanel);
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "LightCom GPS Detector";
            this.Closing += new System.ComponentModel.CancelEventHandler (this.MainForm_Closing);
            this.Load += new System.EventHandler (this.MainForm_Load);
            this.bottomPanel.ResumeLayout (false);
            this.ResumeLayout (false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartScan;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button t2;
        private System.Windows.Forms.Button t1;
        private System.Windows.Forms.TextBox textBox1;
    }
}

