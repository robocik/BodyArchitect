using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.DataAccess.NHibernate;
using BodyArchitect.Model;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate;
using ChampionshipCategoryType = BodyArchitect.Model.ChampionshipCategoryType;
using ChampionshipCustomerType = BodyArchitect.Model.ChampionshipCustomerType;
using ChampionshipResultItem = BodyArchitect.Model.ChampionshipResultItem;
using ChampionshipTryResult = BodyArchitect.Model.ChampionshipTryResult;
using ChampionshipType = BodyArchitect.Model.ChampionshipType;
using ChampionshipWinningCategories = BodyArchitect.Model.ChampionshipWinningCategories;
using EntryObjectStatus = BodyArchitect.Model.EntryObjectStatus;
using Gender = BodyArchitect.Model.Gender;
using ObjectNotFoundException = BodyArchitect.Shared.ObjectNotFoundException;
using SetType = BodyArchitect.Model.SetType;
using BodyArchitect.Portable;

namespace BodyArchitect.Service.V2.Services
{
    class ChampionshipService: ServiceBase
    {
        public ChampionshipService(ISession session, SecurityInfo securityInfo, ServiceConfiguration configuration)
            : base(session, securityInfo, configuration)
        {
        }

        public PagedResult<ChampionshipDTO> GetChampionships(GetChampionshipsCriteria criteria, PartialRetrievingInfo retrievingInfo)
        {
            if (!SecurityInfo.Licence.IsInstructor)
            {
                throw new LicenceException("This feature is allowed for Instructor account");
            }

            using (var trans = Session.BeginGetTransaction())
            {
                var myProfile = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                var idQuery = Session.QueryOver<Championship>().Where(x => x.Profile == myProfile);
                var query=Session.QueryOver<Championship>()
                    .Fetch(x => x.Groups).Eager
                    //.Fetch(x => x.Reservations).Eager
                    //.Fetch(x => x.Customers).Eager
                    .Fetch(x => x.Entries).Eager
                    .Fetch(x => x.Entries.First().Exercise).Eager;

                if(criteria.ChampionshipId.HasValue)
                {
                    idQuery = idQuery.Where(x => x.GlobalId == criteria.ChampionshipId.Value);
                }

                idQuery.ApplyPaging(retrievingInfo);
                if(criteria.SortAscending)
                {
                    idQuery = idQuery.OrderBy(x => x.StartTime).Asc;
                    query = query.OrderBy(x => x.StartTime).Asc;
                }
                else
                {
                    idQuery = idQuery.OrderBy(x => x.StartTime).Desc;
                    query = query.OrderBy(x => x.StartTime).Desc;
                }

                var res = query.ToExPagedResults<ChampionshipDTO, Championship>(retrievingInfo, idQuery);
                trans.Commit();

                return res;
            }
        }

        decimal calculateWilks(Customer customer,decimal bodyWeight,decimal exerciseWeight)
        {
            if(bodyWeight<=0)
            {
                return 0;
            }
            if(customer.Gender==Gender.Male)
            {
                return WilksFormula.CalculateForMenUsingTables(bodyWeight, exerciseWeight);
            }
            if(customer.Gender==Gender.Female)
            {
                return WilksFormula.CalculateForWomenUsingTables(bodyWeight, exerciseWeight);
            }
            throw new ArgumentException("Customer without gender!");
        }

