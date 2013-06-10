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

        public JsonResult getTypes(string className)
        {
            IList<String> allTypes = (from type in db.Type
                                      from classes in db.Class
                                      from class_type in db.Class_Type
                                      where classes.class_name == className &&
                                       classes.class_id == class_type.class_id &&
                                       type.type_id == class_type.type_id
                                      orderby type.type_name
                                      select type.type_name).Distinct().ToList();
            return Json(allTypes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getCompanies(string className, string typeName)
        {
            IList<String> companies = (from classes in db.Class
                                       from company in db.Company
                                       from type in db.Type
                                       from type_company in db.Type_Company
                                       from class_type in db.Class_Type
                                       from company_class in db.Company_Class
                                       where company.company_id == type_company.company_id
                                       && type_company.type_id == type.type_id
                                       && classes.class_name == className
                                       && classes.class_id == class_type.class_id
                                       && type.type_id == class_type.type_id
                                       && type.type_name == typeName &&
                                       classes.class_id == company_class.class_id &&
                                       company.company_id == company_class.company_id
                                       orderby company.company_name
                                       select company.company_name).Distinct().ToList();
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
                                from ap in db.Product_Attribute
                                where

                                classes.class_id == product.class_id &&
                                company.company_id == product.company_id &&
                                types.type_id == product.type_id &&
                                a.attribute_id == ap.attribute_id &&
                                ap.type_id == types.type_id &&
                                ap.company_id == company.company_id &&
                                ap.attribute_id == product.attribute_id &&

                                company.company_name == companyName &&
                                classes.class_name == className &&
                                types.type_name == typeName
                                orderby a.attribute_name
                                select new
                                {
                                    product.product_name,
                                    product.product_image,
                                    product.product_description,
                                    a.attribute_name
                                }).Distinct().ToList();

                var attributes = products.Select(p => p.attribute_name).Distinct().ToList();
                Dictionary<string, List<String>> res = new Dictionary<string, List<string>>();
                List<String> productList = new List<string>();
                foreach (var product in products)
                {
                    productList.Add(product.product_name + "#" + product.product_image + "@" + product.product_description);
                }
                res.Add("products", productList);
                res.Add("attributes", attributes);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            if (!(attributeName.Equals("false")))
            {
                var productsByAttribute = (from company in db.Company
                                           from classes in db.Class
                                           from types in db.Type
                                           from product in db.Products
                                           from a in db.Attributes
                                           from ap in db.Product_Attribute
                                           where

                                           classes.class_id == product.class_id &&
                                           company.company_id == product.company_id &&
                                           types.type_id == product.type_id &&
                                           a.attribute_id == ap.attribute_id &&
                                           ap.type_id == types.type_id &&
                                           ap.company_id == company.company_id &&
                                           ap.attribute_id == product.attribute_id &&

                                           company.company_name == companyName &&
                                           classes.class_name == className &&
                                           types.type_name == typeName &&
                                           a.attribute_name == attributeName
                                           orderby product.product_name
                                           select new
                                           {
                                               product.product_name,
                                               product.product_image,
                                               product.product_description
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

        }
        public JsonResult getByClass(string className)
        {
            var Types = (from classes in db.Class
                         from type in db.Type
                         from class_type in db.Class_Type
                         where classes.class_name == className
                         && classes.class_id == class_type.class_id
                         && type.type_id == class_type.type_id
                         orderby type.type_name
                         select type.type_name).Distinct().ToList();

            var Attributes = (from classes in db.Class
                              from types in db.Type
                              from a in db.Attributes
                              from ap in db.Product_Attribute
                              from class_type in db.Class_Type
                              where classes.class_name == className
                              && a.attribute_id == ap.attribute_id
                              && ap.type_id == types.type_id
                              && classes.class_id == class_type.class_id
                              && types.type_id == class_type.type_id
                              select a.attribute_name).Distinct().ToList();

            var Companies = (from classes in db.Class
                             from cc in db.Company_Class
                             from company in db.Company

                             where classes.class_id == cc.class_id && cc.company_id == company.company_id && classes.class_name == className
                             select company.company_name).Distinct().ToList();
            Dictionary<string, List<String>> allByOneClass = new Dictionary<string, List<string>>();
            allByOneClass.Add("types", Types);
            allByOneClass.Add("attributes", Attributes);
            allByOneClass.Add("companies", Companies);
            return Json(allByOneClass, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getAttributesbyClassType(string className, string typeName)
        {
            List<string> attributes = (from classes in db.Class
                                       from type in db.Type
                                       from a in db.Attributes
                                       from ap in db.Product_Attribute
                                       from class_type in db.Class_Type
                                       where
                                        a.attribute_id == ap.attribute_id &&
                                        classes.class_name == className && type.type_name == typeName
                                        && type.type_id == ap.type_id
                                        && classes.class_id == class_type.class_id
                                        && type.type_id == class_type.type_id
                                       select a.attribute_name).Distinct().ToList();
            return Json(attributes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getCompanybyClassTypeAttribute(string className, string typeName, string attributeName)
        {
            List<string> companies = (from classes in db.Class
                                      from company in db.Company
                                      from tc in db.Type_Company
                                      from type in db.Type
                                      from a in db.Attributes
                                      from ap in db.Product_Attribute
                                      from class_type in db.Class_Type
                                      where
                                      a.attribute_id == ap.attribute_id &&
                                      company.company_id == tc.company_id
                                      && tc.type_id == type.type_id
                                      && company.company_id == ap.company_id
                                      && type.type_id == ap.type_id
                                      && classes.class_name == className
                                      && type.type_name == typeName
                                      && a.attribute_name == attributeName
                                      && classes.class_id == class_type.class_id
                                      && type.type_id == class_type.type_id
                                      select company.company_name).ToList();
            return Json(companies, JsonRequestBehavior.AllowGet);
        }
    }
}


