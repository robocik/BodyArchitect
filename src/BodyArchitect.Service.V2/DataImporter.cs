using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.TrainingPlans;
using NHibernate;
using DosageType = BodyArchitect.Model.DosageType;
using DropSetType = BodyArchitect.Model.DropSetType;
using ExerciseDifficult = BodyArchitect.Model.ExerciseDifficult;
using ExerciseForceType = BodyArchitect.Model.ExerciseForceType;
using ExerciseType = BodyArchitect.Model.ExerciseType;
using MechanicsType = BodyArchitect.Model.MechanicsType;
using ReportStatus = BodyArchitect.Model.ReportStatus;
using SetType = BodyArchitect.Model.SetType;
using TrainingEnd = BodyArchitect.Model.TrainingEnd;
using TrainingPlan = BodyArchitect.Model.TrainingPlan;
using TrainingPlanDifficult = BodyArchitect.Model.TrainingPlanDifficult;
using TrainingType = BodyArchitect.Model.TrainingType;

namespace BodyArchitect.Service.V2
{
    //class DataImporter
    //{
    //    private ISession session;
    //    private Dictionary<Guid, ExerciseDTO> dictExercises;
    //    private ProfileDTO profile;
    //    Dictionary<Guid,Guid> engExercises =new Dictionary<Guid, Guid>();

    //    private List<BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlan> planList =
    //        new List<BodyArchitect.Service.V2.Model.TrainingPlans.TrainingPlan>();

    //    Dictionary<Guid,Guid> privateExercisesMapping = new Dictionary<Guid, Guid>();
    //    Dictionary<Guid, Guid> workoutPlansMapping = new Dictionary<Guid, Guid>();

