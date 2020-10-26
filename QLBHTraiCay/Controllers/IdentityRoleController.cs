using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using QLBHTraiCay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QLBHTraiCay.Controllers
{
    public class IdentityRoleController : Controller
    {
        // GET: IdentityRole
        public ActionResult Index()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            roleManager.Create(new IdentityRole("QuanLy"));
            roleManager.Create(new IdentityRole("NhanVien"));

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var userAdmin = userManager.FindByName("admin@gmail.com");
            userManager.AddToRole(userAdmin.Id, "QuanLy");
            userManager.AddToRole(userAdmin.Id, "NhanVien");

            var userHanh = userManager.FindByName("hanh@gmail.com");
            string userId = userHanh.Id;
            string roleName = "QuanLy";
            userManager.AddToRole(userId, roleName);

            var userHung = userManager.FindByName("binh@gmail.com");
            userManager.AddToRole(userHung.Id, "QuanLy");

            var userTam = userManager.FindByName("tam@gmail.com");
            userManager.AddToRole(userTam.Id, "NhanVien");
            return View();
        }
    }
}