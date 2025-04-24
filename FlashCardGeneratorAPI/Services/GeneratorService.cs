using System.Text.Json;
using FlashCardGeneratorAPI.Models;
using FluentResults;
using OpenAI.Chat;

namespace FlashCardGeneratorAPI.Services;

public class GeneratorService : IGeneratorService
{
    private readonly ChatClient _client;
    private List<ChatMessage> _messages = new();
    private readonly ChatCompletionOptions _options;
    
    public GeneratorService(string apiKey, string model)
    {
        _client = new(model: model, apiKey: apiKey);
        
        // Load the JSON schema from the file
        var jsonSchema = File.ReadAllText("FlashCardListSchema.json") ?? throw new Exception("Failed to read JSON schema");
        _options = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "FlashCardList",
                jsonSchema: BinaryData.FromString(jsonSchema),
                jsonSchemaIsStrict: false),
            
        };
    }
    
    public async Task<Result<List<FlashCard>>> GenerateFlashCards(GenerationRequest request, CancellationToken cancellationToken)
    {
        string serializedRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
        _messages.Add(serializedRequest);
        ChatCompletion completion = await _client.CompleteChatAsync(_messages, _options);
        using JsonDocument structuredJson = JsonDocument.Parse(completion.Content[0].Text);
        List<FlashCard> flashCardsList = new();
        var flashCardsProperty = structuredJson.RootElement.GetProperty("flashCards");
        
        foreach (var flashCardElement in flashCardsProperty.EnumerateArray())
        {
            try
            {
                FlashCard flashCard = Newtonsoft.Json.JsonConvert.DeserializeObject<FlashCard>(flashCardElement.GetRawText())
                                      ?? throw new Exception("Failed to deserialize JSON");
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
    Task<Result<List<FlashCard>>> GenerateFlashCards(GenerationRequest request, CancellationToken cancellationToken);
}