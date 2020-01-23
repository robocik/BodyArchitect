using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BodyArchitect.Model
{
    public interface IPerson : IHasWymiar
    {
        DateTime? BirthdayDate { get; }

        Gender Gender { get;  }
    }

    public interface IHasWymiar
    {
        Wymiary Wymiary
        {
            get; set;
        }
    }
}