    //    void buildEnglishExercises()
    //    {
    //        engExercises.Add(new Guid("9D6A5C48-B2A3-4613-80BB-047DC770C896"), new Guid("C5276D0F-5BAD-479A-B582-0855032A401A"));
    //        engExercises.Add(new Guid("D5D746BA-5DAB-4375-9FB6-167D35DB161B"), new Guid("9934E9E3-50DC-4B34-8B67-092084894740"));
    //        engExercises.Add(new Guid("0D9AB7A4-C73B-4A83-89EB-CE3828AAB90B"), new Guid("8082FF60-2D28-40D6-95B9-858BA0FD4274"));
    //        engExercises.Add(new Guid("920912E8-B2B7-4F4A-B588-13F3E3E4763A"), new Guid("3FBF5036-CD9F-401E-A4D1-E03B76A27C35"));
    //        engExercises.Add(new Guid("1CDF4117-128F-49DD-927F-EB9E0B1BB4BB"), new Guid("E833BAB7-DBCC-48F2-B92F-BF1A78E07DE0"));
    //        engExercises.Add(new Guid("FBA79703-6FAF-4A5F-959A-8F50A068885F"), new Guid("60F86528-650C-47BA-AD9E-6FA08E44DDD5"));
    //        engExercises.Add(new Guid("C98C899B-A4C4-4829-9AB2-2C7A783848DD"), new Guid("63F604B6-DA97-4335-8AA7-24594B13684D"));
    //        engExercises.Add(new Guid("4D6B28B0-C341-43BA-9D69-441773828C90"), new Guid("80ABC7EE-230D-4E90-9244-4E60D1469161"));
    //        engExercises.Add(new Guid("B83D171F-18A7-4997-8F81-8F9A77C694DF"), new Guid("9C4BD158-CEC3-475E-8542-8B7E9E413AAC"));
    //        engExercises.Add(new Guid("439E2060-488B-4DB8-9D09-09CDF2422D51"), new Guid("1C7DF47E-92A3-43F2-B121-4DB038145BE4"));
    //        engExercises.Add(new Guid("E4F2FF97-D09F-4E2A-9D24-DCC58DFEC5A2"), new Guid("B72695CA-77B5-46C8-8EB2-E44CD7BED7A5"));
    //        engExercises.Add(new Guid("1BCC52C4-5D63-4974-A68C-B16941F73C81"), new Guid("A3F08B5B-95A7-4089-81B0-189B874E32C7"));
    //        engExercises.Add(new Guid("198603B6-E379-43DD-9369-1984F7B2C5B2"), new Guid("3EA96687-3643-49B4-A4EB-0AD66ABEC5D6"));
    //        engExercises.Add(new Guid("3930FC43-615B-46A7-AA46-066BE6CEC6F1"), new Guid("B72C04B8-1877-4E34-872A-F5DFD0E85150"));
    //        engExercises.Add(new Guid("0AD2F209-9A5D-4349-A652-42ADE4E3E389"), new Guid("AD5E2E70-8995-4BC3-9E4B-572EA66E5F5C"));
    //        engExercises.Add(new Guid("CACE747B-0460-4EFA-942F-B4A2937D78D6"), new Guid("097476A6-1F24-41F0-BD93-2173758D0A1F"));
    //        engExercises.Add(new Guid("1950E60F-4040-4EF9-A054-CE4254B97D4A"), new Guid("8186BA53-5B43-42CC-930A-D3AFB866D2AE"));
    //        engExercises.Add(new Guid("172C045D-BAAC-4EF3-A7BB-AF343BD4479E"), new Guid("C0613588-CC24-4EC7-BD9B-C906643817EB"));
    //        engExercises.Add(new Guid("DD324460-8630-49D2-91A5-18E0B3BE98F5"), new Guid("0B11D9C5-772C-4C9F-96F5-79DA28E77149"));
    //        engExercises.Add(new Guid("F428F25B-D077-4123-B998-7F0984C29867"), new Guid("C67FE645-D00F-45E4-A624-1951B49F5B4A"));
    //        engExercises.Add(new Guid("0CF31AEA-DDDC-4FCC-AC95-97735DFDA280"), new Guid("74DE54F2-CEDC-41D1-8B50-5B8209420DDB"));
    //        engExercises.Add(new Guid("F747A74E-A544-4FF2-B05E-77EA3476F72D"), new Guid("BA836E4B-13EA-4F71-AD15-E35778754750"));
    //        engExercises.Add(new Guid("95E2F7D6-D75B-4064-906F-E80E7616D8E9"), new Guid("5084EECB-41FD-49D1-9396-1750743247B7"));
    //        engExercises.Add(new Guid("533428A6-A2E4-41F9-B1AB-F2FE8450D38A"), new Guid("71FCB610-AFF9-4098-9F82-A1BEFAAA1645"));
    //        engExercises.Add(new Guid("BD8C45FA-0AAC-4A1F-8FC2-CEDA8C945858"), new Guid("07684561-B086-4E9D-B277-D5CAA7138CDB"));
    //        engExercises.Add(new Guid("C1343295-0FED-419F-83AB-58F63FBA0AB7"), new Guid("DB0677D6-9E4A-4BD3-96B1-BBA05212ECD6"));
    //        engExercises.Add(new Guid("2A73061D-9090-4CDE-AAD8-5B7B89472556"), new Guid("8AD4AD3E-99B7-429B-975A-7157B539C64B"));
    //        engExercises.Add(new Guid("BD06B342-C28A-4799-B889-FAEF80BDFFCB"), new Guid("ECE5DFD7-F995-45AE-BB34-067F26C4F7B4"));
    //        engExercises.Add(new Guid("916DAE2E-AC56-431C-9F2C-9FF408BDBA00"), new Guid("9745A89E-09CE-483A-93CA-6BFB60CE308C"));
    //        engExercises.Add(new Guid("DB7D7256-14C7-4CF2-AC5C-12C88295BFE7"), new Guid("7282357C-EA8B-4792-A5C6-78034554FAEA"));
    //        engExercises.Add(new Guid("F9FF7DA5-7F15-47F8-88F0-37A2E361E4D7"), new Guid("85583670-5D44-445C-8FCD-E4EBE30B0F0C"));
    //        engExercises.Add(new Guid("02EEF7A0-22C3-4A5F-B2A5-F4861AF9AA2B"), new Guid("FEDB8ACD-E737-4AB5-83A1-0E4DAA39F817"));
    //        engExercises.Add(new Guid("9BA4D0C8-0DDB-4A1E-BEDA-AC9F871044E0"), new Guid("841DBC08-8F5C-48ED-A57B-F2FD3A0F2098"));
    //        engExercises.Add(new Guid("8B1D32B9-6590-43E8-A93B-24BD71ECF565"), new Guid("943F6E63-AC82-4DA2-AAA6-A1532AF32D97"));
    //        engExercises.Add(new Guid("A350269C-A915-404C-A8D8-D2F2A93D13AE"), new Guid("2166400E-F37F-464C-8403-C1566952D6E8"));
    //        engExercises.Add(new Guid("A2AAEE3E-EF64-42C0-89AA-A605AAAC538F"), new Guid("B10166D8-942B-4F06-8F2E-FB5AC8AAF31A"));
    //        engExercises.Add(new Guid("104BB7A1-B668-4461-91C2-C37102B92E0C"), new Guid("D265CA35-24F0-4362-82BA-4607B96560BD"));
    //        engExercises.Add(new Guid("BE9FECAB-7CDA-422C-8146-F72047D99CE2"), new Guid("32C99BF1-1256-4FB2-9379-5325EE68CC9A"));
    //        engExercises.Add(new Guid("6C9C673A-7C68-42EF-A59C-E0BDA3751CF6"), new Guid("8FDFEC0A-98DF-4542-8167-CA35D3586370"));
    //        engExercises.Add(new Guid("CE902A64-8249-4BF0-8965-765E4AEF8896"), new Guid("4DA1B922-17E6-4645-94D7-AC27DA5F2C30"));

