﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPortalRestServer.Models
{
    public class Source : DataModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string api { get; set; }
    }
}
