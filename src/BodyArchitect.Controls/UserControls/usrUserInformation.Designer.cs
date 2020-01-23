namespace BodyArchitect.Controls.UserControls
{
    partial class usrUserInformation
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(usrUserInformation));
            this.naviBar1 = new Guifreaks.Navisuite.NaviBar(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnInviteAFriend = new DevExpress.XtraEditors.SimpleButton();
            this.btnAcceptFriend = new DevExpress.XtraEditors.SimpleButton();
            this.btnRejectFriendship = new DevExpress.XtraEditors.SimpleButton();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnOpenWorkoutsLog = new DevExpress.XtraEditors.SimpleButton();
            this.btnReports = new DevExpress.XtraEditors.SimpleButton();
            this.btnEditProfile = new DevExpress.XtraEditors.SimpleButton();
            this.btnSendMessage = new DevExpress.XtraEditors.SimpleButton();
            this.btnAddToFavorites = new DevExpress.XtraEditors.SimpleButton();
            this.btnRemoveFromFavorites = new DevExpress.XtraEditors.SimpleButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // naviBar1
            // 
            this.naviBar1.ActiveBand = null;
            resources.ApplyResources(this.naviBar1, "naviBar1");
            this.naviBar1.Name = "naviBar1";
            this.naviBar1.ShowCollapseButton = false;
            this.naviBar1.ShowMoreOptionsButton = false;
            this.naviBar1.VisibleLargeButtons = 5;
            this.naviBar1.ActiveBandChanged += new System.EventHandler(this.naviBar1_ActiveBandChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.naviBar1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.btnInviteAFriend);
            this.flowLayoutPanel1.Controls.Add(this.btnAcceptFriend);
            this.flowLayoutPanel1.Controls.Add(this.btnRejectFriendship);
            this.flowLayoutPanel1.Controls.Add(this.btnRefresh);
            this.flowLayoutPanel1.Controls.Add(this.btnOpenWorkoutsLog);
            this.flowLayoutPanel1.Controls.Add(this.btnReports);
            this.flowLayoutPanel1.Controls.Add(this.btnEditProfile);
            this.flowLayoutPanel1.Controls.Add(this.btnSendMessage);
            this.flowLayoutPanel1.Controls.Add(this.btnAddToFavorites);
            this.flowLayoutPanel1.Controls.Add(this.btnRemoveFromFavorites);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // btnInviteAFriend
            // 
            resources.ApplyResources(this.btnInviteAFriend, "btnInviteAFriend");
            this.btnInviteAFriend.Image = ((System.Drawing.Image)(resources.GetObject("btnInviteAFriend.Image")));
            this.btnInviteAFriend.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnInviteAFriend.Name = "btnInviteAFriend";
            this.btnInviteAFriend.Click += new System.EventHandler(this.btnInviteAFriend_Click);
            // 
            // btnAcceptFriend
            // 
            resources.ApplyResources(this.btnAcceptFriend, "btnAcceptFriend");
            this.btnAcceptFriend.Image = ((System.Drawing.Image)(resources.GetObject("btnAcceptFriend.Image")));
            this.btnAcceptFriend.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnAcceptFriend.Name = "btnAcceptFriend";
            this.btnAcceptFriend.Click += new System.EventHandler(this.btnAcceptFriend_Click);
            // 
            // btnRejectFriendship
            // 
            this.btnRejectFriendship.Image = ((System.Drawing.Image)(resources.GetObject("btnRejectFriendship.Image")));
            this.btnRejectFriendship.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            resources.ApplyResources(this.btnRejectFriendship, "btnRejectFriendship");
            this.btnRejectFriendship.Name = "btnRejectFriendship";
            this.btnRejectFriendship.Click += new System.EventHandler(this.btnRejectFriendship_Click);
            // 
            // btnRefresh
            // 
            resources.ApplyResources(this.btnRefresh, "btnRefresh");
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnOpenWorkoutsLog
            // 
            this.btnOpenWorkoutsLog.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenWorkoutsLog.Image")));
            this.btnOpenWorkoutsLog.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            resources.ApplyResources(this.btnOpenWorkoutsLog, "btnOpenWorkoutsLog");
            this.btnOpenWorkoutsLog.Name = "btnOpenWorkoutsLog";
            this.btnOpenWorkoutsLog.Click += new System.EventHandler(this.btnOpenWorkoutsLog_Click);
            // 
            // btnReports
            // 
            this.btnReports.Image = ((System.Drawing.Image)(resources.GetObject("btnReports.Image")));
            this.btnReports.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            resources.ApplyResources(this.btnReports, "btnReports");
            this.btnReports.Name = "btnReports";
            this.btnReports.Click += new System.EventHandler(this.btnReports_Click);
            // 
            // btnEditProfile
            // 
            resources.ApplyResources(this.btnEditProfile, "btnEditProfile");
            this.btnEditProfile.Image = ((System.Drawing.Image)(resources.GetObject("btnEditProfile.Image")));
            this.btnEditProfile.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnEditProfile.Name = "btnEditProfile";
            this.btnEditProfile.Click += new System.EventHandler(this.btnEditProfile_Click);
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Image = ((System.Drawing.Image)(resources.GetObject("btnSendMessage.Image")));
            this.btnSendMessage.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            resources.ApplyResources(this.btnSendMessage, "btnSendMessage");
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // btnAddToFavorites
            // 
            this.btnAddToFavorites.Image = ((System.Drawing.Image)(resources.GetObject("btnAddToFavorites.Image")));
            this.btnAddToFavorites.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            resources.ApplyResources(this.btnAddToFavorites, "btnAddToFavorites");
            this.btnAddToFavorites.Name = "btnAddToFavorites";
            this.btnAddToFavorites.Click += new System.EventHandler(this.btnAddToFavorites_Click);
            // 
            // btnRemoveFromFavorites
            // 
            this.btnRemoveFromFavorites.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveFromFavorites.Image")));
            this.btnRemoveFromFavorites.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            resources.ApplyResources(this.btnRemoveFromFavorites, "btnRemoveFromFavorites");
            this.btnRemoveFromFavorites.Name = "btnRemoveFromFavorites";
            this.btnRemoveFromFavorites.Click += new System.EventHandler(this.btnRemoveFromFavorites_Click);
            // 
            // usrUserInformation
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "usrUserInformation";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Guifreaks.Navisuite.NaviBar naviBar1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevExpress.XtraEditors.SimpleButton btnInviteAFriend;
        private DevExpress.XtraEditors.SimpleButton btnAcceptFriend;
        private DevExpress.XtraEditors.SimpleButton btnRejectFriendship;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraEditors.SimpleButton btnOpenWorkoutsLog;
        private DevExpress.XtraEditors.SimpleButton btnReports;
        private DevExpress.XtraEditors.SimpleButton btnEditProfile;
        private DevExpress.XtraEditors.SimpleButton btnSendMessage;
        private DevExpress.XtraEditors.SimpleButton btnAddToFavorites;
        private DevExpress.XtraEditors.SimpleButton btnRemoveFromFavorites;
    }
}
