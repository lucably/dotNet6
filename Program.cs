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

app.MapGet("/products/{code}", ([FromRoute] string code) => {
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

app.MapDelete("/products/{code}", ([FromRoute] string code) => {
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
