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
    public class HealthCasesMapController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HealthCasesMapController(ApplicationDbContext context)
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

            var listDto = new List<HealthCasesMapDto>();
            var healths = await _context.HealthCases.Include(x => x.User).Include(x => x.Disease).Where(x => x.User.BarangayId == user.BarangayId).ToListAsync();

            if (user.IsAdmin)
            {
                healths = await _context.HealthCases.Include(x => x.User).Include(x => x.Disease).ToListAsync();
            }

            var healthCases = healths.GroupBy(x => x.DiseaseId).ToList();
            foreach (var healthCase in healthCases)
            {
                var dto = new HealthCasesMapDto()
                {
                    Title = healthCase.Select(x => x.Disease.DiseaseName).FirstOrDefault(),
                    Description = healthCase.Count().ToString()
                };
                listDto.Add(dto);
            }
            return View(listDto);
        }
        public async Task<List<HealthCasesMapDto>> GetHealthCasesMap()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var listDto = new List<HealthCasesMapDto>();
            var healths = await _context.HealthCases.Include(x => x.User).Include(x => x.Disease).Where(x => x.Status == Data.Enum.ReportStatus.Approve && x.User.BarangayId == user.BarangayId).ToListAsync();

            if (user.IsAdmin)
            {
                healths = await _context.HealthCases.Include(x => x.User).Include(x => x.Disease).ToListAsync();
            }

            var healthCases = healths.ToList();
            foreach (var healthCase in healthCases)
            { 
                var dto = new HealthCasesMapDto()
                {
                    Id = healthCase.DiseaseId,
                    Title = healthCase.Disease.DiseaseName,
                    Lat = healthCase.Latitude,
                    Long = healthCase.Longitude,
                };
                listDto.Add(dto);
            }
            return listDto;
        }
    }
}
