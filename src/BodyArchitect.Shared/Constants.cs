using System;
using System.Drawing;


namespace BodyArchitect.Shared
{
    public static class Constants
    {
        
        public const string APIKey = "14375345-3755-46F7-AF3F-0D328E3A2CC0";
        public static readonly Guid UnsavedGlobalId=Guid.Empty ;
        public static readonly Color PanelBackColor = Color.White;
        public const int ColorFieldLength=8;
        public const int UnsavedObjectId = 0;
        public const int DefaultBatchSize = 20;
        public const string ApplicationName = "BodyArchitect";
        public const string DefaultColor = "FF00BFFF";
        public const decimal NotSet = 0;
        
        
        
        public const bool IsBeta = false;
        public const string ClipboardFormat = "BaTrainingDay";
        public const int NameColumnLength = 100;
        public const int CommentColumnLength = 4000;
        public const int ShortCommentColumnLength = 500;
        public const int ProfileStatusColumnLength = 200;
        public const int DefaultTimeout = 10000;
        

        public const int UrlLength = 1000;
        public const int VoteStarsNumber = 6;

        
        public const string AvalonLayoutFile = @".\AvalonDock.config";

    }
}
