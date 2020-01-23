using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using BodyArchitect.Shared;
using DevExpress.XtraEditors;
using System.Reflection;

namespace BodyArchitect.Controls.Forms
{
	public class SplashScreenWindow : XtraForm
	{
		private const double OpacityDecrement = .08;
		private const double OpacityIncrement = .05;
		private const int TimerInterval = 50;
		private static Boolean FadeMode;
		private static Boolean FadeInOut;
		private static SplashScreenWindow SplashScreenForm;
		private static Thread SplashScreenThread;
		private static Color TransparentKey;
		private System.Windows.Forms.Timer SplashTimer;
		private LabelControl StatusLabel;
		private Panel panel1;
		private Panel panel2;
		private LabelControl lblVersionValue;
		private LabelControl lblVersion;
		private LabelControl labelControl1;
		private LabelControl labelControl2;
		private LabelControl lblBetaVersion;
		private UserControls.ProgressIndicator progressIndicator1;
		private ProgressBarControl progressBarControl1;
		private LabelControl labelControl3;
		private PictureBox pictureBox1;
		private Label label1;
		private Label label2;
		private Panel panel3;
		private IContainer components;
		private delegate void UpdateLabel();
		private delegate void CloseSplash();

		#region Public Properties & Methods

		
		public void SetStatus(string message,params object[] args)
		{
			if (StatusLabel.InvokeRequired)
			{
				Invoke(new Action<string, object[]>(SetStatus), message, args);
			}
			else
			{
				this.StatusLabel.Text = string.Format(message, args);
			}
		}

		public void SetProgressBar(bool visible,int max,int value)
		{
			if (InvokeRequired)
			{
				Invoke(new Action<bool, int, int>(SetProgressBar), visible, max,value);
			}
			else
			{
				progressBarControl1.Visible = visible;
				progressBarControl1.Properties.Maximum = max;
				progressBarControl1.EditValue = value;
			}
		}

		public Color SetTransparentKey
		{
			get { return TransparentKey; }
			set
			{
				TransparentKey = value;
				if (value != Color.Empty)
					TransparencyKey = SetTransparentKey;
			}
		}

		public Boolean SetFade
		{
			get { return FadeInOut; }
			set
			{
				FadeInOut = value;
				Opacity = value ? .00 : 1.00;
			}
		}

		public static SplashScreenWindow Current
		{
			get
			{
				if (SplashScreenForm == null)
					SplashScreenForm = new SplashScreenWindow();
				return SplashScreenForm;
			}
		}

		
		public void ShowSplashScreen()
		{
			SplashScreenThread = new Thread(new ThreadStart(ShowForm));
			SplashScreenThread.IsBackground = true;
			SplashScreenThread.Name = "SplashScreenThread";
			SplashScreenThread.Start();
		}

		public void CloseSplashScreen()
		{
			if (SplashScreenForm != null)
			{
				if(InvokeRequired)
				{
					Delegate ClosingDelegate = new CloseSplash(HideSplash);
					Invoke(ClosingDelegate);
				}
				else
				{
					HideSplash();
				}
			}
		}
		#endregion

		public SplashScreenWindow()
		{
			InitializeComponent();
			this.panel2.BackColor = Color.FromArgb(192, Color.White);
			lblVersionValue.Text = Assembly.GetEntryAssembly().GetName().Version.ToString();
			lblBetaVersion.Visible = Constants.IsBeta;
			
		}

		private static void ShowForm()
		{
			Application.Run(SplashScreenForm);
		}

		private void SplashTimer_Tick(object sender, EventArgs e)
		{
			if(FadeMode) // Form is opening (Increment)
			{
				if (Opacity < 1.00)
					Opacity += OpacityIncrement;
				else
				{
					SplashTimer.Stop();
					progressIndicator1.Start();
				}
			}
			else // Form is closing (Decrement)
			{
				if(Opacity > .00)
					Opacity -= OpacityDecrement;
				else
					Dispose();
			}
			
		}

