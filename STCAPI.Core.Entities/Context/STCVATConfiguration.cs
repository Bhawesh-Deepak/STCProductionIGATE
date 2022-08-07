using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using STCAPI.Core.Entities.STCVAT;
using System;

namespace STCAPI.Core.Entities.Context
{
    public class STCVATConfiguration : IEntityTypeConfiguration<STCVATForm>
    {
        public void Configure(EntityTypeBuilder<STCVATForm> modelBuilder)
        {
            modelBuilder.Property(r => r.IsActive).HasDefaultValue(true);
            modelBuilder.Property(r => r.IsDeleted).HasDefaultValue(false);
            modelBuilder.Property(r => r.CreatedDate).HasDefaultValue(DateTime.Now.Date);
            modelBuilder.Property(r => r.UpdatedDate).HasDefaultValue(null);

        }
    }
}
