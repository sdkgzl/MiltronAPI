using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
namespace DataAccess.Context
{
    public partial class MiltronDbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.Property(e => e.AddedTime).HasColumnType("datetime").HasDefaultValueSql("getdate()");
                entity.HasOne(d => d.AddedByUser)
                    .WithMany(p => p.InverseAddedByUser)
                    .HasForeignKey(d => d.AddedByUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_User_AddedByUserId_User_Id");
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
                entity.HasOne(d => d.LastModifiedByUser)
                    .WithMany(p => p.InverseLastModifiedByUser)
                    .HasForeignKey(d => d.LastModifiedByUserId)
                    .HasConstraintName("FK_User_LastModifiedByUserId_User_Id");
            });            

        }
    }
}
