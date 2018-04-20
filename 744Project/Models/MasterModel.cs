namespace _744Project.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MasterModel : DbContext
    {
        public MasterModel()
            : base("name=DatabaseConnection")
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<CreditCard> CreditCards { get; set; }
        //public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<ProcessCenter> ProcessCenters { get; set; }
        public virtual DbSet<ProcessCenterTransaction> ProcessCenterTransactions { get; set; }
        public virtual DbSet<Relay> Relays { get; set; }
        public virtual DbSet<RelayToProcessCenterConnection> RelayToProcessCenterConnections { get; set; }
        public virtual DbSet<RelayToRelayConnection> RelayToRelayConnections { get; set; }
        public virtual DbSet<RelayTransaction> RelayTransactions { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<StoreTransaction> StoreTransactions { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Questions> Questions { get; set; }
        public virtual DbSet<SecurityQuestion> SecurityQuestions { set; get; }
        public virtual DbSet<Regions> Regions { get; set; }
        public virtual DbSet<StoresToRelays> StoresToRelays { get; set; }

        //not a necessary part of the system, just used for storing locations of nodes
        public virtual DbSet<NodePosition> NodePositions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(e => e.accountNumber)
                .IsUnicode(false);

            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<CreditCard>();
                //.HasMany(e => e.Transactions)
                //.WithRequired(e => e.CreditCard)
                //.HasForeignKey(e => e.cardID);

            //adding many to many (self) relatioship for Relays
           /* modelBuilder.Entity<Relay>()
                .HasMany(e => e.Relay)*/
        }
    }
}
