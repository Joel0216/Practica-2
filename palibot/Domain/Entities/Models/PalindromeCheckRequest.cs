namespace PaliBot.Domain.Models;

public class PalindromeCheckRequest
{
    public required string Text { get; set; }
    public bool IgnoreSpaces { get; set; } = true;
    public bool IgnoreCase { get; set; } = true;
    public bool IgnorePunctuation { get; set; } = true;
}