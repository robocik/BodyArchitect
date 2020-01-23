//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Security;
//using System.Xml;
//using System.Xml.Linq;


//namespace BodyArchitect.Service.V2.Model.TrainingPlans
//{
//    public abstract class XmlSerializationBaseFormatter
//    {
//        protected string dateToString(DateTime date)
//        {
//            return date.ToString(CultureInfo.InvariantCulture);
//        }

//        protected  void addSimpleElement(string name, object value, XDocument doc, XElement parent)
//        {
//            if (value != null)
//            {
//                var node = new XElement(name);
//                node.Value = value.ToString();
//                parent.Add(node);
//            }

//        }

//        protected void addCDataElement(string name, object value, XDocument doc, XElement parent)
//        {

//            if (value != null)
//            {
//                //var node = doc.CreateElement(name);
//                //node.AppendChild(doc.CreateCDataSection(value.ToString()));
//                //parent.AppendChild(node);
//                var node = new XElement(name);
//                var cdata = new XCData(value.ToString());
//                node.Add(cdata);
//                parent.Add(node);
//            }

//        }

//        protected DateTime? readDateTime(XElement parentElement, string propertyName)
//        {
//            string value = readValue(parentElement, propertyName);
//            if (!string.IsNullOrEmpty(value))
//            {
//                return DateTime.Parse(value, CultureInfo.InvariantCulture);
//            }
//            return null;
//        }

//        protected TimeSpan? readTimeSpan(XElement parentElement, string propertyName)
//        {
//            string value = readValue(parentElement, propertyName);
//            if (!string.IsNullOrEmpty(value))
//            {
//                return TimeSpan.Parse(value, CultureInfo.InvariantCulture);
//            }
//            return null;
//        }

//        protected double? readDouble(XElement parentElement, string propertyName)
//        {
//            string value = readValue(parentElement, propertyName);
//            if (!string.IsNullOrEmpty(value))
//            {
//                return double.Parse(value);
//            }
//            return null;
//        }

//        protected int? readInt(XElement parentElement, string propertyName)
//        {
//            string value = readValue(parentElement, propertyName);
//            if (!string.IsNullOrEmpty(value))
//            {
//                return int.Parse(value);
//            }
//            return null;
//        }

//        protected Guid? readGuid(XElement parentElement, string propertyName)
//        {
//            string value = readValue(parentElement, propertyName);
//            if (!string.IsNullOrEmpty(value))
//            {
//                return new Guid(value);
//            }
//            return null;
//        }

//        //private void readExercises(XDocument xml, TrainingPlanPack pack)
//        //{
//        //    foreach (var node in xml.Element("TrainingPlanPack").Element("Exercises").Descendants("ExerciseDTO"))
//        //    {
//        //        ExerciseDTO exercise = new ExerciseDTO();
//        //        exercise.GlobalId = new Guid(node.Element("GlobalId").Value);
//        //        exercise.Description = readValue(node, "Description");
//        //        exercise.Name = readValue(node, "Name");
//        //        exercise.Difficult = readEnum<ExerciseDifficult>(node, "Difficult");
//        //        exercise.ExerciseType = readEnum<ExerciseType>(node, "Muscle");
//        //        exercise.Shortcut = readValue(node, "Shortcut");
//        //        exercise.Url = readValue(node, "Url");
//        //        pack.Exercises.Add(exercise.GlobalId);
//        //    }
//        //}

//        protected string readValue(XElement parentNode, string propertyName)
//        {
//            var node = parentNode.Element(propertyName);
//            if (node != null)
//            {
//                var value = parentNode.Element(propertyName).Value;
//                return value;
//            }
//            return null;
//        }

//        protected T readEnum<T>(XElement parentNode, string propertyName)
//        {
//            string value = readValue(parentNode, propertyName);
//            if (!string.IsNullOrEmpty(value))
//            {
//                return (T)Enum.Parse(typeof(T), value, true);
//            }
//            return default(T);
//        }
//    }
//    public class XmlSerializationTrainingPlanFormatter : XmlSerializationBaseFormatter
//    {
//        #region Save
//        //public void Save(Stream targetStream, TrainingPlanPack planPack, string signKey = null)
//        //{
			
