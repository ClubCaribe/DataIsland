using Streamail.Models.db;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streamail.Classes.db
{
    public class DiStreamailContext : DbContext
    {
        public DbSet<StreamailHeaders> Headers { get; set; }
        public DbSet<StreamailMessage> Messages { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<RawMessage> RawMessages { get; set; }
        public DbSet<StreamailAdministrator> Administrators { get; set; }
        public DbSet<ReadStatus> ReadStatuses { get; set; }
        public DbSet<MessageCustomField> MessageCustomFields { get; set; }

        public DiStreamailContext()
        {

        }

        public DiStreamailContext(DbConnection connection)
            : base(connection, true)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //does not pluralize table names
            modelBuilder.Conventions
                .Remove<PluralizingTableNameConvention>();

        }

        public static void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            //does not pluralize table names
            modelBuilder.Conventions
                .Remove<PluralizingTableNameConvention>();
        }
    }
}
