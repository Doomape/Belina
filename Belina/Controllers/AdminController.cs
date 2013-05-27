using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using Belina.Models;

namespace Belina.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
        BelinaEntities2 db = new BelinaEntities2();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Add()
        {
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }

        public JsonResult insertClasses(string className)
        {
            IList<String> existClass = (from x in db.Class where x.class_name == className select x.class_name).Distinct().ToList();

            if (existClass.Count > 0)
            {
                return Json("Класата веќе постои..среќа што сме бесконечно пописмени од инфопроект па не дозволуваме вакви глупости.", JsonRequestBehavior.AllowGet);
            }
            else
            {
                Class classObj = new Class();
                classObj.class_id = (from x in db.Class select x.class_id).Max() + 1;
                classObj.class_name = className;
                db.Class.Add(classObj);
                db.SaveChanges();
                return Json("Класа е внесена.", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult insertType(string typeName)
        {
            IList<String> existClass = (from x in db.Type where x.type_name == typeName select x.type_name).Distinct().ToList();

            if (existClass.Count > 0)
            {
                return Json("Типот веќе постои..среќа што сме бесконечно пописмени од инфопроект па не дозволуваме вакви глупости.", JsonRequestBehavior.AllowGet);
            }
            else
            {
                Belina.Models.Type typeObj = new Belina.Models.Type();
               
                typeObj.type_id = (from x in db.Type select x.type_id).Max() + 1;
                typeObj.type_name = typeName;
                db.Type.Add(typeObj);
                db.SaveChanges();
                return Json("Класа е внесена.", JsonRequestBehavior.AllowGet);
            }
        }

    }
}