    //        engExercises.Add(new Guid("A0A7673A-F3A2-4A64-88FB-892438575DB4"), new Guid("1D2314F0-0BA3-46F8-94FD-E6B876C38C85"));
    //        engExercises.Add(new Guid("C3BEF822-5146-4F08-A434-3E982ECB95A7"), new Guid("D66E8977-803B-46AD-B608-EACDBA05DDFF"));
    //    }

    //    public DataImporter(ISession session, ProfileDTO profile, IList<ExerciseDTO> exercises)
    //    {
    //        this.session = session;
    //        dictExercises = exercises.ToDictionary(x => x.GlobalId);
    //        this.profile = profile;
    //        buildEnglishExercises();
    //    }

    //    public void Import(DataSet ds,int profileWymiaryId)
    //    {
    //        using (var transactionScope = new TransactionManager(true))
    //        {
    //            Profile dbProfile = session.Get<Profile>(profile.Id);
    //            importExercises(ds, dbProfile);
    //            var wymiary = importWymiary(ds, profileWymiaryId);
    //            dbProfile.Wymiary = wymiary;
    //            session.Update(dbProfile);
    //            importWorkoutPlans(ds, dbProfile);
    //            importTrainingDays(ds, dbProfile);
    //            transactionScope.CommitTransaction();
    //        }
    //    }

    //    void importTrainingDays(DataSet ds,Profile dbProfile)
    //    {
    //         DataTable table=ds.Tables["TrainingDay"];
    //         foreach (DataRow row in table.Rows)
    //         {
    //             TrainingDay day = new TrainingDay(getValue<DateTime>(row["TrainingDate"]));
    //             day.Profile = dbProfile;
    //             int oldId = getValue<int>(row["Id"]);
    //             importSizes(ds, day,oldId);
    //             importStrengthTraining(ds, day, oldId);
    //             importSupplements(ds, day, oldId);
    //             importA6W(ds, day, oldId);
    //             session.SaveOrUpdate(day);
    //         }
    //    }
    //    Dictionary<int, int> myTrainings = new Dictionary<int, int>();

    //    MyTraining importMyTraining(DataSet ds,Profile dbProfile,int oldId)
    //    {
    //        if(myTrainings.ContainsKey(oldId))
    //        {
    //            return session.Load<MyTraining>(myTrainings[oldId]);
    //        }
    //        var table = ds.Tables["MyTraining"];
    //        var rows = table.Select("Id=" + oldId);
    //        if(rows.Length==0)
    //        {
    //            return null;
    //        }
    //        MyTraining myTraining = new MyTraining();
    //        myTraining.Profile = dbProfile;
    //        var endDate=getValue<DateTime?>(rows[0]["EndDate"]);
    //        var startDate = getValue<DateTime>(rows[0]["StartDate"]);
    //        var trainingEnd = getValue<TrainingEnd>(rows[0]["TrainingEnd"]);
    //        myTraining.SetData(startDate,endDate,trainingEnd);
    //        myTraining.Name = getValue<string>(rows[0]["Name"]);
    //        myTraining.PercentageCompleted = getValue<int?>(rows[0]["PercentageCompleted"]);
    //        myTraining.TypeId = A6WEntry.EntryTypeId;
    //        session.Save(myTraining);
    //        myTrainings.Add(oldId,myTraining.Id);
    //        return myTraining;


