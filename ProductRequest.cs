
//A Mesma coisa que ProductDto
//Representação de um payload.
//Uma classe imutável.
public record ProductRequest(
    string Code, string Name, string Description, int CategoryId, List<string> Tags
    );