//        //    //XmlDocument doc = new XmlDocument();
//        //    XDocument xDoc = new XDocument(new XDeclaration("1.0", null, null));
//        //    var node = new XElement("TrainingPlanPack");
//        //    var attr = new XAttribute("Version",planPack.Version);
//        //    node.Add(node);
//        //    //var xmlDeclaration = doc.CreateXmlDeclaration("1.0", null, null);

//        //    //doc.AppendChild(xmlDeclaration);


//        //    //XmlElement node = doc.CreateElement("TrainingPlanPack");
//        //    //var versionAttr = doc.CreateAttribute("Version");
//        //    //versionAttr.Value = planPack.Version;

//        //    //node.Attributes.Append(versionAttr);
//        //    doc.AppendChild(node);
//        //    var exercisesNode=doc.CreateElement("Exercises");
			
//        //    node.AppendChild(exercisesNode);
			
//        //    saveExercises(planPack.Exercises, doc, exercisesNode);
//        //    saveTrainingPlan(planPack.TrainingPlan, doc, node,signKey);

//        //    if (!string.IsNullOrEmpty(signKey))
//        //    {
//        //        DataSigner.SignXml(doc, signKey);
//        //    }
//        //    doc.Save(targetStream);
//        //    //targetStream.Close();
//        //}

		

//        //void saveExercises(List<Guid> exercises,XmlDocument doc,XmlElement parentNode)
//        //{
			
//        //    foreach (var exercise in exercises)
//        //    {
//        //        var exerciseNode = doc.CreateElement("ExerciseDTO");
//        //        parentNode.AppendChild(exerciseNode);
//        //        var exerciseIdAttr = doc.CreateAttribute("GlobalId");
//        //        exerciseIdAttr.Value = exercise.ToString();
//        //        exerciseNode.Attributes.Append(exerciseIdAttr);
//        //        //addSimpleElement("Name", exercise.Name, doc, exerciseNode);
//        //        //addSimpleElement("GlobalId", exercise.GlobalId, doc, exerciseNode);
//        //        //addSimpleElement("Description", exercise.Description, doc, exerciseNode);
//        //        //addSimpleElement("Difficult", exercise.Difficult, doc, exerciseNode);
//        //        //addSimpleElement("Shortcut", exercise.Shortcut, doc, exerciseNode);
//        //        //addSimpleElement("Url", exercise.Url, doc, exerciseNode);
//        //        //addSimpleElement("Muscle", exercise.Muscle, doc, exerciseNode);
//        //        //addSimpleElement("ForceType", exercise.ExerciseForceType, doc, exerciseNode);
//        //        //addSimpleElement("MechanicsType", exercise.MechanicsType, doc, exerciseNode);
//        //    }
//        //}

		

//        public XDocument ToXml(TrainingPlan plan)
//        {
//            XDocument xDoc = new XDocument(new XDeclaration("1.0", null, null));
//            var trainingPlanNode = new XElement("TrainingPlan");
//            xDoc.Add(trainingPlanNode);
//            //XmlDocument doc = new XmlDocument();
//            //var trainingPlanNode = doc.CreateElement("TrainingPlan");
//            //doc.AppendChild(trainingPlanNode);

//            addSimpleElement("Purpose", plan.Purpose, xDoc, trainingPlanNode);
//            addSimpleElement("Language", plan.Language, xDoc, trainingPlanNode);
//            addSimpleElement("Author", plan.Author, xDoc, trainingPlanNode);
//            addSimpleElement("BasedOnId", plan.BasedOnId, xDoc, trainingPlanNode);
//            addCDataElement("Comment", plan.Comment, xDoc, trainingPlanNode);
//            addSimpleElement("CreationDate", dateToString(plan.CreationDate), xDoc, trainingPlanNode);
//            addSimpleElement("Difficult", plan.Difficult, xDoc, trainingPlanNode);
//            addSimpleElement("GlobalId", plan.GlobalId, xDoc, trainingPlanNode);
//            addSimpleElement("Name", plan.Name, xDoc, trainingPlanNode);
//            addSimpleElement("RestSeconds", plan.RestSeconds, xDoc, trainingPlanNode);
//            addSimpleElement("TrainingType", plan.TrainingType, xDoc, trainingPlanNode);
//            addSimpleElement("Url", plan.Url, xDoc, trainingPlanNode);

