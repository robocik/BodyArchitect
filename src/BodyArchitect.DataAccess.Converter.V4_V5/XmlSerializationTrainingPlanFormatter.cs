using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Xml.Linq;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using NHibernate;
using TrainingPlan = BodyArchitect.Model.TrainingPlan;
using TrainingPlanDay = BodyArchitect.Model.TrainingPlanDay;
using TrainingPlanDifficult = BodyArchitect.Model.TrainingPlanDifficult;
using TrainingPlanEntry = BodyArchitect.Model.TrainingPlanEntry;
using TrainingPlanSerie = BodyArchitect.Model.TrainingPlanSerie;
using TrainingType = BodyArchitect.Model.TrainingType;
using BodyArchitect.Logger;

namespace BodyArchitect.DataAccess.Converter.V4_V5
{
    public class XmlSerializationTrainingPlanFormatter
    {
        private Dictionary<Guid, Guid> oldNewGuidMap = new Dictionary<Guid, Guid>();
        private Model.Exercise deletedExercise;

        public XmlSerializationTrainingPlanFormatter(Exercise deletedExercise)
        {
            this.deletedExercise = deletedExercise;
        }

        public Dictionary<Guid, Guid> OldNewGuidMap
        {
            get { return oldNewGuidMap; }
        }

        #region Read

        public TrainingPlan FromXml(IStatelessSession session, string xmlContent)
        {
            try
            {
                TrainingPlan plan = new TrainingPlan();
                XDocument xml = XDocument.Parse(xmlContent);

                var trainingPlanElement = xml.Element("TrainingPlan");
                plan.Author = readValue(trainingPlanElement, "Author");
                plan.Language = readValue(trainingPlanElement, "Language");
                //plan.BasedOnId = readGuid(trainingPlanElement, "BasedOnId");
                plan.Comment = readValue(trainingPlanElement, "Comment");
                plan.CreationDate = readDateTime(trainingPlanElement, "CreationDate").Value;
                plan.Difficult = readEnum<TrainingPlanDifficult>(trainingPlanElement, "Difficult");
                plan.Purpose = readEnum<WorkoutPlanPurpose>(trainingPlanElement, "Purpose");
                plan.GlobalId = readGuid(trainingPlanElement, "GlobalId").Value;
                //plan.ProfileId = readGuid(trainingPlanElement, "ProfileId").Value;
                plan.Name = readValue(trainingPlanElement, "Name");
                plan.Url = readValue(trainingPlanElement, "Url");
                plan.RestSeconds = readInt(trainingPlanElement, "RestSeconds").Value;
                plan.TrainingType = readEnum<TrainingType>(trainingPlanElement, "TrainingType");
                int position = 0;
                foreach (var dayNode in trainingPlanElement.Element("Days").Descendants("Day"))
                {
                    TrainingPlanDay day = new TrainingPlanDay();
                    plan.Days.Add(day);
                    day.TrainingPlan = plan;
                    day.Position = position;
                    day.GlobalId = readGuid(dayNode, "GlobalId").Value;
                    day.Name = readValue(dayNode, "Name");

                    readTrainingEntries(session, day, dayNode);

                    readSuperSets(day, dayNode);
                    position++;
                }

                return plan;
            }
            catch (VerificationException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new FormatException("Selected xml doesn't contain training plan or it is damaged");
            }

        }

        private void readSuperSets(TrainingPlanDay day, XElement dayNode)
        {
            foreach (var superSetNode in dayNode.Element("SuperSets").Descendants("SuperSet"))
            {
                var superSetId = readGuid(superSetNode, "SuperSetId").Value;
                foreach (var entryIdNode in superSetNode.Element("Entries").Descendants("EntryId"))
                {
                    var oldEntryId = new Guid(entryIdNode.Value);
                    if (entriesMap.ContainsKey(oldEntryId))
                    {
                        var entry = entriesMap[oldEntryId];
                        if (entry != null)
                        {
                            entry.GroupName = superSetId.ToString();
                        }
                    }
                    else
                    {
                        Debug.Fail("Here should be an entry!");
                    }

                }
            }
        }

        Dictionary<Guid, TrainingPlanEntry> entriesMap = new Dictionary<Guid, TrainingPlanEntry>();
        private void readTrainingEntries(IStatelessSession session, TrainingPlanDay day, XElement dayNode)
        {
            int position = 0;
            foreach (var entryNode in dayNode.Element("Entries").Descendants("Entry"))
            {
                TrainingPlanEntry entry = new TrainingPlanEntry();
                day.Entries.Add(entry);
                entry.Position = position;
                entry.Day = day;
                entry.GlobalId = readGuid(entryNode, "GlobalId").Value;
                entriesMap.Add(readGuid(entryNode, "GlobalId").Value, entry);
                entry.Comment = readValue(entryNode, "Comment");
                entry.Exercise = session.Get<Exercise>(readGuid(entryNode, "ExerciseId").Value);
                if (entry.Exercise == null)
                {
                    entry.Exercise = deletedExercise;
                }
                entry.RestSeconds = readInt(entryNode, "RestSeconds").Value;
                position++;

                foreach (var setNode in entryNode.Element("Sets").Descendants("Set"))
                {
                    TrainingPlanSerie set = new TrainingPlanSerie();
                    entry.Sets.Add(set);
                    set.Entry = entry;
                    set.Comment = readValue(setNode, "Comment");
                    set.GlobalId = readGuid(setNode, "GlobalId").Value;
                    set.RepetitionNumberMax = readInt(setNode, "RepetitionNumberMax");
                    set.RepetitionNumberMin = readInt(setNode, "RepetitionNumberMin");
                    if (readValue(setNode, "RepetitionsType") == "NotSet")
                    {
                        set.RepetitionsType = SetType.Normalna;
                    }
                    else
                    {
                        set.RepetitionsType = readEnum<SetType>(setNode, "RepetitionsType");
                    }

                    set.DropSet = readEnum<DropSetType>(setNode, "DropSet");
                }

            }
        }

        private DateTime? readDateTime(XElement parentElement, string propertyName)
        {
            string value = readValue(parentElement, propertyName);
            if (!string.IsNullOrEmpty(value))
            {
                return DateTime.Parse(value, CultureInfo.InvariantCulture);
            }
            return null;
        }

        private int? readInt(XElement parentElement, string propertyName)
        {
            string value = readValue(parentElement, propertyName);
            if (!string.IsNullOrEmpty(value))
            {
                return int.Parse(value);
            }
            return null;
        }

        private Guid? readGuid(XElement parentElement, string propertyName)
        {
            string value = readValue(parentElement, propertyName);
            if (!string.IsNullOrEmpty(value))
            {
                return new Guid(value);
            }
            return null;
        }

        string readValue(XElement parentNode, string propertyName)
        {
            var node = parentNode.Element(propertyName);
            if (node != null)
            {
                var value = parentNode.Element(propertyName).Value;
                return value;
            }
            return null;
        }

        T readEnum<T>(XElement parentNode, string propertyName)
        {
            string value = readValue(parentNode, propertyName);
            if (!string.IsNullOrEmpty(value))
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            return default(T);
        }
        #endregion

    }
}
