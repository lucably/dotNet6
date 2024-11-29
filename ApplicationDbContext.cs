using Microsoft.EntityFrameworkCore;


//Classe do banco;

/*
    Libs Utilizadas:

        dotnet add package Microsoft.EntityFrameworkCore
        dotnet add package Microsoft.EntityFrameworkCore.Design
        dotnet add package Microsoft.EntityFrameworkCore.SqlServer

    Para Migrations:
        
        dotnet tool install --global dotnet-ef

        Para Executar a Criação das Migrations:
        
            obs: InitialDb seria a mensagem do commit (doq foi feito)
            dotnet ef migrations add InitialDb

        Para Aplicar as Migrations no Banco de Dados:

            dotnet ef database update

        Para Reverter as Migrations no Banco de Dados:

            dotnet ef migrations remove   (remove o ultimo migration)

        Para Atualizar um Migration ja Existente:
 
            dotnet ef database update {nome_arquivo_migration}
            
*/

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    //Recebe como construtor o options e passa para o pai base(options) o simbolo ":" significa herança
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    //Serve para alterar as configurações das criações das classes.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Aqui to dizendo que na classe Product o campo description possui essas caracteristicas, visto que antes ela era com tam ininito de caracteres
        modelBuilder.Entity<Product>()
            .Property(p => p.Description).HasMaxLength(500).IsRequired(false);
        modelBuilder.Entity<Product>()
            .Property(p => p.Name).HasMaxLength(100).IsRequired();
        modelBuilder.Entity<Product>()
            .Property(p => p.Code).IsRequired();

        //Depois de realizar essas atualizações como mexe no banco de dados é necessario rodar novamente as Migrations.
    }
}

