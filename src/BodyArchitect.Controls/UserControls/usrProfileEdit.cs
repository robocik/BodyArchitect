using System.IO;
using System.Windows.Forms;
using BodyArchitect.WCF;
using BodyArchitect.Common;
using BodyArchitect.Controls.Cache;
using BodyArchitect.Controls.Forms;
using BodyArchitect.Controls.Localization;
using System;
using BodyArchitect.Service.Model;

namespace BodyArchitect.Controls.UserControls
{
    public partial class usrProfileEdit : usrBaseControl
    {
        private ProfileDTO profile;
        public event EventHandler<ControlValidatedEventArgs> ControlValidated;

        private ProfileInformationDTO profileInfo;

        public usrProfileEdit()
        {
            InitializeComponent();
        }
        
        public void Fill(ProfileDTO profile)
        {
            UserContext.RefreshUserData();
            profileInfo = UserContext.ProfileInformation.Clone();
            this.profile = profile.Clone();
            chkAutomaticUpdateMeasurements.Checked = this.profile.Settings.AutomaticUpdateMeasurements;
            usrCreateProfile1.Fill(profile);
            usrWymiaryEditor1.Fill(profileInfo.Wymiary);
            usrProfilePrivacy1.Fill(profile);
            usrProfileNotifications1.Fill(profile);
        }


        public ProfileDTO SaveProfile()
        {
            bool valid = true;
            this.ParentWindow.SynchronizationContext.Send(delegate
            {
                usrProfileNotifications1.Save(profile);
                usrProfilePrivacy1.Save(profile);
                if(!usrCreateProfile1.SaveProfile(profile))
                {
                    valid = false;
                    return;
                }
                var picture = usrProfilePersonalInfo1.Save(profile);
                profileInfo.Wymiary = usrWymiaryEditor1.SaveWymiary(profileInfo.Wymiary);
                if (picture != null && (profile.Picture == null || usrProfilePersonalInfo1.ForceUploadImage || picture.Hash != profile.Picture.Hash))
                {
                    var info = ServiceManager.UploadImage(picture);
                    picture.PictureId = info.PictureId;
                    profile.Picture = info;
                    PicturesCache.Instance.AddToCache(picture.ToPictureCacheItem());
                }
                profile.Settings.AutomaticUpdateMeasurements = chkAutomaticUpdateMeasurements.Checked;
            }, null);

            if (valid)
            {
                ProfileUpdateData data = new ProfileUpdateData();
                data.Profile = profile;
                data.Wymiary = profileInfo.Wymiary;
                var res = ServiceManager.UpdateProfile(data);
                return res;
            }
            return null;
        }


        protected void OnControlValidated(bool isValid)
        {
            if(ControlValidated!=null)
            {
                ControlValidated(this,new ControlValidatedEventArgs(isValid));
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (tpPersonalInfo == e.Page)
            {
                
                ParentWindow.RunAsynchronousOperation(delegate
                        {
                            usrProfilePersonalInfo1.Fill(profile);
                        }, this.GetParentControl<ProfileEditWindow>().UpdateProgressIndicator);
            }
        }

        private void chkAutomaticUpdateMeasurements_CheckedChanged(object sender, EventArgs e)
        {
            usrWymiaryEditor1.ReadOnly = chkAutomaticUpdateMeasurements.Checked;
        }
    }

    class StreamWithProgress : Stream
    {
        private readonly Stream file;
        private readonly long length;

        public class ProgressChangedEventArgs : EventArgs
        {
            public long BytesRead;
            public long Length;

            public ProgressChangedEventArgs(long BytesRead, long Length)
            {
                this.BytesRead = BytesRead;
                this.Length = Length;
            }
        }

        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

        private long bytesRead;

        public StreamWithProgress(Stream file)
        {
            this.file = file;
            length = file.Length;
            bytesRead = 0;
            if (ProgressChanged != null) ProgressChanged(this, new ProgressChangedEventArgs(bytesRead, length));
        }

        public double GetProgress()
        {
            return ((double)bytesRead) / file.Length;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush() { }

        public override long Length
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public override long Position
        {
            get { return bytesRead; }
            set { throw new Exception("The method or operation is not implemented."); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int result = file.Read(buffer, offset, count);
            bytesRead += result;
            if (ProgressChanged != null) ProgressChanged(this, new ProgressChangedEventArgs(bytesRead, length));
            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void SetLength(long value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
