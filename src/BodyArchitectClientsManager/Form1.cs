using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BodyArchitectClientsManager
{
    public partial class Form1 : Form
    {
        private ClientsInfo result;
        private ListViewColumnSorter lvwColumnSorter;
        public Form1()
        {
            InitializeComponent();
            lvwColumnSorter=new ListViewColumnSorter();
            this.lvAll.ListViewItemSorter = lvwColumnSorter;
            dateTimePicker2.Value = DateTime.Now.AddMonths(-2);
            startImplementation();
        }



        void filterLocal(ClientsInfo result)
        {
            try
            {

                List<string> users = new List<string>(txtMyUsers.Text.Split(','));
                for (int i = result.Clients.Count-1; i >= 0; i--)
                {
                    if (users.IndexOf(result.Clients[i].Name.ToLower()) > -1)
                    {
                        result.Clients.RemoveAt(i);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Bład przy filtrowaniu: "+e.Message);
            }
            

        }

        private void Start_Click(object sender, EventArgs e)
        {
            startImplementation();
        }

        private void startImplementation()
        {
            btnStart.Enabled = false;
            try
            {
                ClientsFileParser parser = new ClientsFileParser();
                DateTime? fromDate = null;
                if (chkGetFromDate.Checked)
                {
                    fromDate = dateTimePicker2.Value;
                }
                result = parser.Parse(fromDate, chkAllPaid.Checked);
                fill(result);
            }
            finally
            {
                btnStart.Enabled = true;
            }
        }

        private void fill(ClientsInfo result)
        {
            if (chkFilter.Checked)
            {
                filterLocal(result);
            }
            if(chkRemoveOld.Checked)
            {
                filterOld(result, txtOldDays.Value);
            }
            txtTodayEntries.Text = result.GetEntriesForDay(DateTime.UtcNow).Count.ToString();
            txtTodayUsers.Text = result.GetUsersForDay(DateTime.UtcNow).Count.ToString();
            txtEntriesCount.Text = result.Clients.Count.ToString();
            var uniqueInstances = result.GetUniqueUsers();
            txtUsersNumber.Text = uniqueInstances.Count.ToString();
            txtUniqueInstances.Text = result.GetUniqueInstances().Count.ToString();
            txtFullVersion.Text = result.GetFullVersionCount().ToString();
            fillInstanceTab(uniqueInstances);
            fillAllTab(result);
            fillRaports();
            fillPlatforms();
            fillPaid(result);
            fillOS(result,chkOsDetailed.Checked);
        }

        private void fillPaid(ClientsInfo results)
        {
            lvPaid.Items.Clear();
            cmbPaid.Items.Clear();
            foreach (string profile in results.GetFullVersionsProfiles())
            {
                cmbPaid.Items.Add(new ComboBoxItem(profile, profile));
            }
        }

        private void fillRaports()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.Add(new FirstRaport());
            comboBox1.Items.Add(new PaidRaport());
        }

        void fillPlatforms()
        {
            txtWinForms.Text = result.GetEntriesForPlatform(PlatformType.Windows).Count.ToString();
            txtAndroid.Text = result.GetEntriesForPlatform(PlatformType.Android).Count.ToString();
            txtWindowsPhone.Text = result.GetEntriesForPlatform(PlatformType.WindowsPhone).Count.ToString();
        }

        private void filterOld(ClientsInfo result, decimal value)
        {
            var instances =result.GetUniqueInstances();
            var list = (from i in instances
                        where ((DateTime.Now - i.Value.Max(t => t.Date)).TotalDays > (double) value) 
                        select i).ToDictionary(t => t.Key);

            for (int i = result.Clients.Count - 1; i >= 0; i--)
            {
                if (list.ContainsKey(result.Clients[i].InstanceId) && (!result.Clients[i].Version.StartsWith("Full") || result.Clients[i].Version == "Full 1.0.0"))
                {
                    result.Clients.RemoveAt(i);
                }
            }
        }

        void fillAllTab(ClientsInfo result)
        {
            //lvAll.BeginUpdate();
            lvAll.Items.Clear();
            lvAll.VirtualListSize = result.Clients.Count;
            //foreach (var clientInstance in result.Clients)
            //{
            //    int count = result.GetAllInstancesForUser(clientInstance.Name).Count;
            //    ListViewItem item = new ListViewItem(new string[] { clientInstance.Date.ToLocalTime().ToString(), clientInstance.Language, clientInstance.Version, clientInstance.InstanceId.ToString(), count.ToString(),clientInstance.Name,clientInstance.Platform.ToString() });
            //    bool isFirst=result.IsFirstRun(clientInstance);
            //    if(isFirst)
            //    {
            //        item.ImageKey = "New";
            //    }
            //    lvAll.Items.Add(item);
            //}
            //lvAll.EndUpdate();
        }

        void fillOS(ClientsInfo result,bool detailed)
        {
            lvOS.Items.Clear();
            lvOSUsers.Items.Clear();
            var os = result.GetOS(detailed);
            foreach (var o in os)
            {
                var item = new ListViewItem(new string[] {o.Key, o.Value.Count.ToString()});
                item.Tag = o.Value;
                lvOS.Items.Add(item);
            }
        }

        void fillInstanceTab(Dictionary<string,List<ClientInstance>> data)
        {
            listView1.Items.Clear();
            cmbInstances.Items.Clear();
            
            foreach (KeyValuePair<string, List<ClientInstance>> pair in data)
            {
                cmbInstances.Items.Add(new ComboBoxItem(pair.Value, string.Format("{0}           ({1})", pair.Key, pair.Value.Count)));
            }
        }

        private void cmbInstances_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            ComboBoxItem selectedItem=(ComboBoxItem)cmbInstances.SelectedItem;
            if(selectedItem!=null)
            {
                foreach (var instance in (List<ClientInstance>)selectedItem.Tag)
                {
                    ListViewItem item =
                        new ListViewItem(new string[] { instance.Date.ToLocalTime().ToString(), instance.Language, instance.Version,instance.Platform.ToString(),instance.InstanceId.ToString(),instance.AccountType.ToString() });
                    listView1.Items.Add(item);
                }
                txtNumberOfRuns.Text = listView1.Items.Count.ToString();
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            IReport raport = (IReport)comboBox1.SelectedItem;
            raport.GenerateReport(chart1,result);
        }

        
        private void lvAll_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if(e.Column==0)
            {
                lvwColumnSorter.SortType = typeof (DateTime);
            }
            else if(e.Column==4)
            {
                lvwColumnSorter.SortType = typeof(int);
            }
            else
            {
                lvwColumnSorter.SortType = typeof(string);
            }
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.lvAll.Sort();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            txtTodayEntries.Text = result.GetEntriesForDay(dateTimePicker1.Value.ToUniversalTime()).Count.ToString();
            txtTodayUsers.Text = result.GetUsersForDay(dateTimePicker1.Value.ToUniversalTime()).Count.ToString();
        }

        private void cmbPaid_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvPaid.Items.Clear();
            var item1 = (ComboBoxItem)cmbPaid.SelectedItem;
            string profile = (string)item1.Tag;
            var list=result.GetAllInstancesForUser(profile);
            foreach (var instance in list)
            {
                ListViewItem item =
                    new ListViewItem(new string[] { instance.Date.ToLocalTime().ToString(), instance.Language, instance.Version, instance.Platform.ToString(), instance.InstanceId.ToString() });
                lvPaid.Items.Add(item);
            }
        }

        private void lvAll_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var clientInstance = result.Clients[e.ItemIndex];
            int count = result.GetAllInstancesForUser(clientInstance.Name).Count;
            ListViewItem item = new ListViewItem(new string[] { clientInstance.Date.ToLocalTime().ToString(), clientInstance.Language, clientInstance.Version, clientInstance.InstanceId.ToString(), count.ToString(), clientInstance.Name, clientInstance.Platform.ToString(),clientInstance.AccountType.ToString() });
            bool isFirst = result.IsFirstRun(clientInstance);
            if (isFirst)
            {
                item.ImageKey = "New";
            }
            e.Item = item;
        }

        private void lvOS_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvOSUsers.Items.Clear();
            if(lvOS.SelectedItems.Count>0)
            {
                var items = (List<Tuple<string, int>>)lvOS.SelectedItems[0].Tag;
                foreach (var user in items)
                {
                    ListViewItem item =
                        new ListViewItem(new string[] { user.Item1,user.Item2.ToString() });
                    lvOSUsers.Items.Add(item);
                }
            }
        }

        private void chkOsDetailed_CheckedChanged(object sender, EventArgs e)
        {
            fillOS(result,chkOsDetailed.Checked);
        }
    }

    class ComboBoxItem
    {
        private object tag;
        private string text;

        public ComboBoxItem(object tag, string text)
        {
            this.tag = tag;
            this.text = text;
        }

        public ComboBoxItem()
        {
        }

        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public override string ToString()
        {
            return text;
        }
    }
}
