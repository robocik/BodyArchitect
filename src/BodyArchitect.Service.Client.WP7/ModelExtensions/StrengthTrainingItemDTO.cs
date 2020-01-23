using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BodyArchitect.Service.Client.WP7;
using BodyArchitect.Service.Client.WP7.ModelExtensions;

namespace BodyArchitect.Service.V2.Model
{
    public partial class StrengthTrainingItemDTO : ICommentable
    {
        public StrengthTrainingItemDTO()
        {
            Series=new List<SerieDTO>();
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [UseSameReference]
        public BodyArchitect.Service.V2.Model.ExerciseLightDTO Exercise
        {
            get
            {
                return this.ExerciseField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ExerciseField, value) != true))
                {
                    this.ExerciseField = value;
                    this.RaisePropertyChanged("Exercise");
                }
            }
        }

        public void AddSerie(SerieDTO set)
        {
            Series.Add(set);
            set.StrengthTrainingItem = this;
        }

        public List<StrengthTrainingItemDTO>  GetJoinedItems()
        {
            List<StrengthTrainingItemDTO> items = new List<StrengthTrainingItemDTO>();
            if(!string.IsNullOrEmpty(SuperSetGroup))
            {
                items.AddRange(StrengthTrainingEntry.Entries.Where(x=>x.SuperSetGroup==SuperSetGroup && x!=this));
            }
            return items;
        }
    }
}
