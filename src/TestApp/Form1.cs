using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BodyArchitect.Common;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Logger;
using BodyArchitect.Module.StrengthTraining.Controls;
using BodyArchitect.Service;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                ExceptionHandler.Default.EmailFeaturesEnabled = false;
                //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
                NHibernateFactory.Initialize();

            }
            catch (System.Exception ex)
            {
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            

            
            

            ThreadPool.QueueUserWorkItem(WaitCallback);
            ThreadPool.QueueUserWorkItem(WaitCallback);

        }

        public void WaitCallback(Object state)
        {
            var client = new ClientInformation();
            client.Version = Const.ServiceVersion;
            client.ApplicationLanguage = "pl";
            client.ClientInstanceId = Guid.NewGuid();
            client.Platform = PlatformType.Windows;
            client.PlatformVersion = "1.0";
            client.ApplicationVersion = "1.0.0.0";
            
            for (int i = 0; i < 15; i++)
            {
                var session = NHibernateFactory.OpenSession();
                InternalBodyArchitectService service = new InternalBodyArchitectService(session);
                var sessionData = service.Login(client, "kox", "kwazar".ToSHA1Hash());
                session.Close();
            }
        }
    }

}
