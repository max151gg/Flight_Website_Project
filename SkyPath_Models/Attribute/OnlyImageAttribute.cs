using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models.Models
{
    public class OnlyImageAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null) return false;
            string[] allowedExtensions = { ".jpg", ".png", ".jpeg" };
            string fileName = value.ToString();
            string extension = fileName.Substring(fileName.Length - 4).ToLower();
            foreach (string fileExtension in allowedExtensions)
            {
                if (fileExtension == extension)
                    return true;
            }
            return false;
        }
    }
}