//            var trainingPlanDaysNode = new XElement("Weeks");
//            trainingPlanNode.Add(trainingPlanDaysNode);
//            //var trainingPlanDaysNode = doc.CreateElement("Weeks");
//            //trainingPlanNode.AppendChild(trainingPlanDaysNode);
//            foreach (var day in plan.Days)
//            {
//                var dayNode = new XElement("Day");
//                //var dayNode = doc.CreateElement("Day");
//                //trainingPlanDaysNode.AppendChild(dayNode);
//                trainingPlanDaysNode.Add(dayNode);
//                addSimpleElement("GlobalId", day.GlobalId, xDoc, dayNode);
//                addSimpleElement("Name", day.Name, xDoc, dayNode);

//                saveTrainingPlanEntries(xDoc, dayNode, day);

//                saveSuperSets(xDoc, dayNode, day);
//            }
//            return xDoc;
//        }
//        //void saveTrainingPlan(TrainingPlan plan,XmlDocument doc, XmlElement parentNode,string signKey)
//        //{
//        //    XmlDocument trainingPlanDoc = ToXml(plan);
//        //    if (!string.IsNullOrEmpty(signKey))
//        //    {
//        //        DataSigner.SignXml(trainingPlanDoc, signKey);
//        //    }
//        //    var trainingPlanNode=doc.ImportNode(trainingPlanDoc.DocumentElement, true);
//        //    parentNode.AppendChild(trainingPlanNode);

//        //}

//        private void saveSuperSets(XDocument doc, XElement dayNode, TrainingPlanDay day)
//        {
//            //var superSetsNode = doc.CreateElement("SuperSets");
//            //dayNode.AppendChild(superSetsNode);
//            var superSetsNode = new XElement("SuperSets");
//            dayNode.Add(superSetsNode);
//            foreach (var superSet in day.SuperSets)
//            {
//                //var superSetNode = doc.CreateElement("SuperSet");
//                //superSetsNode.AppendChild(superSetNode);
//                var superSetNode = new XElement("SuperSet");
//                superSetsNode.Add(superSetNode);
//                addSimpleElement("SuperSetId", superSet.SuperSetId, doc, superSetNode);
//                //var entriesNode = doc.CreateElement("Entries");
//                //superSetNode.AppendChild(entriesNode);
//                var entriesNode = new XElement("Entries");
//                superSetNode.Add(entriesNode);
//                foreach (var planEntry in superSet.SuperSets)
//                {
//                    addSimpleElement("EntryId", planEntry.GlobalId, doc, entriesNode);
//                }
//            }
//        }

//        private void saveTrainingPlanEntries(XDocument doc, XElement dayNode, TrainingPlanDay day)
//        {
//            //var entriesNode = doc.CreateElement("Entries");
//            //dayNode.AppendChild(entriesNode);
//            var entriesNode = new XElement("Entries");
//            dayNode.Add(entriesNode);
//            foreach (var entry in day.Entries)
//            {
//                //var entryNode = doc.CreateElement("Entry");
//                //entriesNode.AppendChild(entryNode);
//                var entryNode = new XElement("Entry");
//                entriesNode.Add(entryNode);
//                addCDataElement("Comment", entry.Comment, doc, entryNode);
//                addSimpleElement("ExerciseId", entry.Exercise.GlobalId, doc, entryNode);
//                addSimpleElement("GlobalId", entry.GlobalId, doc, entryNode);
//                addSimpleElement("RestSeconds", entry.RestSeconds, doc, entryNode);

