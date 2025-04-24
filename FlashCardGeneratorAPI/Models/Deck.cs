namespace FlashCardGeneratorAPI.Models;

public class Deck
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string UserId { get; set; }
    public required List<FlashCard> FlashCards { get; set; } = [];
}