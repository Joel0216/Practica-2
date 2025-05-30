using PaliBot.Domain.Models;
using PaliBot.Services.Features.Palindromes;
using Microsoft.AspNetCore.Mvc;

namespace PaliBot.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class PalindromeController : ControllerBase
{
    private readonly PalindromeService _palindromeService;

    public PalindromeController(PalindromeService palindromeService)
    {
        _palindromeService = palindromeService;
    }

    /// <summary>
    /// Verifica si un texto es un palíndromo
    /// </summary>
    [HttpPost("check")]
    public IActionResult CheckPalindrome([FromBody] PalindromeCheckRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { Message = "El texto no puede estar vacío." });

        var result = _palindromeService.CheckPalindrome(request);
        return Ok(result);
    }

    /// <summary>
    /// Verifica rápidamente si un texto es palíndromo (método simple)
    /// </summary>
    [HttpGet("check/{text}")]
    public IActionResult QuickCheck([FromRoute] string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return BadRequest(new { Message = "El texto no puede estar vacío." });

        var request = new PalindromeCheckRequest { Text = text };
        var result = _palindromeService.CheckPalindrome(request);
        
        return Ok(new { 
            Text = text, 
            IsPalindrome = result.IsPalindrome, 
            Message = result.Message 
        });
    }

    /// <summary>
    /// Obtiene todos los palíndromos guardados
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        var palindromes = _palindromeService.GetAll();
        return Ok(palindromes);
    }

    /// <summary>
    /// Obtiene un palíndromo por ID
    /// </summary>
    [HttpGet("{id:int}")]
    public IActionResult GetById([FromRoute] int id)
    {
        var palindrome = _palindromeService.GetById(id);
        if (palindrome == null)
            return NotFound(new { Message = $"Palíndromo con ID {id} no encontrado." });

        return Ok(palindrome);
    }

    /// <summary>
    /// Obtiene palíndromos por categoría
    /// </summary>
    [HttpGet("category/{category}")]
    public IActionResult GetByCategory([FromRoute] string category)
    {
        var palindromes = _palindromeService.GetByCategory(category);
        return Ok(palindromes);
    }

    /// <summary>
    /// Guarda un nuevo palíndromo (solo si es válido)
    /// </summary>
    [HttpPost]
    public IActionResult AddPalindrome([FromBody] PalindromeAddRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { Message = "El texto no puede estar vacío." });

        var palindrome = _palindromeService.AddPalindrome(request.Text);
        
        if (palindrome == null)
            return BadRequest(new { Message = "El texto no es un palíndromo válido o ya existe." });

        return CreatedAtAction(nameof(GetById), new { id = palindrome.Id }, palindrome);
    }

    /// <summary>
    /// Elimina un palíndromo guardado
    /// </summary>
    [HttpDelete("{id:int}")]
    public IActionResult DeletePalindrome([FromRoute] int id)
    {
        var success = _palindromeService.DeletePalindrome(id);
        if (!success)
            return NotFound(new { Message = $"Palíndromo con ID {id} no encontrado." });

        return NoContent();
    }

    /// <summary>
    /// Obtiene estadísticas de los palíndromos
    /// </summary>
    [HttpGet("statistics")]
    public IActionResult GetStatistics()
    {
        var stats = _palindromeService.GetStatistics();
        return Ok(stats);
    }
}

public class PalindromeAddRequest
{
    public required string Text { get; set; }
}