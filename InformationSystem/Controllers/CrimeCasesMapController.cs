using InformationSystem.Data;
using InformationSystem.Data.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InformationSystem.Controllers
{
    public class CrimeCasesMapController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CrimeCasesMapController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            ViewBag.IsAdmin = false;
            ViewBag.IsBarangayOfficial = false;
            if (user.IsAdmin)
                ViewBag.IsAdmin = true;
            else if (user.IsBarangayOfficial)
                ViewBag.IsBarangayOfficial = true;

            var listDto = new List<CrimeCasesMapDto>();
            var crimeCases = await _context.CrimeCases.Include(x => x.User).Include(x => x.Crime).Where(x => x.User.BarangayId == user.BarangayId).ToListAsync();

            if (user.IsAdmin)
            {
                crimeCases = await _context.CrimeCases.Include(x => x.User).Include(x => x.Crime).ToListAsync();
            }

            var Crimes = crimeCases.GroupBy(x => x.CrimeId).ToList();
            foreach (var crime in Crimes)
            {
                var dto = new CrimeCasesMapDto()
                {
                    Id = crime.Key,
                    Title = crime.Select(x => x.Crime.CrimeName).FirstOrDefault(),
                    Lat = crime.Select(x => x.Latitude).FirstOrDefault(),
                    Long = crime.Select(x => x.Longitude).FirstOrDefault(),
                    Description = crime.Count().ToString()
                };
                listDto.Add(dto);
            }
            return View(listDto);
        }
        public async Task<List<CrimeCasesMapDto>> GetCrimeCasesMap()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var listDto = new List<CrimeCasesMapDto>();
            var crimeCases = await _context.CrimeCases.Include(x => x.User).Include(x => x.Crime).Where(x => x.Status == Data.Enum.ReportStatus.Approve && x.User.BarangayId == user.BarangayId).ToListAsync();

            if (user.IsAdmin)
            {
                crimeCases = await _context.CrimeCases.Include(x => x.User).Include(x => x.Crime).ToListAsync();
            }


            var Crimes = crimeCases.ToList();
            foreach (var crime in Crimes)
            { 
                var dto = new CrimeCasesMapDto()
                {
                    Id = crime.CrimeCasesId,
                    Title = crime.Crime.CrimeName,
                    Lat = crime.Latitude,
                    Long = crime.Longitude,
                };
                listDto.Add(dto);
            }
            return listDto;
        }
    }
}
