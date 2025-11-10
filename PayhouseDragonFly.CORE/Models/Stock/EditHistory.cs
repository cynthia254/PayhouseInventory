using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.CORE.Models.Stock
{
    public class EditHistory
    {
        [Key]
        public int EditRequisitionItemHistory { get; set; }

        // Foreign Key to the Requisition Item
        public int RequisitionItemID { get; set; }
        public string EditedBy { get; set; }

        public string FieldName { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public DateTime DateEdited { get; set; } = DateTime.Now;
    }
}
