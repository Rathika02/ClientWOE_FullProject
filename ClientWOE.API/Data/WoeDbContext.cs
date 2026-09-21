using ClientWOE.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientWOE.API.Data;

public class WoeDbContext : DbContext
{
    public WoeDbContext(DbContextOptions<WoeDbContext> options) : base(options) { }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<TestMaster> TestMasters => Set<TestMaster>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<WorkOrderTestDetail> WorkOrderTestDetails => Set<WorkOrderTestDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Client>()
            .HasIndex(x => x.ClientCode)
            .IsUnique();

        modelBuilder.Entity<Patient>()
            .HasIndex(x => x.PatientCode)
            .IsUnique();

        modelBuilder.Entity<TestMaster>()
            .HasIndex(x => x.TestCode)
            .IsUnique();

        modelBuilder.Entity<WorkOrder>()
            .HasIndex(x => x.WoeNumber)
            .IsUnique();

        modelBuilder.Entity<Client>()
            .HasMany(x => x.Patients)
            .WithOne(x => x.Client)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Client>()
            .HasMany(x => x.WorkOrders)
            .WithOne(x => x.Client)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Patient>()
            .HasMany(x => x.WorkOrders)
            .WithOne(x => x.Patient)
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkOrder>()
            .HasMany(x => x.TestDetails)
            .WithOne(x => x.WorkOrder)
            .HasForeignKey(x => x.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TestMaster>()
            .HasMany(x => x.WorkOrderTestDetails)
            .WithOne(x => x.TestMaster)
            .HasForeignKey(x => x.TestId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Client>().HasData(
            new Client { ClientId = 1, ClientCode = "CLI001", ClientName = "Apollo Diagnostics", IsActive = true },
            new Client { ClientId = 2, ClientCode = "CLI002", ClientName = "City Care Hospital", IsActive = true },
            new Client { ClientId = 3, ClientCode = "CLI003", ClientName = "Walk-In Client", IsActive = true }
        );

        modelBuilder.Entity<TestMaster>().HasData(
            new TestMaster { TestId = 1, TestCode = "CBC", TestName = "Complete Blood Count", Rate = 350, IsActive = true },
            new TestMaster { TestId = 2, TestCode = "FBS", TestName = "Fasting Blood Sugar", Rate = 150, IsActive = true },
            new TestMaster { TestId = 3, TestCode = "LFT", TestName = "Liver Function Test", Rate = 700, IsActive = true },
            new TestMaster { TestId = 4, TestCode = "KFT", TestName = "Kidney Function Test", Rate = 650, IsActive = true },
            new TestMaster { TestId = 5, TestCode = "LIPID", TestName = "Lipid Profile", Rate = 550, IsActive = true },
            new TestMaster { TestId = 6, TestCode = "TSH", TestName = "TSH", Rate = 300, IsActive = true }
        );
    }
}
