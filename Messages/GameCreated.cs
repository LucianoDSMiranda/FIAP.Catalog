namespace FIAP.Catalog.Messages;

public class GameCreated
{
    public int GameId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}