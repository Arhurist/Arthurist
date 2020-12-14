using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAuto.Models
{
    partial class Service
    {
       public double Price { get { return Convert.ToDouble(Cost) - (Convert.ToDouble(Cost) * Convert.ToDouble(Discount) / 100); } }
       public string Color { get 
            {
                if(Discount != 0)
                {
                    return "#FF9C1A";
                }
                else
                {
                    return "White";
                }
            } 
        }
      public byte[] Img { get
            {
                if (File.Exists(MainImagePath.Trim()))
                {
                    return File.ReadAllBytes(MainImagePath.Trim());
                }
                else
                {
                    return null;
                }
            } 
       }
    }
}
