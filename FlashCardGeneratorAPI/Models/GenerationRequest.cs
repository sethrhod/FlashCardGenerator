using NLanguageTag;

namespace FlashCardGeneratorAPI.Models;

public class GenerationRequest
{
    public required LanguageLevel Level { get; set; }
    public required LanguageCode TargetLanguage { get; set; }
    public required LanguageCode OriginalLanguage { get; set; }
    public string ? Region { get; set; }
    public int Count { get; set; }
}