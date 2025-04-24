using NLanguageTag;

namespace FlashCardGeneratorAPI.Models;

public class FlashCard
{
    public required string Id { get; set; }
    public required FrontView FrontView { get; set; }
    public required BackView BackView { get; set; }
    public required LanguageLevel Level { get; set; }
    public string? Region { get; set; }
    public Uri? PronunciationUri { get; set; }
}