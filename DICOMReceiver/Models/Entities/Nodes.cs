﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOMReceiver.Models.Entities
{
    public class Nodes
    {
        public virtual string AETitle { get; set; }
        public virtual string Host { get; set; }
        public virtual int Port { get; set; }
    }

}
