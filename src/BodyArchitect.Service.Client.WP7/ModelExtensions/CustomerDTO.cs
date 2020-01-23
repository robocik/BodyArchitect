using System;

namespace BodyArchitect.Service.V2.Model
{
    public interface IPerson
    {
        DateTime? Birthday { get; }

        Gender Gender { get; }

        WymiaryDTO Wymiary { get; }
    }

    public partial class CustomerDTO : IPerson
    {
        public string FullName
        {
            get { return string.Format("{0} {1}", LastName, FirstName); }
        }
    }
}
