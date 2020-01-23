using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using BodyArchitect.Service.Model;
using BodyArchitect.Shared;

namespace BodyArchitect.Controls
{
    public class MyTrainingProgressComponent:Component
    {
        StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel slCurrentTraining;
        private System.Windows.Forms.ToolStripProgressBar tpbMyTrainingProgress;
        private System.Windows.Forms.ToolStripDropDownButton tdbMyTrainings;


        public MyTrainingProgressComponent()
        {
            this.tdbMyTrainings = new System.Windows.Forms.ToolStripDropDownButton();
            this.slCurrentTraining = new System.Windows.Forms.ToolStripStatusLabel();
            this.tpbMyTrainingProgress = new System.Windows.Forms.ToolStripProgressBar();

            this.tdbMyTrainings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;

            
            
            slCurrentTraining.Visible = false;
            tpbMyTrainingProgress.Visible = false;
            
        }

        public StatusStrip StatusBar
        {
            get
            {
                return statusBar;
            }
            set
            {
                if (statusBar != null)
                {
                    this.statusBar.Items.Remove(this.slCurrentTraining);
                    this.statusBar.Items.Remove(this.tpbMyTrainingProgress);
                    this.statusBar.Items.Insert(0, this.tdbMyTrainings);
                }
                statusBar = value;
                if (statusBar != null)
                {
                    this.statusBar.Items.Insert(0,this.slCurrentTraining);
                    this.statusBar.Items.Insert(1,this.tpbMyTrainingProgress);
                    this.statusBar.Items.Insert(0, this.tdbMyTrainings);
                    
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int? MyTrainingId
        {
            get
            {
                var id = UserContext.Settings.GuiState.A6WTrainingId;
                if (id == Constants.UnsavedObjectId)
                {
                    return null;
                }
                else
                {
                    return id;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    UserContext.Settings.GuiState.A6WTrainingId = value.Value;
                }
                else
                {
                    UserContext.Settings.GuiState.A6WTrainingId = 0;
                }
            }
        }

        void fillTrainings(int? myTrainingId)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(Object id)
            {
                fillProgress(myTrainingId);
                fillTrainingList();
            }), myTrainingId);
            
        }



        private void fillTrainingList()
        {
            if (statusBar.InvokeRequired)
            {
                statusBar.BeginInvoke(new Action(fillTrainingList));
            }
            else
            {
                tdbMyTrainings.DropDownItems.Clear();
                //var myTrainings = ServicesManager.GetService<IMyTrainingFactory>().GetStartedTraining(SessionData);
                //var myTrainings = MyTraining.GetStartedTrainings(SessionData.ProfileId);
                //foreach (var myTraining in myTrainings)
                //{
                //    ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem(myTraining.Name);
                //    toolStripMenuItem3.Tag = myTraining;
                //    toolStripMenuItem3.Click += new EventHandler(toolStripMenuItem3_Click);
                //    if (MyTrainingId.HasValue)
                //    {
                //        toolStripMenuItem3.Checked = myTraining.Id == MyTrainingId.Value;
                //    }
                //    tdbMyTrainings.DropDownItems.Add(toolStripMenuItem3);
                //}
                //TODO:Must be implemented
            }
        }

        private void fillProgress(int? myTrainingId)
        {
            if (statusBar.InvokeRequired)
            {
                statusBar.BeginInvoke(new Action<int?>(fillProgress), myTrainingId);
            }
            else
            {
                bool showProgress = myTrainingId.HasValue && MyTrainingId.HasValue && MyTrainingId.Value == myTrainingId;
                if (showProgress)
                {
                    //var myTraining = MyTraining.GetById(MyTrainingId.Value,false);
                    //;// ServicesManager.GetService<IMyTrainingFactory>().GetMyTraining(SessionData, myTrainingId.Value);
                    //showProgress = myTraining != null && myTraining.PercentageCompleted.HasValue;
                    
                    
                    //if (showProgress )
                    //{
                    //    slCurrentTraining.Text = myTraining.Name;
                    //    tpbMyTrainingProgress.Value = myTraining.PercentageCompleted.Value;
                    //}

                    //if (myTraining==null || myTraining.EndDate != null)
                    //{
                    //    MyTrainingId = null;
                    //}
                    //TODO:Must be implemented
                }
                slCurrentTraining.Visible = showProgress;
                this.tpbMyTrainingProgress.Visible = showProgress;
            }
        }

        void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mnuItem = (ToolStripMenuItem)sender;
            mnuItem.Checked = !mnuItem.Checked;
            
            if (mnuItem.Checked)
            {
                MyTrainingDTO myTraining = (MyTrainingDTO)mnuItem.Tag;
                MyTrainingId = myTraining.Id;
            }
            else
            {
                MyTrainingId = null;
            }
            

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(Object id)
            {
                fillProgress((int?)id);
            }), MyTrainingId);
        }

        void myTrainingNotification(Type objectTYpe, object id)
        {
            fillTrainings((int)id);
            
        }

        public void Start()
        {
            fillTrainings(MyTrainingId);
        }
    }
}
