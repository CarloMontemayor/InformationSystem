using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InformationSystem.Data.Dto
{
    public class HealthCasesMapDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public string Description { get; set; }
    }
}
