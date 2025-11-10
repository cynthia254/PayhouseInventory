namespace PayhouseDragonFly.CORE.DTOs.Stock
{
    public class AddPurchaseOrderVm
    {  
        public string BrandName { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public int BuyingPrice { get; set; }

        public int TotalPurchase { get; set; }
        public string SupplierName { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
    


    }
}
