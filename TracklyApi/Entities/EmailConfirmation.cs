using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TracklyApi.Entities;

public enum ConfirmationTypes
{
    Register = 0,
    ResetPassword,
    ChangePassword,
    ChangeEmail
}

public partial class EmailConfirmation
{
    public Guid Id { get; set; }

    public int User { get; set; }

    public ConfirmationTypes ConfirmationType { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresIn { get; set; }

    public virtual User UserNavigation { get; set; } = null!;
}

public class EmailConfirmationTypeConfiguration : IEntityTypeConfiguration<EmailConfirmation>
{
    public void Configure(EntityTypeBuilder<EmailConfirmation> builder)
    {
        builder.HasKey(e => e.Id);

        builder.ToTable("email_confirmations");

        builder.HasIndex(e => e.User, "IXFK_email_confirmations_users");

        builder.Property(e => e.Id);
        builder.Property(e => e.ConfirmationType);
        builder.Property(e => e.CreatedAt);
        builder.Property(e => e.ExpiresIn);
        builder.Property(e => e.User);

        builder.HasOne(d => d.UserNavigation).WithMany(p => p.EmailConfirmations)
            .HasForeignKey(d => d.User)
            .HasConstraintName("FK_email_confirmations_users")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
