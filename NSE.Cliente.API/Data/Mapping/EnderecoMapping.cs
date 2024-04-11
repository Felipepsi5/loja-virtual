using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NSE.Clientes.API.Models;

namespace NSE.Clientes.API.Data.Mapping
{
    public class EnderecoMapping : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Logradouro)
                   .IsRequired()
                   .HasColumnType("VARCHAR(200)");

            builder.Property(c => c.Numero)
                   .IsRequired()
                   .HasColumnType("VARCHAR(50)");

            builder.Property(c => c.Cep)
                  .IsRequired()
                  .HasColumnType("VARCHAR(20)");

            builder.Property(c => c.Complemento)
                  .IsRequired()
                  .HasColumnType("VARCHAR(250)");

            builder.Property(c => c.Bairro)
                  .IsRequired()
                  .HasColumnType("VARCHAR(100)");

            builder.Property(c => c.Cidade)
                  .IsRequired()
                  .HasColumnType("VARCHAR(100)");

            builder.Property(c => c.Estado)
                  .IsRequired()
                  .HasColumnType("VARCHAR(50)");

            builder.ToTable("Enderecos");
        }
    }
}
