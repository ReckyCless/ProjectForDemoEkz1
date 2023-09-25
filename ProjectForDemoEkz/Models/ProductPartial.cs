using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectForDemoEkz.Models
{
    public partial class Product
    {
        public string TitleAndTypeInText
        {
            get
            {
                return Title + " | " + ProductType.Title;
            }
        }
        public string ImagePathInText
        {
            get
            {
                string directory = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\"), @"Resources"));
                if (Image != null && Image != "")
                {
                    return directory + Image;
                }
                else
                {
                    return directory + @"\products\picture.png";
                }
            }
        }
        public string PriceInText
        {
            get
            {
                return MinCostForAgent.ToString("N2") + " ₽";
            }
        }
        public string ListOfMaterialsInText
        {
            get
            {
                List<Product> products = App.Context.Product.ToList();
                string materialNames = "";
                var materialsList = App.Context.ProductMaterial.ToList().Where(u => u.ProductID == ID);
                foreach (var material in materialsList)
                {
                    materialNames += App.Context.Material.ToList().Where(u => u.ID == material.MaterialID).FirstOrDefault().Title + ", ";
                }
                if (materialNames.Length < 2)
                {
                    return "Материалов - НЕТ!";
                }
                else
                {
                    return materialNames.Remove(materialNames.Length - 2); // Отрезаем последнюю запятую
                }
            }
        }
    }
}
