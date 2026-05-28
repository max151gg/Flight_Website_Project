using System.ComponentModel.DataAnnotations;

namespace SkyPath_Models
{
    public class FirstLetterCapitalAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            // null/empty is handled by [Required]
            if (value == null) return true;

            string word = value.ToString();
            if (string.IsNullOrEmpty(word)) return true;

            char firstLetter = word[0];
            return firstLetter >= 'A' && firstLetter <= 'Z';
        }
    }
}