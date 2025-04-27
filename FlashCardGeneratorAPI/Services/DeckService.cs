using FlashCardGeneratorAPI.Models;
using FlashCardGeneratorAPI.Repositories;
using FluentResults;
using NLanguageTag;

namespace FlashCardGeneratorAPI.Services;

public class DeckService : IDeckService
{
    private readonly IDeckRepository _deckRepository;
    private readonly IGeneratorService _generatorService;
    
    public DeckService(IDeckRepository deckRepository, IGeneratorService generatorService)
    {
        _deckRepository = deckRepository;
        _generatorService = generatorService;
    }

    public async Task<Result<Deck>> GetTestDeckAsync(CancellationToken cancellationToken)
    {
        var cards = await _generatorService.GenerateFlashCards(new GenerationRequest
        {
            OriginalLanguage = Language.EN,
            TargetLanguage = Language.PT,
            Level = LanguageLevel.A1,
            Count = 5,
        }, CancellationToken.None);
        
        if (cards.IsFailed)
        {
            return Result.Fail(cards.Errors);
        }

        var deck = new Deck
        {
            Id = Guid.NewGuid().ToString(),
            Name = string.Empty,
            UserId = string.Empty,
            FlashCards = cards.Value
        };

        return deck;
    }
    
    public async Task<Result<Deck>> GetDeckByIdAsync(string id, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.GetDeckByIdAsync(id);
        if (deck.IsFailed)
        {
            return Result.Fail(deck.Errors);
        }

        return deck.Value;
    }
    
    public async Task<Result<Deck>> CreateDeckAsync(GenerationRequest request, CancellationToken cancellationToken)
    {
        var cards = await _generatorService.GenerateFlashCards(request, cancellationToken);
        if (cards.IsFailed)
        {
            return Result.Fail(cards.Errors);
        }
        
        var deck = new Deck
        {
            Id = Guid.NewGuid().ToString(),
            Name = string.Empty,
            UserId = string.Empty,
            FlashCards = cards.Value
        };

        return deck;
    }
    
    public async Task<Result<Deck>> UpdateDeckAsync(string id, Deck deck, CancellationToken cancellationToken)
    {
        var existingDeck = await _deckRepository.GetDeckByIdAsync(id);
        if (existingDeck.IsFailed)
        {
            return Result.Fail(existingDeck.Errors);
        }
        
        existingDeck.Value.Name = deck.Name;
        existingDeck.Value.FlashCards = deck.FlashCards;

        var updatedDeck = await _deckRepository.UpdateDeckAsync(id, existingDeck.Value);
        return updatedDeck;
    }
    
    public async Task<Result<bool>> DeleteDeckAsync(string id, CancellationToken cancellationToken)
    {
        var delete = await _deckRepository.DeleteDeckAsync(id);
        if (delete.IsFailed)
        {
            return false;
        }

        return true;
    }
}

public interface IDeckService
{
    Task<Result<Deck>> GetTestDeckAsync(CancellationToken cancellationToken);
    Task<Result<Deck>> GetDeckByIdAsync(string id, CancellationToken cancellationToken);
    
    Task<Result<Deck>> CreateDeckAsync(GenerationRequest request, CancellationToken cancellationToken);
    Task<Result<Deck>> UpdateDeckAsync(string id, Deck deck, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteDeckAsync(string id, CancellationToken cancellationToken);
}