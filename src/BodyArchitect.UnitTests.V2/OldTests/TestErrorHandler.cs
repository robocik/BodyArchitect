using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Logger;
using NUnit.Framework;
using BodyArchitect.Settings;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestErrorHandler : ExceptionHandlerLogic
    {
        private bool showMessageBox;
        private bool showEmailBox;
        private bool applyAlways;
        private bool send;
        private bool startThread;
        private ErrorEventArgs errorArgs;

        public TestErrorHandler():base(true)
        {
            
        }


        [TestFixtureSetUp]
        public void TestSetup()
        {
            this.ShowEmailReportWindow += new EventHandler<ErrorEventArgs>(TestErrorHandler_ShowEmailReportWindow);
            this.ShowMessageBoxWindow += new EventHandler<ErrorEventArgs>(TestErrorHandler_ShowMessageBoxWindow);
        }

        void TestErrorHandler_ShowMessageBoxWindow(object sender, ErrorEventArgs e)
        {
            showMessageBox = true;
            errorArgs = e;
        }

        void TestErrorHandler_ShowEmailReportWindow(object sender, ErrorEventArgs e)
        {
            showEmailBox = true;
            e.ApplyAlways = applyAlways;
            e.ShouldSend = send;
            errorArgs = e;

        }

        protected override void StartEMailThread(Exception ex, Guid errorId)
        {
            startThread = true;
        }

        [SetUp]
        public void Setup()
        {
            errorArgs = null;
            //messages.Clear();
            showMessageBox = false;
            showEmailBox = false;
            startThread = false;
            errorArgs = null;
            ApplicationSettings.SendExceptionsWithoutQuestion = false;
            ApplicationSettings.AskForSendingException = true;
        }
        [Test]
        public void TestProcess_ExceptionOnly()
        {
            Exception ex = new ApplicationException("Test");
            Process(ex);

            assert(false, false, false, 0);
        }

        [Test]
        public void TestProcess_ExceptionMessageBox()
        {
            Exception ex = new ApplicationException("Test");
            string displayMessage = "msg";
            Process(ex, displayMessage,ErrorWindow.MessageBox);
            assert(true, false, false, 0);
            assertErrorArgs(true, displayMessage, false,null);
        }

        [Test]
        public void TestProcess_ExceptionEmailBox_DontSend()
        {
            Exception ex = new ApplicationException("Test");
            string displayMessage = "msg";
            Process(ex, displayMessage, ErrorWindow.EMailReport);
            assert(false, true, false, 0);
            assertErrorArgs(true, displayMessage, false,false);
        }

        [Test]
        public void TestProcess_ExceptionEmailBox_Send()
        {
            Exception ex = new ApplicationException("Test");
            string displayMessage = "msg";
            send = true;
            Process(ex, displayMessage, ErrorWindow.EMailReport);
            assert(false, true, true, 1);
            assertErrorArgs(true, displayMessage, true,false);
        }

        [Test]
        public void TestProcess_ExceptionEmailBox_SendAndApplyAlwaysSelected()
        {
            Exception ex = new ApplicationException("Test");
            string displayMessage = "msg";
            send = true;
            applyAlways = true;
            Process(ex, displayMessage, ErrorWindow.EMailReport);
            assert(false, true, true, 1);
            assertErrorArgs(true, displayMessage, true,true);
        }

        #region WithClientSettings

        [Test]
        //AskForSendingException=false, SendExceptionsWithoutQuestion=true
        public void TestProcess_ExceptionEmailBox_AutomaticSend()
        {
            Exception ex = new ApplicationException("Test");
            string displayMessage = "msg";
            ApplicationSettings.SendExceptionsWithoutQuestion = true;
            ApplicationSettings.AskForSendingException = false;
            Process(ex, displayMessage, ErrorWindow.EMailReport);
            assert(true, false, true, 1);
            assertErrorArgs(true, displayMessage, true, null);
        }

        [Test]
        //AskForSendingException=false, SendExceptionsWithoutQuestion=true
        public void TestProcess_ExceptionMessageBox_AutomaticSend()
        {
            Exception ex = new ApplicationException("Test");
            string displayMessage = "msg";
            ApplicationSettings.SendExceptionsWithoutQuestion = true;
            ApplicationSettings.AskForSendingException = false;
            Process(ex, displayMessage, ErrorWindow.MessageBox);
            assert(true, false, false, 0);
            assertErrorArgs(true, displayMessage, false, null);
        }
        #endregion

        void assert(bool showMsgBox,bool showEmail,bool startEmailThread,int msgCount)
        {
            Assert.AreEqual(showMsgBox, showMessageBox);
            Assert.AreEqual(showEmail, showEmailBox);
            Assert.AreEqual(startEmailThread, startThread);
            //Assert.AreEqual(msgCount, messages.Count);
            
        }

        void assertErrorArgs(bool errorArgsIsNotNull, string displayMessage, bool send,bool? applyAlways)
        {
            Assert.AreEqual(errorArgsIsNotNull, errorArgs != null);
            if (errorArgsIsNotNull)
            {
                Assert.AreEqual(displayMessage, errorArgs.DisplayMessage);
                Assert.AreEqual(send, errorArgs.ShouldSend);
                Assert.AreEqual(applyAlways, errorArgs.ApplyAlways);
            }
        }
    }
}
