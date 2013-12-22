using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Belina.Models;
using System.Net.Mail;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Collections;

namespace Belina.Controllers
{
    public class HomeController : Controller
    {
        BelinaEntities2 db = new BelinaEntities2();
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult Products(int id, int type, String company, String attr, int? pageId)
        {
            IList<Belina.Models.Type> allTypes;
            IList<Company> companies;

            #region if type == 0
            if (type == 0 && company == "all" && attr == "all")
            {
                allTypes = (from products in db.Products
                                                      join t in db.Type on products.type_id equals t.type_id
                                                      join c in db.Class on products.class_id equals c.class_id
                                                      where
                                                          c.class_id == id
                                                      select t).Distinct().ToList();
                int typeId = Convert.ToInt32(allTypes[0].type_id);
                companies = (from products in db.Products
                                            join t in db.Type on products.type_id equals t.type_id
                                            join comp in db.Company on products.company_id equals comp.company_id
                                            join c in db.Class on products.class_id equals c.class_id
                                                where c.class_id == id && t.type_id == typeId
                                            select comp).Distinct().ToList();
                ViewBag.pageId = id;
                ViewBag.allItems = allTypes;
                ViewBag.Companies = companies;
                int companyID = Convert.ToInt32(companies[0].company_id);
                List<Attributes> attributes = (from p in db.Products
                                               join t in db.Type on p.type_id equals t.type_id
                                               join c in db.Class on p.class_id equals c.class_id
                                               join comp in db.Company on p.company_id equals comp.company_id
                                               join a in db.Attributes on p.attribute_id equals a.attribute_id
                                               where c.class_id == id && t.type_id == typeId
                                               select a).Distinct().ToList();
                ViewBag.Attributes = attributes;
                
                IList<Products> allProducts = (from product in db.Products
                                join t in db.Type on product.type_id equals t.type_id
                                join comp in db.Company on product.company_id equals comp.company_id
                                join c in db.Class on product.class_id equals c.class_id
                                where
                                c.class_id == id && t.type_id == typeId
                                               select product).ToList();

                var Page = pageId != null ? pageId : 0;
                int currentPageId = Convert.ToInt32(Page);
                var mater = allProducts.Skip(currentPageId*12).Take(12).ToList();
                double g = allProducts.Count() / 12.0;
                ViewBag.pagingLength = g <= 1.0 ? 0 : (int)Math.Ceiling(g);
                ViewBag.AllProducts = mater;
            }
            #endregion
            #region if id != 0
            if (id != 0 && type != 0)
            {
                //int type, int company,int attr
                allTypes = (from products in db.Products
                            join t in db.Type on products.type_id equals t.type_id
                            join c in db.Class on products.class_id equals c.class_id
                            where
                                c.class_id == id
                            select t).Distinct().ToList();

                companies = (from products in db.Products
                             join t in db.Type on products.type_id equals t.type_id
                             join comp in db.Company on products.company_id equals comp.company_id
                             join c in db.Class on products.class_id equals c.class_id
                             where c.class_id == id && t.type_id == type
                             select comp).Distinct().ToList();
                ViewBag.pageId = id;
                ViewBag.allItems = allTypes;
                ViewBag.Companies = companies;
                ViewBag.TypeId = type;
                int companyId;
                if (company == "all")
                {
                    
                    List<Attributes> attributes = (from p in db.Products
                                                   join t in db.Type on p.type_id equals t.type_id
                                                   join c in db.Class on p.class_id equals c.class_id
                                                   join comp in db.Company on p.company_id equals comp.company_id
                                                   join a in db.Attributes on p.attribute_id equals a.attribute_id
                                                   where c.class_id == id && t.type_id == type
                                                   select a).Distinct().ToList();
                    ViewBag.Attributes = attributes;
                    ViewBag.companyId = "all";


                    int attribute;
                    if (attr != "all")
                    {
                        attribute = Convert.ToInt32(attr);
                        IList<Products> allProducts = (from product in db.Products
                                                       join t in db.Type on product.type_id equals t.type_id
                                                       join comp in db.Company on product.company_id equals comp.company_id
                                                       join c in db.Class on product.class_id equals c.class_id
                                                       join a in db.Attributes on product.attribute_id equals a.attribute_id
                                                       where
                                                       c.class_id == id && t.type_id == type
                                                       && a.attribute_id == attribute
                                                       select product).Distinct().ToList();
                        var PageSpec = pageId != null ? pageId : 0;
                        int currentPageIdSpec = Convert.ToInt32(PageSpec);
                        var mater = allProducts.Skip(currentPageIdSpec * 12).Take(12).ToList();
                        double g = allProducts.Count() / 12.0;
                        ViewBag.pagingLength = g <= 1.0 ? 0 : (int)Math.Ceiling(g);
                        ViewBag.AllProducts = mater;
                    }
                    if (attr == "all")
                    {
                        var Page = pageId != null ? pageId : 1;
                        int currentPageId = Convert.ToInt32(Page);

                        IList<Products> allProducts = (from product in db.Products
                                                       join t in db.Type on product.type_id equals t.type_id
                                                       join comp in db.Company on product.company_id equals comp.company_id
                                                       join c in db.Class on product.class_id equals c.class_id
                                                       where
                                                       c.class_id == id && t.type_id == type
                                                       select product).Distinct().ToList();

                        var PageSpec = pageId != null ? pageId : 0;
                        int currentPageIdSpec = Convert.ToInt32(PageSpec);
                        var mater = allProducts.Skip(currentPageIdSpec * 12).Take(12).ToList();
                        double g = allProducts.Count() / 12.0;
                        ViewBag.pagingLength = g <= 1.0 ? 0 : (int)Math.Ceiling(g);
                        ViewBag.AllProducts = mater;
                    }
                }
                else
                {
                    companyId = Convert.ToInt32(company);
                    List<Attributes> attributes = (from p in db.Products
                                                   join t in db.Type on p.type_id equals t.type_id
                                                   join c in db.Class on p.class_id equals c.class_id
                                                   join comp in db.Company on p.company_id equals comp.company_id
                                                   join a in db.Attributes on p.attribute_id equals a.attribute_id
                                                   where c.class_id == id && t.type_id == type && comp.company_id == companyId
                                                   select a).Distinct().ToList();
                    ViewBag.Attributes = attributes;
                    ViewBag.companyId = companyId;


                    int attribute;
                    if (attr != "all")
                    {
                        attribute = Convert.ToInt32(attr);
                        IList<Products> allProducts = (from product in db.Products
                                                       join t in db.Type on product.type_id equals t.type_id
                                                       join comp in db.Company on product.company_id equals comp.company_id
                                                       join c in db.Class on product.class_id equals c.class_id
                                                       join a in db.Attributes on product.attribute_id equals a.attribute_id
                                                       where
                                                       c.class_id == id && t.type_id == type && comp.company_id == companyId
                                                       && a.attribute_id == attribute
                                                       select product).Distinct().ToList();
                        var PageSpec = pageId != null ? pageId : 0;
                        int currentPageIdSpec = Convert.ToInt32(PageSpec);
                        var mater = allProducts.Skip(currentPageIdSpec * 12).Take(12).ToList();
                        double g = allProducts.Count() / 12.0;
                        ViewBag.pagingLength = g <= 1.0 ? 0 : (int)Math.Ceiling(g);
                        ViewBag.AllProducts = mater;
                    }
                    if (attr == "all")
                    {
                        var Page = pageId != null ? pageId : 1;
                        int currentPageId = Convert.ToInt32(Page);

                        IList<Products> allProducts = (from product in db.Products
                                                       join t in db.Type on product.type_id equals t.type_id
                                                       join comp in db.Company on product.company_id equals comp.company_id
                                                       join c in db.Class on product.class_id equals c.class_id
                                                       where
                                                       c.class_id == id && t.type_id == type && comp.company_id == companyId
                                                       select product).Distinct().ToList();

                        var PageSpec = pageId != null ? pageId : 0;
                        int currentPageIdSpec = Convert.ToInt32(PageSpec);
                        var mater = allProducts.Skip(currentPageIdSpec * 12).Take(12).ToList();
                        double g = allProducts.Count() / 12.0;
                        ViewBag.pagingLength = g <= 1.0 ? 0 : (int)Math.Ceiling(g);
                        ViewBag.AllProducts = mater;
                    }
                }
               
            }
            #endregion
            return View();
        }
        public ActionResult ProductInfo(int id)
        {
            IList<Products> product = (from products in db.Products where products.product_id == id select products).ToList();
            ViewBag.Product = product;
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        [AllowAnonymous]
        public JsonResult getClasses()
        {
            IList<Class> classes = (from x in db.Class where x.class_name != "ХТЗ Опрема" && x.class_name != "Водовод" && x.class_name != "Електрика" && x.class_name != "Недефинирано" && x.class_name != "Разно" && x.class_id != 1 select x).Distinct().ToList();
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
                             select new { x.product_image, x.product_name, x.product_price, x.product_description,x.product_id }).Distinct().ToList();
            return Json(companies, JsonRequestBehavior.AllowGet);
        }

