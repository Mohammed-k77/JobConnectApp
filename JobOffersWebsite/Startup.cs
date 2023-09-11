using JobOffersWebsite.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JobOffersWebsite.Startup))]
namespace JobOffersWebsite
{
    public partial class Startup
    {
        private ApplicationDbContext Db = new ApplicationDbContext();
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateDefaultRolesAndUsers();
        }

        public void CreateDefaultRolesAndUsers()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(Db));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Db));
            IdentityRole role = new IdentityRole();
            if (!roleManager.RoleExists("Admins"))
            {
                role.Name = "Admins";
                roleManager.Create(role);
                ApplicationUser user = new ApplicationUser();
                user.UserName = "Mohammed";
                user.Email = "Occ716528@gmail.com";

                var check = userManager.Create(user, "Abc+770485277");
                if (check.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admins");
                }
            }

        }
    }
}
