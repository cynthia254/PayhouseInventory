using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class PODetails
    {
        [Key]
        public int Id { get; set; }
        public string PONumber { get; set; }
        public DateTime PODate { get; set; }
        public string Vendor { get; set; }
        public DateTime DateCreated { get; set; }
        public string CaptureStatus { get; set; }
        public string DeliveryStatus { get; set; }
    }
}