        public SaveChampionshipResult SaveChampionship(ChampionshipDTO championship)
        {
            SaveChampionshipResult result = new SaveChampionshipResult();

            if (!SecurityInfo.Licence.IsInstructor)
            {
                throw new LicenceException("This feature is allowed for Instructor account");
            }
            if (championship.IsNew)
            {
                throw new InvalidOperationException("Cannot create a championship using this method");
            }

            using (var trans = Session.BeginSaveTransaction())
            {
                var dbMe = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
                var dbChampionship = Session.QueryOver<Championship>()
                    .Fetch(x=>x.Reservations).Eager
                    .Fetch(x=>x.Reservations.First().Customer).Eager
                    .Where(x => x.GlobalId == championship.GlobalId).SingleOrDefault();

                //for eager fetch only
                Session.QueryOver<Championship>()
                    .Fetch(x => x.Customers).Eager
                    //.Fetch(x => x.Categories).Eager
                    .Fetch(x => x.Entries).Eager
                    .Where(x => x.GlobalId == championship.GlobalId).SingleOrDefault();


                if(dbChampionship.Profile!=dbMe)
                {
                    throw new CrossProfileOperationException("Championship belongs to another user");
                }
                if(dbChampionship.State!=ScheduleEntryState.Done)
                {
                    throw new InvalidOperationException("You can add data to championships with Done state only");
                }
                if (dbChampionship.Version !=championship.Version)
                {
                    throw new StaleObjectStateException("Championship",championship.GlobalId);
                }
                

                dbChampionship.Results.Clear();
                Session.Flush();

                var championshipDb = championship.Map<Championship>();
                
                championshipDb.Profile = dbMe;
                championshipDb.Version = dbChampionship.Version;

                //first clear old result items
                //championshipDb.Results = dbChampionship.Results;
                //championshipDb.Results.Clear();
                

                championshipDb.Reservations = dbChampionship.Reservations;
                //check if all customers are in reservation list (there cannot be customer in Customers list and not in Reservations)
                foreach (var championshipCustomer in championshipDb.Customers)
                {
                    if(championshipDb.Reservations.Where(x=>x.Customer==championshipCustomer.Customer).Count()==0)
                    {
                        throw new ObjectNotFoundException("Customer is not available in reservations");
                    }
                }

                var benchPress = Session.Load<Exercise>( new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
                var deadlift = Session.Load<Exercise>( new Guid("505988e1-5663-41f1-aa1a-9b92ea584263"));
                var sqad = Session.Load<Exercise>( new Guid("3e06a130-b811-4e45-9285-f087403615bf"));

                //calculate max for each exercise
                foreach (var entryDto in championshipDb.Entries)
                {
                    if(championshipDb.ChampionshipType==ChampionshipType.ZawodyWyciskanieSztangi)
                    {
                        if(entryDto.Exercise!=benchPress)
                        {
                            throw new ConsistencyException("Wrong exercise");
                        }
                    }
                    else
                    {
                        if (entryDto.Exercise != benchPress && entryDto.Exercise != deadlift && entryDto.Exercise != sqad)
                        {
                            throw new ConsistencyException("Wrong exercise");
                        }
                    }
                    decimal max = 0;
                    if (entryDto.Try1.Result == ChampionshipTryResult.Success && max < entryDto.Try1.Weight)
                    {
                        max = entryDto.Try1.Weight;
                    }
                    if (entryDto.Try2.Result == ChampionshipTryResult.Success && max < entryDto.Try2.Weight)
                    {
                        max = entryDto.Try2.Weight;
                    }
                    if (entryDto.Try3.Result == ChampionshipTryResult.Success && max < entryDto.Try3.Weight)
                    {
                        max = entryDto.Try3.Weight;
                    }
                    entryDto.Max = max;
                    entryDto.Wilks = calculateWilks(entryDto.Customer.Customer,entryDto.Customer.Weight,max);
                }

                //reset total and wilks. they will be calculated later
                foreach (var customer in championshipDb.Customers)
                {
                    customer.TotalWilks = 0;
                    customer.Total = 0;
                }
                //calculate total sum
                foreach (var group in championshipDb.Entries.GroupBy(x => x.Customer))
                {
                    foreach (var entryDto in group)
                    {
                        group.Key.Total += entryDto.Max;
                    }
                    group.Key.TotalWilks = calculateWilks(group.Key.Customer, group.Key.Weight, group.Key.Total);
                }

                calculateResults(championshipDb);

                calculatePostCategories(championshipDb);

                var records = createStrengthTrainingEntries(dbMe, championshipDb, dbChampionship);
                result.NewRecords = records.Map<IList<SerieDTO>>();
                
                championshipDb=Session.Merge(championshipDb);

                trans.Commit();
                result.Championship= championshipDb.Map<ChampionshipDTO>();
            }
            return result;
        }

        private List<Serie> createStrengthTrainingEntries(Profile dbProfile, Championship championshipDb, Championship originalChampionship)
        {
            List<Serie> records = new List<Serie>();

            foreach (var customer in championshipDb.Entries.GroupBy(x => x.Customer))
            {
                StrengthTrainingEntry strengthEntry = null;
                if(originalChampionship!=null)
                {
                    var origChamCust=originalChampionship.Customers.Where(x => x.Customer == customer.Key.Customer).SingleOrDefault();
                    if(origChamCust!=null)
                    {
                        strengthEntry = origChamCust.StrengthTraining;
                    }
                }
                
                if (strengthEntry == null)
                {
                    var trainingDay =Session.QueryOver<TrainingDay>()
                        .Fetch(x => ((StrengthTrainingEntry)x.Objects.First()).Entries).Eager
                        .Fetch(x => ((StrengthTrainingEntry)x.Objects.First()).MyPlace).Eager
                        .Fetch(x => (((StrengthTrainingEntry)x.Objects.First()).Entries.First().Series)).Eager
                        .Where(x =>x.Profile == dbProfile && x.Customer == customer.Key.Customer &&x.TrainingDate == championshipDb.StartTime.Date).SingleOrDefault();

                    if (trainingDay == null)
                    {
                        trainingDay = new TrainingDay(championshipDb.StartTime);
                        trainingDay.Profile = dbProfile;
                        
                        trainingDay.AllowComments = dbProfile.Settings.AllowTrainingDayComments;
                        trainingDay.Customer = customer.Key.Customer;
                    }
                    strengthEntry=new StrengthTrainingEntry();
                    strengthEntry.Status = EntryObjectStatus.System;
                    strengthEntry.MyPlace = championshipDb.MyPlace;
                    trainingDay.AddEntry(strengthEntry);
                    Session.SaveOrUpdate(trainingDay);
                }

                strengthEntry.Entries.Clear();

                foreach (var entryDto in customer)
                {
                    StrengthTrainingItem item = new StrengthTrainingItem();
                    item.Exercise = entryDto.Exercise;
                    strengthEntry.AddEntry(item);
                    addSet(item, entryDto.Try1);
                    addSet(item, entryDto.Try2);
                    addSet(item, entryDto.Try3);
                }
                Session.SaveOrUpdate(strengthEntry);
                customer.Key.StrengthTraining = strengthEntry;

                TrainingDayService service = new TrainingDayService(Session,SecurityInfo,Configuration,null,null);
                service.EnsureRecords(dbProfile, customer.Key.Customer,new List<StrengthTrainingEntry>(){strengthEntry});

                records.AddRange(strengthEntry.Entries.SelectMany(x => x.Series).Where(x => x.ExerciseProfileData != null));
            }

            return records;
        }

        private void addSet(StrengthTrainingItem item, ChampionshipTry entryTry)
        {
            if (entryTry.Result != ChampionshipTryResult.NotDone)
            {
                Serie set = new Serie();
                set.RepetitionNumber = 1;
                set.Weight = entryTry.Weight;
                set.SetType = entryTry.Result == ChampionshipTryResult.Success
                                  ? SetType.Max
                                  : SetType.MuscleFailure;
                item.AddSerie(set);
            }
        }


        void calculateResults(Championship championshipDb)
        {
            var orderedCustomers=championshipDb.Customers.Where(x=>x.TotalWilks>0 ).OrderByDescending(x => x.TotalWilks);
            var males=orderedCustomers.Where(x => x.Customer.Gender == Gender.Male).ToList();
            var females = orderedCustomers.Where(x => x.Customer.Gender == Gender.Female).ToList();

            foreach (var category in championshipDb.Categories)
            {
                if(category.IsPostCategory)
                {
                    continue;
                }
                IList<ChampionshipCustomer> customers =  males;
                if(category.Gender==Gender.NotSet)
                {
                    customers = orderedCustomers.ToList();
                }
                else if (category.Gender == Gender.Female)
                {
                    customers = females;
                }
                Dictionary<decimal, List<ChampionshipCustomer>> temp = new Dictionary<decimal, List<ChampionshipCustomer>>();

                customers = filterCustomersForCategory(championshipDb, category, customers);

                for (int index = 0; index < customers.Count; index++)
                {
                    var championshipCustomer = customers[index];
                    if (category.Type == ChampionshipCategoryType.Open)
                    {
                        championshipDb.Results.Add(new ChampionshipResultItem()
                        {
                            Customer = championshipCustomer,
                            Category = category,
                            Position = index
                        });
                    }
                    else
                    {
                        var weightCategory = ModelHelper.GetWeightCategory(category.Gender == Gender.Male ? ChampionshipDTO.MenWeights : ChampionshipDTO.WomenWeights, championshipCustomer.Weight);

                        if (!temp.ContainsKey(weightCategory))
                        {
                            temp.Add(weightCategory, new List<ChampionshipCustomer>());
                        }
                        temp[weightCategory].Add(championshipCustomer);

                        championshipDb.Results.Add(new ChampionshipResultItem()
                                                       {
                                                           Customer = championshipCustomer,
                                                           //Category = ChampionshipWinnerCategories.MaleWeight,
                                                           Category = category,
                                                           Value = weightCategory,
                            Position = temp[weightCategory].Count - 1
                        });
                    }
                }
                
            }

        }

        private IList<ChampionshipCustomer> filterCustomersForCategory(Championship championshipDb, ChampionshipCategory category, IList<ChampionshipCustomer> customers)
        {
            customers = customers.Where(x => x.Type!=ChampionshipCustomerType.Disqualified && (!category.IsOfficial || x.Type==ChampionshipCustomerType.Normal) && belongsToCategory(category, x.Customer, category.IsAgeStrict)).ToList();
                
            if(!category.IsAgeStrict)
            {
                //dla kategorii nie strict musimy sprawdzić czy jesli jest user który niby podpada do tej kategorii ale tylko przez loose to czy istnieje jakas inna kategoria do której pasuje jako strict i jesli tak jest to wywalamy usera z tej kategorii loose.
                //czyli mamy dwie kategorie Weterani1 (40-49) loose i Weterani3 (60-69). Mamy też trzech userów (wiek 41, 51 i 61 lat). W tym przypadku user 41 podpada do Weterani1 (jako strict),
                //user 51 także wpada do weterani1 (jako loose) ale user 61 wpada do weterani3 (jako strict). I wlasnie to wpadnięcie usera 61 do weterani3 a nie do weterani1 jest tutaj sprawdzane
                for (int index = customers.Count-1; index >=0; index--)
                {
                    ChampionshipCustomer championshipCustomer = customers[index];
                    var strictCategory = findStrictAgeCategoryForCustomer(championshipCustomer.Customer, championshipDb.Categories);
                    if (strictCategory!=null && strictCategory != category)
                    {
                        customers.RemoveAt(index);
                    }
                }
            }
            return customers;
        }

        bool belongsToCategory(ChampionshipCategory category,Customer customer,bool isStrict)
        {
            if (isStrict)
            {
                if (category.Category == ChampionshipWinningCategories.Seniorzy)
                {
                    return customer.Birthday.Value.GetAge() >= 14;
                }
                else if (category.Category == ChampionshipWinningCategories.JuniorzyMlodsi)
                {
                    return customer.Birthday.Value.GetAge() >= 14 &&
                           DateTime.Today.Year <= customer.Birthday.Value.AddYears(18).Year;
                }
                else if (category.Category == ChampionshipWinningCategories.Juniorzy)
                {
                    return DateTime.Today.Year >= customer.Birthday.Value.AddYears(19).Year &&
                           DateTime.Today.Year <= customer.Birthday.Value.AddYears(23).Year;
                }
                else if (category.Category == ChampionshipWinningCategories.Weterani1)
                {
                    return DateTime.Today.Year >= customer.Birthday.Value.AddYears(40).Year &&
                           DateTime.Today.Year <= customer.Birthday.Value.AddYears(49).Year;
                }
                else if (category.Category == ChampionshipWinningCategories.Weterani2)
                {
                    return DateTime.Today.Year >= customer.Birthday.Value.AddYears(50).Year &&
                           DateTime.Today.Year <= customer.Birthday.Value.AddYears(59).Year;
                }
                else if (category.Category == ChampionshipWinningCategories.Weterani3)
                {
                    return DateTime.Today.Year >= customer.Birthday.Value.AddYears(60).Year &&
                           DateTime.Today.Year <= customer.Birthday.Value.AddYears(69).Year;
                }
                return DateTime.Today.Year >= customer.Birthday.Value.AddYears(70).Year;
            }
            else
            {
                if (category.Category == ChampionshipWinningCategories.Seniorzy)
                {
                    return customer.Birthday.Value.GetAge() >= 14;
                }
                else if (category.Category == ChampionshipWinningCategories.JuniorzyMlodsi)
                {
                    return customer.Birthday.Value.GetAge() >= 14;
                }
                else if (category.Category == ChampionshipWinningCategories.Juniorzy)
                {
                    return DateTime.Today.Year >= customer.Birthday.Value.AddYears(19).Year;
                }
                else if (category.Category == ChampionshipWinningCategories.Weterani1)
                {
                    return DateTime.Today.Year >= customer.Birthday.Value.AddYears(40).Year;
                }
                else if (category.Category == ChampionshipWinningCategories.Weterani2)
                {
                    return DateTime.Today.Year >= customer.Birthday.Value.AddYears(50).Year;
                }
                else if (category.Category == ChampionshipWinningCategories.Weterani3)
                {
                    return DateTime.Today.Year >= customer.Birthday.Value.AddYears(60).Year;
                }
                return DateTime.Today.Year >= customer.Birthday.Value.AddYears(70).Year;
            }
        }

        ChampionshipCategory findStrictAgeCategoryForCustomer(Customer customer, ICollection<ChampionshipCategory> categories)
        {
            var agesCategories=categories.Where(x =>(x.Gender==Gender.NotSet || x.Gender==customer.Gender) && x.Category >= ChampionshipWinningCategories.JuniorzyMlodsi &&x.Category <= ChampionshipWinningCategories.Weterani4).ToList();
            var strictCategory = agesCategories.Where(x => belongsToCategory(x, customer, true)).SingleOrDefault();
            return strictCategory;
        }

        private void calculatePostCategories(Championship championshipDb)
        {
            foreach (var category in championshipDb.Categories)
            {
                if(category.Category==ChampionshipWinningCategories.MistrzMistrzow )
                {
                    var ordered = championshipDb.Results.Where(x => x.Position == 0 && (x.Category.Gender == Gender.NotSet || x.Category.Gender == category.Gender) && x.Customer.Type != ChampionshipCustomerType.Disqualified && (!category.IsOfficial || x.Customer.Type == ChampionshipCustomerType.Normal) && x.Category.Type == ChampionshipCategoryType.Weight && !x.Category.IsPostCategory).OrderByDescending(x => x.Customer.TotalWilks).ToList();
                    for (int index = 0; index < ordered.Count; index++)
                    {
                        var resultItem = ordered[index];
                        championshipDb.Results.Add(new ChampionshipResultItem()
                                                       {
                                                           Customer = resultItem.Customer,
                                                           Category = category,
                                                           Position = index
                                                       });
                    }
                }
                //druzynowo
                if(category.Category==ChampionshipWinningCategories.Druzynowa)
                {
                    Dictionary<ChampionshipGroup,int> temp = new Dictionary<ChampionshipGroup, int>();

                    var ordered = championshipDb.Results.Where(x =>!x.Category.IsPostCategory && x.Customer.Group != null && (category.Gender==Gender.NotSet || x.Category.Gender == category.Gender) && x.Customer.Type!=ChampionshipCustomerType.Disqualified && (!category.IsOfficial || x.Customer.Type==ChampionshipCustomerType.Normal) ).OrderBy(x => x.Position).GroupBy(x => x.Customer.Group);

                    foreach (var grouped in ordered)
                    {
                        if (!temp.ContainsKey(grouped.Key))
                        {
                            temp.Add(grouped.Key, 0);
                        }

                        Dictionary<Guid,ChampionshipResultItem> distinctedItemsByCustomer = new Dictionary<Guid, ChampionshipResultItem>();
                        foreach (var championshipResultItem in grouped)
                        {
                            if(!distinctedItemsByCustomer.ContainsKey(championshipResultItem.Customer.Customer.GlobalId))
                            {
                                distinctedItemsByCustomer.Add(championshipResultItem.Customer.Customer.GlobalId,championshipResultItem);
                            }
                        }
                        int points =0;
                        foreach (var championshipResultItem in distinctedItemsByCustomer.Values.Take(championshipDb.TeamCount))
                        {
                            points += getTeamPoints(championshipResultItem.Position);
                            temp[grouped.Key]= points;
                        }
                    }

                    var groups = temp.OrderByDescending(x => x.Value)
                        .ThenByDescending(v => championshipDb.Results.Where(x => !x.Category.IsPostCategory && x.Customer != null && x.Customer.Group == v.Key && x.Position == 0).Count())
                        .ThenByDescending(v => championshipDb.Results.Where(x =>!x.Category.IsPostCategory && x.Customer != null && x.Customer.Group == v.Key && x.Position == 1).Count())
                        .ThenByDescending(v => championshipDb.Results.Where(x => !x.Category.IsPostCategory && x.Customer != null && x.Customer.Group == v.Key && x.Position == 2).Count())
                        .ThenByDescending(v => championshipDb.Results.Where(x => !x.Category.IsPostCategory && x.Customer != null && x.Customer.Group == v.Key && x.Position == 3).Count())
                        .ThenByDescending(v => championshipDb.Results.Where(x => !x.Category.IsPostCategory && x.Customer != null && x.Customer.Group == v.Key && x.Position == 4).Count())
                        .ThenByDescending(v => championshipDb.Results.Where(x => !x.Category.IsPostCategory && x.Customer != null && x.Customer.Group == v.Key && x.Position == 5).Count())
                        .ToList();
                    for (int index = 0; index < groups.Count; index++)
                    {
                        var keyValuePair = groups[index];
                        championshipDb.Results.Add(new ChampionshipResultItem()
                        {
                            Group = keyValuePair.Key,
                            Value=keyValuePair.Value,
                            Category = category,
                            Position = index
                        });
                    }
                }
            }

            
        }

        int getTeamPoints(int position)
        {
            if(position==0)
            {
                return 12;
            }
            else if(position>0 && position<10)
            {
                return 10 - position;
            }
            return 1;
        }


        //public ChampionshipDTO ChampionshipOperation(ChampionshipOperationParams param)
        //{
        //    using (var trans = Session.BeginSaveTransaction())
        //    {
        //        var dbMe = Session.Load<Profile>(SecurityInfo.SessionData.Profile.GlobalId);
        //        var dbChampionship = Session.QueryOver<Championship>().Fetch(x=>x.Reservations).Eager.Where(x => x.GlobalId == param.ChampionshipId).SingleOrDefault();

        //        if (dbChampionship.Profile != dbMe)
        //        {
        //            throw new CrossProfileOperationException("Championship belongs to another user");
        //        }

        //        if(param.Operation==ChampionshipOperationType.Start)
        //        {
        //            foreach (var reservation in dbChampionship.Reservations)
        //            {
        //                var champCust = new ChampionshipCustomer();
        //                champCust.Customer = reservation.Customer;
        //                dbChampionship.Customers.Add(champCust);
                        
        //            }

        //            Exercise benchPress = Session.Load<Exercise>(new Guid("ece5dfd7-f995-45ae-bb34-067f26c4f7b4"));
        //            if(dbChampionship.ChampionshipType==ChampionshipType.ZawodyWyciskanieSztangi)
        //            {
        //                foreach (var championshipCustomer in dbChampionship.Customers)
        //                {
        //                    ChampionshipEntry entry = new ChampionshipEntry();
        //                    entry.Customer = championshipCustomer;
        //                    entry.Exercise = benchPress;
        //                    dbChampionship.Entries.Add(entry);
        //                }   
        //            }
        //        }
        //        Session.Save(dbChampionship);
        //        trans.Commit();

        //        return dbChampionship.Map<ChampionshipDTO>();
        //    }

            
        //}
    }
}
