
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.WP7.Controls;

namespace BodyArchitect.WP7.ViewModel
{
    public class VoteViewModel:ViewModelBase
    {
        private CommentEntryDTO entry;

        public VoteViewModel()
        {
            
        }
        public VoteViewModel(CommentEntryDTO entry)
        {
            this.Entry = entry;
        }

        public PictureInfoDTO Picture
        {
            get
            {
                if (Entry.User.Picture != null)
                {
                    return Entry.User.Picture;
                }
                return PictureInfoDTO.Empty;
            }
            set { }
        }

        public string UserName
        {
            get { return Entry.User.UserName; }
            set { }
        }

        public string RatingText
        {
            get { return string.Format("{0}/{1}", Entry.Rating, Constants.MaxRatingValue); }
            set { }
        }

        public string Comment
        {
            get { return Entry.ShortComment; }
            set { }
        }

        public string DateTime
        {
            get { return Entry.VotedDate.ToLocalTime().ToRelativeDate(); }
            set { }
        }

        public CommentEntryDTO Entry
        {
            get { return entry; }
            set { entry = value; }
        }
    }
}
