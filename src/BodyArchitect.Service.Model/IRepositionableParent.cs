using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Service.Model
{
    public interface IRepositionableParent
    {
        void RepositionEntry(int index1, int index2);
    }

    public interface IRepositionableChild
    {
        int Position { get; }

        IRepositionableParent RepositionableParent { get; }
    }
  
}