		#region InitComponents

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreenWindow));
			this.SplashTimer = new System.Windows.Forms.Timer(this.components);
			this.StatusLabel = new DevExpress.XtraEditors.LabelControl();
			this.panel1 = new System.Windows.Forms.Panel();
			this.progressBarControl1 = new DevExpress.XtraEditors.ProgressBarControl();
			this.progressIndicator1 = new BodyArchitect.Controls.UserControls.ProgressIndicator();
			this.panel2 = new System.Windows.Forms.Panel();
			this.lblVersionValue = new DevExpress.XtraEditors.LabelControl();
			this.lblVersion = new DevExpress.XtraEditors.LabelControl();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.lblBetaVersion = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.panel3 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.progressBarControl1.Properties)).BeginInit();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// SplashTimer
			// 
			this.SplashTimer.Tick += new System.EventHandler(this.SplashTimer_Tick);
			// 
			// StatusLabel
			// 
			resources.ApplyResources(this.StatusLabel, "StatusLabel");
			this.StatusLabel.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("StatusLabel.Appearance.BackColor")));
			this.StatusLabel.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("StatusLabel.Appearance.ForeColor")));
			this.StatusLabel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.StatusLabel.Name = "StatusLabel";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.White;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.progressBarControl1);
			this.panel1.Controls.Add(this.progressIndicator1);
			this.panel1.Controls.Add(this.StatusLabel);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// progressBarControl1
			// 
			resources.ApplyResources(this.progressBarControl1, "progressBarControl1");
			this.progressBarControl1.Name = "progressBarControl1";
			// 
			// progressIndicator1
			// 
			resources.ApplyResources(this.progressIndicator1, "progressIndicator1");
			this.progressIndicator1.CircleColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.progressIndicator1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.progressIndicator1.Name = "progressIndicator1";
			this.progressIndicator1.Percentage = 0F;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.White;
			this.panel2.Controls.Add(this.lblVersionValue);
			this.panel2.Controls.Add(this.lblVersion);
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			// 
			// lblVersionValue
			// 
			this.lblVersionValue.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblVersionValue.Appearance.Font")));
			resources.ApplyResources(this.lblVersionValue, "lblVersionValue");
			this.lblVersionValue.Name = "lblVersionValue";
			// 
			// lblVersion
			// 
			resources.ApplyResources(this.lblVersion, "lblVersion");
			this.lblVersion.Name = "lblVersion";
			// 
			// labelControl3
			// 
			this.labelControl3.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelControl3.Appearance.Font")));
			this.labelControl3.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("labelControl3.Appearance.ForeColor")));
			resources.ApplyResources(this.labelControl3, "labelControl3");
			this.labelControl3.Name = "labelControl3";
			// 
			// lblBetaVersion
			// 
			this.lblBetaVersion.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblBetaVersion.Appearance.Font")));
			this.lblBetaVersion.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("lblBetaVersion.Appearance.ForeColor")));
			resources.ApplyResources(this.lblBetaVersion, "lblBetaVersion");
			this.lblBetaVersion.Name = "lblBetaVersion";
			// 
			// labelControl2
			// 
			this.labelControl2.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelControl2.Appearance.Font")));
			this.labelControl2.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("labelControl2.Appearance.ForeColor")));
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			// 
			// labelControl1
			// 
			this.labelControl1.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelControl1.Appearance.Font")));
			this.labelControl1.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("labelControl1.Appearance.ForeColor")));
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.Black;
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.BackColor = System.Drawing.Color.Black;
			this.label1.ForeColor = System.Drawing.Color.White;
			this.label1.Name = "label1";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.BackColor = System.Drawing.Color.Black;
			this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
			this.label2.Name = "label2";
			// 
			// panel3
			// 
			this.panel3.BackColor = System.Drawing.Color.Black;
			this.panel3.Controls.Add(this.lblBetaVersion);
			this.panel3.Controls.Add(this.labelControl3);
			this.panel3.Controls.Add(this.pictureBox1);
			this.panel3.Controls.Add(this.labelControl2);
			this.panel3.Controls.Add(this.label1);
			this.panel3.Controls.Add(this.label2);
			this.panel3.Controls.Add(this.labelControl1);
			resources.ApplyResources(this.panel3, "panel3");
			this.panel3.Name = "panel3";
			// 
			// SplashScreenWindow
			// 
			this.BackgroundImageLayoutStore = System.Windows.Forms.ImageLayout.Stretch;
			this.BackgroundImageStore = global::BodyArchitect.Controls.Icons.SplashScreen;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "SplashScreenWindow";
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.SplashScreen_Load);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.progressBarControl1.Properties)).EndInit();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private void SplashScreen_Load(object sender, EventArgs e)
		{
			if (SetFade)
			{
				FadeMode = true;
				SplashTimer.Interval = TimerInterval;
				SplashTimer.Start();
			}
		}

		private void HideSplash()
		{
			if(SetFade)
			{
				FadeMode = false;
				SplashTimer.Start();
			}
			else
				Dispose();
		}
	}
}