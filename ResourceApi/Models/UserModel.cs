using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResourceApi.Models
{
    public class UserModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Preferred_Username { get; set; }
        public string Given_Name { get; set; }
        public string Family_Name { get; set; }
    }
}
