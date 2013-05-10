using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Belina.Models;

namespace Belina.Controllers
{
    public class HomeController : Controller
    {
        BelinaEntities2 db = new BelinaEntities2();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        public JsonResult getClasses()
        {
            IList<Class> classes = (from x in db.Class where x.class_name != "Недефинирано" && x.class_name != "Разно" select x).ToList();
            return Json(classes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getCompanies(string productName)
        {
            IList<String> companies = (from x in db.Company from y in db.Class from xy in db.Company_Class where x.company_id == xy.company_id && xy.class_id == y.class_id && y.class_name == productName orderby x.company_name select x.company_name).ToList();
            return Json(companies, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getTypes(string className,string companyName)
        {
            IList<String> allTypes = (from x in db.Company
                                      from y in db.Type_Company
                                      from z in db.Type
                                      where x.company_id == y.company_id && y.type_id == z.type_id
                                      && x.company_name == companyName && z.class_name == className
                                      orderby z.type_name
                                      select z.type_name).ToList();
            return Json(allTypes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getProducts(string className, string companyName, string typeName)
        {
            IList<String> productName = (from company in db.Company
                                      from classes in db.Class
                                      from types in db.Type
                                      from product in db.Products
                                         where company.company_id == product.company_id && types.type_id == product.type_id
                                      && classes.class_id == product.class_id && company.company_name == companyName
                                      && classes.class_name == className && types.type_name == typeName
                                      orderby product.product_name
                                      select product.product_name).ToList();
            return Json(productName, JsonRequestBehavior.AllowGet);
        }
    }
}
//Select Products.product_name from Products,[Type],Company,Class where

//Company.company_id=Products.company_id and 
//Class.class_id=Products.class_id and
//[Type].type_id=Products.type_id and

//Company.company_name=N'Weber Saint Gobain' and Class.class_name=N'Градежништво' and [Type].type_name=N'Лепаци';