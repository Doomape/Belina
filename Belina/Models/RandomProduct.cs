using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Belina.Models
{
    public class RandomProduct
    {
        public string ClassName { get; set; }
        public int ProductId {get; set;}
        public string product_name { get; set; }
        public string product_image { get; set; }
        public string product_description { get; set; }
        public string type_name { get; set; }
        public string company_name { get; set; }
        public string class_name { get; set; }
        public string attribute_name { get; set; }

        public RandomProduct(string _className, int ProductId, string product_name, string product_image, string product_description, string type_name, string company_name, string class_name, string attribute_name)
        {
            this.ClassName = _className;
            this.ProductId = ProductId;
            this.product_name = product_name;
            this.product_image = product_image;
            this.product_description = product_description;
            this.type_name = type_name;
            this.company_name = company_name;
            this.class_name = class_name;
            this.attribute_name = attribute_name;
        }
    }
}