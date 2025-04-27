using NLanguageTag;

namespace FlashCardGeneratorAPI.Models;

public abstract class CardView
{
    public required string Text { get; set; }
    public required Language Language { get; set; }
    public string? Region { get; set; }
}