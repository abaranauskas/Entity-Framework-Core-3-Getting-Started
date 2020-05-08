using Microsoft.EntityFrameworkCore;
using SamuraiApp.Domain;

namespace SamuraiApp.Data
{
    public class SamuraiContext : DbContext
    {
        public SamuraiContext(DbContextOptions options)
            :base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Clan> Clans { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<SamuraiBattleStat> SamuraiBattleStats { get; set; }

        //public static readonly ILoggerFactory ConsoleLoggerFactory
        //    = LoggerFactory.Create(builder =>
        //    {
        //        builder.AddFilter((category, level) =>                
        //            category == DbLoggerCategory.Database.Command.Name &&
        //            level == LogLevel.Information)
        //        .AddConsole();
        //    });

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder
        //        .UseLoggerFactory(ConsoleLoggerFactory)
        //        .EnableSensitiveDataLogging()
        //        .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=SamuraiAppData;Trusted_Connection=True;MultipleActiveResultSets=true");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SamuraiBattle>().HasKey(s => new { s.SamuraiId, s.BattleId });
            modelBuilder.Entity<Horse>().ToTable("Horses");
            
            //for keyless table
            //wont be tracked by default
            modelBuilder.Entity<SamuraiBattleStat>().HasNoKey().ToView("SamuraiBattleStats");                       
        }
    }
}