//                //var setsNode = doc.CreateElement("Sets");
//                //entryNode.AppendChild(setsNode);
//                var setsNode = new XElement("Sets");
//                entryNode.Add(setsNode);
//                foreach (var series in entry.Sets)
//                {
//                    //var setNode = doc.CreateElement("Set");
//                    //setsNode.AppendChild(setNode);
//                    var setNode=new XElement("Set");
//                    setsNode.Add(setNode);
//                    addCDataElement("Comment", series.Comment, doc, setNode);
//                    addSimpleElement("RepetitionNumberMax", series.RepetitionNumberMax, doc, setNode);
//                    addSimpleElement("RepetitionNumberMin", series.RepetitionNumberMin, doc, setNode);
//                    addSimpleElement("RepetitionsType", series.RepetitionsType, doc, setNode);
//                    addSimpleElement("GlobalId", series.GlobalId, doc, setNode);
//                    addSimpleElement("DropSet", series.DropSet, doc, setNode);
//                }
//            }
//        }

		
//        #endregion

//        #region Read

//        //public TrainingPlanPack Read(Stream sourceStream)
//        //{

//        //    var reader = XmlReader.Create(sourceStream);
//        //    var xml = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
//        //    //sourceStream.Close();
//        //    TrainingPlanPack pack = new TrainingPlanPack();
//        //    readExercises(xml, pack);

			
//        //    var trainingPlanNode = xml.Element("TrainingPlanPack").Element("TrainingPlan");

//        //    string xmlContent = trainingPlanNode.ToString();
			
//        //    pack.TrainingPlan = FromXml(xmlContent);
//        //    //check if there is Signature node in the training plan file. if yes then verifify this plan
//        //    //XNamespace xNamespace = "http://www.w3.org/2000/09/xmldsig#";
//        //    //var signNode = xml.Root.Element(xNamespace + "Signature");
//        //    //if (signNode != null)
//        //    //{
//        //    //    XmlDocument doc = new XmlDocument();
//        //    //    doc.LoadXml(xml.Root.ToString());


//        //    //    bool verified = DataSigner.VerifyXml(doc, Constants.VerificationPublicKey);
//        //    //    if (verified == false)
//        //    //    {
//        //    //        throw new VerificationException("Imporing training plan cannot be verified");
//        //    //    }

//        //    //    pack.Signed = true;
//        //    //}

//        //    return pack;
//        //}

//        public TrainingPlan FromXml(string xmlContent, Func<IEnumerable<Guid>,Dictionary<Guid,ExerciseLightDTO>> getExercises)
//        {
//            try
//            {
//                TrainingPlan plan = new TrainingPlan();
//                //if (DataSigner.IsSigned(doc))
//                //{
//                //    bool verified = DataSigner.VerifyXml(doc, Constants.VerificationPublicKey);
//                //    if (verified == false)
//                //    {
//                //        throw new VerificationException("Imporing training plan cannot be verified");
//                //    }
//                //}
//                XDocument xml = XDocument.Parse(xmlContent);

//                var trainingPlanElement = xml.Element("TrainingPlan");
//                plan.Author = readValue(trainingPlanElement, "Author");
//                plan.Language = readValue(trainingPlanElement, "Language");
//                plan.BasedOnId = readGuid(trainingPlanElement, "BasedOnId");
//                plan.Comment = readValue(trainingPlanElement, "Comment");
//                plan.CreationDate = readDateTime(trainingPlanElement, "CreationDate").Value;
//                plan.Difficult = readEnum<TrainingPlanDifficult>(trainingPlanElement, "Difficult");
//                plan.Purpose = readEnum<WorkoutPlanPurpose>(trainingPlanElement, "Purpose");
//                plan.GlobalId = readGuid(trainingPlanElement, "GlobalId").Value;
//                //plan.ProfileId = readGuid(trainingPlanElement, "ProfileId").Value;
//                plan.Name = readValue(trainingPlanElement, "Name");
//                plan.Url = readValue(trainingPlanElement, "Url");
//                plan.RestSeconds = readInt(trainingPlanElement, "RestSeconds").Value;
//                plan.TrainingType = readEnum<TrainingType>(trainingPlanElement, "TrainingType");

