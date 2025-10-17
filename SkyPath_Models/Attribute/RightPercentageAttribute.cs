using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models.Model
{
    public class RightPercentageAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            string value_string = value.ToString();
            int num = int.Parse(value_string);
            if (num > 0 && num < 101)
                return true;
            return false;
        }
    }
}
