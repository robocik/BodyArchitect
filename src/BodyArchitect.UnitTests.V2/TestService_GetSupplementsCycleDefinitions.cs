using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Service.V2.Model.Instructor;
using NUnit.Framework;
using PublishStatus = BodyArchitect.Model.PublishStatus;
using WorkoutPlanPurpose = BodyArchitect.Model.WorkoutPlanPurpose;

namespace BodyArchitect.UnitTests.V2
{
    [TestFixture]
    public class TestService_GetSupplementsCycleDefinitions:TestServiceBase
    {
        List<Profile> profiles = new List<Profile>();
        List<Suplement> supplements = new List<Suplement>();
        List<SupplementCycleDefinition> definitions = new List<SupplementCycleDefinition>();

        public override void BuildDatabase()
        {
            using (var tx = Session.BeginTransaction())
            {
                profiles.Clear();
                definitions.Clear();
                supplements.Clear();
                profiles.Add(CreateProfile(Session, "test1"));
                profiles.Add(CreateProfile(Session, "test2"));

                var supplement = CreateSupplement("sup1");
                supplements.Add(supplement);
                supplement = CreateSupplement("sup2");
                supplements.Add(supplement);
                supplement = CreateSupplement("sup3");
                supplements.Add(supplement);
                supplement = CreateSupplement("sup4");
                supplements.Add(supplement);

                var def = CreateSupplementCycleDefinition("def1",3,DateTime.UtcNow.Date,profiles[0], supplements[1]);
                def.Language = Language.Languages[0].Shortcut;
                def.Difficult = TrainingPlanDifficult.Professional;
                Session.Update(def);
                definitions.Add(def);
                def = CreateSupplementCycleDefinition("def2", 1, DateTime.UtcNow.Date.AddDays(5), profiles[0], supplements[0]);
                def.Purpose = WorkoutPlanPurpose.Definition;
                Session.Update(def);
                definitions.Add(def);
                def = CreateSupplementCycleDefinition("def3", 2.2f, DateTime.UtcNow.Date.AddDays(-2), profiles[0], supplements[2], supplements[3]);
                definitions.Add(def);
                def = CreateSupplementCycleDefinition("def4", 5.1f, DateTime.UtcNow.Date.AddDays(4), profiles[1], supplements[2], supplements[3], supplements[0]);
                def.Status = PublishStatus.Published;
                Session.Update(def);
                definitions.Add(def);

                def = CreateSupplementCycleDefinition("def5", 0, DateTime.UtcNow.Date.AddDays(7), profiles[0], supplements[1]);
                def.CanBeIllegal = true;
                def.Status = PublishStatus.Published;
                Session.Update(def);
                definitions.Add(def);

                def = CreateSupplementCycleDefinition("def6", 0.3f, DateTime.UtcNow.Date.AddDays(8), profiles[1], supplements[1]);
                def.Status = PublishStatus.Published;
                def.Language = Language.Languages[1].Shortcut;
                Session.Update(def);
                definitions.Add(def);

                def = CreateSupplementCycleDefinition("def7", 0.34f, DateTime.UtcNow.Date.AddDays(5), profiles[1], supplements[1]);
                def.Status = PublishStatus.Private;
                Session.Update(def);

                profiles[0].FavoriteSupplementCycleDefinitions.Add(definitions.Where(x=>x.Name=="def6").Single());
                Session.Update(profiles[0]);
                tx.Commit();
            }
        }

        void assert(PagedResult<SupplementCycleDefinitionDTO> pack, params string[] names)
        {
            Assert.AreEqual(names.Length, pack.AllItemsCount);
            foreach (var index in names)
            {
                Assert.IsNotNull(pack.Items.SingleOrDefault(x => x.GlobalId == definitions.Single(h => h.Name == index).GlobalId));
            }

        }

