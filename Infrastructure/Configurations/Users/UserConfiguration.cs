using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Users;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PersonId)
            .HasColumnName("person_id")
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasConversion(x => x.Value, value => new(value))
            .HasMaxLength(255)
            .HasColumnName("password_hash")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.HasIndex(x => x.PersonId)
            .IsUnique();

        builder.HasOne(x => x.Person)
            .WithOne(x => x.User)
            .HasForeignKey<User>(x => x.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(x => x.UserRoles);
    }
}
