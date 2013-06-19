using System;
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
using System.Text;

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
        public ActionResult Add()
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
                return Json("Класата " + className + " веќе постои..среќа што сме бесконечно пописмени од инфопроект па не дозволуваме вакви глупости.", JsonRequestBehavior.AllowGet);
            }
            else
            {
                Class classObj = new Class();
                classObj.class_id = (from x in db.Class select x.class_id).Max() + 1;
                classObj.class_name = className;
                db.Class.Add(classObj);
                db.SaveChanges();
                return Json("Класата " + className + " е внесена.", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Insert new Types
        public JsonResult insertType(string typeName)
        {
            IList<String> existRow = (from x in db.Type where x.type_name == typeName select x.type_name).Distinct().ToList();

            if (existRow.Count > 0)
            {
                return Json("Типот " + typeName + " веќе постои..среќа што сме бесконечно пописмени од инфопроект па не дозволуваме вакви глупости.", JsonRequestBehavior.AllowGet);
            }
            else
            {
                Belina.Models.Type typeObj = new Belina.Models.Type();

                typeObj.type_id = (from x in db.Type select x.type_id).Max() + 1;
                typeObj.type_name = typeName;
                db.Type.Add(typeObj);
                db.SaveChanges();
                return Json("Типот " + typeName + " е внесен.", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Insert new Companies
        public JsonResult insertCompany(string companyName)
        {
            IList<String> existRow = (from x in db.Company where x.company_name == companyName select x.company_name).Distinct().ToList();

            if (existRow.Count > 0)
            {
                return Json("Производителот " + companyName + " веќе постои..среќа што сме бесконечно пописмени од инфопроект па не дозволуваме вакви глупости.", JsonRequestBehavior.AllowGet);
            }
            else
            {
                Company companyObj = new Company();

                companyObj.company_id = (from x in db.Company select x.company_id).Max() + 1;
                companyObj.company_name = companyName;
                db.Company.Add(companyObj);
                db.SaveChanges();
                return Json("Производителот " + companyName + " е внесен.", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Insert new Attributes
        public JsonResult insertAttribute(string attributeName)
        {
            IList<String> existRow = (from x in db.Attributes where x.attribute_name == attributeName select x.attribute_name).Distinct().ToList();

            if (existRow.Count > 0)
            {
                return Json("Специфичната карактеристика " + attributeName + " веќе постои..среќа што сме бесконечно пописмени од инфопроект па не дозволуваме вакви глупости.", JsonRequestBehavior.AllowGet);
            }
            else
            {
                Attributes attrObj = new Attributes();

                attrObj.attribute_id = (from x in db.Attributes select x.attribute_id).Max() + 1;
                attrObj.attribute_name = attributeName;
                db.Attributes.Add(attrObj);
                db.SaveChanges();
                return Json("Специфичната карактеристика " + attributeName + " е внесена.", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Insert new product
        public JsonResult insertProduct(HttpPostedFileBase file, string productName, int allClasses, int allTypes, int allAttributes, int allCompanies, string productDecription, string discount, string promotion)
        {
            Products products = new Products();

            products.product_name = productName;
            products.class_id = allClasses;
            products.type_id = allTypes;
            products.company_id = allCompanies;
            products.product_discount = discount;
            products.product_promotion = promotion;
            products.attribute_id = allAttributes;

            var company_name = (from x in db.Company where x.company_id == allCompanies select x).FirstOrDefault();

            if (!Directory.Exists(Server.MapPath("~/Content/companies/" + company_name.company_id)))
            {
                Directory.CreateDirectory(Server.MapPath("~/Content/companies/" + company_name.company_id));
            }
            if (file != null)
            {
                var fileName = DateTime.Now.Ticks.ToString() + Path.GetExtension(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/companies/" + company_name.company_id + "/"), fileName);
                file.SaveAs(path);
                products.product_image = "/Content/companies/" + company_name.company_id + "/" + fileName;
            }
            if (file == null)
            {
                products.product_image = "/Content/sliki/belinaLogo5.png";
            }

            db.Products.Add(products);
            db.SaveChanges();

            return Json("Успешно додавање на нов производ", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get product Dependencies
        public JsonResult productDependencies()
        {
            var classes = (from x in db.Class where x.class_name != "Недефинирано" && x.class_name != "Разно" select x).Distinct().ToList();
            var types = (from x in db.Type select x).Distinct().ToList();
            var attributes = (from x in db.Attributes select x).Distinct().ToList();
            var companies = (from x in db.Company select x).Distinct().ToList();
           
            Dictionary<String, List<String>> res = new Dictionary<string, List<String>>();
            res.Add("classes", classes.ConvertAll(x => x.class_name));
            res.Add("types", types.ConvertAll(x => x.type_name));
            res.Add("attributes", attributes.ConvertAll(x => x.attribute_name));
            res.Add("companies", companies.ConvertAll(x => x.company_name));

            res.Add("classesid", classes.ConvertAll(x => x.class_id.ToString()));
            res.Add("typesid", types.ConvertAll(x => x.type_id.ToString()));
            res.Add("attributesid", attributes.ConvertAll(x => x.attribute_id.ToString()));
            res.Add("companiesid", companies.ConvertAll(x => x.company_id.ToString()));

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Generate XML for Products
        public ActionResult XMLForProducts(int orderbyindex, string direction, string fil, string dontPos = "0", string posStart = "0", string count = "50")
        {
            string[] column_names = { "Избриши", "Производ", "Класа", "Тип", "Производител", "Специфична карактеристика", "Детален опис", "Фотографија" };
            string[] columns = { "product_name", "class_name", "type_name", "company_name", "attribute_name", "product_description", "product_image" };

            if (direction == "des") direction = "desc";
            string filters = filters = String.Concat(Enumerable.Repeat(",#text_filter", 6));

            string filters_sql = "";

            //generating the sql query
            var splitted_filters = fil.Split(new string[] { ";;;" }, StringSplitOptions.None);
            var i = 0;
            foreach (var filter in splitted_filters)
            {
                if (filter != "")
                    filters_sql += " and " + columns[i] + " like N'%" + filter + "%'";

                i++;
            }

            int products_count = 0;
            if (filters_sql != "")
            {
                products_count = (int)(from x in db.spCountProducts(columns[orderbyindex], direction, filters_sql) select x).FirstOrDefault();
            }
            else
            {
                products_count = (from x in db.Products select x).Count();
            }

            List<spGetProducts_Result> products = new List<spGetProducts_Result>();
            products = (from x in db.spGetProducts(columns[orderbyindex], direction, int.Parse(posStart), 50, filters_sql) select x).ToList();

            if (string.IsNullOrEmpty(posStart))
                posStart = "0";

            XDocument xdoc = new XDocument();

            XElement root = new XElement("rows", new XAttribute("total_count", products_count.ToString()), new XAttribute("pos", posStart));
            XElement head = new XElement("head");

            if (posStart == "0" && dontPos != "1")
            {
                root.Add(head);
            }

            if (posStart == "0")
            {
                XElement beforeinit = new XElement("afterInit");
                XElement call = new XElement("call", new XAttribute("command", "attachHeader"));
                XElement param = new XElement("param", new XText(filters));
                call.Add(param);
                beforeinit.Add(call);
                head.Add(beforeinit);
                XElement column;
                int counter = 0;
                var Dbclasses = (from x in db.Class where x.class_name != "Недефинирано" && x.class_name != "Разно" select x).Distinct().ToList();
                var Dbtypes = (from x in db.Type select x).Distinct().ToList();
                var Dbcompanies = (from x in db.Company select x).Distinct().ToList();
                var Dbattributes = (from x in db.Attributes select x).Distinct().ToList();
                foreach (var columnName in column_names)
                {
                    if (column_names[0] == columnName)
                    {
                        column = new XElement("column", new XAttribute("type", "ch"), new XAttribute("align", "center"),
                       new XAttribute("width", "65%"), new XAttribute("sort", "str"), new XText(columnName));
                        head.Add(column);
                    }
                    if (column_names[1] == columnName)
                    {
                        column = new XElement("column", new XAttribute("type", "ed"),
                       new XAttribute("width", "200%"), new XAttribute("sort", "server"), new XText(columnName));
                        head.Add(column);
                    }
                    if (column_names[2] == columnName)
                    {
                        column = new XElement("column", new XAttribute("type", "coro"),
                       new XAttribute("width", "200%"), new XAttribute("sort", "server"), new XText(columnName));
                        foreach (var Dbclass in Dbclasses)
                        {
                            column.Add(new XElement("option", new XAttribute("value", Dbclass.class_id), new XText(Dbclass.class_name)));
                        }
                        head.Add(column);
                    }
                    if (column_names[3] == columnName)
                    {
                        column = new XElement("column", new XAttribute("type", "coro"),
                       new XAttribute("width", "200%"), new XAttribute("sort", "server"), new XText(columnName));
                        foreach (var type in Dbtypes)
                        {
                            column.Add(new XElement("option", new XAttribute("value", type.type_id), new XText(type.type_name)));
                        }
                        head.Add(column);
                    }
                    if (column_names[4] == columnName)
                    {
                        column = new XElement("column", new XAttribute("type", "coro"),
                        new XAttribute("width", "200%"), new XAttribute("sort", "server"), new XText(columnName));
                        foreach (var Dbcompany in Dbcompanies)
                        {
                            column.Add(new XElement("option", new XAttribute("value", Dbcompany.company_id), new XText(Dbcompany.company_name)));
                        }
                        head.Add(column);
                    }
                    if (column_names[5] == columnName)
                    {
                        column = new XElement("column", new XAttribute("type", "coro"),
                        new XAttribute("width", "200%"), new XAttribute("sort", "server"), new XText(columnName));
                        foreach (var Dbattribute in Dbattributes)
                        {
                            column.Add(new XElement("option", new XAttribute("value", Dbattribute.attribute_id), new XText(Dbattribute.attribute_name)));
                        }
                        head.Add(column);
                    }
                    if (column_names[6] == columnName)
                    {
                        column = new XElement("column", new XAttribute("type", "txt"),
                                             new XAttribute("width", "200%"), new XAttribute("sort", "server"), new XText(columnName));
                        head.Add(column);
                    }
                    if (column_names[7] == columnName)
                    {
                        column = new XElement("column", new XAttribute("type", "ro"),
                         new XAttribute("width", "130%"), new XAttribute("sort", "server"), new XAttribute("id", "last"), new XText(columnName));
                        head.Add(column);
                    }
                    counter++;
                }
            }

            XElement row;
            XElement cell;
            XElement cell2;
            XElement cell3;
            XElement cell4;
            XElement cell5;
            XElement cell6;
            XElement cell7;
            XElement cell8;
            foreach (var prod in products)
            {
                row = new XElement("row", new XAttribute("id", prod.product_id));
                cell = new XElement("cell", 0);
                cell2 = new XElement("cell", new XText(prod.product_name));
                cell3 = new XElement("cell", new XText(prod.class_name));
                cell4 = new XElement("cell", new XText(prod.type_name));
                cell5 = new XElement("cell", new XText(prod.company_name));
                cell6 = new XElement("cell", new XText(prod.attribute_name));
                if (prod.product_description != null)
                {
                    cell7 = new XElement("cell", new XText(prod.product_description));
                }
                else
                {
                    cell7 = new XElement("cell", new XText("/"));
                }
                cell8 = new XElement("cell", new XText("<img class='imagePosition_" + prod.product_id + "' onmouseover='show_normal_image(" + '"' + prod.product_image + '"' + "," + '"' + prod.product_id + '"' + ")' onmouseout='hide_normal_image()' style='width:15px;height:15px' src='" + prod.product_image + "'/><form name='" + prod.product_id + "' enctype='multipart/form-data' ><input type='file' name='file'></input><br/><button class='button_row' type='button' onclick='uploadPhoto(" + '"' + prod.product_id + '"' + ")'>Прикачи</button></form>"));
                //<form name = '" + prod.product_id + "' enctype='multipart/form-data' method='POST'><input type='file'></input><br/><button class='button_row' type='button' onclick='uploadPhoto(" + prod.product_id + ")'>Внеси</button></form>
                row.Add(cell);
                row.Add(cell2);
                row.Add(cell3);
                row.Add(cell4);
                row.Add(cell5);
                row.Add(cell6);
                row.Add(cell7);
                row.Add(cell8);
                root.Add(row);
            }
            xdoc.Add(root);
            return Content(xdoc.ToString(), "text/xml", Encoding.UTF8);
        }
        #endregion
        #region Generate XML for Classes
        public JsonResult XMLForClasses()
        {
            XDocument doc = new XDocument();
            XElement rows = new XElement("rows");

            XElement head = new XElement("head");

            XElement column;
            column = new XElement("column", new XAttribute("type", "ed"), new XAttribute("align", "left"),
                   new XAttribute("width", "350"), new XAttribute("sort", "str"), new XText("Класи"));
            head.Add(column);
            column = new XElement("column", new XAttribute("type", "ch"), new XAttribute("align", "center"),
                   new XAttribute("width", "290"), new XAttribute("sort", "str"), new XText("Избриши"));

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
                cell2 = new XElement("cell", 0);
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
                   new XAttribute("width", "350"), new XAttribute("sort", "str"), new XText("Типови"));
            head.Add(column);
            column = new XElement("column", new XAttribute("type", "ch"), new XAttribute("align", "center"),
                   new XAttribute("width", "265"), new XAttribute("sort", "str"), new XText("Избриши"));

            head.Add(column);
            rows.Add(head);
            XElement row;
            XElement cell;
            XElement cell2;
            var Dbtypes = (from x in db.Type select x).Distinct().ToList();
            foreach (var type in Dbtypes)
            {
                row = new XElement("row", new XAttribute("id", type.type_id));
                cell = new XElement("cell", new XText(type.type_name));
                cell2 = new XElement("cell", 0);
                row.Add(cell);
                row.Add(cell2);
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
                   new XAttribute("width", "350"), new XAttribute("sort", "str"), new XText("Специфични карактеристики"));
            head.Add(column);
            column = new XElement("column", new XAttribute("type", "ch"), new XAttribute("align", "center"),
                   new XAttribute("width", "265"), new XAttribute("sort", "str"), new XText("Избриши"));

            head.Add(column);
            rows.Add(head);
            XElement row;
            XElement cell;
            XElement cell2;
            var Dbattribute = (from x in db.Attributes select x).Distinct().ToList();
            foreach (var attribute in Dbattribute)
            {
                row = new XElement("row", new XAttribute("id", attribute.attribute_id));
                cell = new XElement("cell", new XText(attribute.attribute_name));
                cell2 = new XElement("cell", 0);
                row.Add(cell);
                row.Add(cell2);
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
                   new XAttribute("width", "350"), new XAttribute("sort", "str"), new XText("Производители"));
            head.Add(column);
            column = new XElement("column", new XAttribute("type", "ch"), new XAttribute("align", "center"),
                  new XAttribute("width", "265"), new XAttribute("sort", "str"), new XText("Избриши"));

            head.Add(column);
            rows.Add(head);
            XElement row;
            XElement cell;
            XElement cell2;
            var Dbcompany = (from x in db.Company select x).Distinct().ToList();
            foreach (var company in Dbcompany)
            {
                row = new XElement("row", new XAttribute("id", company.company_id));
                cell = new XElement("cell", new XText(company.company_name));
                cell2 = new XElement("cell", 0);
                row.Add(cell);
                row.Add(cell2);
                rows.Add(row);
            }
            doc.Add(rows);
            return Json(doc.ToString(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region update Classes
        public JsonResult updateClasses(string old_className, string new_className)
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
        #region update Products
        public void updateProducts(string new_Name, int row_id, int product_column)
        {
            Products productObj = (from x in db.Products
                                   where x.product_id == row_id
                                   select x).First();
            switch (product_column)
            {
                case 1:
                    productObj.product_name = new_Name;
                    break;
                case 2:
                    int class_id=Int32.Parse(new_Name);
                    productObj.class_id = class_id;
                    break;
                case 3:
                    int type_id = Int32.Parse(new_Name);
                    productObj.type_id = type_id;
                    break;
                case 4:
                    int company_id = Int32.Parse(new_Name);
                    productObj.company_id = company_id;
                    break;
                case 5:
                    int attribute_id = Int32.Parse(new_Name);
                    productObj.attribute_id = attribute_id;
                    break;
                case 6:
                    productObj.product_description = new_Name;
                    break;
                default:
                    break;
            }
            db.SaveChanges();
        }
        #endregion

        #region delete Classes
        public JsonResult deleteClasses(string objClasses)
        {
            int[] classes_id = new int[objClasses.Split(',').Length];
            for (int i = 0; i < objClasses.Split(',').Length; i++)
            {
                classes_id[i] = Convert.ToInt32(objClasses.Split(',')[i]);
            }
            var x = (from p in db.Class where classes_id.Contains(p.class_id) select p).ToList();
            foreach (var a in x)
            {
                db.Class.Remove(a);
            }
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region delete Types
        public JsonResult deleteTypes(string objTypes)
        {
            int[] type_id = new int[objTypes.Split(',').Length];
            for (int i = 0; i < objTypes.Split(',').Length; i++)
            {
                type_id[i] = Convert.ToInt32(objTypes.Split(',')[i]);
            }
            var x = (from p in db.Type where type_id.Contains(p.type_id) select p).ToList();
            foreach (var a in x)
            {
                db.Type.Remove(a);
            }
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region delete Attributes
        public JsonResult deleteAttributes(string objAttributes)
        {
            int[] attribute_id = new int[objAttributes.Split(',').Length];
            for (int i = 0; i < objAttributes.Split(',').Length; i++)
            {
                attribute_id[i] = Convert.ToInt32(objAttributes.Split(',')[i]);
            }
            var x = (from p in db.Attributes where attribute_id.Contains(p.attribute_id) select p).ToList();
            foreach (var a in x)
            {
                db.Attributes.Remove(a);
            }
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region delete Companies
        public JsonResult deleteCompanies(string objCompanies)
        {
            int[] company_id = new int[objCompanies.Split(',').Length];
            for (int i = 0; i < objCompanies.Split(',').Length; i++)
            {
                company_id[i] = Convert.ToInt32(objCompanies.Split(',')[i]);
            }
            var x = (from p in db.Company where company_id.Contains(p.company_id) select p).ToList();
            foreach (var a in x)
            {
                db.Company.Remove(a);
            }
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region delete Products
        public JsonResult deleteProducts(string objProducts)
        {
            int rowLenght = objProducts.Split(',').Length;
            int[] product_id = new int[rowLenght];
            for (int i = 0; i < objProducts.Split(',').Length; i++)
            {
                product_id[i] = Convert.ToInt32(objProducts.Split(',')[i]);//get all checked row id's
            }
            var products = (from p in db.Products where product_id.Contains(p.product_id) select p).ToList();//select * for all rows
            
            int count_picture_url;
            //get all product features and then chech their dependencies / if the dependence show only once into the product table, then that dependence should be removed
            foreach (var a in products)
            {
                count_picture_url = (from product in db.Products where product.product_image == a.product_image select product).Take(2).Count();
               
                if (count_picture_url == 1)
                {
                    if (System.IO.File.Exists(Server.MapPath("~" + a.product_image)))
                    {
                        System.IO.File.Delete(Server.MapPath("~" + a.product_image));
                    }
                }
                db.Products.Remove(a);
            }
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult uploadPhoto_con(HttpPostedFileBase file, string id)
        {
            bool cvrci=false;
            int productID = Int32.Parse(id);
            Products productObj = (from x in db.Products
                                   where x.product_id == productID
                                   select x).First();
          
            if (!Directory.Exists(Server.MapPath("~/Content/companies/" + productObj.company_id)))
            {
                Directory.CreateDirectory(Server.MapPath("~/Content/companies/" + productObj.company_id));
            }
            if (file != null)
            {
                var fileName = DateTime.Now.Ticks.ToString() + Path.GetExtension(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/companies/" + productObj.company_id + "/"), fileName);
                file.SaveAs(path);
                productObj.product_image = "/Content/companies/" + productObj.company_id + "/" + fileName;
                cvrci = true;
            }
            db.SaveChanges();
            return Json(cvrci, JsonRequestBehavior.AllowGet);
        }

    }
}
