using gsst.Model;
using gsst.Model.FuelStuff;
using gsst.Model.User;
using Microsoft.EntityFrameworkCore;

namespace gsst.Services
{
    public class AppDbContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<User> Users { get; set; }
        public DbSet<BonusCard> BonusCards { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<Fuel> Fuels { get; set; }
        public DbSet<Good> Goods { get; set; }

        public DbSet<Pump> Pumps { get; set; }
        public DbSet<Tank> Tanks { get; set; }
        public DbSet<FuelType> FuelTypes { get; set; }

        public AppDbContext(string connectionString)
        {
            _connectionString = connectionString;
            Database.EnsureCreated();
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(_connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FullName = "Михайло Стадніков",
                    Username = "admin",
                    Password = "admin",
                    Role = UserRoles.Admin
                },
                new User
                {
                    Id = 2,
                    FullName = "Олексій Романенко",
                    Username = "cashier",
                    Password = "cashier",
                    Role = UserRoles.Cashier
                }
            );

            modelBuilder.Entity<FuelType>().HasData(
                new FuelType { Id = 1, Name = "A-95" },
                new FuelType { Id = 2, Name = "A-92" },
                new FuelType { Id = 3, Name = "Diesel" },
                new FuelType { Id = 4, Name = "Gas" }
            );

            modelBuilder.Entity<Pump>().HasData(
                new Pump { Id = 1, Name = "Pump 1" },
                new Pump { Id = 2, Name = "Pump 2" },
                new Pump { Id = 3, Name = "Pump 3" },
                new Pump { Id = 4, Name = "Pump 4" }
            );

            modelBuilder.Entity<Tank>().HasData(
                new { Id = 1, Capacity = 10000.0, Volume = 5000.0, FuelTypeId = 1, PumpId = 1 },
                new { Id = 2, Capacity = 10000.0, Volume = 5000.0, FuelTypeId = 2, PumpId = 2 },
                new { Id = 3, Capacity = 10000.0, Volume = 5000.0, FuelTypeId = 3, PumpId = 3 },
                new { Id = 4, Capacity = 10000.0, Volume = 5000.0, FuelTypeId = 4, PumpId = 4 }
            );

            modelBuilder.Entity<BonusCard>().HasData(
                new BonusCard { Id = 1, Barcode = "1234", BonusBalance = 100, ClientName = "John Doe" },
                new BonusCard { Id = 2, Barcode = "5678", BonusBalance = 200, ClientName = "Jane Smith" }
            );

            //goods
            modelBuilder.Entity<Good>().HasData(
                new Good { Id = 1, Name = "Coca-Cola", Price = 1.5, BarCode = "1234", Image = new byte[] { 0xFF, 0xD8, 0xFF } },
                new Good { Id = 2, Name = "Pepsi", Price = 1.4, BarCode = "5678", Image = new byte[] { 0xFF, 0xD8, 0xFF } },
                new Good { Id = 3, Name = "Water", Price = 1.0, BarCode = "9012", Image = new byte[] { 0xFF, 0xD8, 0xFF } }
            );
        }
    }
}