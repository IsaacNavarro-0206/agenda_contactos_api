using Microsoft.EntityFrameworkCore;

namespace AgendaContactos.Api;

public sealed class ContactDbContext(
    DbContextOptions<ContactDbContext> options) : DbContext(options)
{
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
            {
                entity.HasMany(user => user.Contacts)
                    .WithOne(contact => contact.User)
                    .HasForeignKey(contact => contact.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(user => user.Name)
                        .HasMaxLength(255)
                        .IsRequired();

                entity.Property(user => user.Email)
                    .HasMaxLength(320)
                    .IsRequired();

                entity.Property(user => user.Password)
                    .HasMaxLength(512)
                    .IsRequired();

                entity.HasIndex(user => user.Email)
                    .IsUnique();
            });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.Property(contact => contact.Name)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(contact => contact.Phone)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(contact => contact.Email)
                .HasMaxLength(320)
                .IsRequired();
        });
    }

    public void UpdateTimestamps()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(entity => entity.CreatedAt).IsModified = false;
                entry.Entity.UpdatedAt = now;
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();

        return base.SaveChangesAsync(cancellationToken);
    }
}