    //    }
    //    void importA6W(DataSet ds,TrainingDay day,int oldTrainingDayId)
    //    {
    //        var entryTable = ds.Tables["EntryObject"];
    //        var sizeTable = ds.Tables["A6WEntry"];
    //        var rows = entryTable.Select("TrainingDay=" + oldTrainingDayId);
    //        if (rows.Length == 0)
    //        {
    //            return;
    //        }

    //        foreach (DataRow dataRow in rows)
    //        {
    //            int id = getValue<int>(dataRow["Id"]);
    //            var sizesRows = sizeTable.Select("Id=" + id);
    //            if (sizesRows.Length == 1)
    //            {
    //                A6WEntry sizeEntry = new A6WEntry();
    //                fillEntryObject(sizeEntry, rows[0]);

    //                sizeEntry.DayNumber = getValue<int>(sizesRows[0]["DayNumber"]);
    //                sizeEntry.Set1 = getValue<int>(sizesRows[0]["Set1"]);
    //                sizeEntry.Set2 = getValue<int>(sizesRows[0]["Set2"]);
    //                sizeEntry.Set3 = getValue<int>(sizesRows[0]["Set3"]);
    //                sizeEntry.Completed = getValue<bool>(sizesRows[0]["Completed"]);
    //                sizeEntry.MyTraining = importMyTraining(ds, day.Profile, getValue<int>(rows[0]["MyTraining"]));
    //                //sizeEntry.MyTraining = getValue<int>(sizesRows[0]["Set3"]);
    //                day.AddEntry(sizeEntry);
    //            }

    //        }
            
    //    }

    //    void importSizes(DataSet ds,TrainingDay day,int oldTrainingDayId)
    //    {
    //        var entryTable=ds.Tables["EntryObject"];
    //        var sizeTable = ds.Tables["SizeEntry"];
    //        var rows=entryTable.Select("TrainingDay=" + oldTrainingDayId);
    //        if(rows.Length==0)
    //        {
    //            return;
    //        }
    //        foreach (DataRow dataRow in rows)
    //        {
    //            int id = getValue<int>(dataRow["Id"]);
    //            var sizesRows = sizeTable.Select("Id=" + id);
    //            if (sizesRows.Length == 1)
    //            {
    //                SizeEntry sizeEntry = new SizeEntry();
    //                fillEntryObject(sizeEntry, dataRow);
    //                sizeEntry.Wymiary = importWymiary(ds, getValue<int>(sizesRows[0]["Wymiary"]));
    //                day.AddEntry(sizeEntry);
    //            }
                
    //        }
            
    //    }

    //    void fillEntryObject(EntryObject entryObject,DataRow row)
    //    {
    //        entryObject.Comment = getValue<string>(row["Comment"]);
    //        entryObject.Name = getValue<string>(row["Name"]);
    //        entryObject.ReportStatus = (ReportStatus) getValue<int>(row["ReportStatus"]);
    //    }

    //    void importStrengthTraining(DataSet ds, TrainingDay day, int oldTrainingDayId)
    //    {
    //        var entryTable = ds.Tables["EntryObject"];
    //        var sizeTable = ds.Tables["StrengthTrainingEntry"];
    //        var rows = entryTable.Select("TrainingDay=" + oldTrainingDayId);
    //        if (rows.Length == 0)
    //        {
    //            return;
    //        }

    //        foreach (DataRow dataRow in rows)
    //        {
    //            int id = getValue<int>(dataRow["Id"]);
    //            var sizesRows = sizeTable.Select("Id=" + id);
    //            if (sizesRows.Length == 1)
    //            {
    //                StrengthTrainingEntry sizeEntry = new StrengthTrainingEntry();
    //                sizeEntry.TrainingPlanItemId = getValue<Guid?>(sizesRows[0]["TrainingPlanItemId"]);
    //                if (sizeEntry.TrainingPlanItemId.HasValue)
    //                {
    //                    sizeEntry.TrainingPlanId = findTrainingPlanId(sizeEntry.TrainingPlanItemId.Value);
    //                }
    //                fillEntryObject(sizeEntry, dataRow);
    //                var itemTable = ds.Tables["StrengthTrainingItem"];
    //                foreach (var itemRow in itemTable.Select("StrengthTrainingEntry=" + id))
    //                {
    //                    StrengthTrainingItem item = new StrengthTrainingItem();
    //                    int itemId = getValue<int>(itemRow["Id"]);
    //                    item.Comment = getValue<string>(itemRow["Comment"]);

