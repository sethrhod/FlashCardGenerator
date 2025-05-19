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
    
    public async Task<Result<IEnumerable<Deck>>> GetDecksAsync(CancellationToken cancellationToken)
    {
        var decks = await _deckRepository.GetDecksAsync(cancellationToken);
        if (decks.IsFailed)
        {
            return Result.Fail(decks.Errors);
        }

        return decks;
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
    
    public async Task<Result<Deck>> CreateDeckAsync(GenerationRequestDTO requestDto, CancellationToken cancellationToken)
    {
        var cards = await _generatorService.GenerateFlashCards(requestDto, cancellationToken);
        if (cards.IsFailed)
        {
            return Result.Fail(cards.Errors);
        }
        
        var deck = new Deck
        {
            Id = Guid.NewGuid().ToString(),
            Name = string.Empty,
            UserId = string.Empty,
            OriginalLanguage = Language.Parse(requestDto.OriginalLanguage),
            TargetLanguage = Language.Parse(requestDto.TargetLanguage),
            Level = Enum.Parse<LanguageLevel>(requestDto.Level),
            FlashCards = cards.Value
        };
        
        var createdDeck = await _deckRepository.CreateDeckAsync(deck);

        return createdDeck;
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
    Task<Result<IEnumerable<Deck>>> GetDecksAsync (CancellationToken cancellationToken);
    Task<Result<Deck>> GetDeckByIdAsync(string id, CancellationToken cancellationToken);
    
    Task<Result<Deck>> CreateDeckAsync(GenerationRequestDTO requestDto, CancellationToken cancellationToken);
    Task<Result<Deck>> UpdateDeckAsync(string id, Deck deck, CancellationToken cancellationToken);
    Task<Result<bool>> DeleteDeckAsync(string id, CancellationToken cancellationToken);
}