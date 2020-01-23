using System;
using System.Windows.Forms;
using BodyArchitect.DataAccess.NHibernate.Mappings;
using BodyArchitect.Model;
using BodyArchitect.Service.V2;
using BodyArchitect.Service.V2.Model;
using BodyArchitect.Shared;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using AccountType = BodyArchitect.Model.AccountType;
using Configuration = NHibernate.Cfg.Configuration;
using PlatformType = BodyArchitect.Model.PlatformType;
using Profile = BodyArchitect.Model.Profile;

namespace BodyArchitectTools
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Configuration createNHConfiguration()
        {
            var cfg = new Configuration();

            ModelMapper mapping = new ModelMapper();
            mapping.AddMappings(typeof(ProfileMapping).Assembly.GetTypes());
            cfg.AddMapping(mapping.CompileMappingForAllExplicitlyAddedEntities());
            cfg.Configure();
            return cfg;
        }

        private void btnCreateDb_Click(object sender, EventArgs e)
        {

            var cfg = createNHConfiguration();

            try
            {
                SchemaExport exp = new SchemaExport(cfg);
               exp.Execute(true, true, false);
                
                //MySql.Data.MySqlClient.MySqlScript script = new MySqlScript();

                //script.Connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
                //script.Query = File.ReadAllText("..\\..\\DbScripts\\MySql.sql");
                //script.Execute();
               var factory = cfg.BuildSessionFactory();
                using (var session = factory.OpenSession())
                using(var tx = session.BeginTransaction())
                {
                    ExercisesBuilderPL builder = new ExercisesBuilderPL();
                    foreach (var exercise in builder.Create())
                    {
                        //exercise.Status = PublishStatus.Published;
                        session.Save(exercise);
                    }
                    SuplementsBuilder suplementsBuilder = new SuplementsBuilder();
                    foreach (var supplement in suplementsBuilder.Create())
                    {
                        session.Save(supplement);
                    }
                    session.Flush();

                    Profile profile = new Profile();
                    profile.UserName = ProfileDTO.AdministratorName;
                    //todo:user better email
                    profile.Email = "shik@email.TK";
                    profile.Birthday = DateTime.Now.AddYears(-30);
                    profile.Privacy=new ProfilePrivacy();
                    profile.Privacy.Searchable = false;
                    profile.Settings=new ProfileSettings();
                    profile.Licence=new BodyArchitect.Model.LicenceInfo();
                    profile.Licence.AccountType = AccountType.Administrator;
                    profile.CountryId = Country.GetByTwoLetters("PL").GeoId;
                    profile.DataInfo = new DataInfo();
                    session.Save(profile);

                    Profile deletedProfile = new Profile();
                    deletedProfile.UserName = "(Deleted)";
                    deletedProfile.Email = "shik@email.TK";
                    deletedProfile.Birthday = DateTime.Now.AddYears(-30);
                    deletedProfile.Privacy = new ProfilePrivacy();
                    deletedProfile.Privacy.Searchable = false;
                    deletedProfile.IsDeleted = true;
                    deletedProfile.CountryId = Country.GetByTwoLetters("PL").GeoId;
                    deletedProfile.DataInfo = new DataInfo();
                    deletedProfile.Settings = new ProfileSettings();
                    deletedProfile.Licence = new BodyArchitect.Model.LicenceInfo();
                    deletedProfile.Licence.AccountType = AccountType.Administrator;
                    session.Save(deletedProfile);

                    APIKey apiKey = new APIKey();
                    apiKey.ApiKey = new Guid("14375345-3755-46f7-af3f-0d328e3a2cc0");
                    apiKey.ApplicationName = "BodyArchitect";
                    apiKey.EMail = "mail@mail.com";
                    apiKey.RegisterDateTime = DateTime.UtcNow;
                    apiKey.Platform = PlatformType.Windows;
                    session.Save(apiKey);

                    apiKey = new APIKey();
                    apiKey.ApiKey = new Guid("A3C9D236-2566-40CF-A430-0802EE439B9C");
                    apiKey.ApplicationName = "BodyArchitect for WP7";
                    apiKey.EMail = "shik@gmail.com";
                    apiKey.RegisterDateTime = DateTime.UtcNow;
                    apiKey.Platform = PlatformType.WindowsPhone;
                    session.Save(apiKey);

                    apiKey = new APIKey();
                    apiKey.ApiKey = new Guid("87AFE52C-9EAC-4BAF-949C-08E919285ADA");
                    apiKey.ApplicationName = "BodyArchitect for iPhone";
                    apiKey.EMail = "shik@gmail.com";
                    apiKey.RegisterDateTime = DateTime.UtcNow;
                    apiKey.Platform = PlatformType.iPhone;
                    session.Save(apiKey);

                    var deletedExercise = new Exercise(Guid.NewGuid());
                    deletedExercise.Name = "(Missing)";
                    deletedExercise.Shortcut = "__D";
                    deletedExercise.IsDeleted = true;
                    session.Save(deletedExercise);
                    tx.Commit();
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            
            
            //fluentConf.BuildSessionFactory();
        }

        private void btnUpdateDb_Click(object sender, EventArgs e)
        {
            var cfg = createNHConfiguration();
            //var factory = cfg.BuildSessionFactory();
            //    using (var session = factory.OpenSession())
            //    using (var tx = session.BeginTransaction())
            //    {
            //        var profiles=session.QueryOver<Profile>().List();
            //        foreach (var profile in profiles)
            //        {
            //            if(profile.DataInfo==null)
            //            {
            //                profile.DataInfo=new DataInfo();
            //                session.Update(profile);
            //            }
            //        }
            //        tx.Commit();
            //    }

                var update = new SchemaUpdate(cfg);
                update.Execute(true, true);
        }

        private void btnGenerateLicenceKey_Click(object sender, EventArgs e)
        {
            LicenceGenerator generator = new LicenceGenerator();
            txtLicenceKey.Text = generator.GenerateLicenceKey((int) numericUpDown1.Value);
        }

        private void btnComputeHash_Click(object sender, EventArgs e)
        {
            txtPasswordHash.Text = txtPassword.Text.ToSHA1Hash();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var cfg = createNHConfiguration();

            try
            {
                var factory = cfg.BuildSessionFactory();
                using (var session = factory.OpenSession())
                using (var tx = session.BeginTransaction())
                {
                    var profile=session.QueryOver<Profile>().Where(x => x.UserName == txtUserNameForPassword.Text).SingleOrDefault();
                    if(profile==null)
                    {
                        MessageBox.Show("Profile nie istnieje","BA",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        return;
                    }
                    profile.Password = txtPassword.Text.ToSHA1Hash();
                    session.Update(profile);
                    tx.Commit();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "BA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
