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

        public JsonResult getTypes(string className)
        {
            IList<String> allTypes = (from type in db.Type
                                      where type.class_name == className
                                      orderby type.type_name
                                      select type.type_name).ToList();
            return Json(allTypes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getCompanies(string className, string typeName)
        {
            IList<String> companies = (from company in db.Company
                                       from type in db.Type
                                       from type_company in db.Type_Company
                                       where company.company_id == type_company.company_id && type_company.type_id == type.type_id && type.class_name == className
                                       && type.type_name == typeName
                                       orderby company.company_name
                                       select company.company_name).ToList();
            return Json(companies, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getProducts(string className, string companyName, string typeName, string attributeName)
        {
            if (attributeName.Equals("false") || attributeName.Equals("Сите"))
            {
                var products = (from company in db.Company
                                from classes in db.Class
                                from types in db.Type
                                from product in db.Products
                                from a in db.Attributes
                                where

                                classes.class_id == product.class_id &&
                                company.company_id == product.company_id &&
                                types.type_id == product.type_id &&
                                a.type_id == types.type_id &&
                                a.company_id == company.company_id &&
                                a.attribute_id == product.attribute_id &&

                                company.company_name == companyName &&
                                classes.class_name == className &&
                                types.type_name == typeName
                                orderby a.attribute_name
                                select new
                                {
                                    product.product_name,
                                    a.attribute_name
                                }).ToList();

                var attributes = products.Select(p => p.attribute_name).Distinct().ToList();
                Dictionary<string, List<String>> res = new Dictionary<string, List<string>>();
                List<String> productList = new List<string>();
                foreach (var product in products)
                {
                    productList.Add(product.product_name);
                }
                res.Add("products", productList);
                res.Add("attributes", attributes);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            if (!(attributeName.Equals("false")))
            {
                var productsAttribute = (from company in db.Company
                                                  from classes in db.Class
                                                  from types in db.Type
                                                  from product in db.Products
                                                  from a in db.Attributes
                                                  where

                                                  classes.class_id == product.class_id &&
                                                  company.company_id == product.company_id &&
                                                  types.type_id == product.type_id &&
                                                  a.type_id == types.type_id &&
                                                  a.company_id == company.company_id &&
                                                  a.attribute_id == product.attribute_id &&

                                                  company.company_name == companyName &&
                                                  classes.class_name == className &&
                                                  types.type_name == typeName &&
                                                  a.attribute_name == attributeName
                                                  orderby product.product_name
                                                  select  product.product_name
                                                  ).ToList();
                Dictionary<string, List<String>> res = new Dictionary<string, List<string>>();
                res.Add("products", productsAttribute);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else return Json("Error!", JsonRequestBehavior.AllowGet);
            
        }
    }
}
//select p.product_name,a.attribute_name

//from Class c,Company com,Type t,Products p,Attributes a 

//where

//c.class_id=p.class_id  and
//com.company_id=p.company_id and 
//t.type_id=p.type_id and 
//a.company_id=com.company_id and
//a.type_id=t.type_id and
//p.attribute_id=a.attribute_id and

//com.company_name=N'Јуб' and 
//c.class_name=N'Бои и лакови' and 
//t.type_name=N'Внатрешни ѕидови' 

//group by p.product_name,a.attribute_name order by a.attribute_name


