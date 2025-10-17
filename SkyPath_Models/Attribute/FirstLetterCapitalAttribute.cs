using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models.Model
{
    public class FirstLetterCapitalAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            string word = value.ToString();
            char FirstLetter = word[0];
            if (FirstLetter < 'A' || FirstLetter > 'Z')
                return false;
            for (int i = 1; i < word.Length; i++)
            {
                if (word[i] < 'a' || word[i] > 'z')
                    return false;
            }
            return true;
        }
    }
}
