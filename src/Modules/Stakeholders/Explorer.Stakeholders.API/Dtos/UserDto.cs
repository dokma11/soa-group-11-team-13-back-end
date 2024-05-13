using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public long PersonId { get; set; }
    }
}
