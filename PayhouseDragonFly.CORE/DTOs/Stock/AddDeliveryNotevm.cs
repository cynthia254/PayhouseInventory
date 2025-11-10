using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class AddDeliveryNotevm
    {
        public string DeliveryNumber { get; set; }
        public int ItemID { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int BatchQuantity { get; set; }
        public string AirWayBillNumber { get; set; }
        public string MeansOfDelivery { get; set; }
        public string PONumber { get; set; }
        
    }
}
