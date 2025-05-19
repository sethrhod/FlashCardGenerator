using FlashCardGeneratorAPI.Models;
using FluentResults;
using NLanguageTag;

namespace FlashCardGeneratorAPI.Repositories;

public class DeckRepository : IDeckRepository
{
    private readonly Dictionary<string, Deck> _decks = new()
    {
        {"1", new Deck
        {
            Id = "1",
            Name = "Spanish Test Deck",
            UserId = "user123",
            OriginalLanguage = Language.ES,
            TargetLanguage = Language.EN,
            Level = LanguageLevel.A1,
            FlashCards = new List<FlashCard>
            {
                new FlashCard
                {
                    Id = "1",
                    FrontView = new FrontView
                    {
                        Text = "Hola",
                        Language = Language.ES
                    },
                    BackView = new BackView
                    {
                        Text = "Hello",
                        Language = Language.EN
                    },
                    Level = LanguageLevel.A1,
                    Region = null,
                    PronunciationUri = null
                }
            }
        }}
    };

    public async Task<Result<IEnumerable<Deck>>> GetDecksAsync(CancellationToken cancellationToken)
    {
        // Simulate async operation
        await Task.Delay(100, cancellationToken);

        if (cancellationToken.IsCancellationRequested)
        {
            return Result.Fail<IEnumerable<Deck>>(new Error("Operation was cancelled."));
        }

        var decks = _decks.Values.ToList();
        if (decks.Count == 0)
        {
            return Result.Fail<IEnumerable<Deck>>(new Error("No decks found."));
        }

        return decks;
    }

    public async Task<Result<Deck>> GetDeckByIdAsync(string id)
    {
        if (_decks.TryGetValue(id, out var deck))
        {
            return deck;
        }

        return Result.Fail<Deck>(new Error($"Deck with ID {id} not found."));
    }

    public async Task<Result<Deck>> CreateDeckAsync(Deck deck)
    {
        var result = _decks.TryAdd(deck.Id, deck);
        return result ? deck : Result.Fail<Deck>(new Error($"Deck with ID {deck.Id} already exists."));
    }
    
    public async Task<Result<Deck>> UpdateDeckAsync(string id, Deck deck)
    {
        if (_decks.ContainsKey(id))
        {
            _decks[id] = deck;
            return deck;
        }

        return Result.Fail<Deck>(new Error($"Deck with ID {id} not found."));
    }
    
    public async Task<Result<bool>> DeleteDeckAsync(string id)
    {
        return _decks.Remove(id) ? true : Result.Fail<bool>(new Error($"Deck with ID {id} not found."));
    }
}

public interface IDeckRepository
{
    Task<Result<IEnumerable<Deck>>> GetDecksAsync(CancellationToken cancellationToken);
    Task<Result<Deck>> GetDeckByIdAsync(string id);
    Task<Result<Deck>> CreateDeckAsync(Deck deck);
    Task<Result<Deck>> UpdateDeckAsync(string id, Deck deck);
    Task<Result<bool>> DeleteDeckAsync(string id);
}