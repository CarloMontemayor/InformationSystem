using InformationSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InformationSystem.Controllers
{
    public class AdminViewController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminViewController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public int GetTeenAgerData()
        {
            return _context.Users.Where(x => !x.IsAdmin).Count(x => x.Age >= 13 && x.Age <= 19);
        }
        public int GetAdultData()
        {
            return _context.Users.Where(x => !x.IsAdmin).Count(x => x.Age >= 20 && x.Age <= 60);
        }
        public int GetSeniorData()
        {
            return _context.Users.Where(x => !x.IsAdmin).Count(x => x.Age >= 61);
        }

        public List<string> GetDisease()
        {
            var result = new List<string>();
            var diseases = _context.Disease.OrderBy(x => x.DiseaseName).ToList();
            foreach(var disease in diseases)
            {
                result.Add(disease.DiseaseName);
            }
            return result;
        }

        public List<int> GetDiseaseData()
        {
            var result = new List<int>();
            var diseases = _context.Disease.OrderBy(x => x.DiseaseName).ToList();
            foreach (var disease in diseases)
            {
                var diseaseCases = _context.HealthCases.Where(x => x.DiseaseId == disease.DiseaseId).Count();
                result.Add(diseaseCases);
            }
            return result;
        }

        public List<string> GetDiseaseBarangay()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var result = new List<string>();
            var diseases = _context.Disease.Include(x => x.User).OrderBy(x => x.DiseaseName).Where(x => x.User.BarangayId == user.BarangayId).ToList();
            foreach (var disease in diseases)
            {
                result.Add(disease.DiseaseName);
            }
            return result;
        }

        public List<int> GetDiseaseDataBarangay()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var result = new List<int>();
            var diseases = _context.Disease.OrderBy(x => x.DiseaseName).ToList();
            foreach (var disease in diseases)
            {
                var diseaseCases = _context.HealthCases.Include(x => x.User).Where(x => x.DiseaseId == disease.DiseaseId && x.User.BarangayId == user.BarangayId).Count();
                result.Add(diseaseCases);
            }
            return result;
        }

        public List<string> GetCrime()
        {
            var result = new List<string>();
            var crimes = _context.Crime.OrderBy(x => x.CrimeName).ToList();
            foreach (var crime in crimes)
            {
                result.Add(crime.CrimeName);
            }
            return result;
        }

        public List<int> GetCrimeData()
        {
            var result = new List<int>();
            var crimes = _context.Crime.OrderBy(x => x.CrimeName).ToList();
            foreach (var crime in crimes)
            {
                var crimesCases = _context.CrimeCases.Where(x => x.CrimeId == crime.CrimeId).Count();
                result.Add(crimesCases);
            }
            return result;
        }

        public List<string> GetCrimeBarangay()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var result = new List<string>();
            var crimes = _context.Crime.Include(x => x.User).OrderBy(x => x.CrimeName).Where(x => x.User.BarangayId == user.BarangayId).ToList();
            foreach (var crime in crimes)
            {
                result.Add(crime.CrimeName);
            }
            return result;
        }

        public List<int> GetCrimeDataBarangay()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var result = new List<int>();
            var crimes = _context.Crime.OrderBy(x => x.CrimeName).ToList();
            foreach (var crime in crimes)
            {
                var crimesCases = _context.CrimeCases.Include(x => x.User).Where(x => x.CrimeId == crime.CrimeId && x.User.BarangayId == user.BarangayId).Count();
                result.Add(crimesCases);
            }
            return result;
        }

        public int GetTeenAgerDataBarangay()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            return _context.Users.Where(x => !x.IsAdmin && x.BarangayId == user.BarangayId).Count(x => x.Age >= 13 && x.Age <= 19);
        }
        public int GetAdultDataBarangay()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            return _context.Users.Where(x => !x.IsAdmin && x.BarangayId == user.BarangayId).Count(x => x.Age >= 20 && x.Age <= 60);
        }
        public int GetSeniorDataBarangay()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            return _context.Users.Where(x => !x.IsAdmin && x.BarangayId == user.BarangayId).Count(x => x.Age >= 61);
        }
    }
}
