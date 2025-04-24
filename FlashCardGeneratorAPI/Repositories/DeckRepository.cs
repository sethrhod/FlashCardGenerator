using FlashCardGeneratorAPI.Models;
using FluentResults;

namespace FlashCardGeneratorAPI.Repositories;

public class DeckRepository : IDeckRepository
{
    private readonly Dictionary<string, Deck> _decks = new();
    
    public async Task<Result<Deck>> GetDeckByIdAsync(string id)
    {
        if (_decks.TryGetValue(id, out var deck))
        {
            return deck;
        }

        return Result.Fail<Deck>(new Error($"Deck with ID {id} not found."));
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
    Task<Result<Deck>> GetDeckByIdAsync(string id);
    Task<Result<Deck>> UpdateDeckAsync(string id, Deck deck);
    Task<Result<bool>> DeleteDeckAsync(string id);
}