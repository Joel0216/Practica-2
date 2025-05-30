namespace PaliBot.Domain.Entities;

public class Palindrome
{
    public int Id { get; set; }
    public required string Text { get; set; }
    public required string CleanText { get; set; } // Texto sin espacios ni caracteres especiales
    public DateTime CreatedAt { get; set; }
    public string? Category { get; set; } // "Palabra", "Frase", "Número"
    public int Length { get; set; }
    
    public Palindrome()
    {
        Text = string.Empty;
        CleanText = string.Empty;
        CreatedAt = DateTime.UtcNow;
    }
}