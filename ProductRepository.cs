using Microsoft.AspNetCore.Mvc;
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

    public static Product GetByCode(string Code) {
        try {
            var product = Products.FirstOrDefault(p => p.Code.Equals(Code));
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

    public static Product Remove([FromBody] string Code) {
        try {
            var product = GetByCode(Code);
            Products.Remove(product);
            return product;
        } catch {
            return null;
        }
    }
}
