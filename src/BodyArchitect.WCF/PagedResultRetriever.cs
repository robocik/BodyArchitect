using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Service.Model;

namespace BodyArchitect.WCF
{
    public class PagedResultRetriever
    {
        public PagedResultRetriever()
        {
            PageSize = 100;
        }
        public int PageSize { get; set; }

        public List<T> GetAll<T>(Func<PartialRetrievingInfo, PagedResult<T>> retrieveMethod)
        {
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            List<T> resultList = new List<T>();
            PagedResult<T> pack=null;
            do
            {
                pack = retrieveMethod(pageInfo);

                resultList.AddRange(pack.Items);
                pageInfo.PageIndex = pageInfo.PageIndex + 1;

            } while (pack.AllItemsCount > resultList.Count);
            return resultList;
        }
    }
}