    //                    Guid exerciseId = getValue<Guid>(itemRow["ExerciseId"]);
    //                    if (privateExercisesMapping.ContainsKey(exerciseId))
    //                    {//private exercises have a new id so we need to use it
    //                        exerciseId = privateExercisesMapping[exerciseId];
    //                    }
    //                    item.ExerciseId = exerciseId;
    //                    if (engExercises.ContainsKey(item.ExerciseId))
    //                    {//check english exercises
    //                        item.ExerciseId = engExercises[item.ExerciseId];
    //                    }

    //                    item.Position = getValue<int>(itemRow["Position"]);
    //                    item.SuperSetGroup = getValue<string>(itemRow["SuperSetGroup"]);
    //                    var serieTable = ds.Tables["Serie"];
    //                    foreach (var serieRow in serieTable.Select("StrengthTrainingItem=" + itemId))
    //                    {
    //                        Serie serie = new Serie();
    //                        serie.Comment = getValue<string>(serieRow["Comment"]);
    //                        serie.DropSet = (DropSetType)getValue<int>(serieRow["DropSet"]);
    //                        serie.IsCiezarBezSztangi = getValue<bool>(serieRow["IsCiezarBezSztangi"]);
    //                        int reps = getValue<int>(serieRow["RepetitionNumber"]);
    //                        if (reps > 0)
    //                        {
    //                            serie.RepetitionNumber = reps;
    //                        }
    //                        float wegiht = getValue<float>(serieRow["Weight"]);
    //                        if (wegiht > 0)
    //                        {
    //                            serie.Weight = wegiht;
    //                        }
    //                        bool withHelp = getValue<bool>(serieRow["IsLastRepetitionWithHelp"]);
    //                        if (withHelp)
    //                        {
    //                            serie.SetType = SetType.MuscleFailure;
    //                        }
    //                        item.AddSerie(serie);
    //                    }
    //                    sizeEntry.AddEntry(item);
    //                }
    //                day.AddEntry(sizeEntry);
    //            }

    //        }

            
    //    }

    //    Guid? findTrainingPlanId(Guid trainingPlanDayId)
    //    {
    //        foreach (var trainingPlan in planList)
    //        {
    //            foreach (var planDay in trainingPlan.Weeks)
    //            {
    //                if(planDay.GlobalId==trainingPlanDayId)
    //                {
    //                    Guid id= trainingPlan.GlobalId;
    //                    if (workoutPlansMapping.ContainsKey(id))
    //                    {
    //                        id = workoutPlansMapping[id];
    //                    }
    //                    return id;
    //                }
    //            }
    //        }
    //        return null;
    //    }

    //    void importSupplements(DataSet ds,TrainingDay day,int oldTrainingDayId)
    //    {
    //        var entryTable = ds.Tables["EntryObject"];
    //        var sizeTable = ds.Tables["SuplementsEntry"];
    //        var rows = entryTable.Select("TrainingDay=" + oldTrainingDayId);
    //        if (rows.Length == 0)
    //        {
    //            return;
    //        }

    //        foreach (DataRow dataRow in rows)
    //        {
    //            int id = getValue<int>(dataRow["Id"]);
    //            var sizesRows = sizeTable.Select("Id=" + id);
    //            if (sizesRows.Length == 1)
    //            {
    //                SuplementsEntry sizeEntry = new SuplementsEntry();
    //                fillEntryObject(sizeEntry, dataRow);
    //                var itemTable = ds.Tables["SuplementItem"];
    //                foreach (var itemRow in itemTable.Select("SuplementsEntry=" + id))
    //                {
    //                    SuplementItem item = new SuplementItem();
    //                    item.Comment = getValue<string>(itemRow["Comment"]);
    //                    item.Dosage = getValue<double>(itemRow["Dosage"]);
    //                    item.DosageType = (DosageType)getValue<int>(itemRow["DosageType"]);
    //                    item.Name = getValue<string>(itemRow["Name"]);
    //                    item.Time = getValue<DateTime>(itemRow["Time"]);
    //                    item.SuplementId = getValue<Guid>(itemRow["SuplementId"]);
    //                    sizeEntry.AddItem(item);
    //                }
    //                day.AddEntry(sizeEntry);
    //            }

    //        }
            
    //    }

