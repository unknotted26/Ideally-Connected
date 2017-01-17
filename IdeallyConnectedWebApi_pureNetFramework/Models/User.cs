﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IdeallyConnected.Models
{
    public class User
    {
        public int UserID { get; set; }
        
        [Required()]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        public virtual List<Skill> Skills { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        //[ConcurrencyCheck()]
        
    }
}