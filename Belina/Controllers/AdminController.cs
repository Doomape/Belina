﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using Belina.Models;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using Belina.Helpers;

namespace Belina.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        BelinaEntities2 db = new BelinaEntities2();

        public ActionResult Index()
        {
           
            return View();
        }
        public ActionResult Add(HttpPostedFileBase file)
        {
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }
        #region Insert new Classes
        public JsonResult insertClasses(string className)
        {
            IList<String> existRow = (from x in db.Class where x.class_name == className select x.class_name).Distinct().ToList();

            if (existRow.Count > 0)
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
        #endregion
        #region Insert new Types
        public JsonResult insertType(string typeName)
        {
            IList<String> existRow = (from x in db.Type where x.type_name == typeName select x.type_name).Distinct().ToList();

            if (existRow.Count > 0)
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
                return Json("Типот е внесен.", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Insert new Companies
        public JsonResult insertCompany(string companyName)
        {
            IList<String> existRow = (from x in db.Company where x.company_name == companyName select x.company_name).Distinct().ToList();

            if (existRow.Count > 0)
            {
                return Json("Типот веќе постои..среќа што сме бесконечно пописмени од инфопроект па не дозволуваме вакви глупости.", JsonRequestBehavior.AllowGet);
            }
            else
            {
                Company companyObj = new Company();

                companyObj.company_id = (from x in db.Company select x.company_id).Max() + 1;
                companyObj.company_name = companyName;
                db.Company.Add(companyObj);
                db.SaveChanges();
                return Json("Производителот е внесен.", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Insert new Attributes
        public JsonResult insertAttribute(string attributeName)
        {
            IList<String> existRow = (from x in db.Attributes where x.attribute_name == attributeName select x.attribute_name).Distinct().ToList();

            if (existRow.Count > 0)
            {
                return Json("Специфичната карактеристика веќе постои..среќа што сме бесконечно пописмени од инфопроект па не дозволуваме вакви глупости.", JsonRequestBehavior.AllowGet);
            }
            else
            {
                Attributes attrObj = new Attributes();

                attrObj.attribute_id = (from x in db.Attributes select x.attribute_id).Max() + 1;
                attrObj.attribute_name = attributeName;
                db.Attributes.Add(attrObj);
                db.SaveChanges();
                return Json("Специфичната карактеристика е внесена.", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Get product Dependencies
        public JsonResult productDependencies()
        {
            var classes = (from x in db.Class where x.class_name != "Недефинирано" && x.class_name != "Разно" select x.class_name).Distinct().ToList();
            var types = (from x in db.Type select x.type_name).Distinct().ToList();
            var attributes = (from x in db.Attributes select x.attribute_name).Distinct().ToList();
            var companies = (from x in db.Company select x.company_name).Distinct().ToList();
            Dictionary<String, List<String>> res = new Dictionary<string, List<String>>();
            res.Add("classes", classes);
            res.Add("types",types);
            res.Add("attributes",attributes);
            res.Add("companies", companies);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Insert new product
        public JsonResult insertProduct(HttpPostedFileBase file, string productName, string allClasses, string allTypes,string allAttributes, string allCompanies, string productDecription, string discount, string promotion)
        {
            Products products = new Products();
            Company_Class c_c = new Company_Class();
            Type_Company t_c = new Type_Company();
            Product_Attribute p_a = new Product_Attribute();
            
            var classID = (from x in db.Class where x.class_name == allClasses select x).First<Class>().class_id;
            var typeID = (from x in db.Type where x.type_name == allTypes select x).First<Belina.Models.Type>().type_id;
            var companyID = (from x in db.Company where x.company_name == allCompanies select x).First<Company>().company_id;
            var attributeID=(from x in db.Attributes where x.attribute_name == allAttributes select x).First<Belina.Models.Attributes>().attribute_id;

            products.product_name = productName;
            products.class_id = classID;
            products.type_id = typeID;
            products.company_id = companyID;
            products.product_discount = discount;
            products.product_promotion = promotion;
            products.attribute_id = attributeID;

            if (!Directory.Exists(Server.MapPath("~/Content/companies/" + allCompanies)))
            {
                Directory.CreateDirectory(Server.MapPath("~/Content/companies/" + allCompanies));
            }
            if (file.FileName != null || file.FileName != "")
            {
                if (System.IO.File.Exists(Server.MapPath("~/Content/companies/" + allCompanies + "/" + file.FileName)))
                {
                    products.product_image = "/Content/companies/" + allCompanies + "/" + file.FileName;
                }
                else
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/companies/" + allCompanies + "/"), fileName);
                    file.SaveAs(path);
                    products.product_image = "/Content/companies/" + allCompanies + "/" + file.FileName;
                }
            }
            else
            {
                products.product_image = "/Content/belinaLogo5.png";
            }

            db.Products.Add(products);

            var existCompany_Class=(from x in db.Company_Class where x.class_id==classID && x.company_id==companyID select x).Count();
            var existType_Company = (from x in db.Type_Company where x.company_id == companyID && x.type_id == typeID select x).Count();
            var existProduct_Attribute = (from x in db.Product_Attribute where x.attribute_id== attributeID && x.company_id == companyID && x.type_id == typeID select x).Count();

            if (existCompany_Class == 0)
            {
                c_c.class_id = classID;
                c_c.company_id = companyID;
                c_c.id = (from x in db.Company_Class select x.id).Max() + 1;
                db.Company_Class.Add(c_c);
            }

            if (existType_Company == 0)
            {
                t_c.type_id = typeID;
                t_c.company_id = companyID;
                t_c.id = (from x in db.Type_Company select x.id).Max() + 1;
                db.Type_Company.Add(t_c);
            }

            if (existProduct_Attribute == 0)
            {
                p_a.attribute_id = attributeID;
                p_a.company_id = companyID;
                p_a.type_id = typeID;
                db.Product_Attribute.Add(p_a);
            }
            
            db.SaveChanges();

            return Json("Успешно додавање на нов производ", JsonRequestBehavior.AllowGet);
        }
       #endregion
        #region Generate XML for Products
        public LargeJsonResult XMLForProducts()
        {
            string[] column_names = { "Производ", "Класа", "Тип",  "Производител",  "Специфична карактеристика", "Детален опис", "Фотографија" };
            XDocument doc = new XDocument();
            XElement rows = new XElement("rows");

            XElement head = new XElement("head");

            XElement column;

            var Dbclasses = (from x in db.Class where x.class_name != "Недефинирано" && x.class_name != "Разно" select x.class_name).Distinct().ToList();
            var Dbtypes = (from x in db.Type select x.type_name).Distinct().ToList();
            var Dbcompanies = (from x in db.Company select x.company_name).Distinct().ToList();
            var Dbattributes = (from x in db.Attributes select x.attribute_name).Distinct().ToList();
            foreach (var columnName in column_names)
            {
                if (column_names[0] == columnName)
                {
                    column = new XElement("column", new XAttribute("type", "ed"),
                    new XAttribute("width", "205"), new XAttribute("sort", "str"), new XText(columnName));
                    head.Add(column);
                }
                if (column_names[1] == columnName)
                {
                    column = new XElement("column", new XAttribute("type", "coro"),
                    new XAttribute("width", "205"), new XAttribute("sort", "str"), new XAttribute("id", "last"), new XText(columnName));
                    foreach (var Dbclass in Dbclasses)
                    {
                        column.Add(new XElement("option", new XAttribute("value", Dbclass), new XText(Dbclass)));
                    }
                    head.Add(column);
                }
                if (column_names[2] == columnName)
                {
                    column = new XElement("column", new XAttribute("type", "coro"),
                    new XAttribute("width", "205"), new XAttribute("sort", "str"), new XAttribute("id", "last"), new XText(columnName));
                    foreach (var type in Dbtypes)
                    {
                        column.Add(new XElement("option", new XAttribute("value", type), new XText(type)));
                    }
                    head.Add(column);
                }
                if (column_names[3] == columnName)
                {
                    column = new XElement("column", new XAttribute("type", "coro"),
                    new XAttribute("width", "205"), new XAttribute("sort", "str"), new XAttribute("id", "last"), new XText(columnName));
                    foreach (var Dbcompany in Dbcompanies)
                    {
                        column.Add(new XElement("option", new XAttribute("value", Dbcompany), new XText(Dbcompany)));
                    }
                    head.Add(column);
                }
                if (column_names[4] == columnName)
                {
                    column = new XElement("column", new XAttribute("type", "coro"),
                    new XAttribute("width", "205"), new XAttribute("sort", "str"), new XAttribute("id", "last"), new XText(columnName));
                    foreach (var Dbattribute in Dbattributes)
                    {
                        column.Add(new XElement("option", new XAttribute("value", Dbattribute), new XText(Dbattribute)));
                    }
                    head.Add(column);
                }
                if (column_names[5] == columnName)
                {
                    column = new XElement("column", new XAttribute("type", "txt"),
                    new XAttribute("width", "205"), new XAttribute("sort", "str"), new XAttribute("id", "last"), new XText(columnName));
                    head.Add(column);
                }
                if (column_names[6] == columnName)
                {
                    column = new XElement("column", new XAttribute("type", "img"),
                    new XAttribute("width", "200"), new XAttribute("sort", "str"), new XAttribute("id", "last"), new XText(columnName));
                    head.Add(column);
                }
            }
            rows.Add(head);
            XElement row;
            XElement cell;
            XElement cell2;
            XElement cell3;
            XElement cell4;
            XElement cell5;
            XElement cell6;
            XElement cell7;
            var products = (from product in db.Products 
                              from classes in db.Class
                                from company in db.Company
                                 from type in db.Type
                                  from attribute in db.Attributes
                                  where
                                    product.class_id == classes.class_id &&
                                    product.company_id == company.company_id &&
                                    product.type_id == type.type_id &&
                                    product.attribute_id == attribute.attribute_id
                                     select new { product.product_name, product.product_id, classes.class_name, type.type_name,
                                        company.company_name, attribute.attribute_name, product.product_description,  product.product_image}).ToList();
            foreach (var prod in products)
            {
                row = new XElement("row", new XAttribute("id", prod.product_id));
                cell = new XElement("cell", new XText(prod.product_name));
                cell2 = new XElement("cell", new XText(prod.class_name));
                cell3 = new XElement("cell", new XText(prod.type_name));
                cell4 = new XElement("cell", new XText(prod.company_name));
                cell5 = new XElement("cell", new XText(prod.attribute_name));
                if (prod.product_description != null)
                {
                    cell6 = new XElement("cell", new XText(prod.product_description));
                }
                else
                {
                    cell6 = new XElement("cell", new XText("/"));
                }
                cell7 = new XElement("cell", new XText(prod.product_image));
                row.Add(cell);
                row.Add(cell2);
                row.Add(cell3);
                row.Add(cell4);
                row.Add(cell5);
                row.Add(cell6);
            //    row.Add(cell7);
                rows.Add(row);
            }
            doc.Add(rows);
            return new LargeJsonResult() { Data = doc.ToString(), MaxJsonLength = int.MaxValue, JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet };
        }
        #endregion
        #region Generate XML for Classes
        public JsonResult XMLForClasses()
        {
            XDocument doc = new XDocument();
            XElement rows = new XElement("rows");

            XElement head = new XElement("head");

            XElement column;
            column = new XElement("column", new XAttribute("type", "ed"),
                   new XAttribute("width", "205"), new XAttribute("sort", "str"), new XText("Класи"));
            head.Add(column);
            column = new XElement("column", new XAttribute("type", "ed"),
                   new XAttribute("width", "205"), new XAttribute("sort", "str"), new XText("Избриши"));
            head.Add(column);
            rows.Add(head);
            XElement row;
            XElement cell;
            XElement cell2;
            var Dbclasses = (from x in db.Class where x.class_name != "Недефинирано" && x.class_name != "Разно" select x).Distinct().ToList();
            foreach (var classes in Dbclasses)
            {
                row = new XElement("row", new XAttribute("id", classes.class_id));
                cell = new XElement("cell", new XText(classes.class_name));
                cell2 = new XElement("cell", new XText("Ili check box ili link so f-ja()"));
                row.Add(cell);
                row.Add(cell2);
                rows.Add(row);
            }
            doc.Add(rows);
            return Json(doc.ToString(), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Generate XML for Types
        public JsonResult XMLForTypes()
        {
            XDocument doc = new XDocument();
            XElement rows = new XElement("rows");

            XElement head = new XElement("head");

            XElement column;
            column = new XElement("column", new XAttribute("type", "ed"),
                   new XAttribute("width", "205"), new XAttribute("sort", "str"), new XText("Типови"));
            head.Add(column);
            rows.Add(head);
            XElement row;
            XElement cell;
            var Dbtypes = (from x in db.Type select x).Distinct().ToList();
            foreach (var type in Dbtypes)
            {
                row = new XElement("row", new XAttribute("id", type.type_id));
                cell = new XElement("cell", new XText(type.type_name));
                row.Add(cell);
                rows.Add(row);
            }
            doc.Add(rows);
            return Json(doc.ToString(), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Generate XML for Attrbutes
        public JsonResult XMLForAttributes()
        {
            XDocument doc = new XDocument();
            XElement rows = new XElement("rows");

            XElement head = new XElement("head");

            XElement column;
            column = new XElement("column", new XAttribute("type", "ed"),
                   new XAttribute("width", "205"), new XAttribute("sort", "str"), new XText("Специфични карактеристики"));
            head.Add(column);
            rows.Add(head);
            XElement row;
            XElement cell;
            var Dbattribute = (from x in db.Attributes select x).Distinct().ToList();
            foreach (var attribute in Dbattribute)
            {
                row = new XElement("row", new XAttribute("id", attribute.attribute_id));
                cell = new XElement("cell", new XText(attribute.attribute_name));
                row.Add(cell);
                rows.Add(row);
            }
            doc.Add(rows);
            return Json(doc.ToString(), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Generate XML for Companies
        public JsonResult XMLForCompanies()
        {
            XDocument doc = new XDocument();
            XElement rows = new XElement("rows");

            XElement head = new XElement("head");

            XElement column;
            column = new XElement("column", new XAttribute("type", "ed"),
                   new XAttribute("width", "205"), new XAttribute("sort", "str"), new XText("Производители"));
            head.Add(column);
            rows.Add(head);
            XElement row;
            XElement cell;
            var Dbcompany = (from x in db.Company select x).Distinct().ToList();
            foreach (var company in Dbcompany)
            {
                row = new XElement("row", new XAttribute("id", company.company_id));
                cell = new XElement("cell", new XText(company.company_name));
                row.Add(cell);
                rows.Add(row);
            }
            doc.Add(rows);
            return Json(doc.ToString(), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region update Classes
        public JsonResult updateClasses(string old_className,string new_className)
        {
            Class classObj = (from x in db.Class
                       where x.class_name == old_className
                       select x).First();
            classObj.class_name = new_className;
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region update Types
        public JsonResult updateTypes(string old_typeName, string new_typeName)
        {
            Belina.Models.Type typeObj = (from x in db.Type
                             where x.type_name == old_typeName
                             select x).First();
            typeObj.type_name = new_typeName;
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region update Attributes
        public JsonResult updateAttributes(string old_attributeName, string new_attributeName)
        {
            Attributes attributeObj = (from x in db.Attributes
                                       where x.attribute_name == old_attributeName
                                          select x).First();
            attributeObj.attribute_name = new_attributeName;
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region update Companies
        public JsonResult updateCompanies(string old_companyName, string new_companyName)
        {
            Company companyObj = (from x in db.Company
                                  where x.company_name == old_companyName
                                       select x).First();
            companyObj.company_name = new_companyName;
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
