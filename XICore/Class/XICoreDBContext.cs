using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

namespace XICore
{
    public class XICoreDbContext : DbContext
    {
        private static Dictionary<string, string> coreAppConnections = new Dictionary<string, string>();
        public XICoreDbContext()
            : base("XIDNADbContext")
        //: base(ConnectionString("XIDNA"))
        {
        }

        public XICoreDbContext(string database)
            : base(ConnectionString(database))
        {
        }

        public static string ConnectionString(string database)
        {
            string DataBaseServer = ConfigurationManager.AppSettings["DataBaseServer"];
            string DataBaseUser = ConfigurationManager.AppSettings["DataBaseUser"];
            string DataBasePassword = ConfigurationManager.AppSettings["DataBasePassword"];
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = DataBaseServer;
            sqlBuilder.InitialCatalog = database;
            sqlBuilder.UserID = DataBaseUser;
            sqlBuilder.Password = DataBasePassword;
            sqlBuilder.MultipleActiveResultSets = true;
            //sqlBuilder.PersistSecurityInfo = true;
            //sqlBuilder.IntegratedSecurity = true;
            return sqlBuilder.ToString();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<XICoreDbContext>(null);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<XIDApplication> XIApplications { get; set; }
        public DbSet<XIURLMappings> XIUrlMappings { get; set; }
        //public DbSet<CDLayout> Layout { get; set; }
        public DbSet<XIDOrganisation> Organisation { get; set; }
        public DbSet<XIDMasterData> MasterData { get; set; }
        public DbSet<XIParameter> XIParameter { get; set; }
    }
}