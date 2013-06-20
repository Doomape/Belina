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
            IList<Class> classes = (from x in db.Class where x.class_name != "Недефинирано" && x.class_name != "Разно" && x.class_id != 1 select x).Distinct().ToList();
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

        }
        public JsonResult getByClass(int classid)
        {
            var Types = (from products in db.Products
                         join t in db.Type on products.type_id equals t.type_id
                         join c in db.Class on products.class_id equals c.class_id
                         where
                             c.class_id == classid
                         select t).Distinct().ToList();

            var Attributes = (from products in db.Products
                              join c in db.Class on products.class_id equals c.class_id
                              join a in db.Attributes on products.attribute_id equals a.attribute_id
                              where
                              c.class_id == classid
                              select  a).Distinct().ToList();


            var Companies = (from products in db.Products
                             join c in db.Class on products.class_id equals c.class_id
                             join comp in db.Company on products.company_id equals comp.company_id
                             where
                             c.class_id == classid
                             select comp).Distinct().ToList();

            Dictionary<string, List<String>> allByOneClass = new Dictionary<string, List<string>>();
            allByOneClass.Add("types", Types.ConvertAll(x => x.type_name));
            allByOneClass.Add("attributes", Attributes.ConvertAll(x => x.attribute_name));
            allByOneClass.Add("companies", Companies.ConvertAll(x => x.company_name));

            allByOneClass.Add("typesid", Types.ConvertAll(x => x.type_id.ToString()));
            allByOneClass.Add("attributesid", Attributes.ConvertAll(x => x.attribute_id.ToString()));
            allByOneClass.Add("companiesid", Companies.ConvertAll(x => x.company_id.ToString()));
            return Json(allByOneClass, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getAttributesbyClassType(int classID, int typeID)
        {
            List<Attributes> attributes = (from p in db.Products
                                       join t in db.Type on p.type_id equals t.type_id
                                       join c in db.Class on p.class_id equals c.class_id
                                       join a in db.Attributes on p.attribute_id equals a.attribute_id
                                       where c.class_id == classID && t.type_id == typeID
                                        select a).Distinct().ToList();

            return Json(attributes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getCompanybyClassTypeAttribute(int classID, int typeID, int attributeNameID)
        {
            var companies = (from p in db.Products
                             join comp in db.Company on p.company_id equals comp.company_id
                             join t in db.Type on p.type_id equals t.type_id
                             join c in db.Class on p.class_id equals c.class_id
                             join a in db.Attributes on p.attribute_id equals a.attribute_id
                             where c.class_id == classID && t.type_id == typeID && a.attribute_id == attributeNameID
                             select comp.company_name).Distinct().ToList();

            return Json(companies, JsonRequestBehavior.AllowGet);
        }
        public JsonResult get_promotionProduct()
        {
            var companies = (from x in db.Products
                             where x.product_discount == true
                             select new { x.product_image, x.product_name, x.product_price }).Distinct().ToList();
            return Json(companies, JsonRequestBehavior.AllowGet);
        }
    }
}


