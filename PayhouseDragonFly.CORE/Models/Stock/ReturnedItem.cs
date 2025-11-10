using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class ReturnedItem
    {
        [Key]
        public int ReturnID { get; set; }
       public string ReasonForReturn { get; set; }
        public string ReturnedBy { get; set; }
        public string ReturnedCondition { get; set; }
        public int ReturnedQuantity { get; set; }
        public string SerialNumber { get; set; }
        public string IMEI1 { get; set; } = "None";
        public string IMEI2 { get; set; } = "None";
        public int IssuedId { get; set; }
        public DateTime DateReturned { get; set; } = DateTime.Now;
        public string ReturnedStatus { get; set; } = "Returned";
        public string FaultyDescription { get; set; }
        public int FaultyQuantity { get; set; } = 0;
        public string SerialFaulty { get; set; } = "None";
        public string BrandName { get; set; }
        public string ItemName { get; set; }
        public string CategoryName { get; set; }
    }
}
