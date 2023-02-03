using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Dto
{
    public class EventsMapDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
    }
}