        public static void SendMail(string subject, string body)
        {

            var fromAddress = new MailAddress("belinaskopje@gmail.com", "Belina");
            string fromPassword = "belinaljupce";

            try
            {
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(fromAddress.Address, fromPassword)
                };

                System.Web.UI.WebControls.MailDefinition md = new System.Web.UI.WebControls.MailDefinition();
                md.From = "belinaskopje@gmail.com";
                md.IsBodyHtml = false;
                md.Subject = subject;
                System.Collections.Specialized.ListDictionary replacements = new System.Collections.Specialized.ListDictionary();
                MailMessage msg = md.CreateMailMessage("belinaljupce@belina.com.mk", replacements, body, new System.Web.UI.Control());
                smtp.Send(msg);
            }
            catch
            {

            }
        }
        public JsonResult Sendform(string ime, string broj, string email, string poraka)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            if ((ime != "") && (email != "") && (poraka != ""))
            {
                string porakadoBelina = "Name or Company: " + ime + "\n" + "Email: " + email + "\n" + "Number: " + broj + "\n" + "Message:" + "\n" + poraka;
                SendMail("Нова порака", porakadoBelina);
                res.Add("msg", " Успешно испратена порака ");
            }
            else
            {
                res.Add("msg", " Неуспешно испратена порака ");
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getFooter()
        {
            IList<Class> classes = (from x in db.Class where x.class_name != "ХТЗ Опрема" && x.class_name != "Водовод" && x.class_name != "Електрика" && x.class_name != "Недефинирано" && x.class_name != "Разно" && x.class_id != 1 select x).Distinct().ToList();
            Dictionary<String, IList<Belina.Models.Type>> res = new Dictionary<String, IList<Belina.Models.Type>>();
            IList<Belina.Models.Type> allTypes;
            for (int i = 0; i < classes.Count;i++ )
            {
                int p = Convert.ToInt32(classes[i].class_id);
                 allTypes = (from products in db.Products
                                                      join t in db.Type on products.type_id equals t.type_id
                                                      join c in db.Class on products.class_id equals c.class_id
                                                      where
                                                          c.class_id == p
                                                      select t).Distinct().ToList();
                res.Add(classes[i].class_id+"#"+classes[i].class_name, allTypes);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
}


//var randomProduct = (from x in db.Products
//                               join c in db.Class on x.class_id equals c.class_id
//                               join t in db.Type on x.type_id equals t.type_id
//                               join com in db.Company on x.company_id equals com.company_id
//                               join atr in db.Attributes on x.attribute_id equals atr.attribute_id
//                               where x.class_id == id
//                               select
//                                   new
//                                   {
//                                       x.product_id,
//                                       x.product_name,
//                                       x.product_image,
//                                       x.product_description,
//                                       c.class_name,
//                                       t.type_name,
//                                       com.company_name,
//                                       atr.attribute_name
//                                   }).ToList();
//var rand = new Random();
//var randomPt = randomProduct[rand.Next(randomProduct.Count())];
//ViewBag.randomProd = new RandomProduct(randomPt.class_name, randomPt.product_id, randomPt.product_name, randomPt.product_image, randomPt.product_description, randomPt.type_name, randomPt.company_name, randomPt.class_name, randomPt.attribute_name);
