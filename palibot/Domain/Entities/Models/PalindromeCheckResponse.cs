namespace PaliBot.Domain.Models;

public class PalindromeCheckResponse
{
    public string OriginalText { get; set; } = string.Empty;
    public string ProcessedText { get; set; } = string.Empty;
    public bool IsPalindrome { get; set; }
    public string ReversedText { get; set; } = string.Empty;
    public int Length { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}