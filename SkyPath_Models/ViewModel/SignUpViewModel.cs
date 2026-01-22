using SkyPath_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyPath_Models.ViewModel
{
    public class SignUpViewModel
    {
        public User user { get; set; }
        public List<City> cities { get; set; }
    }
}
