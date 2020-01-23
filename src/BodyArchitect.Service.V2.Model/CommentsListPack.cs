using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.V2.Model
{
    [DataContract(Namespace = Const.ServiceNamespace)]
    public class PagedResult<T>:IRetrievedDateTime
    {
        public PagedResult(IList<T> comments, int allItemsCount, int pageIndex)
        {
            Items = comments;
            AllItemsCount = allItemsCount;
            PageIndex = pageIndex;
        }

        public static PagedResult<T> CreateEmpty()
        {
            PagedResult<T> res = new PagedResult<T>(new List<T>(),0,0 );
            return res;
        }

        [DataMember]
        public IList<T> Items { get; private set; }

        [DataMember]
        public int AllItemsCount { get; private set; }

        [DataMember]
        public int PageIndex { get; private set; }

        [DataMember]
        public DateTime RetrievedDateTime
        {
            get;
            set;
        }
    }

    //[DataContract(Namespace = Const.ServiceNamespace)]
    //public class CommentsListPack
    //{
    //    public CommentsListPack(IList<CommentEntryDTO> comments, int allItemsCount, int pageIndex)
    //    {
    //        Comments = comments;
    //        AllItemsCount = allItemsCount;
    //        PageIndex = pageIndex;
    //    }

    //    [DataMember]
    //    public IList<CommentEntryDTO> Comments { get; private set; }

    //    [DataMember]
    //    public int AllItemsCount { get; private set; }

    //    [DataMember]
    //    public int PageIndex { get; private set; }
    //}
}
