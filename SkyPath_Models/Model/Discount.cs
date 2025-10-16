using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models.Model
{
    public class Discount
    {
        string discount_Id;
        string description;
        int percentage;
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
        public int Percentage
        {
            get { return percentage; }
            set { percentage = value; }
        }
        [Required(ErrorMessage = "Valid from is required")]
        public string Valid_From
        {
            get { return valid_From; }
            set { valid_From = value; }
        }
        [Required(ErrorMessage = "Valid to is required")]
        public string Valid_To
        {
            get { return valid_To; }
            set { valid_To = value; }
        }
    }
}