        SupplementCycleDefinition CreateSupplementCycleDefinition(string name,float rating,DateTime creationDate,Profile profile=null,params Suplement[] supplements)
        {
            SupplementCycleDefinition definition = new SupplementCycleDefinition();
            definition.Name = name;
            definition.Language = "en";
            definition.CreationDate = creationDate;
            definition.Rating = rating;
            definition.Profile = profile;
            SupplementCycleWeek week = new SupplementCycleWeek();
            week.Definition = definition;
            week.CycleWeekStart = 1;
            week.CycleWeekEnd = 4;
            definition.Weeks.Add(week);
            foreach (var supplement in supplements)
            {
                SupplementCycleDosage dosage = new SupplementCycleDosage();
                dosage.Supplement = supplement;
                dosage.Week = week;
                week.Dosages.Add(dosage);
            }
            insertToDatabase(definition);
            return definition;
        }

        [Test]
        public void ForOnePurpose()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            param.Purposes.Add(Service.V2.Model.WorkoutPlanPurpose.Definition);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            assert(result, "def2");
        }

        [Test]
        public void ByPlanId()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            param.PlanId = definitions[0].GlobalId;

            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            assert(result, definitions[0].Name);
        }

        [Test]
        public void ForOneDifficult()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            param.Difficults.Add(Service.V2.Model.TrainingPlans.TrainingPlanDifficult.Professional);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            assert(result, "def1");
        }

        [Test]
        public void OnlyMine()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            param.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Mine);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            assert(result, "def1", "def2", "def3", "def5");
        }

        [Test]
        public void OnlyOneLanguage()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            param.Languages.Add(Language.Languages[0].Shortcut);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            assert(result, "def1");
        }

        [Test]
        public void OnlyTwoLanguages()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            param.Languages.Add(Language.Languages[0].Shortcut);
            param.Languages.Add(Language.Languages[1].Shortcut);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            assert(result, "def1", "def6");
        }

        [Test]
        public void OnlyFavorites()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            param.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Favorites);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            assert(result, "def6");
        }

        [Test]
        public void OnlyOther()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            param.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Other);
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            assert(result, "def4");
        }

        [Test]
        public void All()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            Assert.AreEqual(definitions.Count, result.AllItemsCount);
            Assert.AreEqual(definitions.Count, result.Items.Count);
        }

        [Test]
        public void PagedResults()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 2;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService Service)
            {
                result = Service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            Assert.AreEqual(definitions.Count, result.AllItemsCount);
            Assert.AreEqual(2, result.Items.Count);
        }

        #region Sort order
        [Test]
        public void SortNewest_Asc()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     param.SortAscending = true;
                                     param.SortOrder = SearchSortOrder.Newest;
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            Assert.AreEqual("def3", result.Items[0].Name);
            Assert.AreEqual("def1", result.Items[1].Name);
            Assert.AreEqual("def4", result.Items[2].Name);
            Assert.AreEqual("def2", result.Items[3].Name);
            Assert.AreEqual("def5", result.Items[4].Name);
        }

        [Test]
        public void SortNewest_Desc()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                param.SortAscending = false;
                param.SortOrder = SearchSortOrder.Newest;
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            Assert.AreEqual("def6", result.Items[0].Name);
            Assert.AreEqual("def5", result.Items[1].Name);
            Assert.AreEqual("def2", result.Items[2].Name);
            Assert.AreEqual("def4", result.Items[3].Name);
            Assert.AreEqual("def1", result.Items[4].Name);
            Assert.AreEqual("def3", result.Items[5].Name);
        }

        [Test]
        public void SortHighestRating_Desc()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                param.SortAscending = false;
                param.SortOrder = SearchSortOrder.HighestRating;
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            Assert.AreEqual("def4", result.Items[0].Name);
            Assert.AreEqual("def1", result.Items[1].Name);
            Assert.AreEqual("def3", result.Items[2].Name);
            Assert.AreEqual("def2", result.Items[3].Name);
        }

        [Test]
        public void SortHighestRating_Asc()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                param.SortAscending = true;
                param.SortOrder = SearchSortOrder.HighestRating;
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            Assert.AreEqual("def5", result.Items[0].Name);
            Assert.AreEqual("def6", result.Items[1].Name);
            Assert.AreEqual("def2", result.Items[2].Name);
            Assert.AreEqual("def3", result.Items[3].Name);
            Assert.AreEqual("def1", result.Items[4].Name);
            Assert.AreEqual("def4", result.Items[5].Name);
        }

        [Test]
        public void SortName_Asc()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                param.SortAscending = true;
                param.SortOrder = SearchSortOrder.Name;
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            Assert.AreEqual("def1", result.Items[0].Name);
            Assert.AreEqual("def2", result.Items[1].Name);
            Assert.AreEqual("def3", result.Items[2].Name);
            Assert.AreEqual("def4", result.Items[3].Name);
        }

        [Test]
        public void SortName_Desc()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                param.SortAscending = false;
                param.SortOrder = SearchSortOrder.Name;
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            Assert.AreEqual("def6", result.Items[0].Name);
            Assert.AreEqual("def5", result.Items[1].Name);
            Assert.AreEqual("def4", result.Items[2].Name);
            Assert.AreEqual("def3", result.Items[3].Name);
            Assert.AreEqual("def2", result.Items[4].Name);
            Assert.AreEqual("def1", result.Items[5].Name);
        }
        #endregion

        [Test]
        public void GetUserRating()
        {
            RatingUserValue rating = new RatingUserValue();
            rating.RatedObjectId = definitions[0].GlobalId;
            rating.Rating = 2;
            rating.ProfileId = profiles[0].GlobalId;
            rating.VotedDate = DateTime.UtcNow;
            insertToDatabase(rating);
            rating = new RatingUserValue();
            rating.RatedObjectId = definitions[0].GlobalId;
            rating.Rating = 5;
            rating.ProfileId = profiles[1].GlobalId;
            rating.VotedDate = DateTime.UtcNow;
            insertToDatabase(rating);

            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            Assert.AreEqual(2, result.Items.Where(x => x.GlobalId == definitions[0].GlobalId).Single().UserRating);
            Assert.AreEqual(null, result.Items.Where(x => x.GlobalId == definitions[1].GlobalId).Single().UserRating);
        }

        [Test]
        public void OnlyLegal()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                param.LegalCriteria=CanBeIllegalCriteria.OnlyLegal;
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            assert(result, "def1", "def2", "def3", "def4", "def6");
        }

        [Test]
        public void OnlyIllegal()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                param.LegalCriteria = CanBeIllegalCriteria.OnlyIllegal;
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            assert(result, "def5");
        }

        [Test]
        public void ForProfile_MySelf()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
                                 {
                                     param.UserId = profiles[0].GlobalId;
                                     param.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Mine);
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            assert(result,"def1", "def2", "def3","def5");
        }

        [Test]
        public void ForProfile_OtherProfile()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                param.UserId = profiles[1].GlobalId;
                param.SearchGroups.Add(WorkoutPlanSearchCriteriaGroup.Mine);
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            assert(result, "def6","def4");
        }

        [Test]
        public void Supplements_One()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                param.Supplements.Add(supplements[0].GlobalId);
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            assert(result, "def2","def4");
        }

        [Test]
        public void Supplements_Two_Or()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                param.Supplements.Add(supplements[0].GlobalId);
                param.Supplements.Add(supplements[2].GlobalId);
                param.SupplementsListOperator = CriteriaOperator.Or;
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            assert(result, "def2", "def3","def4");
        }

        [Test]
        public void Supplements_Two_And_ReturnsEmptyList()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                param.Supplements.Add(supplements[1].GlobalId);
                param.Supplements.Add(supplements[3].GlobalId);
                param.SupplementsListOperator = CriteriaOperator.And;
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            Assert.AreEqual(0,result.AllItemsCount);
        }

        [Test]
        public void Supplements_Two_And_ReturnsDefinition()
        {
            var param = new GetSupplementsCycleDefinitionsParam();
            var profile = (ProfileDTO)profiles[0].Tag;
            SessionData data = CreateNewSession(profile, ClientInformation);
            PartialRetrievingInfo pageInfo = new PartialRetrievingInfo();
            pageInfo.PageSize = 50;

            PagedResult<SupplementCycleDefinitionDTO> result = null;
            RunServiceMethod(delegate(InternalBodyArchitectService service)
            {
                param.Supplements.Add(supplements[0].GlobalId);
                param.Supplements.Add(supplements[3].GlobalId);
                param.SupplementsListOperator = CriteriaOperator.And;
                result = service.GetSupplementsCycleDefinitions(data.Token, param, pageInfo);
            });
            assert(result,  "def4");
        }
    }
}
