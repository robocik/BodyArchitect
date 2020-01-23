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

namespace BodyArchitect.Service.V2.Model
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "A6WDay", Namespace = "http://schemas.datacontract.org/2004/07/BodyArchitect.Service.V2.Model")]
    public class A6WDay : System.ComponentModel.INotifyPropertyChanged
    {

        private int dayField;

        private int repetitionNumberField;

        private int setNumberField;

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public int day
        {
            get
            {
                return this.dayField;
            }
            set
            {
                if ((this.dayField.Equals(value) != true))
                {
                    this.dayField = value;
                    this.RaisePropertyChanged("day");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public int repetitionNumber
        {
            get
            {
                return this.repetitionNumberField;
            }
            set
            {
                if ((this.repetitionNumberField.Equals(value) != true))
                {
                    this.repetitionNumberField = value;
                    this.RaisePropertyChanged("repetitionNumber");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public int setNumber
        {
            get
            {
                return this.setNumberField;
            }
            set
            {
                if ((this.setNumberField.Equals(value) != true))
                {
                    this.setNumberField = value;
                    this.RaisePropertyChanged("setNumber");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
