using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//Primeiros comandos executados.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>();
var app = builder.Build();

//Add a função Init(); para ser executada quando o codigo rodar;
var configuration = app.Configuration;
ProductRepository.Init(configuration);




app.MapGet("/", () => "Hello World!");
app.MapGet("/user", () => new {Name = "Teste", Age = 20});

app.MapGet("/addHeader", (HttpResponse response) => {
    response.Headers.Append("Teste", "Valor do Teste");
    return "Header adicionado!";
    }
);

app.MapPost("/saveProduct",(Product product) => {
    return product.Code + " - " + product.Name;
});

//http://localhost:3000/getProduct?dateStart=x&dateEnd=y
app.MapGet("getProduct", ([FromQuery] string dateStart, [FromQuery] string dateEnd) => {
    return dateStart + " - " + dateEnd;
});

//http://localhost:3000/getProduct/10
app.MapGet("getProduct/{code}", ([FromRoute] string code) => {
    return "O codigo é: " + code;
});

//Parametro pelo Header => Add o product-code no HEADER do Request.
app.MapGet("/getProductWithHeader", (HttpRequest request) => {
    return request.Headers["product-code"].ToString();
});

//Refatorando os metodos CRUD para o padrão e add Status Code:

//Results  => Tem os statusCode.
app.MapPost("/products", (Product product) => {
    ProductRepository.Add(product);
    return Results.Created($"/products/{product.Code}", product.Code);
});

app.MapGet("/products/{code}", ([FromRoute] int code) => {
    var product = ProductRepository.GetByCode(code);
    if(product != null) {
        return Results.Ok(product);
    }
    return Results.NotFound();
});

app.MapGet("/products", () => {
    return Results.Ok(ProductRepository.GetAllProduct());
});

app.MapPut("/products", ([FromBody] Product product) => {
    var editProduct = ProductRepository.Edit(product);
    if(editProduct != null) {
        return Results.Ok(editProduct);
    }
    return Results.NotFound();
});

app.MapDelete("/products/{code}", ([FromRoute] int code) => {
    var product = ProductRepository.Remove(code);
    if(product != null) {
        return Results.Ok(product);
    }
    return Results.NotFound();
});

//Adicionando variaveis de configuração;
if(app.Environment.IsDevelopment()) {
    // Pega somente os dados do appsettings.Development.json (Repare que a config do banco de dados são diferentes do Development e o de Production).
    // Para funcionar deve mudar no arquivo Properties/launchSettings.json o apontamento 
    
    /* 
        "environmentVariables": {
            "ASPNETCORE_ENVIRONMENT": "Development" <= Aqui tem q ser o "Development" se quiser de produção coloque "Production"; 
        }
    */
    app.MapGet("/configuration/database", (IConfiguration configuration) => {
        return Results.Ok($"Connection: {configuration["Database:Connection"]}, Port: {configuration["Database:Port"]}");
    });
}

app.Run();

//passando a função para static para ela "sobreviver" a cada requisição.
//Criando a class ProductRepository responsavel por fazer os CRUD;
public static class ProductRepository {

    public static List<Product> Products {get; set;}

    //Inicializando a lista de Produtos com os dados que vieram da variavel de ambiente. (appsettings.json);
    public static void Init(IConfiguration configuration) {
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;
    }

    public static void Add(Product product) {
        if(Products == null) {
            Products = new List<Product>();
        }

        Products.Add(product);
    }

    public static List<Product> GetAllProduct() {
        try {
            return Products;
        } catch {
            return null;
        }
    }

    public static Product GetByCode(int Code) {
        try {
            var product = Products.FirstOrDefault(p => p.Code == Code);
            return product;
        } catch {
            return null;
        }
    }

    public static Product Edit([FromBody] Product product)  {
        try {
            var productSave = GetByCode(product.Code);
            productSave.Name = product.Name;
            return productSave;
        } catch {
            return null;
        }
    }

    public static Product Remove([FromBody] int Code) {
        try {
            var product = GetByCode(Code);
            Products.Remove(product);
            return product;
        } catch {
            return null;
        }
    }
}


//Relacionamento Category <-> Product one to one
public class Category
{
    public int Id { get; set; }

    public string Name { get; set; }
}


//Relacionamento Product -> Tag One to Many
public class Tag
{
    public int Id { get; set; }

    public string Name { get; set; }

    /*
        Como queremos que o ProductId seja Obrigatório aqui
        precisamos referenciar aqui tb com o Id do produto
    */
    public int ProductId { get; set; }

}

//Classe do produto;
public class Product
{
    public int Id { get; set; }

    public int Code { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public Category Category { get; set; }

    /*
        Para fazer a chave estrangeira precisa ter o nome da variavel (Propriedade de navegação)
        que criamos acima, no caso, Category e ter a chave unica da Classe Category juntamente com o msm tipo ou seja, int Id
        Tornando a obrigatória, ou seja, not null.
    */
    public int CategoryId {  get; set; }

    public List<Tag> Tags { get; set; }
}


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

public class ApplicationDbContext: DbContext
{
    public DbSet<Product> Products { get; set; }

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=localhost;Database=Products;User Id=sa;Password=@Sql2019;MultipleActiveResultSets=true;Encrypt=YES;TrustServerCertificate=YES");
}