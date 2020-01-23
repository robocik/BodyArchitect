using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Service.V2.Model
{
    public interface IServiceResult<T>
    {
        T MyResult { get; }
    }
    public interface IServicePagedResult<T> where T : BAGlobalObject
    {
        IPagedResult<T> MyResult { get; }
    }
    public interface IPagedResult<T> where T:BAGlobalObject
    {
        int AllItemsCount
        {
            get; set;
        }

        System.Collections.Generic.List<T> Items { get;
            set;
        }

        int PageIndex
        { 
            get;
            set;
        }

        System.DateTime RetrievedDateTime
        {
            get; set;
        }
    }
}
