using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BodyArchitect.Service.Model
{
    public interface IMovable
    {
        void Move(DateTime newDateTime);
    }
}
