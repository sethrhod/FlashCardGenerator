using NLanguageTag;

namespace FlashCardGeneratorAPI.Models;

public class GenerationRequest
{
    public required LanguageLevel Level { get; set; }
    public required Language TargetLanguage { get; set; }
    public required Language OriginalLanguage { get; set; }
    public string ? Region { get; set; }
    public int Count { get; set; }
}