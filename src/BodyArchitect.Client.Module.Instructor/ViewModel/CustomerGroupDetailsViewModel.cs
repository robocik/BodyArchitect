using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using BodyArchitect.Client.Common;
using BodyArchitect.Client.Module.Instructor.Controls.Groups;
using BodyArchitect.Client.UI;
using BodyArchitect.Client.UI.Cache;
using BodyArchitect.Client.UI.Windows;
using BodyArchitect.Service.V2.Model.Instructor;

namespace BodyArchitect.Client.Module.Instructor.ViewModel
{
    public class CustomerGroupDetailsViewModel : ViewModelBase, IWeakEventListener
    {
        private CustomerGroupDTO group;
        ObservableCollection<GroupMemberItem> members = new ObservableCollection<GroupMemberItem>();
        ObservableCollection<GroupMemberItem> customers = new ObservableCollection<GroupMemberItem>();
        private IBAWindow parentView;

        public CustomerGroupDetailsViewModel(IBAWindow parentView,CustomerGroupDTO @group)
        {
            this.parentView = parentView;
            this.group = group;
            CollectionChangedEventManager.AddListener(CustomersReposidory.Instance, this);
            foreach (var customerDto in group.Customers)
            {
                members.Add(new GroupMemberItem(customerDto));
            }
            fillCustomers();
            ValidateCustomers();
            ValidateMembers();
        }

        private void fillCustomers()
        {
            customers.Clear();
//use only not virtual customers. Virtual customers cannot be part of a group
            foreach (var customerDto in CustomersReposidory.Instance.Items.Values.Where(x => !x.IsVirtual))
            {
                customers.Add(new GroupMemberItem(customerDto));
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            parentView.SynchronizationContext.Send(state =>
            {
                fillCustomers();
            }, null);

            return true;
        }

        #region Properties

        public CustomerGroupDTO Group
        {
            get { return group; }
        }

        public bool IsNotRestricted
        {
            get { return group.RestrictedType == CustomerGroupRestrictedType.None; }
            set
            {
                if (value)
                {
                    group.RestrictedType = CustomerGroupRestrictedType.None;
                    ValidateMembers();
                    ValidateCustomers();
                }
            }
        }

        public bool IsPartiallyRestricted
        {
            get { return group.RestrictedType == CustomerGroupRestrictedType.Partially; }
            set
            {
                if (value)
                {
                    group.RestrictedType = CustomerGroupRestrictedType.Partially;
                    ValidateMembers();
                    ValidateCustomers();
                }
            }
        }

        public bool IsFullyRestricted
        {
            get { return group.RestrictedType == CustomerGroupRestrictedType.Full; }
            set
            {
                if (value)
                {
                    group.RestrictedType = CustomerGroupRestrictedType.Full;
                    ValidateMembers();
                    ValidateCustomers();
                }
            }
        }

        public IList<GroupMemberItem> Members
        {
            get { return members; }
        }

        public IList<GroupMemberItem> Customers
        {
            get { return customers; }
        }
        #endregion

        public void AddMember(CustomerDTO customer)
        {
            if (Members.Any(x => x.Customer.GlobalId == customer.GlobalId))
            {
                BAMessageBox.ShowError("Error_CustomerGroupDetailsViewModel_AddMember_AlreadyInGroup".TranslateInstructor());
                return;
            }
            var viewModel = new GroupMemberItem(customer);

            Members.Add(viewModel);
            ValidateMembers();
            ValidateCustomers();
        }

        public void DeleteMember(CustomerDTO customer)
        {
            var customerViewModel = Members.SingleOrDefault(x => x.Customer.GlobalId == customer.GlobalId);
            Members.Remove(customerViewModel);
            ValidateMembers();
            ValidateCustomers();
        }

        public void ValidateMembers()
        {
            foreach (var memberItem in Members)
            {
                var customerInOtherGroups = CustomerGroupsReposidory.Instance.Items.Values.Where(x => x.GlobalId != group.GlobalId).SelectMany(
                    x => x.Customers).Where(x => x.GlobalId == memberItem.Customer.GlobalId).ToList();
                memberItem.IsError = group.RestrictedType == CustomerGroupRestrictedType.Full && customerInOtherGroups.Count > 0;
                if(memberItem.IsError)
                {
                    memberItem.ErrorToolTip = "Error_CustomerGroupDetailsViewModel_AddMember_InAnotherGroup".TranslateInstructor();
                }
                else
                {
                    memberItem.ErrorToolTip = null;
                }

                if (!memberItem.IsError)
                {
                    customerInOtherGroups =CustomerGroupsReposidory.Instance.Items.Values.Where(x => x.GlobalId != group.GlobalId && x.RestrictedType != CustomerGroupRestrictedType.None).
                            SelectMany(x => x.Customers).Where(x => x.GlobalId == memberItem.Customer.GlobalId).ToList();
                    memberItem.IsError = group.RestrictedType != CustomerGroupRestrictedType.None && customerInOtherGroups.Count > 0;
                    if (memberItem.IsError)
                    {
                        memberItem.ErrorToolTip = "Error_CustomerGroupDetailsViewModel_AddMember_InRestrictedGroup".TranslateInstructor();
                    }
                    else
                    {
                        memberItem.ErrorToolTip = null;
                    }
                }
            }
        }

        public void ValidateCustomers()
        {
            foreach (var memberItem in Customers)
            {
                var customerInOtherGroups = CustomerGroupsReposidory.Instance.Items.Values.Where(x => x.GlobalId != group.GlobalId).SelectMany(
                    x => x.Customers).Where(x => x.GlobalId == memberItem.Customer.GlobalId).ToList();
                memberItem.IsEnabled = !(group.RestrictedType == CustomerGroupRestrictedType.Full && customerInOtherGroups.Count > 0);


                if (memberItem.IsEnabled)
                {
                    customerInOtherGroups =
                        CustomerGroupsReposidory.Instance.Items.Values.Where(
                            x => x.GlobalId != group.GlobalId && x.RestrictedType != CustomerGroupRestrictedType.None).
                            SelectMany(
                                x => x.Customers).Where(x => x.GlobalId == memberItem.Customer.GlobalId).ToList();
                    memberItem.IsEnabled = !(group.RestrictedType != CustomerGroupRestrictedType.None && customerInOtherGroups.Count > 0);
                }
            }
        }

        public void Apply()
        {
            group.Customers.Clear();
            group.Customers.AddRange(Members.Select(x=>x.Customer));
        }
    }
}
