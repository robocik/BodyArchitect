using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Client.UI.SchedulerEngine;
using BodyArchitect.Client.WCF;
using BodyArchitect.Logger;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Instructor
{
    public class ReminderOperationServiceCommand : IServiceCommand
    {
        private ReminderItemDTO reminder;
        private ReminderOperationType operation;

        public ReminderOperationServiceCommand(ReminderItemDTO reminder, ReminderOperationType operation)
        {
            this.reminder = reminder;
            this.operation = operation;
        }

        public void Execute()
        {
            try
            {
                ReminderOperationParam param = new ReminderOperationParam();
                param.Operation = operation;
                param.ReminderItemId = reminder.GlobalId;
                var temp = ServiceManager.ReminderOperation(param);
                if (temp == null)
                {
                    ReminderItemsReposidory.Instance.Remove(reminder.GlobalId);
                }
                else
                {
                    //ReminderItemsReposidory.Instance.Update(temp);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Default.Process(ex);
            }


        }
    }
}
