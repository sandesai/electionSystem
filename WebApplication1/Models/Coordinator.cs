using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApplication1.Models
{
    public class Coordinator
    {
       
        [DataMember(Name = "winner")]
        public int Winner { get; set; }
       
        [DataMember(Name = "responseMSG")]
        public string ResponseMsg { get; set; }
    }
}