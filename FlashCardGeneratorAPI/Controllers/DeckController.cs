using FlashCardGeneratorAPI.Models;
using FlashCardGeneratorAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

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

    [HttpGet("GetDeckById/{id}")]
    [ProducesResponseType(typeof(Deck), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
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
    [ProducesResponseType(typeof(Deck), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> CreateDeck(GenerationRequest request, CancellationToken cancellationToken)
    {
        var result = await _deckService.CreateDeckAsync(request, cancellationToken);
        if (result.IsFailed)
        {
            return BadRequest(result.Errors);
        }
        
        return CreatedAtAction(nameof(CreateDeck), new { id = result.Value.Id }, result.Value);
    }
    
    [HttpPut("UpdateDeck/{id}")]
    [ProducesResponseType(typeof(Deck), 201)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> UpdateDeck([FromRoute] string id, [FromBody] Deck deck, CancellationToken cancellationToken)
    {
        var result = await _deckService.UpdateDeckAsync(id, deck, cancellationToken);
        if (result.IsFailed)
        {
            return NotFound(result.Errors);
        }

        return CreatedAtAction(nameof(UpdateDeck), new { id = result.Value.Id }, result.Value);
    }
    
    [HttpDelete("DeleteDeck")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
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