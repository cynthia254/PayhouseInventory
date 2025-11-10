using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PayhouseDragonFly.CORE.DTOs.RegisterVms;
using PayhouseDragonFly.CORE.DTOs.Stock.Invoicing_vm;
using PayhouseDragonFly.CORE.Models.departments;
using PayhouseDragonFly.CORE.Models.Designation;
using PayhouseDragonFly.CORE.Models.Roles;
using PayhouseDragonFly.CORE.Models.statusTable;
using PayhouseDragonFly.CORE.Models.Stock;
using PayhouseDragonFly.CORE.Models.Stock.Invoicing;
using PayhouseDragonFly.CORE.Models.TicketRegistration;
using PayhouseDragonFly.CORE.Models.UserRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayhouseDragonFly.INFRASTRUCTURE.DataContext
{
    public class DragonFlyContext : IdentityDbContext<PayhouseDragonFlyUsers>
    {
        public DragonFlyContext(DbContextOptions<DragonFlyContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<PayhouseDragonFlyUsers>(entity =>
            {
                entity.ToTable(name: "PayhouseDragonFlyUsers");
            });
        }
        public DbSet<PayhouseDragonFlyUsers> PayhouseDragonFlyUsers { get; set; }

        public DbSet<Tickets> Tickets { get; set; }
        public DbSet<Departments> Departments { get; set; }
        public DbSet<RolesTable> RolesTable { get; set; }
        public DbSet<UserStatusTable> UserStatusTable { get; set; }
        public DbSet<Designation> Designation { get; set; }
        public DbSet<RoleClaimsTable> RoleClaimsTable { get; set; }
        public DbSet<OtherRoles> OtherRoles { get; set; }
        public DbSet<Claim_Role_Map> Claim_Role_Map { get; set; }
        public DbSet<AddBrand> AddBrand { get; set; }
        public DbSet<AddItem> AddItem { get; set; }
        public DbSet<AddStock> AddStock { get; set; }
        public DbSet<AddCustomer> Customer { get; set; }
        public DbSet<AddSupplier> Suppliers { get; set; }
        public DbSet<AddSalesOrder> Sales { get; set; }
        public DbSet<AddPurchaseOrder> Purchases { get; set; }
        public DbSet<purchaseStatus> PurchaseStatusTable { get; set; }
        public DbSet<AddReturnedStatus> ReturnedStatusTable { get; set; }
        public DbSet<AddReturnedStock> ReturnedStock { get; set; }
        public DbSet<AddCategory> Category { get; set; }
        public DbSet<lpoNo> LPO_No { get; set; }
        public DbSet<InvoiceNo> InvoiceNo { get; set; }
        public DbSet<StockIn> StockIn { get; set; }
        public DbSet<InvoiceLines> InvoiceLines { get; set; }
        public DbSet<InvoiceLinesDetails> InvoiceLinesDetails { get; set; }
        public DbSet<AddProductDetails> AddProductDetails { get; set; }
        //public DbSet<AddBatchDetail> AddBatchDetail { get; set; }
        //public DbSet<AddPart> SparePart { get; set; }
        public DbSet<Invoice_Item_Quantity> Invoice_Item_Quantity { get; set;}
        public DbSet<Item_Numbering_Stock> Item_Numbering_Stock { get; set; }
       // public DbSet<AddSparePart> AddSparePart { get; set; }
       public DbSet<AddPart> AddPart { get; set; }
        public DbSet<RequisitionApplication> RequisitionApplication { get; set; }
        public DbSet<AddSpareParts> AddSpareParts { get; set; }
        public DbSet<ProductNumbering> ProductNumbering { get; set; }
        public DbSet<PODetails> PODetails { get; set; }
        public DbSet<UploadPOItem> UploadPOItem { get; set; }
        public DbSet<UploadPOFile> UploadPOFile { get; set; }
        public DbSet<PurchaseOrderss> PurchaseOrderss { get; set; }
        public DbSet<AdjustStock> AdjustStock { get; set; }
      // public DbSet<UploadPOFiles> UploadPOFiles { get; set; }
        public DbSet<RequisitionForm> RequisitionForm { get; set; }
        public DbSet<ApplyRequistionForm> ApplyRequistionForm { get; set; }
        public DbSet<IssueProcess> IssueProcess { get; set; }
        public DbSet<SelectSerial> SelectSerial { get; set; }
        public DbSet<AddDeliveryNote> AddDeliveryNote { get; set; }
        public DbSet<BatchNo> BatchNumber { get; set; }
        public DbSet<StockAdjustment> StockAdjustment { get; set; }
        public DbSet<ApprovalBatch> ApprovalBatch { get; set; }
        public DbSet<POApproval> POApproval { get; set; }
        public DbSet<ApprovalPODelivery> ApprovalPODelivery { get; set; }
        public DbSet<OrderNo> OrderNo { get; set; }
        public DbSet<selectedSetialNumber> selectedSetialNumber { get; set; }
        public DbSet<ReturnedItem> ReturnedItem { get; set; }
        public DbSet<ApprovalRetun> ApprovalRetun { get; set; }
        public DbSet<PriceTable> PriceRecord { get; set; }
        public DbSet<ApplyRequisitionItem> ApplyRequisitionItem { get; set; }
        public DbSet<MonthlyData> MonthlyData { get; set; }
        public DbSet<EditHistory> EditHistory { get; set; }
    }
}
