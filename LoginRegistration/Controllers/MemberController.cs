using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LoginRegistration.Controllers
{
    public class MemberController : Controller
    {
        // GET: Member
        [Authorize]
        public ActionResult HomeMember()
        {
            return View();
        }
    }
}