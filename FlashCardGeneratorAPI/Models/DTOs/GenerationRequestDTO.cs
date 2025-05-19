using NLanguageTag;

namespace FlashCardGeneratorAPI.Models;

public class GenerationRequestDTO
{
    public required string DeckName { get; set; }
    public required string Level { get; set; }
    public required string TargetLanguage { get; set; }
    public required string OriginalLanguage { get; set; }
    public string ? Region { get; set; }
    public int Count { get; set; }
}