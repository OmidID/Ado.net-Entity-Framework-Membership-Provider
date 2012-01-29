using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Sample2.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            var membership = System.Web.Security.Membership.Provider;
            var role = System.Web.Security.Roles.Provider;
            var installer = new OmidID.Web.Security.Installer();

            installer.MembershipProvider = membership;
            installer.RoleProvider = role;

            //Install database:
            ViewBag.DatabaseInstalled = installer.CreateIfNotExist();

            Roles.CreateRole("Administrators");
            Roles.CreateRole("Moderators");
            Roles.CreateRole("Resellers");

            var ms = System.Web.Security.MembershipCreateStatus.Success;
            var member = membership.CreateUser("Omid", "12345", "omid@omidid.com", "u?", "Me", true, null, out ms);

            var Tick = Environment.TickCount;
            for (var i = 0; i < 100; i++) {

                var ms2 = System.Web.Security.MembershipCreateStatus.Success;
                var member2 = membership.CreateUser("User_" + i.ToString(), "123345", "omid" + i.ToString() + "@omidid.com", "u?", "Me", true, null, out ms2);

            }
            var time = new TimeSpan(0, 0, 0, 0, Environment.TickCount - Tick);
            ViewBag.Message = string.Format("100 Users added in <font color=\"red\">{0}</font>", time.ToString());

            Tick = Environment.TickCount;
            var allRec = 0;
            var listAll1 = Membership.GetAllUsers();
            var listAll2 = Membership.FindUsersByName("User");
            var listAll3 = Membership.GetAllUsers(2, 50, out allRec);

            time = new TimeSpan(0, 0, 0, 0, Environment.TickCount - Tick);
            ViewBag.Message += string.Format("<br /> 250 Users Readed in <font color=\"red\">{0}</font>", time.ToString());

            Roles.AddUserToRole("Omid", "Administrators");
            Roles.AddUserToRole("User_1", "Moderators");
            Roles.AddUsersToRoles(new string[] { "Omid", "User_1", "User_2", "User_3" }, new string[] { "Administrators", "Moderators" });
            Roles.AddUsersToRoles(new string[] { "User_4", "User_5", "User_6", "User_7" }, new string[] { "Resellers" });
            Roles.AddUsersToRoles(new string[] { "User_6", "User_7" }, new string[] { "Resellers", "Moderators" });

            var all = role.GetAllRoles();
            var allForOmid = role.GetRolesForUser("Omid");
            var allForUser_1 = role.GetRolesForUser("User_1");
            var Moderators = Roles.GetUsersInRole("Moderators");
            var Resellers = Roles.GetUsersInRole("Resellers");
            var Administrators = Roles.GetUsersInRole("Administrators");

            var user = Membership.GetUser("User_1");
            user.Comment = "Hello";
            Membership.UpdateUser(user);

            Membership.DeleteUser("User_101");

            ViewBag.Message += string.Format("<br />All Roles: <font color=\"red\">{0}</font>", string.Join(", ", all));
            ViewBag.Message += string.Format("<br />Omid Roles: <font color=\"red\">{0}</font>", string.Join(", ", allForOmid));
            ViewBag.Message += string.Format("<br />User_1 Roles: <font color=\"red\">{0}</font>", string.Join(", ", allForUser_1));
            ViewBag.Message += string.Format("<br />Administrators: <font color=\"red\">{0}</font>", string.Join(", ", Administrators));
            ViewBag.Message += string.Format("<br />Resellers: <font color=\"red\">{0}</font>", string.Join(", ", Resellers));
            ViewBag.Message += string.Format("<br />Moderators: <font color=\"red\">{0}</font>", string.Join(", ", Moderators));

            ViewBag.Providers = string.Format("Membership Type: <font color=\"red\">{0}</font>", membership.GetType().GetGenericArguments()[0]);
            ViewBag.Providers += string.Format("<br />Role Type: <font color=\"red\">{0}</font>", role.GetType().GetGenericArguments()[0]);

            ViewBag.Providers += string.Format("<br /><br />Membership Table Prefix: <font color=\"red\">{0}</font>", ((dynamic)membership).TablePrefix);
            ViewBag.Providers += string.Format("<br />Role Table Prefix: <font color=\"red\">{0}</font>", ((dynamic)role).TablePrefix);

            return View();
        }

        public ActionResult About() {
            return View();
        }
    }
}
