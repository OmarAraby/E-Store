using Estore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Estore.Infrastructure.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.FileName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(pi => pi.ImagePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(pi => pi.UploadedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");


            // already don in product config 
            builder.HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