//                Dictionary<Guid, Guid> exerciseIds = new Dictionary<Guid, Guid>();
//                foreach (var dayNode in trainingPlanElement.Element("Weeks").Descendants("Day"))
//                {
//                    TrainingPlanDay day = new TrainingPlanDay();
//                    plan.AddDay(day);
//                    day.GlobalId = readGuid(dayNode, "GlobalId").Value;
//                    day.Name = readValue(dayNode, "Name");

//                    readTrainingEntries(day, dayNode, exerciseIds);

//                    readSuperSets(day, dayNode);
//                }
//                var exercises = getExercises(exerciseIds.Values);
//                fillExercises(plan,exercises);
//                return plan;
//            }
//            catch(VerificationException)
//            {
//                throw;
//            }
//            catch(Exception)
//            {
//                throw new FormatException("Selected xml doesn't contain training plan or it is damaged");
//            }
			
//        }

//        private void fillExercises(TrainingPlan plan,IDictionary<Guid, ExerciseLightDTO> exercises)
//        {
//            foreach (var planDay in plan.Days)
//            {
//                foreach (var planEntry in planDay.Entries)
//                {
//                    if(exercises.ContainsKey(planEntry.Exercise.GlobalId))
//                    {
//                        planEntry.Exercise = exercises[planEntry.Exercise.GlobalId];
//                    }
//                }
//            }
//        }

//        private void readSuperSets(TrainingPlanDay day, XElement dayNode)
//        {
//            foreach (var superSetNode in dayNode.Element("SuperSets").Descendants("SuperSet"))
//            {
//                SuperSet superSet = new SuperSet();
//                day.SuperSets.Add(superSet);
//                superSet.SuperSetId = readGuid(superSetNode, "SuperSetId").Value;
//                foreach (var entryIdNode in superSetNode.Element("Entries").Descendants("EntryId"))
//                {
//                    var entry = day.GetEntry(new Guid(entryIdNode.Value));
//                    superSet.SuperSets.Add(entry);
//                }
//            }
//        }

//        private void readTrainingEntries(TrainingPlanDay day, XElement dayNode, Dictionary<Guid, Guid> exerciseIds)
//        {
//            foreach (var entryNode in dayNode.Element("Entries").Descendants("Entry"))
//            {
//                TrainingPlanEntry entry = new TrainingPlanEntry();
//                day.AddEntry(entry);
//                entry.GlobalId = readGuid(entryNode, "GlobalId").Value;
//                entry.Comment = readValue(entryNode, "Comment");


//                var exerciseId = readGuid(entryNode, "ExerciseId").Value;
//                if (!exerciseIds.ContainsKey(exerciseId))
//                {
//                    exerciseIds.Add(exerciseId,exerciseId);
//                }
//                entry.Exercise = new ExerciseLightDTO() { GlobalId = exerciseId, Name = "Exercise not found" };//temporary only

//                entry.RestSeconds = readInt(entryNode, "RestSeconds").Value;

//                foreach (var setNode in entryNode.Element("Sets").Descendants("Set"))
//                {
//                    TrainingPlanSerie set = new TrainingPlanSerie();
//                    entry.Sets.Add(set);
//                    set.Comment = readValue(setNode, "Comment");
//                    set.GlobalId = readGuid(setNode, "GlobalId").Value;
//                    set.RepetitionNumberMax = readInt(setNode, "RepetitionNumberMax");
//                    set.RepetitionNumberMin = readInt(setNode, "RepetitionNumberMin");
//                    if (readValue(setNode, "RepetitionsType") == "NotSet")
//                    {
//                        set.RepetitionsType = SetType.Normalna;
//                    }
//                    else
//                    {
//                        set.RepetitionsType = readEnum<SetType>(setNode, "RepetitionsType");
//                    }

//                    set.DropSet = readEnum<DropSetType>(setNode, "DropSet");
//                }

//            }
//        }

		
//        #endregion
//    }
//}
