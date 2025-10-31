using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyPath_Models;

namespace SkyPath_Models.Models
{
    public class Discount : Model
    {
        string discount_Id;
        string description;
        short percentage;
        string valid_From;
        string valid_To;

        public string Discount_Id
        {
            get { return discount_Id; }
            set { discount_Id = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        [Required(ErrorMessage = "Percentage is required")]
        [IsDigits(ErrorMessage = "Must be digits only")]
        [RightPercentage(ErrorMessage = "the percentage must be between 1 and 100 percent")]
        public short Percentage
        {
            get { return percentage; }
            set { percentage = value; ValidateProperty(value, "Percentage"); }
        }
        [Required(ErrorMessage = "Valid from is required")]
        public string Valid_From
        {
            get { return valid_From; }
            set { valid_From = value; ValidateProperty(value, "Valid_From"); }
        }
        [Required(ErrorMessage = "Valid to is required")]
        public string Valid_To
        {
            get { return valid_To; }
            set { valid_To = value; ValidateProperty(value, "Valid_To"); }
        }
    }
}
