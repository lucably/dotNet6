
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
