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
    public class AccidentMapController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccidentMapController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var eventMaps = await _context.AccidentProne.Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId).ToListAsync();
            ViewBag.IsAdmin = false;
            ViewBag.IsBarangayOfficial = false;
            if (user.IsAdmin)
            {
                ViewBag.IsAdmin = true;
                eventMaps = await _context.AccidentProne.Include(x => x.User).ToListAsync();
            }
            else if (user.IsBarangayOfficial)
                ViewBag.IsBarangayOfficial = true;

            var listDto = new List<EventsMapDto>();

            foreach (var eventMap in eventMaps)
            {
                var dto = new EventsMapDto()
                {
                    Title = eventMap.Name
                };
                listDto.Add(dto);
            }
            return View(listDto);
        }
        public async Task<List<EventsMapDto>> GetEventsMap()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var listDto = new List<EventsMapDto>();
            var eventMaps = await _context.AccidentProne.Include(x => x.User).Where(x => x.User.BarangayId == user.BarangayId).ToListAsync();
            if(user.IsAdmin)
                eventMaps = await _context.AccidentProne.Include(x => x.User).ToListAsync();
            foreach (var eventMap in eventMaps)
            {
                var dto = new EventsMapDto()
                {
                    Id = eventMap.AccidentProneId,
                    Title = eventMap.Name,
                    Lat = eventMap.Latitude,
                    Long = eventMap.Longitude
                };
                listDto.Add(dto);
            }
            return listDto;
        }
    }
}
