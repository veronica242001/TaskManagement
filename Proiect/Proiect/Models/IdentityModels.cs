using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Task_Management.Models;

namespace Proiect.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public IEnumerable<SelectListItem> AllRoles { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext,
           Proiect.Migrations.Configuration>("DefaultConnection"));
        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task_Management.Models.Task> Tasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Member> Members { get; set; }
        
        /*
                protected override void OnModelCreating(DbModelBuilder modelBuilder)
                {

                    modelBuilder.Entity<Task_Management.Models.Task>().HasMany(t => t.Members).WithRequired(a => a.Tasks).WillCascadeOnDelete(false); 

                } */
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Entity<Task_Management.Models.Task>()

          .HasOptional(t => t.Member)
          .WithMany(t => t.Tasks)
         .HasForeignKey(d => d.MemberId) // new { b.TeamId }
         .WillCascadeOnDelete(false);
         //.OnDelete(DeleteBehavior.SetNull);




        }
            





        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}