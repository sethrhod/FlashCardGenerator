using System.Text.Json;
using FlashCardGeneratorAPI.Models;
using FluentResults;
using Newtonsoft.Json.Linq;
using NLanguageTag;
using OpenAI.Chat;

namespace FlashCardGeneratorAPI.Services;

public class GeneratorService : IGeneratorService
{
    private readonly ChatClient _client;
    private List<ChatMessage> _messages =
    [
        ..new ChatMessage[]
        {
            new SystemChatMessage("You are a language learning flash card deck generator. You will receive JSON which will include information correlating to what that should be put in the deck, including the original language, a target language, a language fluency level, and possibly a region. You will output a number of flashcard objects that contain the most important phrases and words of the target language at a specific fluency level of a specific region that uses that target language. It is important to prioritize the target language when choosing phrases. The words and phrases should take into account the dialect and natural language of the specific region and should be a mix of casual and formal. The JSON schema for the response will be attached."),
        }
    ];
    
    private readonly ChatCompletionOptions _options;
    
    public GeneratorService(string apiKey, string model)
    {
        _client = new(model: model, apiKey: apiKey);
        
        // Load the JSON schema from the file
        var jsonSchema = File.ReadAllText("GeneratedCardsSchema.json") ?? throw new Exception("Failed to read JSON schema");
        _options = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "GeneratedCards",
                jsonSchema: BinaryData.FromString(jsonSchema),
                jsonSchemaIsStrict: true),
        };
    }
    
    public async Task<Result<List<FlashCard>>> GenerateFlashCards(GenerationRequestDTO requestDto, CancellationToken cancellationToken)
    {
        string serializedRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestDto);
        _messages.Add(serializedRequest);
        ChatCompletion completion = await _client.CompleteChatAsync(_messages, _options);
        JObject structuredJson = JObject.Parse(completion.Content[0].Text);
        var flashCardsProperty = structuredJson["flashCards"] ?? throw new Exception("FlashCards property is null");
        
        List<FlashCard> flashCardsList = new();
        
        foreach (var flashCardElement in flashCardsProperty.Children())
        {
            try
            {
                var flashCard = new FlashCard
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = Enum.Parse<LanguageLevel>(requestDto.Level),
                    FrontView = new FrontView
                    {
                        Language = Language.Parse(requestDto.OriginalLanguage),
                        Text = flashCardElement["OriginalLanguage"]?["Text"]?.ToString() ??
                               throw new Exception("Original language text is null"),
                    },
                    BackView = new BackView
                    {
                        Language = Language.Parse(requestDto.TargetLanguage),
                        Text = flashCardElement["TargetLanguage"]?["Text"]?.ToString() ?? 
                               throw new Exception("Target language text is null"),
                    }
                };
                flashCardsList.Add(flashCard);
            }
            catch (Exception e)
            {
                return Result.Fail(new Error(e.Message));
            }
        }

        return flashCardsList;
    }
}

public interface IGeneratorService
{
    Task<Result<List<FlashCard>>> GenerateFlashCards(GenerationRequestDTO requestDto, CancellationToken cancellationToken);
}