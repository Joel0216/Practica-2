using PaliBot.Domain.Entities;
using PaliBot.Domain.Models;
using System.Text.RegularExpressions;
using System.Text;

namespace PaliBot.Services.Features.Palindromes;

public class PalindromeService
{
    private readonly List<Palindrome> _palindromes;
    private int _nextId = 1;

    public PalindromeService()
    {
        _palindromes = new List<Palindrome>();
        
        // Algunos ejemplos pre-cargados
        AddSamplePalindromes();
    }

    private void AddSamplePalindromes()
    {
        var samples = new[]
        {
            "radar", "ana", "somos", "reconocer", "anita lava la tina", 
            "a man a plan a canal panama", "madam", "racecar"
        };

        foreach (var sample in samples)
        {
            var cleanText = CleanText(sample, true, true, true);
            if (IsPalindromeCheck(cleanText))
            {
                _palindromes.Add(new Palindrome
                {
                    Id = _nextId++,
                    Text = sample,
                    CleanText = cleanText,
                    Length = cleanText.Length,
                    Category = DetermineCategory(sample),
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
    }

    public PalindromeCheckResponse CheckPalindrome(PalindromeCheckRequest request)
    {
        var cleanText = CleanText(request.Text, request.IgnoreSpaces, 
                                 request.IgnoreCase, request.IgnorePunctuation);
        
        var isPalindrome = IsPalindromeCheck(cleanText);
        var reversedText = new string(cleanText.Reverse().ToArray());

        return new PalindromeCheckResponse
        {
            OriginalText = request.Text,
            ProcessedText = cleanText,
            IsPalindrome = isPalindrome,
            ReversedText = reversedText,
            Length = cleanText.Length,
            Category = DetermineCategory(request.Text),
            Message = isPalindrome 
                ? $"¡'{request.Text}' ES un palíndromo! 🎉" 
                : $"'{request.Text}' NO es un palíndromo. 😔"
        };
    }

    private string CleanText(string text, bool ignoreSpaces, bool ignoreCase, bool ignorePunctuation)
    {
        var result = text;

        if (ignoreCase)
            result = result.ToLowerInvariant();

        if (ignoreSpaces)
            result = result.Replace(" ", "");

        if (ignorePunctuation)
            result = Regex.Replace(result, @"[^\w\s]", "");

        // Remover acentos
        result = RemoveAccents(result);

        return result;
    }

    private string RemoveAccents(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = char.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    private bool IsPalindromeCheck(string text)
    {
        if (string.IsNullOrEmpty(text)) return false;
        
        var left = 0;
        var right = text.Length - 1;

        while (left < right)
        {
            if (text[left] != text[right])
                return false;
            
            left++;
            right--;
        }

        return true;
    }

    private string DetermineCategory(string text)
    {
        if (text.Contains(' '))
            return "Frase";
        
        if (text.All(char.IsDigit))
            return "Número";
        
        return "Palabra";
    }

    // Operaciones CRUD para palíndromos guardados
    public IEnumerable<Palindrome> GetAll()
    {
        return _palindromes.OrderByDescending(p => p.CreatedAt);
    }

    public Palindrome? GetById(int id)
    {
        return _palindromes.FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<Palindrome> GetByCategory(string category)
    {
        return _palindromes.Where(p => 
            string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase));
    }

    public Palindrome? AddPalindrome(string text)
    {
        var cleanText = CleanText(text, true, true, true);
        
        if (!IsPalindromeCheck(cleanText))
            return null; // No es un palíndromo

        // Verificar si ya existe
        if (_palindromes.Any(p => string.Equals(p.CleanText, cleanText, StringComparison.OrdinalIgnoreCase)))
            return null; // Ya existe

        var palindrome = new Palindrome
        {
            Id = _nextId++,
            Text = text,
            CleanText = cleanText,
            Length = cleanText.Length,
            Category = DetermineCategory(text),
            CreatedAt = DateTime.UtcNow
        };

        _palindromes.Add(palindrome);
        return palindrome;
    }

    public bool DeletePalindrome(int id)
    {
        var palindrome = _palindromes.FirstOrDefault(p => p.Id == id);
        if (palindrome != null)
        {
            _palindromes.Remove(palindrome);
            return true;
        }
        return false;
    }

    public Dictionary<string, int> GetStatistics()
    {
        return new Dictionary<string, int>
        {
            ["TotalPalindromes"] = _palindromes.Count,
            ["Words"] = _palindromes.Count(p => p.Category == "Palabra"),
            ["Phrases"] = _palindromes.Count(p => p.Category == "Frase"),
            ["Numbers"] = _palindromes.Count(p => p.Category == "Número"),
            ["AverageLength"] = _palindromes.Any() ? (int)_palindromes.Average(p => p.Length) : 0
        };
    }
}