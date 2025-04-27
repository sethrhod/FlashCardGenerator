using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FlashCardGeneratorAPI.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum LanguageLevel
{
    A1,
    A2,
    B1,
    B2,
    C1,
    C2
}