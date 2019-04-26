using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace WebApplication1.Models
{
    [DataContract]
    public class WeatherInfo
    {
        [DataMember(Name = "location")]
        public string Location { get; set; }
        [DataMember(Name = "degree")]
        public float Degree { get; set; }
        [DataMember(Name = "dateTime")]
        public DateTime DateTime { get; set; }
       [DataMember(Name = "responsetime")]
        public int ResponseTime { get; set; }
        [DataMember(Name = "availability")]
        public int Availability { get; set; }
    }
}