using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaContactos.Api.Data.Configurations;

public sealed class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.Property(contact => contact.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(contact => contact.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(contact => contact.Phone)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(contact => contact.User)
            .WithMany(user => user.Contacts)
            .HasForeignKey(contact => contact.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}