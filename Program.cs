using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//Primeiros comandos executados.
var builder = WebApplication.CreateBuilder(args);

//Dado que veio do appsettings => Database:SqlServer
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServer"]);
var app = builder.Build();

//Add a função Init(); para ser executada quando o codigo rodar;
var configuration = app.Configuration;
ProductRepository.Init(configuration);


//O Payload vem do body = productRequest; E oq é o serviço do aspnet = context;
app.MapPost("/products", (ProductRequest productRequest, ApplicationDbContext context) => {

    var category = context.Categories.Where(c => c.Id == productRequest.CategoryId).First();
    var product = new Product
    {
        Code = productRequest.Code,
        Name = productRequest.Name,
        Description = productRequest.Description,
        Category = category
    };

    if(productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach ( var item in productRequest.Tags)
        {
            product.Tags.Add(new Tag{ Name = item });
        }
    }
    context.Products.Add(product);
    //Fazer o commit no banco
    context.SaveChanges(); 
    return Results.Created($"/products/{product.Id}", product.Id);
});

app.MapGet("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) => {
    var product = context.Products
    .Include(p => p.Category)
    .Include(p => p.Tags)
    .Where(p => p.Id == id).First();
    if(product != null) {
        return Results.Ok(product);
    }
    return Results.NotFound();
});

app.MapGet("/products", (ApplicationDbContext context) => {

    var products = context.Products.ToList();

    return Results.Ok(products);
});

app.MapPut("/products/{id}", ([FromRoute] int id, ProductRequest productRequest, ApplicationDbContext context) => {
    
    var category = context.Categories.Where(c => c.Id == productRequest.CategoryId).First();

    var editProduct = context.Products
    .Include(p => p.Tags)
    .Where(p => p.Id == id).FirstOrDefault();

    if (editProduct != null)
    {
        editProduct.Code = productRequest.Code;
        editProduct.Name = productRequest.Name;
        editProduct.Description = productRequest.Description;
        editProduct.Category = category;
        editProduct.Tags = new List<Tag>();

        if (productRequest.Tags != null)
        {
            editProduct.Tags = new List<Tag>();
            foreach (var item in productRequest.Tags)
            {
                editProduct.Tags.Add(new Tag { Name = item });
            }
        }
        context.SaveChanges();
        return Results.Ok();
    }
    return Results.NotFound();
});

app.MapDelete("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) => {
    var product = context.Products.Where(p => p.Id == id).FirstOrDefault();
    if(product != null) {
        context.Products.Remove(product);
        context.SaveChanges();
        return Results.Ok("Produto: "+product.Name+ ". Deletado com sucesso.");
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