    //    void importExercises(DataSet ds,Profile dbProfile)
    //    {
    //        DataTable table=ds.Tables["Exercise"];
    //        foreach (DataRow row in table.Rows)
    //        {
    //            Guid exId = (Guid)row["GlobalId"];
                
    //            if (!dictExercises.ContainsKey(exId) && !engExercises.ContainsKey(exId))
    //            {
    //                Guid newId = Guid.NewGuid();
    //                privateExercisesMapping.Add(exId,newId);
    //                Exercise exercise = new Exercise(newId);
    //                exercise.Profile = dbProfile;
    //                exercise.Name = getValue<string>(row["Name"]);
    //                exercise.Description = getValue<string>(row["Description"]);
    //                exercise.Url = getValue<string>(row["Url"]);
    //                exercise.MechanicsType =(MechanicsType)getValue<int>( row["MechanicsType"]);
    //                exercise.ExerciseForceType = (ExerciseForceType)getValue<int>(row["ExerciseForceType"]);
    //                exercise.ExerciseType = (ExerciseType)getValue<int>(row["Muscle"]);
    //                exercise.Shortcut =getValue<string>(row["Shortcut"]);
    //                exercise.Difficult = (ExerciseDifficult)getValue<int>(row["Difficult"]);
    //                session.Save(exercise);
    //            }
    //        }
    //    }

    //    T getValue<T>(object o)
    //    {
    //        if(o ==DBNull.Value)
    //        {
    //            return default(T);
    //        }
    //        return (T) o;
    //    }

    //    Wymiary importWymiary(DataSet ds,int wymiaryId)
    //    {
    //        DataTable table = ds.Tables["Wymiary"];
    //        var rows = table.Select("Id=" + wymiaryId);
    //        Wymiary wymiary = null;
    //        if(rows.Length>0)
    //        {
    //            wymiary=new Wymiary();
    //            wymiary.Klatka = (float)rows[0]["Klatka"];
    //            wymiary.LeftBiceps = (float)rows[0]["LeftBiceps"];
    //            wymiary.LeftForearm = (float)rows[0]["LeftForearm"];
    //            wymiary.LeftUdo = (float)rows[0]["LeftUdo"];
    //            wymiary.Pas = (float)rows[0]["Pas"];
    //            wymiary.RightBiceps = (float)rows[0]["RightBiceps"];
    //            wymiary.RightForearm = (float)rows[0]["RightForearm"];
    //            wymiary.RightUdo = (float)rows[0]["RightUdo"];
    //            wymiary.Weight = (float)rows[0]["Weight"];
    //            wymiary.Height = (int)rows[0]["Height"];
    //            wymiary.IsNaCzczo = (bool)rows[0]["IsNaCzczo"];

    //            wymiary.DateTime = (DateTime)rows[0]["DateTime"];
    //            session.Save(wymiary);
    //        }
    //        return wymiary;
    //    }

    //    void importWorkoutPlans(DataSet ds,Profile dbProfile)
    //    {
    //         DataTable table=ds.Tables["TrainingPlanDTO"];
    //         foreach (DataRow row in table.Rows)
    //         {
                 
    //             string planContent = getValue<string>(row["PlanContent"]);
    //             XmlSerializationTrainingPlanFormatter formatter = new XmlSerializationTrainingPlanFormatter();
    //             var trainingPlan=formatter.FromXml(planContent);

    //             //first use a new id
    //             Guid newId = Guid.NewGuid();
    //             workoutPlansMapping.Add(trainingPlan.GlobalId, newId);
    //             trainingPlan.GlobalId = newId;

    //             TrainingPlan plan = new TrainingPlan();
    //             plan.Author = trainingPlan.Author;
    //             plan.CreationDate = trainingPlan.CreationDate;
    //             plan.DaysCount = trainingPlan.Weeks.Count;
    //             plan.Difficult = (TrainingPlanDifficult) trainingPlan.Difficult;
    //             plan.Profile = dbProfile;
    //             plan.GlobalId = trainingPlan.GlobalId;
    //             plan.Language = trainingPlan.Language = "pl";//Hardcoded because I think all persons which are converting data are Polish
    //             plan.Name = trainingPlan.Name;
    //             plan.PlanContent = formatter.ToXml(trainingPlan).ToString();
    //             plan.TrainingType = (TrainingType) trainingPlan.TrainingType;
    //             session.Save(plan);
    //             planList.Add(trainingPlan);
    //         }
            
    //    }
    //}
}
