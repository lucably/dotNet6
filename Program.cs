using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

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

// EM baixo esta os Crud do produto;

app.MapPost("/saveProductWithFakeData", (Product product) => {
    ProductRepository.Add(product);
    
});

app.MapGet("/getProductWithFakeData/{code}", ([FromRoute] int code) => {
    var product = ProductRepository.GetByCode(code);
    return product;
});

app.MapPut("/editProductWithFakeData", ([FromBody] Product product) => {
    return ProductRepository.Edit(product);
});

app.MapDelete("/deleteProductWithFakeData/{code}", ([FromRoute] int code) => {
    return ProductRepository.Remove(code);
});

app.Run();

//passando a função para static para ela "sobreviver" a cada requisição.
//Criando a class ProductRepository responsavel por fazer os CRUD;
public static class ProductRepository {

    public static List<Product> Products {get; set;}

    public static void Add(Product product) {
        if(Products == null) {
            Products = new List<Product>();
        }

        Products.Add(product);
    }

    public static Product GetByCode(int code) {
       var product = Products.FirstOrDefault(p => p.Code == code);
       return product;
    }

    public static String Edit([FromBody] Product product)  {
       var productSave = Products.FirstOrDefault(p => p.Code == product.Code);
       if(productSave == null) return "Produto não encontrado!";

       productSave.Name = product.Name;

       return "Produto editado com sucesso, novo valor: " + productSave.Name;
    }

    public static String Remove(int code) {
       var product = Products.FirstOrDefault(p => p.Code == code);

       if(Products.Remove(product)) {
        return "Produto: " +product.Name+ " Removido com sucesso!";
       } else {
        return "Produto não encontrado!";
       } 
    }
}

//Classe do producto;
public class Product {
    public int Code { get; set; }

    public string Name { get; set; }
}