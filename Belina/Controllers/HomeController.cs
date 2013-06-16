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
            IList<Class> classes = (from x in db.Class where x.class_name != "Недефинирано" && x.class_name != "Разно" select x).Distinct().ToList();
            return Json(classes, JsonRequestBehavior.AllowGet);

        }

        public JsonResult getTypes(int classID)
        {
            IList<Belina.Models.Type> allTypes = (from products in db.Products
                                      join t in db.Type on products.type_id equals t.type_id
                                      join c in db.Class on products.class_id equals c.class_id
                                      where
                                          c.class_id == classID
                                      select t).Distinct().ToList();
            return Json(allTypes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getCompanies(int classID, int typeID)
        {
            IList<Company> companies = (from products in db.Products
                                       join t in db.Type on products.type_id equals t.type_id
                                       join comp in db.Company on products.company_id equals comp.company_id
                                       join c in db.Class on products.class_id equals c.class_id
                                       where c.class_id == classID && t.type_id == typeID
                                       select comp).Distinct().ToList();
            return Json(companies, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getProducts(int classID, int companyID, int typeID, string attributeName)
        {
            if (attributeName.Equals("false") || attributeName.Equals("Сите"))
            {
                var products = (from product in db.Products
                                join t in db.Type on product.type_id equals t.type_id
                                join comp in db.Company on product.company_id equals comp.company_id
                                join c in db.Class on product.class_id equals c.class_id
                                join a in db.Attributes on product.attribute_id equals a.attribute_id
                                where
                                c.class_id == classID && t.type_id == typeID && comp.company_id == companyID
                                select new
                                {
                                    product.product_name,
                                    product.product_image,
                                    product.product_description,
                                    a.attribute_name,
                                    a.attribute_id
                                }).Distinct().ToList();

                var attributesName = products.Select(p => p.attribute_name).Distinct().ToList();
                var attributesID = products.Select(p => p.attribute_id.ToString()).Distinct().ToList();
                Dictionary<string, List<String>> res = new Dictionary<string, List<string>>();
                List<String> productList = new List<string>();
                foreach (var product in products)
                {
                    productList.Add(product.product_name + "#" + product.product_image + "@" + product.product_description);
                }
                res.Add("products", productList);
                res.Add("attributes", attributesName);
                res.Add("attributesID", attributesID);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            if (!(attributeName.Equals("false")))
            {
                var productsByAttribute = (from products in db.Products
                                           join t in db.Type on products.type_id equals t.type_id
                                           join comp in db.Company on products.company_id equals comp.company_id
                                           join c in db.Class on products.class_id equals c.class_id
                                           join a in db.Attributes on products.attribute_id equals a.attribute_id
                                           where
                                           c.class_id == classID && t.type_id == typeID && comp.company_id == companyID && a.attribute_name == attributeName
                                           select new
                                           {
                                               products.product_name,
                                               products.product_image,
                                               products.product_description

                                           }).Distinct().ToList();
                Dictionary<string, List<String>> res = new Dictionary<string, List<string>>();
                List<String> productList = new List<string>();
                foreach (var product in productsByAttribute)
                {
                    productList.Add(product.product_name + "#" + product.product_image + "@" + product.product_description);
                }
                res.Add("products", productList);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else return Json("Error!", JsonRequestBehavior.AllowGet);

        }//
        public JsonResult getByClass(string className)
        {
            var Types = (from products in db.Products
                         join t in db.Type on products.type_id equals t.type_id
                         join c in db.Class on products.class_id equals c.class_id
                         where
                             c.class_name == className
                         select t.type_name).Distinct().ToList();

            var Attributes = (from products in db.Products
                              join c in db.Class on products.class_id equals c.class_id
                              join a in db.Attributes on products.attribute_id equals a.attribute_id
                              where
                              c.class_name == className
                              select  a.attribute_name).Distinct().ToList();


            var Companies = (from products in db.Products
                             join c in db.Class on products.class_id equals c.class_id
                             join comp in db.Company on products.company_id equals comp.company_id
                             where
                             c.class_name == "Бои и лакови"
                             select comp.company_name).Distinct().ToList();

            Dictionary<string, List<String>> allByOneClass = new Dictionary<string, List<string>>();
            allByOneClass.Add("types", Types);
            allByOneClass.Add("attributes", Attributes);
            allByOneClass.Add("companies", Companies);
            return Json(allByOneClass, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getAttributesbyClassType(string className, string typeName)
        {
            List<string> attributes = (from p in db.Products
                                       join t in db.Type on p.type_id equals t.type_id
                                       join c in db.Class on p.class_id equals c.class_id
                                       join a in db.Attributes on p.attribute_id equals a.attribute_id
                                       where c.class_name == className && t.type_name == typeName
                                        select a.attribute_name).Distinct().ToList();

            return Json(attributes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getCompanybyClassTypeAttribute(string className, string typeName, string attributeName)
        {
            var companies = (from p in db.Products
                             join comp in db.Company on p.company_id equals comp.company_id
                             join t in db.Type on p.type_id equals t.type_id
                             join c in db.Class on p.class_id equals c.class_id
                             join a in db.Attributes on p.attribute_id equals a.attribute_id
                             where c.class_name == className && t.type_name == typeName && a.attribute_name == attributeName
                             select comp.company_name).Distinct().ToList();

            return Json(companies, JsonRequestBehavior.AllowGet);
        }
    }
}


