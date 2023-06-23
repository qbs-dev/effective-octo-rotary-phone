using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TracklyApi.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool? IsEmailVerified { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string MiddleName { get; set; } = null!;

    public DateTime RegistrationDate { get; set; }

    public DateTime LastAccessDate { get; set; }

    public virtual ICollection<EmailConfirmation> EmailConfirmations { get; set; } = new List<EmailConfirmation>();

    public virtual ICollection<ManagedUrl> ManagedUrls { get; set; } = new List<ManagedUrl>();

    public virtual ICollection<RefreshSession> RefreshSessions { get; set; } = new List<RefreshSession>();
}

public class UserTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);

        builder.ToTable("users");

        builder.HasIndex(e => e.Email, "UNQ_email").IsUnique();

        builder.Property(e => e.Id).UseIdentityAlwaysColumn();
        builder.Property(e => e.Email).HasMaxLength(128);
        builder.Property(e => e.FirstName).HasMaxLength(64);
        builder.Property(e => e.IsEmailVerified);
        builder.Property(e => e.LastAccessDate);
        builder.Property(e => e.LastName).HasMaxLength(64);
        builder.Property(e => e.MiddleName).HasMaxLength(64);
        builder.Property(e => e.Password).HasMaxLength(64);
        builder.Property(e => e.RegistrationDate);
    }
}
