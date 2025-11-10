using DocumentFormat.OpenXml.Office2010.Excel;
using EnumsNET;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayhouseDragonFly.CORE.ConnectorClasses.Response;
using PayhouseDragonFly.CORE.ConnectorClasses.Response.BseResponse;
using PayhouseDragonFly.CORE.ConnectorClasses.Response.StockResponse;
using PayhouseDragonFly.CORE.DTOs.Stock;
using PayhouseDragonFly.CORE.DTOs.Stock.Invoicing_vm;
using PayhouseDragonFly.CORE.Models.Stock;
using PayhouseDragonFly.INFRASTRUCTURE.Services.ExtraServices.RoleChecker;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IExtraServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IStockServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.RoleServices;

namespace PayhouseDragonFly.API.Controllers.Stock
{
    [Route("api/[controller]", Name = "Stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockServices _stockServices;

        private readonly ILoggeinUserServices _loggeinuser;

        private readonly IRoleChecker _rolechecker;
        private readonly IRoleServices _roleservices;

        public StockController(IStockServices stockServices, ILoggeinUserServices loggeinuser,
            IRoleChecker rolechecker,
            IRoleServices roleservices)
        {
            _stockServices = stockServices;

            _rolechecker = rolechecker;
            _roleservices = roleservices;
            _loggeinuser = loggeinuser;
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddBrand")]
        public async Task<StockResponse> AddBrand(AddBrandvm addBrandvm)
        {
            var roleclaimname = "AddBrand";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
           var master_rolechecker_response= await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.AddBrand(addBrandvm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }
              
             
                
           
            

        }
        [Authorize(AuthenticationSchemes = "Bearer")]

        [HttpGet]
        [Route("GetAllBrands")]
        public async Task<StockResponse> GetAllBrand()
        {
            var roleclaimname = "ViewBrands";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.GetAllBrand();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddItem")]
        public async Task<StockResponse> AddItem(AddItemvm addItemvm)
        {
            var roleclaimname = "AddItems";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.AddItem(addItemvm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetSerialByOrderNo")]
        public async Task<StockResponse> GetSerialNumberByIssueId(int issueid)
        {
            return await _stockServices.GetSerialNumberByIssueId(issueid);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetSerialByItemID")]
        public async Task<StockResponse> GetSerialByItemId(int ItemId)
        {
            return await _stockServices.GetSerialByItemId(ItemId);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllItems")]
        public async Task<StockResponse> GetAllItems()
        {
            var roleclaimname = "ViewItems";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.GetAllItems();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }


        }

        [HttpPost]
        [Route("AddStock")]
        public async Task<StockResponse> AddStock(AddStockvm addStockvm)
        {
            return await _stockServices.AddStock(addStockvm);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetIssuedItem")]
        public async Task<StockResponse> GetNameToUse()
        {
            return await _stockServices.GetNameToUse();

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddCustomer")]
        public async Task<StockResponse> AddCustomer(AddCustomervm addCustomervm)
        {
            var roleclaimname = "AddCustomer";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.AddCustomer(addCustomervm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }


        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllCustomers")]
        public async Task<StockResponse> GetAllCustomers()
        {
            var roleclaimname = "ViewCustomers";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.GetAllCustomers();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }


        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllStock")]
        public async Task<StockResponse> GetAllStock()
        {
            return await _stockServices.GetAllStock();

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddSupplier")]
        public async Task<StockResponse> AddSupplier(AddSupplierVm addSupplierVm)
        {
            var roleclaimname = "AddSupplier";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.AddSupplier(addSupplierVm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }


        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllSuppliers")]
        public async Task<StockResponse> GetAllSuppliers()
        {
            var roleclaimname = "ViewSuppliers";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.GetAllSuppliers();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }


        }
        [HttpPost]
        [Route("AddSales")]
        public async Task<StockResponse> AddSales(AddSalesOrdersVm addSalesOrdersVm)
        {
            return await _stockServices.AddSales(addSalesOrdersVm);

        }

        [HttpGet]
        [Route("GetAllSales")]
        public async Task<StockResponse> GetAllSales()
        {
            return await _stockServices.GetAllSales();

        }

        [HttpPost]
        [Route("AddPurchases")]
        public async Task<StockResponse> AddPurchase(AddPurchaseOrderVm addPurchaseOrderVm)
        {
            return await _stockServices.AddPurchase(addPurchaseOrderVm);
        }

        [HttpGet]
        [Route("GetAllPurchases")]
        public async Task<StockResponse> GetAllPurchases()
        {
            return await _stockServices.GetAllPurchases();
        }

        [HttpPost]
        [Route("UpdateStockQuantity")]
        public async Task<StockResponse> UpdateStockQuantity(int itemid, int quantityadded)
        {
            return await _stockServices.UpdateStockQuantity(itemid, quantityadded);

        }
        [HttpPost]
        [Route("GetStockByName")]
        public async Task<StockResponse> GetItemByName(string itemname)
        {
            return await _stockServices.GetItemByName(itemname);
        }

        [HttpPost]
        [Route("GetStockById")]
        public async Task<StockResponse> GetItemById(int itemid)
        {
            return await _stockServices.GetItemById(itemid);
        }
        [HttpPost]
        [Route("GetSerialById")]
        public async Task<StockResponse> GetSelectedSerialsByID(int ID)
        {
            return await _stockServices.GetSelectedSerialsByID(ID);
        }

        [HttpPost]
        [Route("GetIssueByID")]
        public async Task<StockResponse> GetFormIssuedByID(int Issueid)
        {
            return await _stockServices.GetFormIssuedByID(Issueid);
        }
        [HttpPost]
        [Route("GetPurchaseById")]
        public async Task<StockResponse> GetPurchaseById(int purchaseId)
        {
            return await _stockServices.GetPurchaseById(purchaseId);
        }
        [HttpPost]
        [Route("ChangePurchaseStatus")]
        public async Task<StockResponse> ChangePurchaseStatus(PurchaseStatusvm purchaseStatusvm)
        {
            return await _stockServices.ChangePurchaseStatus(purchaseStatusvm);

        }
        [HttpPost]
        [Route("GetSalesbyId")]
        public async Task<StockResponse> GetSalesbyId(int salesId)
        {
            return await _stockServices.GetSalesbyId(salesId);
        }
        [HttpPost]
        [Route("ChangeSalesStatus")]
        public async Task<StockResponse> ChangeSalesStatus(ChangeSalesStatusvm changeSalesStatusvm)
        {
            return await _stockServices.ChangeSalesStatus(changeSalesStatusvm);

        }

        [HttpPost]
        [Route("AddReturnedStatus")]
        public async Task<StockResponse> AddReturnedStatus(AddReturnedStatusvm addReturnedStatusvm)
        {
            return await _stockServices.AddReturnedStatus(addReturnedStatusvm);
        }
        [HttpPost]
        [Route("GetSelectedSerialIssue")]
        public async Task<StockResponse> GetSelectedSerialsToIssueByID(int ID)
        {
            return await _stockServices.GetSelectedSerialsToIssueByID(ID);
        }
        [HttpPost]
        [Route("GetDnoteItemsByReference")]
        public async Task<StockResponse> GetDnoteItemsByReference(string referenceNumber)
        {
            return await _stockServices.GetDnoteItemsByReference(referenceNumber);
        }
        [HttpPost]
        [Route("GetSelectedSerialByNo")]
        public async Task<StockResponse> GetSelectedSerialsByIssueNo(int issuedNo)
        {
            return await _stockServices.GetSelectedSerialsByIssueNo(issuedNo);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllReturnedStatus")]
        public async Task<StockResponse> GetAllReturnedStatus()
        {
            return await _stockServices.GetAllReturnedStatus();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("SelectSerialNumberToissue")]
        public async Task<StockResponse> SelectSerialNumber(SelectedSerialvm addBrandvm)
        {
            return await _stockServices.SelectSerialNumber(addBrandvm);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddReturnedStock")]
        public async Task<StockResponse> AddReturnedStock(AddReturnedStockvm addReturnedStockvm)
        {
            return await _stockServices.AddReturnedStock(addReturnedStockvm);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllReturnedStock")]
        public async Task<StockResponse> GetAllReturnedStock()
        {
            return await _stockServices.GetAllReturnedStock();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetRequisitionbyClientName")]
        public async Task<StockResponse> GetAllRequisitionApplicationbyClientName(string clientName)
        {
            return await _stockServices.GetAllRequisitionApplicationbyClientName(clientName);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetCustomerByName")]
        public async Task<StockResponse> GetCustomerByName(string clientName)
        {
            return await _stockServices.GetCustomerByName(clientName);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("SearchStock")]
        public async Task<StockResponse> SearchForStock(string search_query)
        {
            return await _stockServices.SearchForStock(search_query);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("SearchStockIn")]
        public async Task<StockResponse> SearchForStockIn(string search_query)
        {
            return await _stockServices.SearchForStockIn(search_query);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("SearchForPO")]
        public async Task<StockResponse> SearchForPO(string search_query)
        {
            return await _stockServices.SearchForPO(search_query);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("SearchStockOut")]
        public async Task<StockResponse> SearchForStockOut(string search_query)
        {
            return await _stockServices.SearchForStockOut(search_query);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("SearchCustomer")]
        public async Task<StockResponse> SearchForCustomer(string search_query)
        {
            return await _stockServices.SearchForCustomer(search_query);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("SearchSupplier")]
        public async Task<StockResponse> SearchForSupplier(string search_query)
        {
            return await _stockServices.SearchForSupplier(search_query);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("DeleteStockOut")]
        public async Task<StockResponse> DeleteStockOut(int salesId)
        {
            return await _stockServices.DeleteStockOut(salesId);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetReturnedStockByIssuedId")]
        public async Task<StockResponse> GetReturnedDataById(int Id)
        {
            return await _stockServices.GetReturnedDataById(Id);
        }
        [HttpPost]
        [Route("EditSparePart")]
        public async Task<StockResponse> EditSparePart(editPartVm editPartVm)
        {
            return await _stockServices.EditSparePart(editPartVm);
        }
        [HttpPost]
        [Route("DeleteSparePart")]
        public async Task<StockResponse> DeleteSparePart(int partId)
        {
            return await _stockServices.DeleteSparePart(partId);
        }

        [HttpPost]
        [Route("EditStockOut")]
        public async Task<StockResponse> EditSales(editSalesvm salesvm)
        {
            return await _stockServices.EditSales(salesvm);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("EditCustomer")]
        public async Task<StockResponse> EditCustomer(EditCustomervm editCustomervm)
        {
            var roleclaimname = "EditCustomer";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {
                return await _stockServices.EditCustomer(editCustomervm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetCustomerByID")]
        public async Task<StockResponse> GetCustomerById(int customerId)
        {
            return await _stockServices.GetCustomerById(customerId);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetSupplierbyId")]
        public async Task<StockResponse> GetSupplierById(int supplierId)
        {
            return await _stockServices.GetSupplierById(supplierId);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("EditSupplier")]
        public async Task<StockResponse> EditSupplier(editSuppliervm suppliervm)
        {
            var roleclaimname = "EditSupplier";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.EditSupplier(suppliervm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetBrandById")]
        public async Task<StockResponse> GetBrandById(int BrandId)
        {
            return await _stockServices.GetBrandById(BrandId);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetItemById")]
        public async Task<StockResponse> GetItemsById(int ItemId)
        {
            return await _stockServices.GetItemsById(ItemId);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetStockItemByItemName")]
        public async Task<StockResponse> GetStockItemByName(string itemname, string referenceNumber)
        {
            return await _stockServices.GetStockItemByName(itemname,referenceNumber);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("EditBrand")]
        public async Task<StockResponse> EditBrand(EditBrandvm editBrandvm)
        {
            var roleclaimname = "EditBrand";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.EditBrand(editBrandvm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("EditItem")]
        public async Task<StockResponse> EditItem(EditItemvm editItemvm)
        {
            var roleclaimname = "EditItem";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.EditItem(editItemvm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("EditStockIn")]
        public async Task<StockResponse> EditStockIn(EditPurchasevm editPurchasevm)
        {
            return await _stockServices.EditStockIn(editPurchasevm);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GenerateExcel")]
        public async Task<StockResponse> GenerateExcel()
        {
            return await _stockServices.GenerateExcel();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddCategory")]
        public async Task<StockResponse> AddCategory(AddCategoryvm addCategoryvm)
        {
            var roleclaimname = "AddCategory";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.AddCategory(addCategoryvm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllCategory")]
        public async Task<StockResponse> GetAllCategory()
        {
            var roleclaimname = "ViewCategories";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.GetAllCategory();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddInvoiceDetails")]
        public async Task<StockResponse> AddInvoiceDetails(StockInvm stockInvm)
        {
            return await _stockServices.AddInvoiceDetails(stockInvm);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetInvoiceDetails")]
        public async Task<StockResponse> GetInvoiceDetails()
        {
            return await _stockServices.GetInvoiceDetails();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddBatchDetails")]
        public async Task<StockResponse> AddBatchDetails(AddBatchDetailsvm addBatchDetailsvm)
        {
            var roleclaimname = "AddPOItems";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.AddBatchDetails(addBatchDetailsvm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetInvoiceLines")]
        public async Task<StockResponse> GetInvoiceLines()
        {
            return await _stockServices.GetInvoiceLines();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddProductDetails")]
        public async Task<StockResponse> AddProductDetails(AddProductDetailvm addProductDetailvm)
        {
            var roleclaimname = "AddSerialNumber";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.AddProductDetails(addProductDetailvm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetProductDetails")]
        public async Task<StockResponse> GetProductDetails()
        {
            var roleclaimname = "GetSerialNumbers";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.GetProductDetails();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetInvoiceByNumber")]
        public async Task<StockResponse> GetInvoiceByNumber(string InvoiceNumber)
        {
            return await _stockServices.GetInvoiceByNumber(InvoiceNumber);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddParts")]
        public async Task<StockResponse> AddSparePart(AddPartvm addPartvm)
        {
            return await _stockServices.AddSparePart(addPartvm);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllParts")]
        public async Task<StockResponse> GetAllParts()
        {
            return await _stockServices.GetAllParts();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddSpareParts")]
        public async Task<StockResponse> AddPartsSpare(AddSparePartvm addSparePartvm)
        {
            return await _stockServices.AddPartsSpare(addSparePartvm);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllSpareParts")]
        public async Task<StockResponse> GetAllSpareParts()
        {
            return await _stockServices.GetAllSpareParts();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddInvoiceQuantity")]
        public async Task<StockResponse> Add_Invoice_Item_Quantity(invoice_item_quantity_vm vm)
        {
            return await _stockServices.Add_Invoice_Item_Quantity(vm);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetGeneratedref")]
        public async Task<string> GetGeneratedref()
        {
            return await _stockServices.GetGeneratedref();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetInvoiceLinesByInvoiceNumber")]
        public async Task<StockResponse> GetInvoiceLinByNumber(string invoiceNumber)
        {
            return await _stockServices.GetInvoiceLinByNumber(invoiceNumber);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetInvoiceItemByID")]
        public async Task<StockResponse> GetInvoiceItemByID(int invoicelineId)
        {
            return await _stockServices.GetInvoiceItemByID(invoicelineId);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetProductDetailsbyid")]
        public async Task<StockResponse> GetProductDetailsbyid(string BatchNumber)
        {
            return await _stockServices.GetProductDetailsbyid(BatchNumber);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetBatchByPO")]
        public async Task<StockResponse> GetBatchByPONumber(string poNumber)
        {
            return await _stockServices.GetBatchByPONumber(poNumber);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetProductByBatch")]
        public async Task<StockResponse> GetProductDetailsByBatchNumber(string BatchNumber)
        {
            return await _stockServices.GetProductDetailsByBatchNumber(BatchNumber);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetSerialNumberByBatch")]
        public async Task<StockResponse> GetSerialNumberByBatch(string BatchNumber)
        {
            return await _stockServices.GetSerialNumberByBatch(BatchNumber);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetProductNumbering")]
        public async Task<StockResponse> GetProduct_Numbers_ByReference(string reference)
        {
            return await _stockServices.GetProduct_Numbers_ByReference(reference);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetProductlinebyid")]
        public async Task<StockResponse> GetProduvctLineyId(int product_line_id)
        {
            return await _stockServices.GetProduvctLineyId(product_line_id);
        }

        public class SerialNumberRequest
        {
            public string SerialNumber { get; set; }
        }

        [HttpPost]
        [Route("GetSerialNumberDetails")]
        public async Task<BaseResponse> GetProductDetailsBySerialNumber([FromBody] SerialNumberRequest request)
        {
            return await _stockServices.GetProductDetailsBySerialNumber(request.SerialNumber);
        }

        [HttpGet]
        [Route("GetAllSerialNumbers")]
        public async Task<StockResponse> GetAllSerialNumbers()
        {
            return await _stockServices.GetAllSerialNumbers();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("SearchInvoice")]
        public async Task<StockResponse> SearchForInvoice(string search_query)
        {
            return await _stockServices.SearchForInvoice(search_query);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("SearchItem")]
        public async Task<StockResponse> SearchForItem(string search_query)
        {
            return await _stockServices.SearchForItem(search_query);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GettingItemByBrandName")]
        public async Task<StockResponse> GetItemsbyBrandName(string BrandName)
        {
            return await _stockServices.GetItemsbyBrandName(BrandName);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("SearchInvoiceLines")]
        public async Task<StockResponse> SearchForInvoiceLines(string search_query)
        {
            return await _stockServices.SearchForInvoiceLines(search_query);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("upload_bulk")]
        public async Task<StockResponse> UploadData([FromBody] List<uploadDatavm> data)
        {
            return await _stockServices.UploadData(data);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("uploading")]
        public async Task<StockResponse> UploadingData(IFormFile file,string BatchNumber, int batchID)
        {
            var roleclaimname = "AddSerialNumber";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.UploadingData(file, BatchNumber, batchID);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("EditSerialNumber")]
        public async Task<StockResponse> EditSerialNumber(EditSerialNumbervm editSerialNumbervm)
        {
            var roleclaimname = "EditSerialNumber";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.EditSerialNumber(editSerialNumbervm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("MarkRequisitionIssued")]
        public async Task<StockResponse> MarkRequisitionAsIssuedAsync(int requisitionItemId, string issuedBy)
        {
           

                return await _stockServices.MarkRequisitionAsIssuedAsync(requisitionItemId,issuedBy);

            
           
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetSerialNumberByReferenceNumberAndItemName")]
        public async Task<StockResponse> GetSerialByReferenceNumberAndItemName(string referenceNumber, string brandName, string itemName)
        {


            return await _stockServices.GetSerialByReferenceNumberAndItemName(referenceNumber, brandName,itemName);



        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetSerialByNumber")]
        public async Task<StockResponse> GetSerialByPONumber(string poNumber)
        {
            return await _stockServices.GetSerialByPONumber(poNumber);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetSerialNumber")]
        public async Task<StockResponse> GetSerialNumberbyid(int itemID)
        {
            return await _stockServices.GetSerialNumberbyid(itemID);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("ScannedData")]
        public async Task<StockResponse> PostScannedData([FromBody] ScannedDataModel data)
        {
            return await _stockServices.PostScannedData(data);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("UploadPDFFile")]
        public async Task<StockResponse> Upload([FromBody] PODetailsvm pODetailsvm)
        {
            return await _stockServices.Upload(pODetailsvm);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("UploadPODetails")]
        public async Task<StockResponse> UploadingPODetails(PODetailsvm pODetailsvm)
        {
            return await _stockServices.UploadingPODetails(pODetailsvm);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllPOS")]
        public async Task<StockResponse> GetAllPOs()
        {
            var roleclaimname = "ViewPOs";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.GetAllPOs();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllPurchaseOrders")]
        public async Task<StockResponse> GetAllPurchaseOrders()
        {
            var roleclaimname = "ViewPOs";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.GetAllPurchaseOrders();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("UploadPOItems")]
        public async Task<StockResponse> UploadingPOItems([FromBody] DataWrapper dataWrapper)
        {
            return await _stockServices.UploadingPOItems(dataWrapper);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("Uploading>>>")]
        public async Task UploadingItemsPO(IFormFile file, string PONumber)
        {
            await _stockServices.UploadingItemsPO(file, PONumber);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetItemsByPO")]
        public async Task<StockResponse> GetItemsByPO(string PONumber)
        {
            return await _stockServices.GetItemsByPO(PONumber);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("UploadingPO>>>>>")]
        public async Task<StockResponse> UploadingPO(IFormFile file)
        {
            return await _stockServices.UploadingPO(file);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllPOSDetails")]
        public async Task<StockResponse> GetAllPOSDetails()
        {
            return await _stockServices.GetAllPOSDetails();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GettingItemInPO")]
        public async Task<StockResponse> GetItemsByPOS(string PONumber)
        {
            return await _stockServices.GetItemsByPOS(PONumber);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddingPurchaseOrdersDetails")]
        public async Task<StockResponse> AddingPurchaseOrdersDetails(PurchaseOrderssvm purchaseOrderssvm)
        {
            var roleclaimname = "AddPO";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.AddPurchaseOrdersDetails(purchaseOrderssvm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddingPOItemLines")]
        public async Task<StockResponse> AddPOItemLines(AddPOItemLinesvm addBatchDetailsvm)
        {
            var roleclaimname = "AddPOItems";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.AddPOItemLines(addBatchDetailsvm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("MarkingPOComplete")]
        public async Task<StockResponse> MarkPOComplete(string PONumber)
        {
            return await _stockServices.MarkPOComplete(PONumber);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("MarkingPOLinesComplete")]
        public async Task<StockResponse> MarkPOLinesAsComplete(string PONumber)
        {
            return await _stockServices.MarkPOLinesAsComplete(PONumber);
            
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetSerialByItemName")]
        public async Task<StockResponse> GetSerialByItemName(string brandName, string itemName)
        {
            return await _stockServices.GetSerialByItemName(brandName,itemName);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetIssuedBySerialNumber")]
        public async Task<StockResponse> GetIsusedSerialNumberData(string brandName, string itemName)
        {
            return await _stockServices.GetIsusedSerialNumberData(brandName, itemName);
        }
        [HttpPost]
        [Route("GettingSerialUnderItem")]
        public async Task<StockResponse> GetItemByPONumber(string PONumber)
        {
            return await _stockServices.GetItemByPONumber(PONumber);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllPurchaseOrderss")]
        public async Task<StockResponse> GetAllPurchaseOrderDetails()
        {
            var roleclaimname = "ViewPOs";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);
            
            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.GetAllPurchaseOrderDetails();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllProducts")]
        public async Task<StockResponse> GetAllProducts()
        {
            var roleclaimname = "ViewPOs";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.GetAllProducts();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("BatchAndDeliveryReport")]
        public async Task<StockResponse> BatchAndDeliveryReport()
        {
            var roleclaimname = "ViewPOs";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.BatchAndDeliveryReport();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("IssueReport")]
        public async Task<StockResponse> GetIssueReport()
        {
            var roleclaimname = "ViewPOs";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.GetIssueReport();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AdjustStock")]
        public async Task<StockResponse> AdjustStock(AdjustStockvm adjustStockvm)
        {
            return await _stockServices.AdjustStock(adjustStockvm);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetPOItemsbyID")]
        public async Task<StockResponse> GetPOItemsByID(int ItemId)
        {
            return await _stockServices.GetPOItemsByID(ItemId);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetAdjustedStockByID")]
        public async Task<StockResponse> GetAdjustedStockById(string batchNumber)
        {
            return await _stockServices.GetAdjustedStockById(batchNumber);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetAllMonthlyRecord")]
        public async Task<StockResponse> GetMonthlyRecord(int Year)
        {
            return await _stockServices.GetMonthlyRecord(Year);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetDeliveredItemPerMonth")]
        public async Task<List<AddDeliveryNote>> GetDeliveredItemsByMonthAndYear(int year, int month)
        {
            return await _stockServices.GetDeliveredItemsByMonthAndYear(year,month);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetDamagedItemsPerMonth")]
        public async Task<List<StockAdjustment>> GetDamagedItemsByMonthAndYear(int year, int month)
        {
            return await _stockServices.GetDamagedItemsByMonthAndYear(year, month);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetIssuedItemPerMonth")]
        public async Task<List<SelectSerial>> GetIssuedItemsByMonthandYear(int year, int month)
        {
            return await _stockServices.GetIssuedItemsByMonthandYear(year, month);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllItemsStock")]
        public async Task<StockResponse> GetAllItemStock()
        {
            return await _stockServices.GetAllItemStock();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetIssuedSerialNumberItems")]
        public async Task<StockResponse> GetIssuedSerialNumber()
        {
            return await _stockServices.GetIssuedSerialNumber();
        }

        public class ReferenceRequest
        {
            public string ReferenceNumber { get; set; }
        }

        [HttpPost]
        [Route("GetItemByReferenceNumber")]
        public async Task<BaseResponse> GetItemByReferenceNumber([FromBody] ReferenceRequest request)
        {
            return await _stockServices.GetItemByReferenceNumber(request.ReferenceNumber);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetItemByRequisitionItemID")]
        public async Task<StockResponse> GetItemsByRequiestItemID(int requisitionItemID)
        {
            return await _stockServices.GetItemsByRequiestItemID(requisitionItemID);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("MarkRequisitionComplete")]
        public async Task<StockResponse> MarkRequisitionComplete(string referenceNumber)
        {
            return await _stockServices.MarkRequisitionComplete(referenceNumber);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("EditRequisitionItem")]
        public async Task<StockResponse> EditRequisitionItem(EditRequisitionItemDto dto)
        {
            return await _stockServices.EditRequisitionItem(dto);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("ApplyRequisition")]
        public async Task<StockResponse> ApplyRequisition(AddRequisition addRequisition)
        {
            return await _stockServices.ApplyRequisition(addRequisition);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("ApplyingRequisitionForm")]
        public async Task<StockResponse> ApplyRequisitionForm(ApplyRequistionvm addRequisition)
        {
            return await _stockServices.ApplyRequisitionForm(addRequisition);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("ApplicationStatus")]
        public async Task<StockResponse> ApplicationStatus(ApprovalProcessvm approvalProcessvm)
        {
            var loggedinuser = _loggeinuser.LoggedInUser().Result;

            if (loggedinuser.Checker==true)
            {


                return await _stockServices.ApplicationStatus(approvalProcessvm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("ApprovalReturn")]
        public async Task<StockResponse> ReturnReview(ApprovalReturnedStockvm approvalProcessvm)
        {
            return await _stockServices.ReturnReview(approvalProcessvm);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllRequisition")]
        public async Task<StockResponse> GetAllRequisitionApplication()
        {
            return await _stockServices.GetAllRequisitionApplication();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetFormByID")]
        public async Task<StockResponse> GetRequisitionbyId(int Id)
        {
            return await _stockServices.GetRequisitionbyId(Id);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("IssueProcess")]
        public async Task<StockResponse> IssueProcess(int id)
        {
            var roleclaimname = "IssueStock";
          
            var loggedinuser = await _loggeinuser.LoggedInUser();
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.IssueProcess(id);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetFormbyUserEmail")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<StockResponse> GetRequisitionByEmail()
        {
            return await _stockServices.GetRequisitionByEmail();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetFormByReferenceNumber")]
        public async Task<StockResponse> GetRequisitionByReferenceNumber(string referenceNumber)
        {
            return await _stockServices.GetRequisitionByReferenceNumber(referenceNumber);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetFormWithStatusPending")]
        public async Task<StockResponse> GetFormByStatusPending()
        {
            var loggedinuser = await _loggeinuser.LoggedInUser();

            if (loggedinuser.Checker == true)
            {

                return await _stockServices.GetFormByStatusPending();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllItemsToBeReturned")]
        public async Task<StockResponse> GetAllItemToBeReturned()
        {
            var loggedinuser = await _loggeinuser.LoggedInUser();

            if (loggedinuser.Checker == true)
            {

                return await _stockServices.GetAllItemToBeReturned();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetFormStatusWithApproved")]

        public async Task<StockResponse> GetFormStatusApproved()
        {
            var loggedinuser = await _loggeinuser.LoggedInUser();

            if (loggedinuser.Issuer == true)
            {

                return await _stockServices.GetFormStatusApproved();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("SelectSerialNumber")]
        public async Task<StockResponse> SelectSerialNumber(SelectSerialvm selectSerialvm)
        {
            return await _stockServices.SelectSerialNumber(selectSerialvm);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetSerialByBrandAndItem")]
        public async Task<StockResponse> GetSerialByBrandAndItem(string brandName, string itemName)
        {
            return await _stockServices.GetSerialByBrandAndItem(brandName,itemName);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddPriceItemRecord")]
        public async Task<StockResponse> AddPriceRecord(PriceRecordVm priceVm)
        {
            var roleclaimname = "AddSellingPrice";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {


                return await _stockServices.AddPriceRecord(priceVm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("ActivatePriceRecord")]
        public async Task<StockResponse> ActivatePriceRecord(int itemPriceId, string activatedBy)
        {
            var roleclaimname = "ActivatePriceRecord";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.ActivatePriceRecord(itemPriceId, activatedBy);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("FlagPriceRecord")]
        public async Task<StockResponse> DeletePriceRecord(int itemPriceId, string deletedBy)
        {
            var roleclaimname = "FlagPriceRecord";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.DeletePriceRecord(itemPriceId, deletedBy);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("EditPriceRecord")]
        public async Task<StockResponse> EditPriceRecord(int itemPriceId, decimal? newSellingPrice, string newCurrency, DateTime? newEffectiveFrom, DateTime? newEffectiveTo, string updatedBy)
        {
            var roleclaimname = "EditItemPrice";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {


                return await _stockServices.EditPriceRecord(itemPriceId, newSellingPrice, newCurrency, newEffectiveFrom, newEffectiveTo, updatedBy);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetPriceHistory")]
        public async Task<StockResponse> GetPriceHistory(string itemId)
        {
            var roleclaimname = "GetItemPriceHistory";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.GetPriceHistory(itemId);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetPricePerPriceID")]
        public async Task<StockResponse> GetPriceRecordPerPriceID(int itemPriceId)
        {
            return await _stockServices.GetPriceRecordPerPriceID(itemPriceId);
        }



        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetSelectedByIssueID")]
        public async Task<StockResponse> GetSelectedSerials(int issueID)
        {
            return await _stockServices.GetSelectedSerials(issueID);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetNotIssuedSerial")]
        public async Task<StockResponse> GetSerialByIssued()
        {
            return await _stockServices.GetSerialByIssued();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetStatusIssued")]
        public async Task<StockResponse> GetFormByStatusIssued()
        {
            return await _stockServices.GetFormByStatusIssued();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddDeliveryNote")]
        public async Task<StockResponse> AddDeliveryNote(AddDeliveryNotevm addDeliveryNotevm)
        {
            var roleclaimname = "AddDeliveryData";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.AddDeliveryNote(addDeliveryNotevm);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetDeliveryByBatchNumber")]
        public async Task<StockResponse> GetBatchByItems(int itemId)
        {
            return await _stockServices.GetBatchByItems(itemId);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddRequisitionItem")]
        public async Task<StockResponse> AddRequisitionItem(AddRequisitionItemDto dto)
        {
            return await _stockServices.AddRequisitionItem(dto);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetPOLinesbyid")]
        public async Task<StockResponse> GetPOLinesNByID(int itemId)
        {
            return await _stockServices.GetPOLinesNByID(itemId);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AmmendDiscount")]
        public async Task<StockResponse> AmendDiscount(int requisitionItemId, decimal newDiscountNumerator, decimal newDiscountDenominator, string amendedBy,string Reason)

        {
            return await _stockServices.AmendDiscount(requisitionItemId,newDiscountNumerator,newDiscountDenominator,amendedBy,Reason);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("RemoveDiscount")]
        public async Task<StockResponse> RemoveDiscount(int requisitionItemId, string approver, string Reason)

        {
            return await _stockServices.RemoveDiscount(requisitionItemId, approver, Reason);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("IssueItemsAnsyc")]
        public async Task<StockResponse> IssueRequisitionItemAsync(
       int requisitionItemId,
       int quantityToDispatch,
       string issuedBy,
       string comment = null,
       List<string> selectedSerialNumbers = null)

        {
            return await _stockServices.IssueRequisitionItemAsync(requisitionItemId, quantityToDispatch,issuedBy,comment, selectedSerialNumbers);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetProductbyBatchNumber")]
        public async Task<StockResponse> GetProductbyBatchid(string BatchNumber)
        {
            return await _stockServices.GetProductbyBatchid(BatchNumber);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("StockAdjustment")]
        public async Task<StockResponse> StockAdjustment([FromBody] StockAdjustvm adjustStockvm)
        {
            return await _stockServices.StockAdjustment(adjustStockvm);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetBatchWithStatusComplete")]

        public async Task<StockResponse> GetBatchCompleteStatus()
        {
            //var roleclaimname = "CanViewBatchWithComplete";
            var loggedinuser = await _loggeinuser.LoggedInUser();
           
            if (loggedinuser.Checker==true)
            {

                return await _stockServices.GetBatchCompleteStatus();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("ApproversReview")]
        public async Task<StockResponse> ApprovalsReview(ApproveBatchvm approvalProcessvm)
        {

            return await _stockServices.ApprovalsReview(approvalProcessvm);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetItemByBatchNumber")]
        public async Task<StockResponse> GetBatchByBatchNumber(string BatchNumber)
        {
            return await _stockServices.GetBatchByBatchNumber(BatchNumber);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("ApprovalofPO")]
        public async Task<StockResponse> POApprovalReview(POApprovalvm pOApprovalvm)
        {
            return await _stockServices.POApprovalReview(pOApprovalvm);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetPOWithPendingStatus")]
        public async Task<StockResponse> GetPOByStatusPending()
        {
            var loggedinuser = await _loggeinuser.LoggedInUser();

            if (loggedinuser.Checker == true)
            {

                return await _stockServices.GetPOByStatusPending();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetPOWithStatusComplete")]
        public async Task<StockResponse> GetAllPOSWithStatusComplete()
        {
            return await _stockServices.GetAllPOSWithStatusComplete();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetPONumberbyNumber")]
        public async Task<StockResponse> GetPONumberbyNumber(string POnumber)
        {
            return await _stockServices.GetPONumberbyNumber(POnumber);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("MarkBatchAsComplete")]
        public async Task<StockResponse> MarkBatchCompplete(string BatchNumber)
        {
            return await _stockServices.MarkBatchCompplete(BatchNumber);

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetAllPOSWithStatusPending")]
        public async Task<StockResponse> GetAllPOSWithStatusPending()
        {
            var loggedinuser = await _loggeinuser.LoggedInUser();

            if (loggedinuser.Checker == true)
            {

                return await _stockServices.GetAllPOSWithStatusPending();

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }

        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("PODeliveryReview")]
        public async Task<StockResponse> PODeliveryReview(ApprovalPODeliveryvm pOApprovalvm)
        {
            return await _stockServices.PODeliveryReview(pOApprovalvm);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetItemByClient")]
        public async Task<StockResponse> GetItemByClient(string OrderNumber)
        {
           

                return await _stockServices.GetItemByClient(OrderNumber);
            
         
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("GetItemIssuedByClientName")]
        public async Task<StockResponse> GetFormByStatusIssuedByClient(string ClientName)
        {


            return await _stockServices.GetFormByStatusIssuedByClient(ClientName);


        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet]
        [Route("GetItemsToBeReordered")]
        public async Task<StockResponse> GetAllItemsReorder()
        {
            return await _stockServices.GetAllItemsReorder();
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("AddReturnedItem")]
        public async Task<StockResponse> AddReturnedStock(ReturnedStockvm returnedStockvm)
        {
            return await _stockServices.AddReturnedStock(returnedStockvm);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("ReactivatePriceRecord")]
        public async Task<StockResponse> ReactivatePriceRecord(int priceRecordId, DateTime newEffectiveFrom, DateTime? newEffectiveTo, string reactivatedBy)
        {
            return await _stockServices.ReactivatePriceRecord(priceRecordId,newEffectiveFrom,newEffectiveTo,reactivatedBy);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("RequisitionApplication")]
        public async Task<StockResponse> CreateRequisitionHeader(RequisitionDatavm header)
        {
            var roleclaimname = "ApplyRequisition";
            var loggedinuser = _loggeinuser.LoggedInUser().Result;
            var master_rolechecker_response = await _roleservices.MasterRoleChecker(loggedinuser.Id, roleclaimname);

            if (master_rolechecker_response || loggedinuser.RoleId == 1)
            {

                return await _stockServices.CreateRequisitionHeader(header);

            }
            else
            {
                return new StockResponse(false, "You have no permission to access this", null);
            }
        }
    }
}
