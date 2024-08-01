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

app.Run();


public class Product {
    public int Code { get; set; }

    public string Name { get; set; }
}