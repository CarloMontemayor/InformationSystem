using InformationSystem.Data.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using InformationSystem.Data.Enum;

namespace InformationSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<WebAppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<InformationSystem.Data.Entity.Municipality> Municipality { get; set; }
        public DbSet<InformationSystem.Data.Entity.MunicipalityOfficials> MunicipalityOfficials { get; set; }
        public DbSet<InformationSystem.Data.Entity.BarangayOfficial> BarangayOfficial { get; set; }
        public DbSet<InformationSystem.Data.Entity.Barangay> Barangay { get; set; }
        public DbSet<InformationSystem.Data.Entity.Crime> Crime { get; set; }
        public DbSet<InformationSystem.Data.Entity.Disease> Disease { get; set; }
        public DbSet<InformationSystem.Data.Entity.HealthCases> HealthCases { get; set; }
        public DbSet<InformationSystem.Data.Entity.CrimeCases> CrimeCases { get; set; }
        public DbSet<InformationSystem.Data.Entity.Events> Events { get; set; }
        public DbSet<InformationSystem.Data.Entity.Reports> Reports { get; set; }
        public DbSet<InformationSystem.Data.Entity.Complaint> Complaint { get; set; }
        public DbSet<InformationSystem.Data.Entity.FeedBack> FeedBacks { get; set; }
        public DbSet<InformationSystem.Data.Entity.AccidentProne> AccidentProne { get; set; }
        public DbSet<InformationSystem.Data.Entity.Notification> Notification { get; set; }
        public DbSet<InformationSystem.Data.Entity.AuditLog> AuditLogs { get; set; }
        public DbSet<InformationSystem.Data.Entity.ResidentList> ResidentLists { get; set; }
    }
}
