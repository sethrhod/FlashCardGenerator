using System.Net.Mime;
using FlashCardGeneratorAPI.Models;
using FlashCardGeneratorAPI.Services;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using NLanguageTag;

namespace FlashCardGeneratorAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class DeckController : ControllerBase
{
    private readonly ILogger<DeckController> _logger;
    private readonly IDeckService _deckService;

    public DeckController(ILogger<DeckController> logger, IDeckService deckService)
    {
        _deckService = deckService;
        _logger = logger;
    }
    
    [HttpGet("GetAvailableLanguages")]
    [ProducesResponseType<Result<IEnumerable<Language>>>(StatusCodes.Status200OK)]
    public IActionResult GetAvailableLanguages()
    {
        var languages = new List<Language>
        {
            Language.EN,
            Language.FR,
            Language.DE,
            Language.ES,
            Language.IT,
            Language.RU,
            Language.ZH,
            Language.JA,
            Language.KO,
            Language.AR,
            Language.PT,
            Language.NL
        };
        var result = Result.Ok(languages);
        return Ok(result);
    }
    
    [HttpGet("GetDecks")]
    [ProducesResponseType<Result<IEnumerable<Deck>>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTestDecks(CancellationToken cancellationToken)
    {
        var result = await _deckService.GetDecksAsync(cancellationToken);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result);
    }

    [HttpGet("GetDeckById/{id}")]
    [ProducesResponseType<Result<Deck>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDeckById([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await _deckService.GetDeckByIdAsync(id, cancellationToken);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return Ok(result);
    }

    [HttpPost("CreateDeck")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDeck(GenerationRequestDTO requestDto, CancellationToken cancellationToken)
    {
        var result = await _deckService.CreateDeckAsync(requestDto, cancellationToken);
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }
        
        return CreatedAtAction(nameof(GetDeckById), new { id = result.Value.Id }, result.Value);
    }
    
    [HttpPut("UpdateDeck/{id}")]
    [ProducesResponseType<Result<Deck>>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDeck([FromRoute] string id, [FromBody] Deck deck, CancellationToken cancellationToken)
    {
        var result = await _deckService.UpdateDeckAsync(id, deck, cancellationToken);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return CreatedAtAction(nameof(GetDeckById), new { id = result.Value.Id }, result.Value);
    }
    
    [HttpDelete("DeleteDeck")]
    [ProducesResponseType<Result<bool>>(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDeck([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await _deckService.DeleteDeckAsync(id, cancellationToken);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return NoContent();
    }
}