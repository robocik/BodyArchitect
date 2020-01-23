//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security;
//using System.Text;
//using System.Xml.Linq;
//using BodyArchitect.Service.V2.Model.TrainingPlans;

//namespace BodyArchitect.Service.V2.Model.SupplementsCycleEngine
//{
//    public class XmlSerializationSupplementsCycleDefinitionFormatter : XmlSerializationBaseFormatter
//    {
//        #region Save

//        public XDocument ToXml(SupplementCycleDefinitionDTO cycle)
//        {
//            XDocument xDoc = new XDocument(new XDeclaration("1.0", null, null));
//            var supplementsCycleNode = new XElement("SupplementsCycle");
//            xDoc.Add(supplementsCycleNode);
//            addSimpleElement("Name", cycle.Name, xDoc, supplementsCycleNode);
//            addSimpleElement("GlobalId", cycle.GlobalId, xDoc, supplementsCycleNode);

//            var cycleDaysNode = new XElement("Weeks");
//            supplementsCycleNode.Add(cycleDaysNode);
//            //var trainingPlanDaysNode = doc.CreateElement("Weeks");
//            //trainingPlanNode.AppendChild(trainingPlanDaysNode);
//            foreach (var day in cycle.Weeks)
//            {
//                var dayNode = new XElement("Week");
//                //var dayNode = doc.CreateElement("Day");
//                //trainingPlanDaysNode.AppendChild(dayNode);
//                cycleDaysNode.Add(dayNode);
//                addSimpleElement("GlobalId", day.GlobalId, xDoc, dayNode);
//                addSimpleElement("CycleWeekStart", day.CycleWeekStart, xDoc, dayNode);
//                addSimpleElement("CycleWeekEnd", day.CycleWeekEnd, xDoc, dayNode);
//                addSimpleElement("Name", day.Name, xDoc, dayNode);
                
//                saveDosages(xDoc, dayNode, day);
//            }
//            return xDoc;
//        }

//        private void saveDosages(XDocument doc, XElement dayNode, SupplementCycleWeekDTO week)
//        {
//            var entriesNode = new XElement("Dosages");
//            dayNode.Add(entriesNode);
//            foreach (var entry in week.Dosages)
//            {
//                //var entryNode = doc.CreateElement("Entry");
//                //entriesNode.AppendChild(entryNode);
//                var entryNode = new XElement("DosageItem");
//                entriesNode.Add(entryNode);
//                addSimpleElement("Repetitions", entry.Repetitions, doc, entryNode);
//                addSimpleElement("Dosage", entry.Dosage, doc, entryNode);
//                addSimpleElement("DosageType", entry.DosageType, doc, entryNode);
//                addSimpleElement("DosageUnit", entry.DosageUnit, doc, entryNode);
//                addSimpleElement("SupplementId", entry.Supplement.GlobalId, doc, entryNode);
//                addSimpleElement("GlobalId", entry.GlobalId, doc, entryNode);
//                addSimpleElement("Time", entry.TimeType, doc, entryNode);
//                addSimpleElement("GroupName", entry.GroupName, doc, entryNode);

//            }
//        }

//        #endregion
//        public SupplementCycleDefinitionDTO FromXml(string xmlContent, Func<IEnumerable<Guid>, Dictionary<Guid, SuplementDTO>> getSupplements)
//        {
//            try
//            {
//                var plan = new SupplementCycleDefinitionDTO();
//                //if (DataSigner.IsSigned(doc))
//                //{
//                //    bool verified = DataSigner.VerifyXml(doc, Constants.VerificationPublicKey);
//                //    if (verified == false)
//                //    {
//                //        throw new VerificationException("Imporing training plan cannot be verified");
//                //    }
//                //}
//                XDocument xml = XDocument.Parse(xmlContent);

//                var trainingPlanElement = xml.Element("SupplementsCycle");
//                plan.Name = readValue(trainingPlanElement, "Name");
//                plan.GlobalId = readGuid(trainingPlanElement, "GlobalId").Value;
                
//                Dictionary<Guid, Guid> exerciseIds = new Dictionary<Guid, Guid>();
//                foreach (var dayNode in trainingPlanElement.Element("Weeks").Descendants("Week"))
//                {
//                    SupplementCycleWeekDTO week = new SupplementCycleWeekDTO();
//                    plan.Weeks.Add(week);
//                    week.GlobalId = readGuid(dayNode, "GlobalId").Value;
//                    week.Name = readValue(dayNode, "Name");
//                    week.CycleWeekStart = readInt(dayNode, "CycleWeekStart").Value;
//                    week.CycleWeekEnd = readInt(dayNode, "CycleWeekEnd").Value;
                    

//                    readDosages(week, dayNode, exerciseIds);
//                }
//                var supplements = getSupplements(exerciseIds.Values);
//                fillSupplements(plan, supplements);
//                return plan;
//            }
//            catch (VerificationException)
//            {
//                throw;
//            }
//            catch (Exception)
//            {
//                throw new FormatException("Selected xml doesn't contain training plan or it is damaged");
//            }
//        }

//        private void fillSupplements(SupplementCycleDefinitionDTO plan, Dictionary<Guid, SuplementDTO> supplements)
//        {
//            foreach (var planDay in plan.Weeks)
//            {
//                foreach (var planEntry in planDay.Dosages)
//                {
//                    if (supplements.ContainsKey(planEntry.Supplement.GlobalId))
//                    {
//                        planEntry.Supplement = supplements[planEntry.Supplement.GlobalId];
//                    }
//                }
//            }
//        }

//        private void readDosages(SupplementCycleWeekDTO week, XElement dayNode, Dictionary<Guid, Guid> exerciseIds)
//        {
//            foreach (var entryNode in dayNode.Element("Dosages").Descendants("DosageItem"))
//            {
//                var entry = new SupplementCycleDosageDTO();
//                week.Dosages.Add(entry);
//                entry.GlobalId = readGuid(entryNode, "GlobalId").Value;
//                entry.DosageType = readEnum<DosageType>(entryNode, "DosageType");
//                entry.DosageUnit = readEnum<DosageUnit>(entryNode, "DosageUnit");
//                entry.Dosage = readDouble(entryNode, "Dosage").Value;
//                entry.GroupName = readValue(entryNode, "GroupName");
//                entry.Repetitions = readEnum<SupplementCycleDayRepetitions>(entryNode, "Repetitions");

//                var exerciseId = readGuid(entryNode, "SupplementId").Value;
//                if (!exerciseIds.ContainsKey(exerciseId))
//                {
//                    exerciseIds.Add(exerciseId, exerciseId);
//                }
//                entry.Supplement = new SuplementDTO() { GlobalId = exerciseId, Name = "Supplement not found" };//temporary only

//                entry.TimeType = readEnum<TimeType>(entryNode, "Time");

//            }
//        }
//    }
//}
