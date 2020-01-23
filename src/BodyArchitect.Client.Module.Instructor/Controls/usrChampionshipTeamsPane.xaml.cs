using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using BodyArchitect.Client.Common;
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Client.Module.Instructor.Controls
{
    /// <summary>
    /// Interaction logic for usrChampionshipTeamsPane.xaml
    /// </summary>
    public partial class usrChampionshipTeamsPane 
    {
        
        private string comment;
        private ChampionshipView championshipView;


        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                ChampionshipView.Championship.Comment = comment;
                ChampionshipView.SetModifiedFlag();
                NotifyOfPropertyChange(() => Comment);
            }
        }

        public ChampionshipView ChampionshipView
        {
            get { return championshipView; }
        }

        public usrChampionshipTeamsPane()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void Fill(ChampionshipView championshipView)
        {
            this.championshipView = championshipView;
            
            if (championshipView.Championship != null)
            {
                
                Comment = championshipView.Championship.Comment;
            }
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            ListItem<ChampionshipGroupDTO> teamItem = e.Item as ListItem<ChampionshipGroupDTO>;
            e.Accepted= teamItem.Value != null;
        }
    }
}
