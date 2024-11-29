
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
