using System;
using System.Collections.Generic;
using System.Text;

namespace Mall.Model.Entity
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string[] Roles { get; set; }
        public string Password { get; set; }
    }
}
