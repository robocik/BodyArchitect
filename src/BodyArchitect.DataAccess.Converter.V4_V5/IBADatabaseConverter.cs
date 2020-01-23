using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BodyArchitect.DataAccess.Converter.V4_V5
{

    [ServiceContract]
    public interface IBADatabaseConverter
    {
        [OperationContract]
        void Convert();

        [OperationContract]
        void CreateDb();
        
        [OperationContract]
        void FixTrainingPlanSetsPositions();
    }

    [Serializable]
    public class BADatabaseCallbackParam
    {
        public BADatabaseCallbackParam()
        {
            
        }
        public BADatabaseCallbackParam(string mainOperation)
        {
            MainOperation = mainOperation;
        }

        public BADatabaseCallbackParam(string detailOpeation,int current,int max)
        {
            DetailOperation = detailOpeation;
            Current = current;
            Max = max;
        }
        public string MainOperation { get; set; }

        public string DetailOperation { get; set; }

        public int Max { get; set; }

        public int Current { get; set; }
    }

    public interface IBADatabaseCallback
    {
        [OperationContract(IsOneWay = true)]
        void ConvertProgressChanged(BADatabaseCallbackParam param);

    }
}
