using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Service.V2.Model
{
    public interface IMovable
    {
        void Move(DateTime newDateTime);
    }

    public interface IHasName
    {
        string Name { get; set; }
    }
}
