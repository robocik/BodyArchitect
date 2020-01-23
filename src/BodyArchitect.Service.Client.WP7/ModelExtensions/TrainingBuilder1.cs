using System;
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
using BodyArchitect.Service.V2.Model;

namespace BodyArchitect.Service.V2.Model.TrainingPlans
{
    public partial class TrainingBuilder
    {
        public void PrepareCopiedStrengthTraining(StrengthTrainingItemDTO dto, CopyStrengthTrainingMode mode)
        {
            if (Settings.CopyStrengthTrainingMode == CopyStrengthTrainingMode.OnlyExercises)
            {
                dto.Series.Clear();
            }
            else if (Settings.CopyStrengthTrainingMode == CopyStrengthTrainingMode.WithoutSetsData)
            {
                foreach (var serieDto in dto.Series)
                {
                    serieDto.RepetitionNumber = null;
                    serieDto.Weight = null;
                }
            }
        }

        public void PrepareCopiedStrengthTraining(StrengthTrainingEntryDTO entry, CopyStrengthTrainingMode mode)
        {
            foreach (var dto in entry.Entries)
            {
                PrepareCopiedStrengthTraining(dto, mode);
            }
            
        }


        public void SetPreviewSets(StrengthTrainingItemDTO origItem, StrengthTrainingItemDTO item)
        {
            for (int i = 0; i < origItem.Series.Count; i++)
            {
                item.Series[i].Tag = origItem.Series[i];
            }
        }
        public void SetPreviewSets(StrengthTrainingEntryDTO origEntry, StrengthTrainingEntryDTO entry)
        {
            var origSets=origEntry.Entries.SelectMany(x=>x.Series).ToList();
            var copiedSets = entry.Entries.SelectMany(x => x.Series).ToList();
            for (int i = 0; i < origSets.Count; i++)
            {
                copiedSets[i].Tag = origSets[i];
            }
        }

        public void CleanSingleSupersets(StrengthTrainingEntryDTO entry)
        {
            var entries = entry.Entries.Where(x => !string.IsNullOrEmpty(x.SuperSetGroup)).ToList();

            foreach (var item in entries)
            {
                if (entry.Entries.Count(x => x.SuperSetGroup == item.SuperSetGroup) < 2)
                {
                    item.SuperSetGroup = null;
                }
            }
        }
    }
}
