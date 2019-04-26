using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApplication1.Models
{
    public class Participant3
    {
        [DataMember(Name = "winner")]
        public int Winner { get; set; }
        [DataMember(Name = "noOfParticipant")]
        public int NoOfParticipant { get; set; }
        [DataMember(Name = "responsetime")]
        public int ResponseTime { get; set; }
        [DataMember(Name = "availability")]
        public int Availability { get; set; }
        [DataMember(Name = "electionNo")]
        public int ElectionNo { get; set; }

    }
}