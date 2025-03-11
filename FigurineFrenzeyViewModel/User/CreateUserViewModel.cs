﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigurineFrenzeyViewModel.User
{
    public class CreateUserViewModel
    {
        public string FullName {  get; set; }
        public string Address {  get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }

    }
}
