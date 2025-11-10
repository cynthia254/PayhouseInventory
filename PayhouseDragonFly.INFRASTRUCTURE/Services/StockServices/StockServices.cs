using Azure.Core;
using ClosedXML.Excel;
using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using PayhouseDragonFly.CORE.ConnectorClasses.Response.StockResponse;
using PayhouseDragonFly.CORE.DTOs.Stock;
using PayhouseDragonFly.CORE.DTOs.Stock.Invoicing_vm;
using PayhouseDragonFly.CORE.Models.Stock;
using PayhouseDragonFly.CORE.Models.Stock.Invoicing;
using PayhouseDragonFly.INFRASTRUCTURE.DataContext;
using PayhouseDragonFly.INFRASTRUCTURE.Services.ExtraServices;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IStockServices;
using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using PdfSharp.Pdf.IO;
using static PdfSharp.Pdf.AcroForms.PdfAcroField;
using PdfSharp.Pdf.AcroForms;
using NPOI;
using PdfReader = iTextSharp.text.pdf.PdfReader;
using iTextSharp.text.pdf.parser;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.Extensions.Logging;
using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.HPSF;
using Path = System.IO.Path;
using NPOI.OpenXmlFormats.Dml;
using PayhouseDragonFly.CORE.ConnectorClasses.Response.BseResponse;
using PayhouseDragonFly.INFRASTRUCTURE.Services.IServiceCoreInterfaces.IEmailServices;
using PayhouseDragonFly.CORE.DTOs.EmaillDtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using DocumentFormat.OpenXml.Bibliography;
using Azure.Identity;
using DocumentFormat.OpenXml.InkML;

namespace PayhouseDragonFly.INFRASTRUCTURE.Services.StockServices
{
    public class StockServices : IStockServices
    {
        private readonly DragonFlyContext _dragonFlyContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IEExtraServices _extraServices;
        private readonly ILogger<IStockServices> _logger;
        private readonly IEmailServices _iemail_service;
        public StockServices(DragonFlyContext dragonFlyContext, IServiceScopeFactory serviceScopeFactory, IEmailServices iemail_service,
            IEExtraServices extraServices,
            ILogger<IStockServices> logger
            )
        {
            _dragonFlyContext = dragonFlyContext;
            _serviceScopeFactory = serviceScopeFactory;
            _extraServices = extraServices;
            _logger = logger;
            _iemail_service = iemail_service;

        }

        public async Task<StockResponse> AddBrand(AddBrandvm addBrandvm)
        {
            try
            {
                if (addBrandvm.BrandName == "")
                {

                    return new StockResponse(false, "Kindly provide a brand name to add brand", null);
                }
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    //check if role exists 

                    var brandexists = await scopedcontext.AddBrand.Where(x => x.BrandName == addBrandvm.BrandName).FirstOrDefaultAsync();

                    if (brandexists != null)
                    {
                        return new StockResponse(false, $" Brand  '{addBrandvm.BrandName}' already exist, if  must add a similar brand kindly change the " +
                             $"latter cases from lower to upper and vice versa depending on the existing  brand . The existsing role is '{brandexists}' with brand id {brandexists.BrandId} ", null);
                    }
                    var brandclass = new AddBrand
                    {
                        BrandName = addBrandvm.BrandName,
                    };
                    await scopedcontext.AddAsync(brandclass);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, $"Brand '{addBrandvm.BrandName}'  created successfully", null);

                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }

        }
        public async Task<StockResponse> GetAllBrand()
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allbrands = await scopedcontext.AddBrand.ToListAsync();

                    if (allbrands == null)
                    {
                        return new StockResponse(false, "Brand doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allbrands);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> AddItem(AddItemvm addItemvm)
        {
            try
            {
                if (addItemvm.ItemName == "")
                {

                    return new StockResponse(false, "Kindly provide an item name to add item", null);
                }
                if (addItemvm.Category == "")
                {
                    return new StockResponse(false, "Kindly provide category", null);

                }
                if (addItemvm.BrandName == "")
                {
                    return new StockResponse(false, "Kindly provide brand name", null);

                }
                if (addItemvm.Currency == "")
                {
                    return new StockResponse(false, "Kindly provide currency", null);
                }
                if (addItemvm.IndicativePrice < 0)
                {
                    return new StockResponse(false, "Kindly provide indicative price", null);
                }
                if (addItemvm.ReOrderLevel < 0)
                {
                    return new StockResponse(false, "Kindly provide reorder level", null);
                }
                if (addItemvm.ItemDescription == null)
                {
                    return new StockResponse(false, "Kindly provide description", null);
                }
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();



                    var itemexists = await scopedcontext.AddItem.Where(x => x.ItemName == addItemvm.ItemName && x.BrandName == addItemvm.BrandName).FirstOrDefaultAsync();

                    if (itemexists != null)
                    {
                        return new StockResponse(false, $" Item  '{addItemvm.BrandName}-{addItemvm.ItemName}' already exist", null);
                    }

                    var itemclass = new AddItem
                    {
                        ItemName = addItemvm.ItemName,
                        Category = addItemvm.Category,
                        Currency = addItemvm.Currency,
                        IndicativePrice = addItemvm.IndicativePrice,
                        ReOrderLevel = addItemvm.ReOrderLevel,
                        BrandName = addItemvm.BrandName,
                        ItemDescription = addItemvm.ItemDescription,

                    };

                    await scopedcontext.AddAsync(itemclass);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, $"Item '{addItemvm.ItemName}'  created successfully", null);

                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }

        }
        public async Task<StockResponse> GetAllItems()
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allitems = await scopedcontext.AddItem.ToListAsync();

                    if (allitems == null)
                    {
                        return new StockResponse(false, "Item doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allitems);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> AddStock(AddStockvm addStockvm)
        {
            try
            {
                if (addStockvm.BrandName == "")
                {
                    return new StockResponse(false, "BrandName cannot be empty", null);
                }
                if (addStockvm.ItemName == "")
                {
                    return new StockResponse(false, "ItemName cannot be empty", null);
                }

                if (addStockvm.ReOrderLevel == 0)
                {
                    return new StockResponse(false, "ReorderLevel cannot be empty", null);
                }
                if (addStockvm.BuyingPrice < 0)
                {
                    return new StockResponse(false, "Buying price cannot be empty", null);
                }
                if (addStockvm.Quantity < 0)
                {
                    return new StockResponse(false, "Quantity cannot be empty", null);
                }


                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();




                    var itemclass = new AddStock
                    {
                        ItemName = addStockvm.ItemName,
                        BrandName = addStockvm.BrandName,
                        ReOrderLevel = addStockvm.ReOrderLevel,
                        BuyingPrice = addStockvm.BuyingPrice,
                        DateAdded = addStockvm.DateAdded,
                        Currency = addStockvm.Currency,
                        StockInTransit = addStockvm.StockInTransit,
                        Quantity = addStockvm.Quantity,
                        SalesCurrency = addStockvm.SalesCurrency,
                        ReorderRequired = addStockvm.ReorderRequired,


                    };


                    itemclass.OpeningStock = addStockvm.Quantity;
                    itemclass.AvailableStock = addStockvm.Quantity;

                    if (itemclass.AvailableStock > addStockvm.ReOrderLevel)
                    {
                        itemclass.Status = "Good";
                    }
                    else if (itemclass.AvailableStock < addStockvm.ReOrderLevel && itemclass.AvailableStock > 0 || itemclass.AvailableStock == addStockvm.ReOrderLevel)
                    {
                        itemclass.Status = "Low";
                    }
                    else
                    {
                        itemclass.Status = "Out";
                    }
                    if (itemclass.AvailableStock == addStockvm.ReOrderLevel || itemclass.AvailableStock < addStockvm.ReOrderLevel)
                    {
                        itemclass.ReorderRequired = "Yes";
                    }
                    else
                    {
                        itemclass.ReorderRequired = "No";
                    }


                    var checkitemexits = await scopedcontext.AddStock
                        .Where(y => y.ItemName == addStockvm.ItemName && y.BrandName == addStockvm.BrandName)
                        .FirstOrDefaultAsync();
                    if (checkitemexits != null)
                    {
                        return new StockResponse(false, "item already exists", null);
                    }
                    else
                    {
                        await scopedcontext.AddAsync(itemclass);
                        await scopedcontext.SaveChangesAsync();
                        return new StockResponse(true, "Stock added successfully", null);
                    }

                };

            }


            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }


        }
        public async Task<StockResponse> UpdateStockQuantity(int itemid, int quantityadded)
        {

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var lastupdate = await scopedcontext.AddStock.Where(y => y.StockId == itemid)
                        .FirstOrDefaultAsync();
                    if (lastupdate == null)
                    {
                        return new StockResponse(false, "stock not found", null);
                    }
                    lastupdate.Quantity = lastupdate.Quantity + quantityadded;
                    lastupdate.AvailableStock = quantityadded + lastupdate.AvailableStock;
                    if (lastupdate.AvailableStock > lastupdate.ReOrderLevel)
                    {
                        lastupdate.Status = "Good";
                    }
                    else if (lastupdate.AvailableStock < lastupdate.ReOrderLevel && lastupdate.AvailableStock > 0)
                    {
                        lastupdate.Status = "Low";
                    }
                    else
                    {
                        lastupdate.Status = "Out";
                    }
                    scopedcontext.Update(lastupdate);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, "Stock updated successfully", null);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> AddCustomer(AddCustomervm addCustomervm)
        {
            try
            {
                if (addCustomervm.CustomerName == "")
                {

                    return new StockResponse(false, "Kindly provide  customer name to add customer", null);
                }
                if (addCustomervm.CompanyName == "")
                {

                    return new StockResponse(false, "Kindly provide  company name to add customer", null);
                }
                if (addCustomervm.Email == "")
                {

                    return new StockResponse(false, "Kindly provide an email to add customer", null);
                }
                if (addCustomervm.PhoneNumber == "")
                {

                    return new StockResponse(false, "Kindly provide phoneNumber to add customer", null);
                }
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();



                    var customerexists = await scopedcontext.Customer.Where(x => x.Email == addCustomervm.Email).FirstOrDefaultAsync();

                    if (customerexists != null)
                    {
                        return new StockResponse(false, $" Customer  '{addCustomervm.Email}' already exists", null);
                    }
                    var itemclass = new AddCustomer
                    {
                        CustomerName = addCustomervm.CustomerName,
                        CompanyName = addCustomervm.CompanyName,
                        Email = addCustomervm.Email,
                        PhoneNumber = addCustomervm.PhoneNumber,
                    };
                    await scopedcontext.AddAsync(itemclass);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, $" '{addCustomervm.CustomerName}'  has been added successfully", null);

                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }

        }

        public async Task<StockResponse> GetAllCustomers()
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allcustomers = await scopedcontext.Customer.ToListAsync();

                    if (allcustomers == null)
                    {
                        return new StockResponse(false, "Customer doesn't exist", null);
                    }

                    return new StockResponse(true, "Successfully queried", allcustomers);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetItemByName(string itemname)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var itemexists = await scopedcontext.AddStock.Where(y => y.ItemName == itemname).FirstOrDefaultAsync();
                    if (itemexists == null)
                    {
                        return new StockResponse(false, "nothing to show ", null);
                    }
                    return new StockResponse(true, "Queried successfully", itemexists);


                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetStockItemByName(string itemname, string referenceNumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var item = await scopedcontext.AddItem
                        .FirstOrDefaultAsync(y => y.ItemName == itemname);

                    if (item == null)
                    {
                        return new StockResponse(false, "Item not found", null);
                    }

                    var currentDate = DateTime.Now;

                    // ✅ Get the requisition to determine the currency
                    var requisition = await scopedcontext.RequisitionApplication
                        .FirstOrDefaultAsync(r => r.ReferenceNumber == referenceNumber);

                    if (requisition == null)
                    {
                        return new StockResponse(false, "Requisition not found", null);
                    }

                    var currency = requisition.Currency;

                    // ✅ Get the most recent active price for this item in the requisition's currency
                    var price = await scopedcontext.PriceRecord
                        .Where(p => p.ItemId == item.ItemID.ToString() &&
                                    !p.IsDeleted &&
                                    p.Status == "Active" &&
                                    p.Currency == currency &&
                                    p.EffectiveFrom <= currentDate &&
                                    (p.EffectiveTo == null || p.EffectiveTo >= currentDate))
                        .OrderByDescending(p => p.EffectiveFrom)
                        .FirstOrDefaultAsync();

                    var result = new
                    {
                        item.ItemID,
                        item.ItemName,
                        item.BrandName,
                        item.Category,
                        Currency = currency, // Currency selected during requisition
                        Price = price != null ? price.SellingPrice : (decimal?)null,
                        PriceStatus = price?.Status,
                        EffectiveFrom = price?.EffectiveFrom,
                        EffectiveTo = price?.EffectiveTo
                    };

                    return new StockResponse(true, "Queried successfully", result);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> GetAllStock()
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allstock = await scopedcontext.AddStock.OrderByDescending(x => x.DateAdded).ToListAsync();

                    if (allstock == null)
                    {
                        return new StockResponse(false, "Stock doesn't exist", null);
                    }
                    List<All_Stocks_List> stocklist = new List<All_Stocks_List>();

                    foreach (var stock in allstock)
                    {
                        var newstockfound = new All_Stocks_List
                        {
                            StockId = stock.StockId,
                            BrandName = stock.BrandName,
                            ItemName = stock.ItemName,
                            Quantity = stock.Quantity,
                            ReOrderLevel = stock.ReOrderLevel,
                            BuyingPrice = stock.BuyingPrice,
                            SellingPrice = stock.SellingPrice,
                            AvailableStock = stock.AvailableStock,
                            DateAdded = stock.DateAdded,
                            Status = stock.Status,
                            Currency = stock.Currency,
                            SalesCurrency = stock.SalesCurrency,
                            StockInTransit = stock.StockInTransit,
                            OpeningStock = stock.OpeningStock,
                            ReorderRequired = stock.ReorderRequired,
                            StockOut = stock.StockOut,
                            TotalReturnedStock = stock.TotalReturnedStock,
                            StockIn = stock.StockIn,
                        };
                        allstock.Sum(x => x.Quantity);
                        newstockfound.TotalBuyingPrice = stock.BuyingPrice * stock.AvailableStock;
                        newstockfound.TotalSellingPrice = stock.SellingPrice * stock.AvailableStock;
                        stocklist.Add(newstockfound);

                    }
                    return new StockResponse(true, "Successfully queried", stocklist);

                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> AddSupplier(AddSupplierVm addSupplierVm)
        {
            try
            {
                if (addSupplierVm.SupplierName == "")
                {

                    return new StockResponse(false, "Kindly provide  supplier name to add supplier", null);
                }
                if (addSupplierVm.CompanyName == "")
                {

                    return new StockResponse(false, "Kindly provide  company name to add supplier", null);
                }
                if (addSupplierVm.Email == "")
                {

                    return new StockResponse(false, "Kindly provide an email to add supplier", null);
                }
                if (addSupplierVm.PhoneNumber == "")
                {

                    return new StockResponse(false, "Kindly provide phoneNumber to add supplier", null);
                }
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();



                    var customerexists = await scopedcontext.Suppliers.Where(x => x.Email == addSupplierVm.Email).FirstOrDefaultAsync();

                    if (customerexists != null)
                    {
                        return new StockResponse(false, $" Supplier  '{addSupplierVm.Email}' already exists", null);
                    }
                    var itemclass = new AddSupplier
                    {
                        SupplierName = addSupplierVm.SupplierName,
                        CompanyName = addSupplierVm.CompanyName,
                        Email = addSupplierVm.Email,
                        PhoneNumber = addSupplierVm.PhoneNumber,
                    };
                    await scopedcontext.AddAsync(itemclass);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, $" '{addSupplierVm.SupplierName}'  has been added successfully", null);

                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }

        }
        public async Task<StockResponse> GetAllSuppliers()
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allsuppliers = await scopedcontext.Suppliers.ToListAsync();

                    if (allsuppliers == null)
                    {
                        return new StockResponse(false, "Supplier doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allsuppliers);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> AddSales(AddSalesOrdersVm addSalesOrdersVm)
        {
            try
            {
                if (addSalesOrdersVm.BrandName == "")
                {

                    return new StockResponse(false, "Kindly provide  brand name to add sales", null);
                }
                if (addSalesOrdersVm.ItemName == "")
                {

                    return new StockResponse(false, "Kindly provide  item name to add sales", null);
                }
                if (addSalesOrdersVm.Quantity == 0)
                {

                    return new StockResponse(false, "Kindly provide quantity to add sales", null);
                }
                if (addSalesOrdersVm.CustomerName == "")
                {

                    return new StockResponse(false, "Kindly provide customer name to add sales", null);
                }
                if (addSalesOrdersVm.Comments == "")
                {
                    return new StockResponse(false, "Kindly provide comments to add stock out ", null);
                }
                if (addSalesOrdersVm.Department == "")
                {
                    return new StockResponse(false, "Kindly provide department details", null);
                }

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var itemexists = await scopedcontext.AddStock
                    .Where(y => y.BrandName == addSalesOrdersVm.BrandName &&
                    y.ItemName == addSalesOrdersVm.ItemName).FirstOrDefaultAsync();
                    if (itemexists == null)
                    {
                        return new StockResponse(false, "Does not exist", null);
                    }



                    var itemclass = new AddSalesOrder
                    {
                        BrandName = addSalesOrdersVm.BrandName,
                        ItemName = addSalesOrdersVm.ItemName,
                        Quantity = addSalesOrdersVm.Quantity,
                        CustomerName = addSalesOrdersVm.CustomerName,
                        Comments = addSalesOrdersVm.Comments,
                        DateAdded = DateTime.Now,
                        Department = addSalesOrdersVm.Department,







                    };

                    itemexists.Quantity -= addSalesOrdersVm.Quantity;
                    itemexists.StockOut += addSalesOrdersVm.Quantity;
                    if (itemexists.AvailableStock == 0)
                    {
                        return new StockResponse(false, "No available stock please restock first....", null);
                    }
                    if (itemexists.AvailableStock < itemclass.Quantity)
                    {
                        return new StockResponse(false, $"Note:You can only stockOut from '{itemexists.AvailableStock}'!!! ", null);
                    }
                    itemexists.AvailableStock -= addSalesOrdersVm.Quantity;

                    if (itemexists.AvailableStock > itemexists.ReOrderLevel)
                    {
                        itemexists.Status = "Good";
                    }
                    else if (itemexists.AvailableStock < itemexists.ReOrderLevel && itemexists.AvailableStock > 0 || itemexists.AvailableStock == itemexists.ReOrderLevel)
                    {
                        itemexists.Status = "Low";
                    }
                    else
                    {
                        itemexists.Status = "Out";
                    }
                    if (itemexists.AvailableStock < itemexists.ReOrderLevel || itemexists.AvailableStock == itemexists.ReOrderLevel)
                    {
                        itemexists.ReorderRequired = "Yes";
                    }
                    else
                    {
                        itemexists.ReorderRequired = "No";
                    }



                    await scopedcontext.AddAsync(itemclass);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, "StockOut has been added successfully", null);

                }


            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }

        }
        public async Task<StockResponse> GetAllSales()
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allsales = await scopedcontext.Sales.ToListAsync();

                    if (allsales == null)
                    {
                        return new StockResponse(false, "Sale doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allsales);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> AddPurchase(AddPurchaseOrderVm addPurchaseOrderVm)
        {
            try
            {
                if (addPurchaseOrderVm.BrandName == "")
                {

                    return new StockResponse(false, "Kindly provide  brand name ", null);
                }
                if (addPurchaseOrderVm.ItemName == "")
                {

                    return new StockResponse(false, "Kindly provide  item name ", null);
                }
                if (addPurchaseOrderVm.Quantity == 0)
                {

                    return new StockResponse(false, "Kindly provide quantity", null);
                }
                if (addPurchaseOrderVm.SupplierName == "")
                {

                    return new StockResponse(false, "Kindly provide supplier name", null);
                }
                if (addPurchaseOrderVm.DeliveryDate == DateTime.MaxValue)
                {

                    return new StockResponse(false, "Kindly provide delivery date", null);
                }
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var itemexists = await scopedcontext.AddStock
                        .Where(y => y.BrandName == addPurchaseOrderVm.BrandName &&
                        y.ItemName == addPurchaseOrderVm.ItemName).FirstOrDefaultAsync();
                    if (itemexists == null)
                    {
                        return new StockResponse(false, "Does not exist", null);
                    }


                    var itemclass = new AddPurchaseOrder
                    {
                        BrandName = addPurchaseOrderVm.BrandName,
                        ItemName = addPurchaseOrderVm.ItemName,
                        Quantity = addPurchaseOrderVm.Quantity,
                        SupplierName = addPurchaseOrderVm.SupplierName,
                        DeliveryDate = addPurchaseOrderVm.DeliveryDate,
                        PurchaseDate = addPurchaseOrderVm.PurchaseDate,
                        TotalPurchase = addPurchaseOrderVm.TotalPurchase,
                        Status = "Ordered",
                        ReasonforStatus = "New",
                        PurchaseStatus = "Ordered",
                        DateAdded = DateTime.Now,



                    };
                    itemclass.TotalPurchase = addPurchaseOrderVm.Quantity * itemexists.BuyingPrice;
                    itemexists.AvailableStock += addPurchaseOrderVm.Quantity;
                    itemexists.Quantity += addPurchaseOrderVm.Quantity;
                    itemexists.StockIn += addPurchaseOrderVm.Quantity;
                    itemclass.Currency = itemexists.Currency;
                    if (itemexists.AvailableStock > itemexists.ReOrderLevel)
                    {
                        itemexists.Status = "Good";
                    }
                    else if (itemexists.AvailableStock < itemexists.ReOrderLevel && itemexists.AvailableStock > 0 || itemexists.AvailableStock == itemexists.ReOrderLevel)
                    {
                        itemexists.Status = "Low";
                    }
                    else
                    {
                        itemexists.Status = "Out";
                    }
                    if (itemexists.AvailableStock == itemexists.ReOrderLevel || itemexists.AvailableStock < itemexists.ReOrderLevel)
                    {
                        itemexists.ReorderRequired = "Yes";
                    }
                    else
                    {
                        itemexists.ReorderRequired = "No";
                    }




                    await scopedcontext.AddAsync(itemclass);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, "Stock  has been added successfully", null);

                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }

        }
        public async Task<StockResponse> GetAllPurchases()
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allpurchases = await scopedcontext.Purchases.ToListAsync();

                    if (allpurchases == null)
                    {
                        return new StockResponse(false, "Purchase doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allpurchases);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetItemById(int itemid)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var itemexist = await scopedcontext.AddStock.Where(u => u.StockId == itemid).FirstOrDefaultAsync();
                    if (itemexist == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", itemexist);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetPurchaseById(int purchaseId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var purchaseexists = await scopedcontext.Purchases.Where(u => u.PurchaseId == purchaseId).FirstOrDefaultAsync();
                    if (purchaseexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", purchaseexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetSerialByItemId(int ItemId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var purchaseexists = await scopedcontext.AddProductDetails.Where(u => u.ItemID == ItemId).FirstOrDefaultAsync();
                    if (purchaseexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", purchaseexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> ChangePurchaseStatus(PurchaseStatusvm purchaseStatusvm)
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();


                    var purchaseexists = await scopedcontext.Purchases.Where(u => u.PurchaseId == purchaseStatusvm.PurchaseId).FirstOrDefaultAsync();
                    if (purchaseexists == null)
                    {

                        return new StockResponse(false, "purchase does not exist", null);
                    }
                    var itemexists = await scopedcontext.AddStock
                       .Where(y => y.BrandName == purchaseexists.BrandName &&
                       y.ItemName == purchaseexists.ItemName).FirstOrDefaultAsync();
                    if (itemexists == null)
                    {
                        return new StockResponse(false, "Does not exist", null);
                    }



                    purchaseexists.PurchaseId = purchaseStatusvm.PurchaseId;
                    purchaseexists.PurchaseStatus = purchaseStatusvm.PurchaseStatus;
                    purchaseexists.ReasonforStatus = purchaseStatusvm.ReasonforStatus;
                    purchaseexists.DateAdded = Convert.ToDateTime(purchaseexists.DateAdded);

                    if (purchaseexists.PurchaseStatus == "Delivered")
                    {
                        itemexists.AvailableStock += purchaseexists.Quantity;
                        itemexists.Quantity += purchaseexists.Quantity;
                    }
                    if (purchaseexists.PurchaseStatus == "In Transit")
                    {
                        itemexists.StockInTransit = purchaseexists.Quantity + itemexists.StockInTransit;
                    }
                    if (itemexists.AvailableStock > itemexists.ReOrderLevel)
                    {
                        itemexists.Status = "Good";
                    }
                    else if (itemexists.AvailableStock < itemexists.ReOrderLevel && itemexists.AvailableStock > 0)
                    {
                        itemexists.Status = "Low";
                    }
                    else
                    {
                        itemexists.Status = "Out";
                    }







                    scopedcontext.Update(purchaseexists);
                    await scopedcontext.SaveChangesAsync();
                    ;

                    return new StockResponse(true, $"{purchaseexists.BrandName} {purchaseexists.ItemName} status changged success fully to '{purchaseStatusvm.PurchaseStatus}'", null);

                }

            }
            catch (Exception ex)

            {

                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetSalesbyId(int salesId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var purchaseexists = await scopedcontext.Sales.Where(u => u.SalesId == salesId).FirstOrDefaultAsync();
                    if (purchaseexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", purchaseexists);


                }


            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GenerateExcel()
        {


            using var wbook = new XLWorkbook();
            var ws = wbook.AddWorksheet("Stock");
            ws.FirstCell().Value = "Product ID";
            ws.Cell("B1").Value = "Product Name";
            ws.Cell("C1").Value = "Stock In";
            ws.Cell("D1").Value = "Stock Out";
            ws.Cell("E1").Value = "Buying Price";

            ws.Column(2).AdjustToContents();
            ws.Column(1).AdjustToContents();
            ws.Column(3).AdjustToContents();
            ws.Column(4).AdjustToContents();
            ws.Column(5).AdjustToContents();

            wbook.SaveAs("Product.xlsx");

            return new StockResponse(true, "Success", null);





        }




        //  ws.FirstCell().Value = "Product Name";
        // ws.Cell(3, 2).Value = "ASP.NET CORE MVC";
        // ws.Cell("A6").SetValue("Youtube Channel");
        // ws.Column(2).AdjustToContents();



        public async Task<StockResponse> ChangeSalesStatus(ChangeSalesStatusvm changeSalesStatusvm)
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();




                    var salesexists = await scopedcontext.Sales.Where(u => u.SalesId == changeSalesStatusvm.salesId).FirstOrDefaultAsync();
                    if (salesexists == null)
                    {

                        return new StockResponse(false, "purchase does not exist", null);
                    }
                    var itemexists = await scopedcontext.AddStock
                       .Where(y => y.BrandName == salesexists.BrandName &&
                       y.ItemName == salesexists.ItemName).FirstOrDefaultAsync();
                    if (itemexists == null)
                    {
                        return new StockResponse(false, "Does not exist", null);
                    }


                    salesexists.SalesId = changeSalesStatusvm.salesId;
                    salesexists.SalesStatus = changeSalesStatusvm.SalesStatus;
                    salesexists.ReasonForSalesStatus = salesexists.ReasonForSalesStatus;
                    salesexists.DateAdded = Convert.ToDateTime(salesexists.DateAdded);

                    if (salesexists.SalesStatus == "Delivered")
                    {
                        itemexists.AvailableStock -= salesexists.Quantity;
                        itemexists.Quantity -= salesexists.Quantity;
                    }
                    if (salesexists.SalesStatus == "In Transit")
                    {
                        itemexists.StockInTransit = salesexists.Quantity + itemexists.StockInTransit;
                    }
                    if (itemexists.AvailableStock > itemexists.ReOrderLevel)
                    {
                        itemexists.Status = "Good";
                    }
                    else if (itemexists.AvailableStock < itemexists.ReOrderLevel && itemexists.AvailableStock > 0)
                    {
                        itemexists.Status = "Low";
                    }
                    else
                    {
                        itemexists.Status = "Out";
                    }






                    scopedcontext.Update(salesexists);
                    await scopedcontext.SaveChangesAsync();
                    ;

                    return new StockResponse(true, $"{salesexists.BrandName} {salesexists.ItemName} status changged success fully to '{changeSalesStatusvm.SalesStatus}'", null);

                }

            }
            catch (Exception ex)

            {

                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> AddReturnedStatus(AddReturnedStatusvm addReturnedStatusvm)
        {
            try
            {
                if (addReturnedStatusvm.ReturnedStatus == "")
                {

                    return new StockResponse(false, "Kindly provide  returned status to add status", null);
                }
                if (addReturnedStatusvm.ReturnedDescription == "")
                {

                    return new StockResponse(false, "Kindly provide  reason for status  to add status", null);
                }

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();



                    var returnstatusexists = await scopedcontext.ReturnedStatusTable.Where(x => x.ReturnedID == addReturnedStatusvm.ReturnedID).FirstOrDefaultAsync();

                    if (returnstatusexists != null)
                    {
                        return new StockResponse(false, $" Returned status  '{addReturnedStatusvm.ReturnedStatus}' already exists", null);
                    }
                    var itemclass = new AddReturnedStatus
                    {
                        ReturnedStatus = addReturnedStatusvm.ReturnedStatus,
                        ReturnedDescription = addReturnedStatusvm.ReturnedDescription,

                    };

                    await scopedcontext.AddAsync(itemclass);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, $" '{addReturnedStatusvm.ReturnedStatus}'  has been added successfully", null);

                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }


        }
        public async Task<StockResponse> GetAllReturnedStatus()
        {
            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allreturnedstatus = await scopedcontext.ReturnedStatusTable.ToListAsync();

                    if (allreturnedstatus == null)
                    {
                        return new StockResponse(false, "Returned status doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allreturnedstatus);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }

        }
        public async Task<StockResponse> AddReturnedStock(AddReturnedStockvm addReturnedStockvm)
        {
            try
            {
                if (addReturnedStockvm.ReturnedStatus == "")
                {

                    return new StockResponse(false, "Kindly provide  returned quantity to add returned stock", null);
                }
                if (addReturnedStockvm.ReturnedQuantity < 0)
                {

                    return new StockResponse(false, "Kindly provide  quantity  to add returned stock", null);
                }
                if (addReturnedStockvm.BrandName == "")
                {
                    return new StockResponse(false, "Kindly provide brandname to add returned stock", null);

                }
                if (addReturnedStockvm.ItemName == "")
                {
                    return new StockResponse(false, "Kindly provide itemname to add returned stock", null);

                }
                if (addReturnedStockvm.ReturnReason == "")
                {
                    return new StockResponse(false, "Kindly provide return reason to add returned stock", null);
                }


                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var itemexists = await scopedcontext.AddStock
                      .Where(y => y.BrandName == addReturnedStockvm.BrandName &&
                      y.ItemName == addReturnedStockvm.ItemName).FirstOrDefaultAsync();
                    if (itemexists == null)
                    {
                        return new StockResponse(false, "Does not exist", null);
                    }




                    var returnstatusexists = await scopedcontext.ReturnedStock.Where(x => x.ReturnedId == addReturnedStockvm.ReturnedId).FirstOrDefaultAsync();

                    if (returnstatusexists != null)
                    {
                        return new StockResponse(false, $" Returned stock  '{addReturnedStockvm.ItemName}' already exists", null);
                    }
                    var itemclass = new AddReturnedStock
                    {
                        ReturnedStatus = addReturnedStockvm.ReturnedStatus,
                        ReturnedQuantity = addReturnedStockvm.ReturnedQuantity,
                        ReturnReason = addReturnedStockvm.ReturnReason,
                        DateReturned = addReturnedStockvm.DateReturned,
                        BrandName = addReturnedStockvm.BrandName,
                        ItemName = addReturnedStockvm.ItemName,


                    };
                    itemexists.AvailableStock += addReturnedStockvm.ReturnedQuantity;
                    itemexists.Quantity += addReturnedStockvm.ReturnedQuantity;
                    itemexists.TotalReturnedStock += addReturnedStockvm.ReturnedQuantity;
                    if (itemexists.AvailableStock == itemexists.ReOrderLevel || itemexists.AvailableStock < itemexists.ReOrderLevel)
                    {
                        itemexists.ReorderRequired = "Yes";
                    }
                    else
                    {
                        itemexists.ReorderRequired = "No";
                    }
                    if (itemexists.AvailableStock > itemexists.ReOrderLevel)
                    {
                        itemexists.Status = "Good";
                    }
                    else if (itemexists.AvailableStock < itemexists.ReOrderLevel && itemexists.AvailableStock > 0 || itemexists.AvailableStock == itemexists.ReOrderLevel)
                    {
                        itemexists.Status = "Low";
                    }
                    else
                    {
                        itemexists.Status = "Out";
                    }
                    await scopedcontext.AddAsync(itemclass);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, $" '{addReturnedStockvm.ItemName}'  has been added to return stock successfully ", null);

                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }


        }
        public async Task<StockResponse> GetAllReturnedStock()
        {
            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allreturnedstatus = await scopedcontext.ReturnedStock.ToListAsync();

                    if (allreturnedstatus == null)
                    {
                        return new StockResponse(false, "Returned stock doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allreturnedstatus);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }

        }
        public async Task<StockResponse> SearchForStock(string search_query)
        {

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var allstock = await scopedcontext.AddStock.Where
                        (u => EF.Functions.Like(u.BrandName, $"%{search_query}%") ||
                        EF.Functions.Like(u.ItemName, $"%{search_query}%") ||
                        EF.Functions.Like(u.Status, $"%{search_query}%") ||
                        EF.Functions.Like(u.ReorderRequired, $"%{search_query}%")
                        ).ToListAsync();

                    if (allstock == null)
                        return new StockResponse(false, "", null);

                    return new StockResponse(true, "Successfully queried", allstock);


                }
            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> SearchForStockIn(string search_query)
        {

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var allstock = await scopedcontext.Purchases.Where
                         (u => EF.Functions.Like(u.BrandName, $"%{search_query}%") ||
                        EF.Functions.Like(u.ItemName, $"%{search_query}%") ||
                        EF.Functions.Like(u.SupplierName, $"%{search_query}%") ||
                        EF.Functions.Like(u.Currency, $"%{search_query}%")

                        ).ToListAsync();

                    if (allstock == null)
                        return new StockResponse(false, "", null);

                    return new StockResponse(true, "Successfully queried", allstock);


                }
            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> SearchForStockOut(string search_query)
        {

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var allstock = await scopedcontext.Sales.Where
                         (u => EF.Functions.Like(u.BrandName, $"%{search_query}%") ||
                        EF.Functions.Like(u.ItemName, $"%{search_query}%") ||
                        EF.Functions.Like(u.CustomerName, $"%{search_query}%") ||
                        EF.Functions.Like(u.Comments, $"%{search_query}%") ||
                         EF.Functions.Like(u.Department, $"%{search_query}%")

                        ).ToListAsync();

                    if (allstock == null)
                        return new StockResponse(false, "", null);

                    return new StockResponse(true, "Successfully queried", allstock);


                }
            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }



        public async Task<StockResponse> SearchForCustomer(string search_query)
        {

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var allstock = await scopedcontext.Customer.Where
                         (u => EF.Functions.Like(u.CustomerName, $"%{search_query}%") ||
                        EF.Functions.Like(u.CompanyName, $"%{search_query}%") ||
                        EF.Functions.Like(u.Email, $"%{search_query}%") ||
                        EF.Functions.Like(u.PhoneNumber, $"%{search_query}%")

                        ).ToListAsync();

                    if (allstock == null)
                        return new StockResponse(false, "", null);

                    return new StockResponse(true, "Successfully queried", allstock);


                }
            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> SearchForSupplier(string search_query)
        {

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var allstock = await scopedcontext.Suppliers.Where
                         (u => EF.Functions.Like(u.SupplierName, $"%{search_query}%") ||
                        EF.Functions.Like(u.CompanyName, $"%{search_query}%") ||
                        EF.Functions.Like(u.Email, $"%{search_query}%") ||
                        EF.Functions.Like(u.PhoneNumber, $"%{search_query}%")

                        ).ToListAsync();

                    if (allstock == null)
                        return new StockResponse(false, "", null);

                    return new StockResponse(true, "Successfully queried", allstock);


                }
            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> EditSales(editSalesvm salesvm)
        {
            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var salesexists = await scopedcontext.Sales.Where(u => u.SalesId == salesvm.SalesId).FirstOrDefaultAsync();

                    if (salesexists == null)
                    {

                        return new StockResponse(false, "stock does not exist", null);
                    }

                    if (salesvm.BrandName == "string")
                    {
                        salesexists.BrandName = salesexists.BrandName;

                    }
                    else
                    {
                        salesexists.BrandName = salesvm.BrandName;
                    }


                    if (salesvm.ItemName == "string")
                    {
                        salesexists.ItemName = salesexists.ItemName;

                    }
                    else
                    {
                        salesexists.ItemName = salesvm.ItemName;
                    }
                    if (salesvm.CustomerName == "string")
                    {
                        salesexists.CustomerName = salesexists.CustomerName;

                    }
                    else
                    {
                        salesexists.CustomerName = salesvm.CustomerName;
                    }
                    if (salesvm.Comments == "string")
                    {
                        salesexists.Comments = salesexists.Comments;

                    }
                    else
                    {
                        salesexists.Comments = salesvm.Comments;
                    }
                    if (salesvm.Department == "string")
                    {
                        salesexists.Department = salesexists.Department;

                    }
                    else
                    {
                        salesexists.Department = salesvm.Department;
                    }




                    scopedcontext.Update(salesexists);
                    await scopedcontext.SaveChangesAsync();

                    return new StockResponse(true, "Sucessfully updated stock out details", salesexists);


                }

            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> DeleteStockOut(int salesId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopecontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var departmentexists = await scopecontext.Sales.Where(x => x.SalesId == salesId).FirstOrDefaultAsync();

                    if (departmentexists == null)
                    {
                        return new StockResponse(false, "stock does not exist ", null);
                    }
                    scopecontext.Remove(departmentexists);
                    await scopecontext.SaveChangesAsync();

                    return new StockResponse(true, "stockOut deleted successfully", null);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }


        }
        public async Task<StockResponse> EditCustomer(EditCustomervm editCustomervm)
        {
            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var salesexists = await scopedcontext.Customer.Where(u => u.CustomerId == editCustomervm.CustomerId).FirstOrDefaultAsync();

                    if (salesexists == null)
                    {

                        return new StockResponse(false, "stock does not exist", null);
                    }

                    if (editCustomervm.CustomerName == "string")
                    {
                        salesexists.CustomerName = salesexists.CustomerName;

                    }
                    else
                    {
                        salesexists.CustomerName = editCustomervm.CustomerName;
                    }


                    if (editCustomervm.Email == "string")
                    {
                        salesexists.Email = salesexists.Email;

                    }
                    else
                    {
                        salesexists.Email = editCustomervm.Email;
                    }
                    if (editCustomervm.CompanyName == "string")
                    {
                        salesexists.CompanyName = salesexists.CompanyName;

                    }
                    else
                    {
                        salesexists.CompanyName = editCustomervm.CompanyName;
                    }
                    if (editCustomervm.PhoneNumber == "string")
                    {
                        salesexists.PhoneNumber = salesexists.PhoneNumber;

                    }
                    else
                    {
                        salesexists.PhoneNumber = editCustomervm.PhoneNumber;
                    }





                    scopedcontext.Update(salesexists);
                    await scopedcontext.SaveChangesAsync();

                    return new StockResponse(true, "Sucessfully updated stock out details", salesexists);


                }

            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetCustomerById(int customerId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var customerexists = await scopedcontext.Customer.Where(u => u.CustomerId == customerId).FirstOrDefaultAsync();
                    if (customerexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", customerexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetSupplierById(int supplierId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.Suppliers.Where(u => u.SupplierId == supplierId).FirstOrDefaultAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> EditSupplier(editSuppliervm suppliervm)
        {
            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var salesexists = await scopedcontext.Suppliers.Where(u => u.SupplierId == suppliervm.SupplierId).FirstOrDefaultAsync();

                    if (salesexists == null)
                    {

                        return new StockResponse(false, "stock does not exist", null);
                    }

                    if (suppliervm.SupplierName == "string")
                    {
                        salesexists.SupplierName = salesexists.SupplierName;

                    }
                    else
                    {
                        salesexists.SupplierName = suppliervm.SupplierName;
                    }


                    if (suppliervm.Email == "string")
                    {
                        salesexists.Email = salesexists.Email;

                    }
                    else
                    {
                        salesexists.Email = suppliervm.Email;
                    }
                    if (suppliervm.CompanyName == "string")
                    {
                        salesexists.CompanyName = salesexists.CompanyName;

                    }
                    else
                    {
                        salesexists.CompanyName = suppliervm.CompanyName;
                    }
                    if (suppliervm.PhoneNumber == "string")
                    {
                        salesexists.PhoneNumber = salesexists.PhoneNumber;

                    }
                    else
                    {
                        salesexists.PhoneNumber = suppliervm.PhoneNumber;
                    }





                    scopedcontext.Update(salesexists);
                    await scopedcontext.SaveChangesAsync();

                    return new StockResponse(true, "Sucessfully updated stock out details", salesexists);


                }

            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetBrandById(int BrandId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.AddBrand.Where(u => u.BrandId == BrandId).FirstOrDefaultAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetItemsById(int ItemId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.AddItem.Where(u => u.ItemID == ItemId).FirstOrDefaultAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> EditBrand(EditBrandvm editBrandvm)
        {
            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var salesexists = await scopedcontext.AddBrand.Where(u => u.BrandId == editBrandvm.BrandId).FirstOrDefaultAsync();

                    if (salesexists == null)
                    {

                        return new StockResponse(false, "stock does not exist", null);
                    }

                    if (editBrandvm.BrandName == "string")
                    {
                        salesexists.BrandName = salesexists.BrandName;

                    }
                    else
                    {
                        salesexists.BrandName = editBrandvm.BrandName;
                    }




                    scopedcontext.Update(salesexists);
                    await scopedcontext.SaveChangesAsync();

                    return new StockResponse(true, "Sucessfully updated stock out details", salesexists);


                }

            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> EditItem(EditItemvm editItemvm)
        {
            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var salesexists = await scopedcontext.AddItem.Where(u => u.ItemID == editItemvm.ItemId).FirstOrDefaultAsync();

                    if (salesexists == null)
                    {

                        return new StockResponse(false, "item does not exist", null);
                    }

                    if (editItemvm.ItemName == "string")
                    {
                        salesexists.ItemName = salesexists.ItemName;

                    }
                    else
                    {
                        salesexists.ItemName = editItemvm.ItemName;
                    }
                    if (editItemvm.BrandName == "string")
                    {
                        salesexists.BrandName = salesexists.BrandName;

                    }
                    else
                    {
                        salesexists.BrandName = editItemvm.BrandName;
                    }
                    if (editItemvm.Category == "string")
                    {
                        salesexists.Category = salesexists.Category;

                    }
                    else
                    {
                        salesexists.Category = editItemvm.Category;
                    }
                    if (editItemvm.Currency == "string")
                    {
                        salesexists.Currency = salesexists.Currency;

                    }
                    else
                    {
                        salesexists.Currency = editItemvm.Currency;
                    }
                    if (editItemvm.ItemDescription == "string")
                    {
                        salesexists.ItemDescription = salesexists.ItemDescription;

                    }
                    else
                    {
                        salesexists.ItemDescription = editItemvm.ItemDescription;
                    }
                    if (editItemvm.IndicativePrice < 0)
                    {
                        salesexists.IndicativePrice = salesexists.IndicativePrice;

                    }
                    else
                    {
                        salesexists.IndicativePrice = editItemvm.IndicativePrice;
                    }
                    if (editItemvm.ReOrderLevel < 0)
                    {
                        salesexists.ReOrderLevel = salesexists.ReOrderLevel;

                    }
                    else
                    {
                        salesexists.ReOrderLevel = editItemvm.ReOrderLevel;
                    }





                    scopedcontext.Update(salesexists);
                    await scopedcontext.SaveChangesAsync();

                    return new StockResponse(true, "Sucessfully updated stock out details", salesexists);


                }

            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> EditStockIn(EditPurchasevm editPurchasevm)
        {
            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var purchaseexists = await scopedcontext.Purchases.Where(u => u.PurchaseId == editPurchasevm.PurchaseId).FirstOrDefaultAsync();

                    if (purchaseexists == null)
                    {

                        return new StockResponse(false, "stock does not exist", null);
                    }

                    if (editPurchasevm.BrandName == "string")
                    {
                        purchaseexists.BrandName = purchaseexists.BrandName;

                    }
                    else
                    {
                        purchaseexists.BrandName = editPurchasevm.BrandName;
                    }


                    if (editPurchasevm.ItemName == "string")
                    {
                        purchaseexists.ItemName = purchaseexists.ItemName;

                    }
                    else
                    {
                        purchaseexists.ItemName = editPurchasevm.ItemName;
                    }
                    if (editPurchasevm.SupplierName == "string")
                    {
                        purchaseexists.SupplierName = purchaseexists.SupplierName;

                    }
                    else
                    {
                        purchaseexists.SupplierName = editPurchasevm.SupplierName;
                    }






                    scopedcontext.Update(purchaseexists);
                    await scopedcontext.SaveChangesAsync();

                    return new StockResponse(true, "Sucessfully updated stock in details", purchaseexists);


                }

            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> AddCategory(AddCategoryvm addCategoryvm)
        {
            try
            {
                if (addCategoryvm.CategoryName == "")
                {

                    return new StockResponse(false, "Kindly provide a category name to add category", null);
                }
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    //check if role exists 

                    var categoryexists = await scopedcontext.Category.Where(x => x.CategoryName == addCategoryvm.CategoryName).FirstOrDefaultAsync();

                    if (categoryexists != null)
                    {
                        return new StockResponse(false, $" Category  '{addCategoryvm.CategoryName}' already exist, if  must add a similar category kindly change the " +
                             $"latter cases from lower to upper and vice versa depending on the existing  category . The existsing role is '{categoryexists}' with category id {categoryexists.CategoryID} ", null);
                    }
                    var categoryclass = new AddCategory
                    {
                        CategoryName = addCategoryvm.CategoryName,
                        CategoryDesc = addCategoryvm.CategoryDesc,
                    };
                    await scopedcontext.AddAsync(categoryclass);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, $"Category '{addCategoryvm.CategoryName}'  created successfully", null);

                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }

        }
        public async Task<StockResponse> GetAllCategory()
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allcategory = await scopedcontext.Category.ToListAsync();

                    if (allcategory == null)
                    {
                        return new StockResponse(false, "category doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allcategory);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> AddInvoiceDetails(StockInvm stockInvm)
        {

            try
            {
                if (stockInvm.SupplierName == "")
                {

                    return new StockResponse(false, "Kindly provide an suplier name ", null);
                }


                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();



                    var itemexists = await scopedcontext.StockIn.Where(x => x.InvoiceDate == stockInvm.InvoiceDate && x.SupplierName == stockInvm.SupplierName).FirstOrDefaultAsync();

                    if (itemexists != null)
                    {
                        return new StockResponse(false, $" Invoice  already exist... ", null);
                    }

                    var lpo_no_obj = GetLpo_Number().Result;
                    var lponumber = "LPO-" + lpo_no_obj;
                    var invoice_no_obj = GetInvoiceNumber().Result;
                    var invoicenumber = "INV-" + invoice_no_obj;
                    var itemclass = new StockIn
                    {
                        SupplierName = stockInvm.SupplierName,
                        InvoiceDate = stockInvm.InvoiceDate,
                        LPODate = stockInvm.LPODate,
                        LPONumber = lponumber,
                        InvoiceNumber = invoicenumber,
                        Status = "Incomplete",



                    };
                    var invoicedateexists = await scopedcontext.StockIn.Where(x => x.InvoiceDate == itemclass.InvoiceDate && x.SupplierName == stockInvm.SupplierName).FirstOrDefaultAsync();

                    if (invoicedateexists != null)
                    {
                        return new StockResponse(false, $" Invoice with this date  already exist... ", null);
                    }
                    await scopedcontext.AddAsync(itemclass);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, $"Invoice created successfully", null);

                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }



        }
        public async Task<StockResponse> GetReturnedDataById(int Id)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.ReturnedItem.Where(u => u.IssuedId == Id).ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }


        public async Task<int> GetLpo_Number()
        {

            var last_number_obj = await _dragonFlyContext.LPO_No
                .OrderByDescending(y => y.DateCreated).FirstOrDefaultAsync();


            if (last_number_obj == null)
            {
                var newvalue = new lpoNo();

                newvalue.LpoNo = 1;
                await _dragonFlyContext.AddAsync(newvalue);
                await _dragonFlyContext.SaveChangesAsync();
                return newvalue.LpoNo;
            }

            last_number_obj.LpoNo = last_number_obj.LpoNo + 1;
            _dragonFlyContext.Update(last_number_obj);
            await _dragonFlyContext.SaveChangesAsync();

            return last_number_obj.LpoNo;
        }
        public async Task<int> GetInvoiceNumber()
        {

            var last_number_obj = await _dragonFlyContext.InvoiceNo
                .OrderByDescending(y => y.DateCreated).FirstOrDefaultAsync();


            if (last_number_obj == null)
            {
                var newvalue = new InvoiceNo();

                newvalue.InvoieNo = 1;
                await _dragonFlyContext.AddAsync(newvalue);
                await _dragonFlyContext.SaveChangesAsync();
                return newvalue.InvoieNo;
            }

            last_number_obj.InvoieNo = last_number_obj.InvoieNo + 1;
            _dragonFlyContext.Update(last_number_obj);
            await _dragonFlyContext.SaveChangesAsync();

            return last_number_obj.InvoieNo;
        }
        public async Task<StockResponse> GetInvoiceDetails()
        {
            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allinvoice = await scopedcontext.StockIn.OrderByDescending(y => y.StockInDate).ToListAsync();

                    if (allinvoice == null)
                    {
                        return new StockResponse(false, "Invoice doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allinvoice);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }

        }
        public async Task<StockResponse> AddBatchDetails(AddBatchDetailsvm addBatchDetailsvm)
        {
            try
            {
                if (addBatchDetailsvm.ItemName == "")
                {

                    return new StockResponse(false, "Kindly provide an item name ", null);
                }
                if (addBatchDetailsvm.CategoryName == "")
                {
                    return new StockResponse(false, "Kindly provide category", null);

                }

                if (addBatchDetailsvm.Currency == "")
                {
                    return new StockResponse(false, "Kindly provide currency", null);
                }
                if (addBatchDetailsvm.UnitPrice < 0)
                {
                    return new StockResponse(false, "Kindly provide unit price", null);
                }
                if (addBatchDetailsvm.Quantity < 0)
                {
                    return new StockResponse(false, "Kindly provide quantity", null);
                }
                if (addBatchDetailsvm.Warranty < 0)
                {
                    return new StockResponse(false, "Kindly provide warranty", null);
                }
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var invoiceexists = await scopedcontext.StockIn
                 .Where(y => y.InvoiceNumber == addBatchDetailsvm.InvoiceNumber).OrderByDescending(y => y.InvoiceNumber).FirstOrDefaultAsync();
                    if (invoiceexists == null)
                    {
                        return new StockResponse(false, "Invoice Number does not exist", null);
                    }

                    var loggedinuserobject = await _extraServices.LoggedInUser();

                    var userEmail = loggedinuserobject.FirstName + ' ' + loggedinuserobject.LastName;

                    if (loggedinuserobject == null)
                    {

                        return new StockResponse(false, "user not logged in. login again", null);

                    }




                    var itemNameexists = await scopedcontext.AddItem
                   .Where(y => y.ItemName == addBatchDetailsvm.ItemName).FirstOrDefaultAsync();
                    if (itemNameexists == null)
                    {
                        return new StockResponse(false, "Item name does not exist", null);
                    }

                    var itemclass = new InvoiceLinesDetails
                    {
                        ItemName = addBatchDetailsvm.ItemName,
                        CategoryName = addBatchDetailsvm.CategoryName,
                        Currency = addBatchDetailsvm.Currency,
                        UnitPrice = addBatchDetailsvm.UnitPrice,
                        Warranty = addBatchDetailsvm.Warranty,
                        Quantity = addBatchDetailsvm.Quantity,
                        WarrantyStartDate = addBatchDetailsvm.WarrantyStartDate,
                        UpdatedBy = userEmail,
                        UpdatedOn = addBatchDetailsvm.UpdatedOn,
                        InvoiceNumber = addBatchDetailsvm.InvoiceNumber,
                        BrandName = addBatchDetailsvm.BrandName,







                    };
                   
                    var itemexists = await scopedcontext.InvoiceLinesDetails.Where(x => x.ItemName == addBatchDetailsvm.ItemName && x.BrandName == addBatchDetailsvm.BrandName && x.InvoiceNumber == addBatchDetailsvm.InvoiceNumber && x.CategoryName == itemNameexists.Category).FirstOrDefaultAsync();


                    itemclass.CategoryName = itemNameexists.Category;

                    if (itemexists != null)
                    {
                        return new StockResponse(false, $" Invoice {addBatchDetailsvm.InvoiceNumber} with'{addBatchDetailsvm.BrandName}-{addBatchDetailsvm.ItemName} already exists ", null);
                    }


                    itemclass.Reference_Number = await GetGeneratedref();
                    itemclass.TotalUnitPrice = itemclass.UnitPrice * itemclass.Quantity;

                    if (itemclass.CategoryName == "Accesory")
                    {
                        itemclass.Status = "Complete";
                    }
                    else
                    {
                        itemclass.Status = "Incomplete";
                    }




                    itemclass.WarrantyEndDate = itemclass.WarrantyStartDate.AddMonths(itemclass.Warranty);
                    await scopedcontext.AddAsync(itemclass);
                    await scopedcontext.SaveChangesAsync();
                    if (itemclass.CategoryName == "Product")
                    {
                        var new_numb = 0;



                        while (new_numb < itemclass.Quantity)
                        {
                            new_numb++;
                            var new_numbering = new ProductNumbering
                            {

                                NumberValue = new_numb,
                                Reference_Number = itemclass.Reference_Number,
                                Type = "Product",
                                Status = "UNASSIGNED"


                            };







                            await _dragonFlyContext.AddAsync(new_numbering);
                            await _dragonFlyContext.SaveChangesAsync();
                        }
                    }

                    return new StockResponse(true, $"Item '{addBatchDetailsvm.BrandName}-{addBatchDetailsvm.ItemName}' in invoice{addBatchDetailsvm.InvoiceNumber}  created successfully", null);

                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }

        }
        public async Task<StockResponse> GetPriceRecordPerPriceID(int itemPriceId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    // Fetch price record by itemPriceId
                    var priceHistory = await context.PriceRecord
                        .Where(p => p.ItemPriceID == itemPriceId)
                        .OrderBy(p => p.EffectiveFrom)
                        .ToListAsync();

                    if (priceHistory == null || !priceHistory.Any())
                        return new StockResponse(false, "No price history found for this item.", null);

                    var currentDate = DateTime.Now; // If EffectiveFrom is saved in local time


                    foreach (var price in priceHistory)
                    {
                        if (price.IsDeleted)
                        {
                            price.Status = "Soft Delete";
                        }
                        else if (price.EffectiveFrom > currentDate)
                        {
                            price.Status = "Pending";
                        }
                        else if (price.EffectiveTo.HasValue && price.EffectiveTo.Value < currentDate)
                        {
                            price.Status = "Expired";
                        }
                        else
                        {
                            price.Status = "Active";
                        }
                    }


                    // Save updated statuses to DB
                    await context.SaveChangesAsync();

                    return new StockResponse(true, "Price history retrieved successfully.", priceHistory);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, $"An error occurred: {ex.Message}", null);
            }
        }

        public async Task<StockResponse> AddPriceRecord(PriceRecordVm priceVm)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(priceVm.ItemId))
                    return new StockResponse(false, "Item ID is required.", null);

                if (priceVm.SellingPrice <= 0)
                    return new StockResponse(false, "Selling price must be greater than zero.", null);

                if (string.IsNullOrWhiteSpace(priceVm.Currency))
                    return new StockResponse(false, "Currency is required.", null);

                if (priceVm.EffectiveFrom == default)
                    return new StockResponse(false, "Effective From date is required.", null);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    // Check for overlap per item and currency with only active, non-deleted records
                    var existingOverlap = await context.PriceRecord
                        .Where(p => p.ItemId == priceVm.ItemId &&
                                    p.Currency == priceVm.Currency &&
                                    !p.IsDeleted && // Only consider non-deleted records
                                    p.Status == "Active" && // Only consider active records
                                    (
                                        (p.EffectiveTo == null && priceVm.EffectiveTo == null) || // Both open-ended
                                        (p.EffectiveTo == null && priceVm.EffectiveFrom <= p.EffectiveFrom) || // New overlaps open-ended
                                        (priceVm.EffectiveTo == null && p.EffectiveFrom <= priceVm.EffectiveFrom) || // New open-ended overlaps existing
                                        (p.EffectiveTo != null && priceVm.EffectiveTo != null &&
                                         priceVm.EffectiveFrom <= p.EffectiveTo &&
                                         priceVm.EffectiveTo >= p.EffectiveFrom) // Ranges overlap
                                    ))
                        .FirstOrDefaultAsync();

                    if (existingOverlap != null)
                    {
                        return new StockResponse(
                            false,
                            $"There is already an active price in {priceVm.Currency} for this item in the given date range.",
                            null
                        );
                    }


                    // Create and save new price record
                    var priceRecord = new PriceTable
                    {
                        ItemId = priceVm.ItemId,
                        SellingPrice = priceVm.SellingPrice,
                        Currency = priceVm.Currency,
                        EffectiveFrom = priceVm.EffectiveFrom,
                        EffectiveTo = priceVm.EffectiveTo,
                        AddedBy = priceVm.AddedBy,
                        DateAdded = DateTime.UtcNow,
                        Status = (priceVm.EffectiveTo == null || priceVm.EffectiveFrom <= DateTime.UtcNow)
                                 ? "Active"
                                 : "Pending"
                    };

                    await context.PriceRecord.AddAsync(priceRecord);
                    await context.SaveChangesAsync();

                    return new StockResponse(true, "Price record added successfully.", null);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> GetPriceHistory(string itemId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(itemId))
                    return new StockResponse(false, "Item ID is required.", null);

                if (!int.TryParse(itemId, out var itemIdInt))
                    return new StockResponse(false, "Invalid Item ID format.", null);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    // Fetch the item data
                    var itemData = await context.AddItem
                        .Where(p => p.ItemID == itemIdInt)
                        .Select(p => new
                        {
                            p.ItemName,
                            p.BrandName // optional
                        })
                        .FirstOrDefaultAsync();

                    if (itemData == null)
                        return new StockResponse(false, "Item not found.", null);

                    // Fetch price history

                    var currentDate = DateTime.Now;
                    var priceHistory = await context.PriceRecord
                        .Where(p => p.ItemId == itemId)  // Fix the filter to itemIdInt (not itemId which is string)
                        .OrderByDescending(p => p.EffectiveFrom)
                        .Select(p => new
                        {
                            p.ItemPriceID,
                            p.ItemId,
                            ItemName = itemData.BrandName + " " + itemData.ItemName,
                            p.SellingPrice,
                            p.Currency,
                            p.EffectiveFrom,
                            EffectiveTo = p.EffectiveTo.HasValue ? p.EffectiveTo : null,
                            p.AddedBy,
                            p.DateAdded,
                            Status = p.IsDeleted ? "Soft Delete" :
             (p.EffectiveFrom > currentDate) ? "Pending" :
             (p.EffectiveTo.HasValue && p.EffectiveTo.Value < currentDate) ? "Expired" :
             "Active"
                        })
                        .ToListAsync();

                    if (priceHistory == null || priceHistory.Count == 0)
                        return new StockResponse(false, "No price history found for this item.", null);

                    return new StockResponse(true, "Price history fetched successfully.", priceHistory);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> ReactivatePriceRecord(int priceRecordId, DateTime newEffectiveFrom, DateTime? newEffectiveTo, string reactivatedBy)
        {
            try
            {
                // Validate EffectiveFrom date
                if (newEffectiveFrom == default)
                    return new StockResponse(false, "Effective From date is required.", null);

                // Check if EffectiveTo is set correctly
                if (newEffectiveTo.HasValue && newEffectiveTo < newEffectiveFrom)
                    return new StockResponse(false, "'Effective To' date cannot be earlier than 'Effective From'.", null);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    // Find the price record
                    var priceRecord = await context.PriceRecord
                        .FirstOrDefaultAsync(p => p.ItemPriceID == priceRecordId);

                    if (priceRecord == null)
                        return new StockResponse(false, "Price record not found.", null);

                    // Validate that it's a deactivated/inactive record
                    if (!priceRecord.IsDeleted && priceRecord.EffectiveTo == null)
                        return new StockResponse(false, "This price record is already active.", null);

                    // Check for overlapping active price ranges for the same item and currency
                    var overlapping = await context.PriceRecord
                        .Where(p => p.ItemPriceID != priceRecordId &&
                                    p.ItemId == priceRecord.ItemId &&
                                    p.Currency == priceRecord.Currency &&
                                    !p.IsDeleted &&
                                    (
                                        (p.EffectiveTo == null && newEffectiveTo == null) || // Both are open-ended
                                        (p.EffectiveTo == null && newEffectiveFrom <= p.EffectiveFrom) || // New overlaps open-ended
                                        (newEffectiveTo == null && p.EffectiveFrom <= newEffectiveFrom) || // New open-ended overlaps existing
                                        (p.EffectiveTo != null && newEffectiveTo != null &&
                                         newEffectiveFrom <= p.EffectiveTo &&
                                         newEffectiveTo >= p.EffectiveFrom) // Ranges overlap
                                    ))
                        .FirstOrDefaultAsync();

                    if (overlapping != null)
                        return new StockResponse(false, $"There is already an active price in {priceRecord.Currency} for this item in the given date range.", null);

                    // Reactivate the price record
                    priceRecord.EffectiveFrom = newEffectiveFrom;
                    priceRecord.EffectiveTo = newEffectiveTo;
                    priceRecord.IsDeleted = false; // Mark as not deleted
                    priceRecord.Status = newEffectiveFrom <= DateTime.Now ? "Active" : "Pending"; // Status depends on the date
                    priceRecord.ReactivatedBy = reactivatedBy;
                    priceRecord.DateReactivated = DateTime.UtcNow;

                    // Update the record in the database
                    context.PriceRecord.Update(priceRecord);
                    await context.SaveChangesAsync();

                    return new StockResponse(true, "Price record reactivated successfully.", null);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, $"An error occurred: {ex.Message}", null);
            }
        }

        public async Task<StockResponse> EditPriceRecord(int itemPriceId, decimal? newSellingPrice, string newCurrency, DateTime? newEffectiveFrom, DateTime? newEffectiveTo, string updatedBy)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    // Fetch the price record
                    var priceRecord = await context.PriceRecord
                        .Where(p => p.ItemPriceID == itemPriceId && !p.IsDeleted) // Ensure the price is not marked as deleted
                        .FirstOrDefaultAsync();

                    if (priceRecord == null)
                        return new StockResponse(false, "Price record not found.", null);

                    // Only update fields if new values are provided, else retain old values
                    if (newSellingPrice.HasValue)
                        priceRecord.SellingPrice = newSellingPrice.Value;  // Update SellingPrice if new value is provided

                    if (!string.IsNullOrWhiteSpace(newCurrency))
                        priceRecord.Currency = newCurrency;  // Update Currency if new value is provided

                    priceRecord.EffectiveFrom = newEffectiveFrom ?? priceRecord.EffectiveFrom;  // Only update EffectiveFrom if new value is provided
                    priceRecord.EffectiveTo = newEffectiveTo ?? priceRecord.EffectiveTo;  // Only update EffectiveTo if new value is provided

                    // Update the updated timestamp and who updated the record
                    priceRecord.DateUpdated = DateTime.Now;
                    priceRecord.UpdatedBy = updatedBy;

                    // Save changes
                    await context.SaveChangesAsync();

                    return new StockResponse(true, "Price record updated successfully.", priceRecord);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> ActivatePriceRecord(int itemPriceId, string activatedBy)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    // Fetch the price record
                    var priceRecord = await context.PriceRecord
                        .Where(p => p.ItemPriceID == itemPriceId && !p.IsDeleted) // Ensure the price is not marked as deleted
                        .FirstOrDefaultAsync();

                    if (priceRecord == null)
                        return new StockResponse(false, "Price record not found.", null);

                    // Set status to "Active"
                    priceRecord.Status = "Active";

                    // You can also adjust EffectiveFrom and EffectiveTo here if necessary
                    priceRecord.EffectiveFrom = DateTime.Now; // Or some other date logic
                    priceRecord.EffectiveTo = null;  // Reset EffectiveTo for an active record

                    // Set the activation details
                    priceRecord.DateActivated = DateTime.Now;
                    priceRecord.ActivatedBy = activatedBy;

                    // Save changes
                    await context.SaveChangesAsync();

                    return new StockResponse(true, "Price record activated successfully.", priceRecord);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> DeletePriceRecord(int itemPriceId, string deletedBy)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    // Fetch the price record
                    var priceRecord = await context.PriceRecord
                        .Where(p => p.ItemPriceID == itemPriceId && !p.IsDeleted) // Ensure the price is not marked as deleted
                        .FirstOrDefaultAsync();

                    if (priceRecord == null)
                        return new StockResponse(false, "Price record not found.", null);

                    // Mark the record as deleted
                    priceRecord.IsDeleted = true;

                    // Set the deletion details
                    priceRecord.DateDeleted = DateTime.Now;
                    priceRecord.DeletedBy = deletedBy;
                    priceRecord.Status = "Soft Delete";

                    // Save changes
                    await context.SaveChangesAsync();

                    return new StockResponse(true, "Price record deleted successfully.", priceRecord);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }


        public async Task<StockResponse> GetInvoiceLines()
        {
            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allinvoice = await scopedcontext.InvoiceLinesDetails.OrderByDescending(y => y.UpdatedOn).ToListAsync();

                    if (allinvoice == null)
                    {
                        return new StockResponse(false, "Invoice doesn't exist", null);
                    }

                    return new StockResponse(true, "Successfully queried", allinvoice);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> AddProductDetails(AddProductDetailvm addProductDetailvm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(addProductDetailvm.SerialNumber))
                {
                    return new StockResponse(false, "Kindly provide serial number", null);
                }

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var batchdetails = await scopedcontext.AddDeliveryNote
                        .Where(x => x.BatchNumber == addProductDetailvm.BatchNumber)
                        .FirstOrDefaultAsync();
                    var PONumberData = await scopedcontext.UploadPOFile
                       .Where(x => x.PONumber == batchdetails.PONumber)
                       .FirstOrDefaultAsync();

                    if (batchdetails == null)
                    {
                        return new StockResponse(false, "Batch does not exist", null);
                    }

                    var serialNumbersCount = await scopedcontext.AddProductDetails
                        .Where(y => y.BatchNumber == addProductDetailvm.BatchNumber)
                        .CountAsync();

                    if (serialNumbersCount + 1 > batchdetails.BatchQuantity)
                    {
                        return new StockResponse(false, $"Serial numbers count ({serialNumbersCount + 1}) exceeds the desired quantity ({batchdetails.BatchQuantity}).", null);
                    }
                    var getItemID = await scopedcontext.AddItem
       .Where(x => x.BrandName == batchdetails.BrandName && x.ItemName == batchdetails.ItemName)
       .FirstOrDefaultAsync();

                    var itemclass = new AddProductDetails
                    {
                        SerialNumber = addProductDetailvm.SerialNumber,
                        IMEI1 = addProductDetailvm.IMEI1 ?? "N/A",
                        IMEI2 = addProductDetailvm.IMEI2 ?? "N/A",
                        ItemID = batchdetails.ItemID,
                        SerialStatus = "Not Issued",
                        BatchNumber = addProductDetailvm.BatchNumber,
                        ItemStatus = "Okay",
                        Quantity = batchdetails.BatchQuantity,
                        ItemName = batchdetails.ItemName,
                        BrandName = batchdetails.BrandName,
                        BatchStatus = batchdetails.ProductStatus,
                        PONumber = batchdetails.PONumber,
                        IsIssued =false,
                        IssuedBy ="None",
                        ReferenceNumber ="None",
                        ClientName ="None",
                        ItemIDdetails = getItemID.ItemID,
                        WarrantyStartDate = PONumberData.WarrantyStartDate,
                        WarrantyEndDate = PONumberData.WarrantyEndDate,
                        
                    };
                    itemclass.WarrantyStatus = itemclass.WarrantyEndDate < DateTime.Now
      ? "Expired"
      : "Under Warranty";

                    var serialExists = await scopedcontext.AddProductDetails
                        .FirstOrDefaultAsync(y => y.SerialNumber == itemclass.SerialNumber);

                    if (serialExists != null)
                    {
                        return new StockResponse(false, $"Serial Number '{itemclass.SerialNumber}' already exists", null);
                    }

               

                    // Check IMEI2 if it's not null/empty and not the placeholder
                    // Check IMEI1 if it's not null/empty and not a placeholder
                    if (!string.IsNullOrWhiteSpace(itemclass.IMEI1) && itemclass.IMEI1 != "N/A" && itemclass.IMEI1 != "None")
                    {
                        var imei1Exists = await scopedcontext.AddProductDetails
                            .FirstOrDefaultAsync(y => y.IMEI1 == itemclass.IMEI1);

                        if (imei1Exists != null)
                        {
                            return new StockResponse(false, $"IMEI1 '{itemclass.IMEI1}' already exists", null);
                        }
                    }

                    // Check IMEI2 if it's not null/empty and not a placeholder
                    if (!string.IsNullOrWhiteSpace(itemclass.IMEI2) && itemclass.IMEI2 != "N/A" && itemclass.IMEI2 != "None")
                    {
                        var imei2Exists = await scopedcontext.AddProductDetails
                            .FirstOrDefaultAsync(y => y.IMEI2 == itemclass.IMEI2);

                        if (imei2Exists != null)
                        {
                            return new StockResponse(false, $"IMEI2 '{itemclass.IMEI2}' already exists", null);
                        }
                    }


                    await scopedcontext.AddAsync(itemclass);

                    if (serialNumbersCount + 1 == itemclass.Quantity)
                    {
                        itemclass.ProductStatus = "Complete";
                        batchdetails.ProductStatus = "Complete";
                        scopedcontext.Update(batchdetails);
                    }

                    await scopedcontext.SaveChangesAsync();

                    return new StockResponse(true, $"Item '{itemclass.SerialNumber}' created successfully", null);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<BaseResponse> GetProductDetailsBySerialNumber(string serialNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(serialNumber))
                {
                    return new BaseResponse("400", "Serial number is required.", null);
                }

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var product = await scopedcontext.AddProductDetails
                        .FirstOrDefaultAsync(p => p.SerialNumber == serialNumber);

                    if (product == null)
                    {
                        return new BaseResponse("404", "Serial number not found.", null);
                    }

                    var item = await scopedcontext.AddItem
                        .FirstOrDefaultAsync(i => i.ItemID == product.ItemID);

                    string category = item?.Category ?? "Device";

                    string warrantyStatus;
                    int isProductUnderWarranty = 0;

                    bool hasStandardWarranty = product.WarrantyStartDate != DateTime.MinValue &&
                                               product.WarrantyEndDate != DateTime.MinValue;

                    bool isDeviceWarrantyExpired = !hasStandardWarranty || product.WarrantyEndDate < DateTime.Now;

                    if (!isDeviceWarrantyExpired)
                    {
                        warrantyStatus = "Under Warranty";
                        isProductUnderWarranty = 1;
                    }
                    else if (category == "Spare Parts" &&
                             product.WarrantyStartDate != DateTime.MinValue &&
                             DateTime.Now <= product.WarrantyStartDate.AddMonths(1))
                    {
                        warrantyStatus = "Under Spare Part Warranty";
                    }
                    else
                    {
                        warrantyStatus = hasStandardWarranty ? "Expired" : "None";
                    }

                    var allSpareParts = await scopedcontext.AddItem
                        .Where(i => i.Category == "Spare Parts")
                        .ToListAsync();

                    var usedSpareParts = await scopedcontext.AddProductDetails
                        .Where(p => allSpareParts.Select(i => i.ItemID).Contains(p.ItemID))
                        .ToListAsync();

                    var sparePartsList = allSpareParts.Select(sp =>
                    {
                        var usedSpare = usedSpareParts
                            .FirstOrDefault(p => p.ItemID == sp.ItemID && p.ReferenceNumber == product.ReferenceNumber);

                        string spareWarrantyStatus;
                        int isSparePartUnderWarranty = 0;

                        if (usedSpare != null)
                        {
                            spareWarrantyStatus = "Used – Not Under Warranty";
                        }
                        else if (!isDeviceWarrantyExpired)
                        {
                            spareWarrantyStatus = "Under Warranty";
                            isSparePartUnderWarranty = 1;
                        }
                        else if (product.WarrantyEndDate != DateTime.MinValue &&
                                 DateTime.Now <= product.WarrantyEndDate.AddMonths(1))
                        {
                            spareWarrantyStatus = "Under Spare Part Warranty";
                            isSparePartUnderWarranty = 1;
                        }
                        else
                        {
                            spareWarrantyStatus = "Expired – Charges Apply";
                        }

                        return new
                        {
                            sp.ItemID,
                            sp.ItemName,
                            sp.BrandName,
                            sp.IndicativePrice,
                            sp.Currency,
                            WarrantyStatus = spareWarrantyStatus,
                            IsSparePartUnderWarranty = isSparePartUnderWarranty,
                            
                        };
                    }).ToList();

                    var result = new
                    {
                        product.SerialNumber,
                        product.BrandName,
                        product.ItemName,
                        product.IMEI1,
                        product.IMEI2,
                        product.WarrantyStartDate,
                        product.WarrantyEndDate,
                        product.SerialStatus,
                        product.BatchNumber,
                        product.IssuedBy,
                        product.ReferenceNumber,
                        product.ClientName,
                        product.PONumber,
                        Category = category,
                        WarrantyStatus = warrantyStatus,
                        IsProductUnderWarranty = isProductUnderWarranty,
                        SpareParts = sparePartsList
                    };

                    return new BaseResponse("200", "Product found.", result);
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse("500", ex.Message, null);
            }
        }

        public async Task<StockResponse> GetProductDetails()
        {
            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allinvoice = await scopedcontext.AddProductDetails.ToListAsync();

                    if (allinvoice == null)
                    {
                        return new StockResponse(false, "Product doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allinvoice);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetInvoiceByNumber(string InvoiceNumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var invoicenumber = await scopedcontext.StockIn.Where(u => u.InvoiceNumber == InvoiceNumber).FirstOrDefaultAsync();
                    if (invoicenumber == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", invoicenumber);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> DeleteSparePart(int partId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var part = await scopedContext.AddSpareParts.FirstOrDefaultAsync(x => x.PartID == partId);

                    if (part == null)
                    {
                        return new StockResponse(false, "Part not found", null);
                    }

                    scopedContext.AddSpareParts.Remove(part);
                    await scopedContext.SaveChangesAsync();

                    return new StockResponse(true, $"Part '{part.PartName}' deleted successfully", null);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> EditSparePart(editPartVm editPartVm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(editPartVm.PartName))
                {
                    return new StockResponse(false, "Part name cannot be empty", null);
                }

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var existingPart = await scopedContext.AddSpareParts
                        .FirstOrDefaultAsync(x => x.PartID == editPartVm.PartID);

                    if (existingPart == null)
                    {
                        return new StockResponse(false, "Part not found", null);
                    }

                    // Optional: prevent duplicate name
                    var duplicatePart = await scopedContext.AddSpareParts
                        .FirstOrDefaultAsync(x => x.PartName == editPartVm.PartName && x.PartID != editPartVm.PartID);

                    if (duplicatePart != null)
                    {
                        return new StockResponse(false, $"Another part with name '{editPartVm.PartName}' already exists", null);
                    }

                    existingPart.PartName = editPartVm.PartName;
                    existingPart.PartDescription = editPartVm.PartDescription;

                    await scopedContext.SaveChangesAsync();

                    return new StockResponse(true, $"Part '{editPartVm.PartName}' updated successfully", null);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> AddSparePart(AddPartvm addPartvm)
        {
            try
            {
                if (addPartvm.PartName == "")
                {

                    return new StockResponse(false, "Kindly provide a part name to add item", null);
                }
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();



                    var partexists = await scopedcontext.AddPart.Where(x => x.PartName == addPartvm.PartName).FirstOrDefaultAsync();

                    if (partexists != null)
                    {
                        return new StockResponse(false, $" Part  '{addPartvm.PartName}' already exist", null);
                    }
                    var partclass = new AddPart
                    {
                        PartName = addPartvm.PartName,
                        PartDescription = addPartvm.PartDescription,
                    };
                    await scopedcontext.AddAsync(partclass);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, $"Part '{addPartvm.PartName}'  created successfully", null);

                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }

        }
        public async Task<StockResponse> GetAllParts()
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allpart = await scopedcontext.AddSpareParts.ToListAsync();

                    if (allpart == null)
                    {
                        return new StockResponse(false, "Part doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allpart);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }


        public string GenerateReferenceNumber(int length)
        {
            string ValidChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnop";

            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                int randomIndex = random.Next(ValidChars.Length);
                sb.Append(ValidChars[randomIndex]);
            }
            return sb.ToString();
        }

        public async Task<string> GetGeneratedref()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    int length = 8;
                    var paymentref = "PH_invoice_" + GenerateReferenceNumber(length);
                    //check reference exists

                    return paymentref;
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        public async Task<StockResponse> Add_Invoice_Item_Quantity(invoice_item_quantity_vm vm)
        {

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var new_invoicee_item = new Invoice_Item_Quantity
                    {
                        Invoce_No = vm.Invoce_No,
                        Quantity = vm.Quantity,
                        Type = vm.Type
                    };
                    var string_resp = GetGeneratedref().Result;

                    new_invoicee_item.Invoice_quantity_Unique_id = string_resp;





                    await scopedcontext.AddAsync(new_invoicee_item);
                    await scopedcontext.SaveChangesAsync();

                    if (new_invoicee_item.Type == "Product")
                    {
                        int number = 0;
                        while (new_invoicee_item.Quantity > 0)
                        {
                            number++;
                            var new_numbering = new Item_Numbering_Stock
                            {

                                Product_No = number,
                                Invoice_quantity_Id = new_invoicee_item.Invoice_quantity_Unique_id,
                                Status = "UNASSIGNED"


                            };

                        };
                    }
                    return new StockResponse(true, "Successfully updated items on invoice ", null);

                }

            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> AddPartsSpare(AddSparePartvm addSparePartvm)
        {
            try
            {
                if (addSparePartvm.PartName == "")
                {

                    return new StockResponse(false, "Kindly provide a part name ", null);
                }
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    //check if role exists 

                    var brandexists = await scopedcontext.AddSpareParts.Where(x => x.PartName == addSparePartvm.PartName).FirstOrDefaultAsync();

                    if (brandexists != null)
                    {
                        return new StockResponse(false, $" Device  '{addSparePartvm.PartName}' already exist", null);
                    }
                    var brandclass = new AddSpareParts
                    {
                        PartName = addSparePartvm.PartName,
                        PartDescription = addSparePartvm.PartDescription,
                    };
                    await scopedcontext.AddAsync(brandclass);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, $"Spare part '{addSparePartvm.PartName}'  created successfully", null);


                }


            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }

        }
        public async Task<StockResponse> GetAllSpareParts()
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allparts = await scopedcontext.AddSpareParts.ToListAsync();

                    if (allparts == null)
                    {
                        return new StockResponse(false, "spare parts doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allparts);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }

        }
        public async Task<StockResponse> GetSerialByPONumber(string poNumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.AddProductDetails.Where(u => u.PONumber == poNumber).ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetInvoiceLinByNumber(string invoiceNumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.InvoiceLinesDetails.Where(u => u.InvoiceNumber == invoiceNumber).OrderByDescending(u => u.UpdatedOn).ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetInvoiceItemByID(int invoicelineId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.InvoiceLinesDetails.Where(u => u.InvoiceLineId == invoicelineId).FirstOrDefaultAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetProductDetailsbyid(string BatchNumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.AddProductDetails.Where(u => u.BatchNumber == BatchNumber).ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> GetProduct_Numbers_ByReference(string reference)
        {



            try
            {

                var allnumberings = await _dragonFlyContext.ProductNumbering
                    .Where(y => y.Status == "UNASSIGNED" && y.Reference_Number == reference).ToListAsync();
                return new StockResponse(true, "successfully queried", allnumberings);
            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> GetProduvctLineyId(int product_line_id)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var product_exist = await scopedcontext.UploadPOFile.Where(u => u.ID == product_line_id).FirstOrDefaultAsync();
                    if (product_exist == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", product_exist);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        //public async Task<StockResponse> GetAllProductNumbering(int productNumberID)
        //{

        //    try
        //    {

        //        using (var scope = _serviceScopeFactory.CreateScope())
        //        {
        //            var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
        //            var allnumbervalue = await scopedcontext.ProductNumbering.Where(x => x.ProductNumberID==productNumberID).FirstOrDefaultAsync();

        //            if (allnumbervalue == null)
        //            {
        //                return new StockResponse(false, "Product number doesnt exist", null);
        //            }
        //            List<AllProductNumberingm> productnumbering = new List<AllProductNumberingm>();
        //            int maxNumber = productnumbering.Max(n => n.NumberValue);
        //            var batchdetails=await scopedcontext.InvoiceLinesDetails.Where(x=>x.Quantity==maxNumber).FirstOrDefaultAsync();
        //            if (maxNumber == batchdetails.Quantity)
        //            {
        //                batchdetails.Status = "COMPLETE";
        //            }


        //                                return new StockResponse(true, "Successfully queried", productnumbering);

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return new StockResponse(false, ex.Message, null);
        //    }
        //}
        public async Task<StockResponse> SearchForInvoice(string search_query)
        {

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var allstock = await scopedcontext.StockIn.Where
                        (u => EF.Functions.Like(u.LPONumber, $"%{search_query}%") ||
                        EF.Functions.Like(u.InvoiceNumber, $"%{search_query}%")
                        ).ToListAsync();

                    if (allstock == null)
                        return new StockResponse(false, "", null);

                    return new StockResponse(true, "Successfully queried", allstock);


                }
            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> SearchForItem(string search_query)
        {

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var allstock = await scopedcontext.AddItem.Where
                        (u => EF.Functions.Like(u.BrandName, $"%{search_query}%") ||
                        EF.Functions.Like(u.ItemName, $"%{search_query}%") ||
                        EF.Functions.Like(u.ItemDescription, $"%{search_query}%") ||
                        EF.Functions.Like(u.Category, $"%{search_query}%") ||
                        EF.Functions.Like(u.Currency, $"%{search_query}%")
                        ).ToListAsync();

                    if (allstock == null)
                        return new StockResponse(false, "", null);

                    return new StockResponse(true, "Successfully queried", allstock);


                }
            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetItemsbyBrandName(string BrandName)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.AddItem.Where(u => u.BrandName == BrandName).ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> SearchForInvoiceLines(string search_query)
        {

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var allstock = await scopedcontext.InvoiceLinesDetails.Where
                        (u => EF.Functions.Like(u.ItemName, $"%{search_query}%") ||
                        EF.Functions.Like(u.CategoryName, $"%{search_query}%") ||
                          EF.Functions.Like(u.Currency, $"%{search_query}%") ||
                            EF.Functions.Like(u.UpdatedBy, $"%{search_query}%") ||
                               EF.Functions.Like(u.BrandName, $"%{search_query}%") ||

                              EF.Functions.Like(u.Status, $"%{search_query}%")

                        ).ToListAsync();

                    if (allstock == null)
                        return new StockResponse(false, "", null);

                    return new StockResponse(true, "Successfully queried", allstock);


                }
            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> UploadData([FromBody] List<uploadDatavm> data)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    foreach (var item in data)
                    {
                        var new_product_item = new AddProductDetails
                        {
                            IMEI1 = item.IMEI1,
                            IMEI2 = item.IMEI2,
                            SerialNumber = item.SerialNumber,
                        };


                        await scopedcontext.AddAsync(new_product_item);
                        await scopedcontext.SaveChangesAsync();

                    }


                    return new StockResponse(true, "Added successful", null);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }


        }
        public async Task<StockResponse> UploadingData(IFormFile file, string BatchNumber, int batchID)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                  


                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                            if (worksheet != null)
                            {
                                var serialNumbers = new List<string>();
                                // Fetch the associated AddDeliveryNote
                                var batchdetails = await scopedcontext.AddDeliveryNote
                                    .Where(x => x.BatchNumber == BatchNumber)
                                    .FirstOrDefaultAsync();
                                var PONumberData = await scopedcontext.UploadPOFile
   .Where(x => x.PONumber == batchdetails.PONumber)
   .FirstOrDefaultAsync();

                                if (batchdetails == null)
                                {
                                    return new StockResponse(false, "Batch does not exist", null);
                                }
                                var getItemID = await scopedcontext.AddItem
.Where(x => x.BrandName == batchdetails.BrandName && x.ItemName == batchdetails.ItemName)
.FirstOrDefaultAsync();
                                int rowCount = worksheet.Dimension.Rows;
                                for (int row = 2; row <= rowCount; row++) // Assuming the first row contains headers
                                {
                                    var serialNumber = worksheet.Cells[row, 1].Value?.ToString();
                                    var imei1 = worksheet.Cells[row, 2].Value?.ToString();
                                    var imei2 = worksheet.Cells[row, 3].Value?.ToString();

                                    if (!string.IsNullOrEmpty(serialNumber))
                                    {
                                        serialNumbers.Add(serialNumber);

                                        // Your existing code to add products with serial numbers and IMEI values goes here
                                        try
                                        {
                                            var product = new AddProductDetails
                                            {
                                                SerialNumber = serialNumber,
                                                IMEI1 = imei1,
                                                IMEI2 = imei2,
                                                BatchNumber = BatchNumber,
                                                ItemID = batchID,
                                                BatchStatus=batchdetails.ProductStatus,
                                                BrandName=batchdetails.BrandName,
                                                ItemName=batchdetails.ItemName,
                                                PONumber = batchdetails.PONumber,
                                                SerialStatus= "Not Issued",
                                                Quantity = batchdetails.BatchQuantity,
                                                IsIssued = false,
                                                IssuedBy = "None",
                                                ItemIDdetails = getItemID.ItemID,
                                                ReferenceNumber = "None",
                                                ClientName = "None",
                                                ItemStatus ="Okay",
                                                WarrantyStartDate = PONumberData.WarrantyStartDate,
                                                WarrantyEndDate = PONumberData.WarrantyEndDate,



                                            };
                                            product.WarrantyStatus = product.WarrantyEndDate < DateTime.Now
? "Expired"
: "Under Warranty";
                                            var TotalDelivered = await scopedcontext.UploadPOFile
                                       .Where(x => x.ID == product.ItemID)
                                       .FirstOrDefaultAsync();

                                            if (TotalDelivered == null)
                                            {
                                                return new StockResponse(false, "PO Item doesn't exist", null);
                                            }

                                            if (TotalDelivered.TotalDelivered == TotalDelivered.Quantity)
                                            {
                                                TotalDelivered.ProductStatus = "Complete";
                                            }
                                            else
                                            {
                                                TotalDelivered.ProductStatus = "Incomplete";
                                            }

                                            scopedcontext.Update(TotalDelivered);
                                            var checkexistence = await scopedcontext.AddProductDetails
                                 .Where(x => x.SerialNumber == product.SerialNumber)
                                 .FirstOrDefaultAsync();
                                            if (checkexistence != null)
                                            {
                                                return new StockResponse(false, "Serial number " + serialNumber + " already exists.", null);
                                            }
                                            var imeiexistence = await scopedcontext.AddProductDetails
                                                                      .Where(x => x.IMEI1 == product.IMEI1)
                                                                      .FirstOrDefaultAsync();
                                            if (imeiexistence != null)
                                            {
                                                return new StockResponse(false, "IMEI1 " + imei1 + " already exists.", null);
                                            }
                                            var imei2existence = await scopedcontext.AddProductDetails
                                                                    .Where(x => x.IMEI2 == product.IMEI2)
                                                                    .FirstOrDefaultAsync();
                                            if (imei2existence != null)
                                            {
                                                return new StockResponse(false, "IMEI1 " + imei2 + " already exists.", null);
                                            }

                                            // Check if either of the IMEIs already exists
                                            //if (checkexistence.Contains(imei1) || checkexistence.Contains(imei2))
                                            //{
                                            //    return new StockResponse(false, "IMEI1 or IMEI2 already exists.", null);
                                            //}
                                            // Set other properties of product as needed

                                            //product.ItemID = product.BatchID;
                                            await scopedcontext.AddAsync(product);
                                            Console.WriteLine("Uploaded serial number: " + serialNumber);
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Error uploading serial number " + serialNumber + ": " + ex.Message);
                                        }
                                    }
                                }

                                if (serialNumbers.Count == 0)
                                {
                                    return new StockResponse(false, "No serial numbers uploaded.", null);
                                }

                                // Fetch all existing serial numbers for the given batch number

                                var existingSerialNumbers = await scopedcontext.AddProductDetails
                                .Where(x => x.BatchNumber == BatchNumber)
                                .Select(x => x.SerialNumber)
                                .ToListAsync();
                                // Add the newly uploaded serial numbers to the existing list
                                existingSerialNumbers.AddRange(serialNumbers);

                                // Update the delivery note's ProductStatus based on the total count
                                var deliveryNote = await scopedcontext.AddDeliveryNote.FirstOrDefaultAsync(x => x.BatchNumber == BatchNumber);
                                
                                if (deliveryNote != null)
                                {
                                    int batchQuantityInDatabase = deliveryNote.BatchQuantity;
                                    if (existingSerialNumbers.Count >= batchQuantityInDatabase)
                                    {
                                        deliveryNote.ProductStatus = "Complete";
                                        scopedcontext.Update(deliveryNote);
                                    }
                                    else
                                    {
                                        deliveryNote.ProductStatus = "Incomplete";
                                        scopedcontext.Update(deliveryNote);
                                    }
                                   

                                    // Check if the total count exceeds the batch quantity
                                    if (existingSerialNumbers.Count > batchQuantityInDatabase)
                                    {
                                        //await scopedcontext.SaveChangesAsync();
                                        return new StockResponse(false, "Total serial numbers count cannot exceed the specified quantity for BatchNumber: " + BatchNumber, null);
                                    }
                                    

                                    await scopedcontext.SaveChangesAsync();

                                    return new StockResponse(true, "Uploaded successfully", serialNumbers);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }

            return new StockResponse(false, "Invalid file or file format.", null);
        }
        public async Task<StockResponse> AddReturnedStock(ReturnedStockvm returnedStockvm)
        {
            try
            {
                if (returnedStockvm.ReturnedBy == "")
                {
                    return new StockResponse(false, "Kindly provide a returned by person ", null);
                }
                if (returnedStockvm.ReasonForReturn == "")
                {
                    return new StockResponse(false, "Kindly provide a reason for return ", null);
                }
                if (returnedStockvm.ReturnedCondition == "")
                {
                    return new StockResponse(false, "Kindly provide a returned condition for the item ", null);
                }

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    if (returnedStockvm.CategoryName == "Product")
                    {
                        var matchingImei = await scopedcontext.AddProductDetails
                            .FirstOrDefaultAsync(u => u.SerialNumber == returnedStockvm.SerialNumber);

                        if (matchingImei == null)
                        {
                            return new StockResponse(false, $"Serial number {returnedStockvm.SerialNumber} doesn't exist", null);
                        }

                        var returnClass = new ReturnedItem
                        {
                            ReturnedCondition = returnedStockvm.ReturnedCondition,
                            ReasonForReturn = returnedStockvm.ReasonForReturn,
                            ReturnedBy = returnedStockvm.ReturnedBy,
                            DateReturned = DateTime.Now,
                            ReturnedQuantity = returnedStockvm.ReturnedQuantity,
                            IssuedId = returnedStockvm.IssuedId,
                            ReturnedStatus = "Issued",
                            SerialNumber = returnedStockvm.SerialNumber,
                            FaultyQuantity = 0,
                            SerialFaulty = "None",
                            FaultyDescription = "None",
                            IMEI1 = matchingImei.IMEI1,
                            IMEI2 = matchingImei.IMEI2,
                            CategoryName=returnedStockvm.CategoryName,
                        };

                        var IssueDetails = await scopedcontext.SelectSerial.Where(u => u.IssueID == returnClass.IssuedId).FirstOrDefaultAsync();
                        if (IssueDetails != null)
                        {
                            if (returnClass.ReturnedQuantity > IssueDetails.QuantityDispatched)
                            {
                                return new StockResponse(false, "The quantity returned cant be more than the dispatched quantity", null);
                            }
                        }
                        returnClass.BrandName = IssueDetails.BrandName;
                        returnClass.ItemName = IssueDetails.ItemName;
                        await scopedcontext.AddAsync(returnClass);
                        await scopedcontext.SaveChangesAsync();
                        return new StockResponse(true, "Item returned successfully please wait for approval", null);
                    }
                    else
                    {
                        var returnClass = new ReturnedItem
                        {
                            ReturnedCondition = returnedStockvm.ReturnedCondition,
                            ReasonForReturn = returnedStockvm.ReasonForReturn,
                            ReturnedBy = returnedStockvm.ReturnedBy,
                            DateReturned = DateTime.Now,
                            ReturnedQuantity = returnedStockvm.ReturnedQuantity,
                            IssuedId = returnedStockvm.IssuedId,
                            ReturnedStatus = "Issued",
                            SerialNumber = returnedStockvm.SerialNumber,
                            FaultyQuantity = 0,
                            SerialFaulty = "None",
                            FaultyDescription = "None",
                            CategoryName=returnedStockvm.CategoryName,
                        };

                        var IssueDetails = await scopedcontext.SelectSerial.Where(u => u.IssueID == returnClass.IssuedId).FirstOrDefaultAsync();
                        if (IssueDetails != null)
                        {
                            if (returnClass.ReturnedQuantity > IssueDetails.QuantityDispatched)
                            {
                                return new StockResponse(false, "The quantity returned cant be more than the dispatched quantity", null);
                            }
                        }
                        returnClass.BrandName = IssueDetails.BrandName;
                        returnClass.ItemName = IssueDetails.ItemName;
                        await scopedcontext.AddAsync(returnClass);
                        await scopedcontext.SaveChangesAsync();
                        return new StockResponse(true, "Item returned successfully please wait for approval", null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }





        public async Task<StockResponse> EditSerialNumber(EditSerialNumbervm editSerialNumbervm)
        {
            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var salesexists = await scopedcontext.AddProductDetails.Where(u => u.BatchID == editSerialNumbervm.ItemID).FirstOrDefaultAsync();
                    var itemexists = await scopedcontext.AddProductDetails
                    .Where(y => y.SerialNumber == editSerialNumbervm.SerialNumber || y.IMEI1 == editSerialNumbervm.IMEI1 || y.IMEI2 == editSerialNumbervm.IMEI2).FirstOrDefaultAsync();
                    if (itemexists != null)
                    {
                        return new StockResponse(false, $"Serial Number '{editSerialNumbervm.SerialNumber}'  already exists", null);

                    }


                    if (salesexists == null)
                    {

                        return new StockResponse(false, "product does not exist", null);
                    }

                    if (editSerialNumbervm.SerialNumber == "string")
                    {
                        salesexists.SerialNumber = salesexists.SerialNumber;

                    }
                    else
                    {
                        salesexists.SerialNumber = editSerialNumbervm.SerialNumber;
                    }


                    if (editSerialNumbervm.IMEI1 == "")
                    {
                        salesexists.IMEI1 = salesexists.IMEI1;

                    }
                    else
                    {
                        salesexists.IMEI1 = editSerialNumbervm.IMEI1;
                    }
                    if (editSerialNumbervm.IMEI2 == "")
                    {
                        salesexists.IMEI2 = salesexists.IMEI2;

                    }
                    else
                    {
                        salesexists.IMEI2 = editSerialNumbervm.IMEI2;
                    }

                    //    if (itemexists != null)
                    //    {
                    //        return new StockResponse(false, $"Serial Number '{editSerialNumbervm.SerialNumber}' already exists", null);

                    //    }
                    //    var imei1exists = await scopedcontext.AddProductDetails
                    //   .Where(y => y.IMEI1 == editSerialNumbervm.IMEI1).FirstOrDefaultAsync();

                    //    if (imei1exists != null)
                    //    {
                    //        return new StockResponse(false, $"IMEI1 '{editSerialNumbervm.IMEI1}' already exists", null);

                    //    }
                    //    var imei2exists = await scopedcontext.AddProductDetails
                    //.Where(y => y.IMEI2 == editSerialNumbervm.IMEI2).FirstOrDefaultAsync();

                    //    if (imei2exists != null)
                    //    {
                    //        return new StockResponse(false, $"IMEI2 '{editSerialNumbervm.IMEI2}' already exists", null);

                    //    }

                    scopedcontext.Update(salesexists);
                    await scopedcontext.SaveChangesAsync();

                    return new StockResponse(true, "Sucessfully updated product details", salesexists);


                }

            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> GetAllSerialNumbers()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var serialNumbers = await scopedcontext.AddProductDetails
                        .Where(u => !string.IsNullOrEmpty(u.SerialNumber)) // Optional: skip empty/null serials
                        .Select(u => new
                        {
                            u.SerialNumber,
                            u.ItemName,
                            u.ItemID,
                            u.BrandName,
                            u.IMEI1,
                            u.IMEI2,
                            u.ProductStatus,
                            u.SerialStatus,
                            u.BatchNumber,
                            u.WarrantyStatus
                        })
                        .ToListAsync();

                    if (serialNumbers == null || serialNumbers.Count == 0)
                    {
                        return new StockResponse(false, "No serial numbers found", null);
                    }

                    return new StockResponse(true, "Serial numbers retrieved successfully", serialNumbers);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> GetSerialNumberbyid(int itemID)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var product_exist = await scopedcontext.AddProductDetails.Where(u => u.BatchID == itemID).FirstOrDefaultAsync();
                    if (product_exist == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", product_exist);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetSerialNumberByIssueId(int issueid)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var product_exist = await scopedcontext.selectedSetialNumber.Where(u => u.IssuedNo == issueid ).ToListAsync();
                    if (product_exist == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", product_exist);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> PostScannedData([FromBody] ScannedDataModel data)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    // Process the scanned data
                    var serialNumber = data.SerialNumber;
                    var imei1 = data.IMEI1;
                    var imei2 = data.IMEI2;
                }


                // Perform further actions with the scanned data


                return new StockResponse(true, "Successfully scanned the details", null);
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> Upload([FromBody] PODetailsvm pODetailsvm)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();


                var new_podetails = new PODetails
                {
                    PONumber = pODetailsvm.PONumber,
                    PODate = Convert.ToDateTime(pODetailsvm.PODate),
                    Vendor = pODetailsvm.Vendor,
                };
                await scopedcontext.AddAsync(new_podetails);
                await scopedcontext.SaveChangesAsync();
                return new StockResponse(true, "Uploaded file successfully", new_podetails);

            }


            // Save the student data to the database or perform any other actions
            // For simplicity, let's just return the received data as a response

            //return new StockResponse(true, "Uploaded data successfully", null);

        }
        public async Task<StockResponse> UploadingPODetails(PODetailsvm pODetailsvm)
        {
            try
            {
                if (pODetailsvm.PONumber == "")
                {

                    return new StockResponse(false, "Kindly upload a file", null);
                }
                if (pODetailsvm.PODate == "")
                {

                    return new StockResponse(false, "Kindly upload a file", null);
                }
                if (pODetailsvm.Vendor == "")
                {

                    return new StockResponse(false, "Kindly provide a vendor name to add po", null);
                }
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    //check if role exists 

                    var poexists = await scopedcontext.PODetails.Where(x => x.PONumber == pODetailsvm.PONumber).FirstOrDefaultAsync();

                    if (poexists != null)
                    {
                        return new StockResponse(false, $" PO  '{pODetailsvm.PONumber}' already exists", null);
                    }
                    var poclass = new PODetails
                    {
                        PONumber = pODetailsvm.PONumber,
                        PODate = Convert.ToDateTime(pODetailsvm.PODate),
                        Vendor = pODetailsvm.Vendor,
                        DateCreated = DateTime.Now,
                        CaptureStatus = "Incomplete",
                        DeliveryStatus = "Incomplete",

                    };
                    await scopedcontext.AddAsync(poclass);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, $"PO-Number '{pODetailsvm.PONumber}'  created successfully", poclass);

                }

            }

            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }

            return new StockResponse(false, "Invalid file or file format.", null);
        }
        public async Task<StockResponse> GetAllPOs()
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allpos = await scopedcontext.PODetails.ToListAsync();

                    if (allpos == null)
                    {
                        return new StockResponse(false, "PO doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allpos);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> BatchAndDeliveryReport()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    // Get delivery notes
                    var allpos = await scopedContext.AddDeliveryNote
                        .Select(note => new
                        {
                            note.BatchNumber,
                            note.PONumber,
                            note.DeliveryDate,
                            note.BatchQuantity,
                            note.MeansOfDelivery,
                            note.AirWayBillNumber,
                            note.ItemName,
                            note.BrandName,
                            Products = scopedContext.AddProductDetails
                                .Where(p => p.BatchNumber == note.BatchNumber )
                                .Select(p => new
                                {
                                    p.SerialNumber,
                                    p.ItemName,
                                    p.BrandName,
                                    p.IMEI1,
                                    p.IMEI2,
                                    p.IsIssued,
                                    p.SerialStatus,
                                    p.ItemStatus,
                                }).ToList()
                        }).ToListAsync();

                    if (allpos == null || !allpos.Any())
                    {
                        return new StockResponse(false, "Details don't exist", null);
                    }

                    return new StockResponse(true, "Successfully queried", allpos);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> GetAllProducts()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allpos = await scopedcontext.AddProductDetails.ToListAsync();

                    if (allpos == null || allpos.Count == 0)
                    {
                        return new StockResponse(false, "PO doesn't exist", null);
                    }

                    var enhancedProducts = allpos.Select(p => new
                    {
                        p.SerialNumber,
                        p.IMEI1,
                        p.IMEI2,
                        p.ItemID,
                        p.SerialStatus,
                        p.BatchNumber,
                        p.ItemStatus,
                        p.Quantity,
                        p.ItemName,
                        p.BrandName,
                        p.BatchStatus,
                        p.PONumber,
                        p.IsIssued,
                        p.IssuedBy,
                        p.ReferenceNumber,
                        p.ClientName,
                        p.ItemIDdetails,
                        p.WarrantyStartDate,
                        p.WarrantyEndDate,

                        // Add computed field
                        WarrantyStatus = p.WarrantyEndDate.Date < DateTime.UtcNow.Date
                            ? "Expired"
                            : "Under Warranty"
                    }).ToList();

                    return new StockResponse(true, "Successfully queried", enhancedProducts);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }


        public async Task<StockResponse> GetAllPurchaseOrders()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var uploadPoFiles = await scopedContext.UploadPOFile.ToListAsync();
                    var purchaseOrders = await scopedContext.PurchaseOrderss.ToListAsync();

                    if (purchaseOrders == null || purchaseOrders.Count == 0)
                    {
                        return new StockResponse(false, "No Purchase Orders found", null);
                    }

                    var groupedPurchaseOrders = purchaseOrders
                        .Select(po => new
                        {
                            PONumber = po.PONumber,
                            Vendor = po.Vendor,
                            PODate = po.PODate,
                            DeliveryStatus = po.DeliveryStatus,
                            CaptureStatus = po.CaptureStatus,
                            DateCreated = po.DateCreated,

                            Items = uploadPoFiles
                                .Where(x => x.PONumber == po.PONumber)
                                .Select(item => new
                                {
                                    item.ItemName,
                                    item.BrandName,
                                    item.Quantity,
                                    item.UnitPrice,
                                    item.TotalUnitPrice,
                                    item.Warranty,
                                    item.WarrantyStartDate,
                                    item.WarrantyEndDate,
                                    item.CategoryName,
                                    item.Status,
                                    item.ProductStatus,
                                    item.CaptureStatus,
                                    item.UpdatedBy,
                                    item.UpdatedOn,

                                    // Warranty Status
                                    WarrantyStatus = item.WarrantyEndDate.Date < DateTime.UtcNow.Date
                                        ? "Expired"
                                        : "Under Warranty"
                                }).ToList()
                        })
                        .ToList(); // <-- this closing is required

                    return new StockResponse(true, "Successfully queried", groupedPurchaseOrders);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }






        public async Task<StockResponse> UploadingPOItems([FromBody] DataWrapper dataWrapper)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    //Console.WriteLine("Uploaded items", uploadPOItemvm);
                    foreach (var item in dataWrapper.ArrayList)
                    {
                        Console.WriteLine(item);
                    }


                    scopedcontext.SaveChanges();
                }



                return new StockResponse(true, "Data received and stored successfully.", null);
            }


            // var purchaseOrder = JsonConvert.DeserializeObject<PurchaseOrderModel>(data);
            //    if (uploadPOItemvm.ItemName == "")
            //    {

            //        return new StockResponse(false, "Kindly upload a file", null);
            //    }
            //    if (uploadPOItemvm.Amount == "")
            //    {

            //        return new StockResponse(false, "Kindly upload a file", null);
            //    }
            //    if (uploadPOItemvm.Rate == "")
            //    {

            //        return new StockResponse(false, "Kindly upload a file", null);
            //    }
            //    if (uploadPOItemvm.Quantity == 0)
            //    {

            //        return new StockResponse(false, "Kindly upload a file", null);
            //    }
            //    using (var scope = _serviceScopeFactory.CreateScope())
            //    {
            //        var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

            //        //check if role exists 

            //        var itemexists = await scopedcontext.UploadPOItem.Where(x => x.ItemName == uploadPOItemvm.ItemName).FirstOrDefaultAsync();

            //        if (itemexists != null)
            //        {
            //            return new StockResponse(false, $" Item  '{uploadPOItemvm.ItemName}' already exists! please update quantity", null);
            //        }
            //        var items = new List<UploadPOItem>();
            //        var itempoclass = new UploadPOItem
            //        {
            //            ItemName = uploadPOItemvm.ItemName,
            //            Amount = uploadPOItemvm.Amount,
            //            Rate = uploadPOItemvm.Rate,
            //            Quantity = uploadPOItemvm.Quantity,
            //        };
            //        items.Add(itempoclass);
            //        foreach (var each_item in items)
            //        {
            //            // Save the list of students to the database
            //            await scopedcontext.AddAsync(each_item);
            //            await scopedcontext.SaveChangesAsync();
            //        }

            //        return new StockResponse(true, $"Item '{uploadPOItemvm.ItemName}'  created successfully", itempoclass);




            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }

            return new StockResponse(false, "Invalid file or file format.", null);
        }
        public async Task UploadingItemsPO(IFormFile file, string PONumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {


                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        using (var package = new ExcelPackage(stream))
                        {

                            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                            if (worksheet != null)
                            {

                                var products = new List<UploadPOItem>();
                                int rowCount = worksheet.Dimension.Rows;
                                for (int row = 5; row <= rowCount; row++) // Assuming the first row contains headers
                                {


                                    var product = new UploadPOItem

                                    {
                                        //Id = int.Parse(worksheet.Cells[row, 1].Value?.ToString()),
                                        ItemName = worksheet.Cells[row, 2].Value?.ToString(),
                                        Rate = worksheet.Cells[row, 6].Value?.ToString(),
                                        Quantity = worksheet.Cells[row, 4].Value?.ToString(),
                                        Amount = worksheet.Cells[row, 7].Value?.ToString(),
                                        PONumber = PONumber,


                                    };
                                    PONumber = product.PONumber;

                                    // product.Id = product.Id;
                                    await scopedcontext.AddAsync(product);
                                    await scopedcontext.SaveChangesAsync();
                                    _logger.LogInformation("saved successfully");
                                    //products.Add(product);
                                    //await scopedcontext.SaveChangesAsync();

                                }
                                //foreach (var each_product in products)
                                //{
                                //    // Save the list of students to the database
                                //    await scopedcontext.AddAsync(each_product);
                                //    await scopedcontext.SaveChangesAsync();
                                //}



                                //_logger.LogInformation("Uploaded successfully");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }

            _logger.LogInformation("Invalid file or file format.");

        }
        public async Task<StockResponse> GetItemsByPO(string PONumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var itemexists = await scopedcontext.UploadPOItem.Where(y => y.PONumber == PONumber).ToListAsync();
                    if (itemexists == null)
                    {
                        return new StockResponse(false, "nothing to show ", null);
                    }
                    return new StockResponse(true, "Queried successfully", itemexists);


                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> UploadingPO(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                // Handle the case where the file is not provided or empty
                return new StockResponse(false, "Please provide a valid file.", null);
            }

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        using (var package = new ExcelPackage(stream))
                        {
                            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                            if (worksheet != null)
                            {
                                var products = new List<UploadPOFile>();
                                int rowCount = worksheet.Dimension.Rows;

                                int totalStock = 0;
                                for (int row = 2; row <= rowCount; row++) // Assuming the first row contains headers
                                {
                                    string item_name = worksheet.Cells[row, 2].Value?.ToString();
                                    int quantity = int.Parse(worksheet.Cells[row, 4].Value?.ToString());
                                    string BrandName = worksheet.Cells[row, 3].Value?.ToString();

                                    var itemexists = await scopedcontext.UploadPOFile
                                        .Where(x => x.ItemName == item_name && x.BrandName == BrandName)
                                        .OrderByDescending(y => y.DateAdded)
                                        .FirstOrDefaultAsync();


                                    var product = new UploadPOFile
                                    {
                                        PONumber = worksheet.Cells[row, 1].Value?.ToString(),
                                        ItemName = item_name,
                                        BrandName = BrandName,
                                        Quantity = quantity,
                                        ReOrderLevel = int.Parse(worksheet.Cells[row, 5].Value?.ToString()),
                                        Rate = worksheet.Cells[row, 6].Value?.ToString(),
                                        Amount = worksheet.Cells[row, 7].Value?.ToString(),
                                        Warranty = int.Parse(worksheet.Cells[row, 8].Value?.ToString()),
                                        WarrantyStartDate = Convert.ToDateTime(worksheet.Cells[row, 9].Value?.ToString()),
                                        CategoryName = worksheet.Cells[row, 10].Value?.ToString(),
                                        DateAdded = DateTime.Now,
                                        Reference_Number = await GetGeneratedref(),
                                        ProductStatus = "Incomplete",

                                    };
                                    product.WarrantyStatus = product.WarrantyEndDate < DateTime.Now
    ? "Expired"
    : "Under Warranty";
                                    var itemidexissst = await scopedcontext.AddDeliveryNote.Where(u => u.ItemID == product.ID).FirstOrDefaultAsync();

                                    product.OpeningStock = quantity;
                                    product.ActualQuantity = quantity;

                                    if (itemidexissst != null)
                                    {
                                        product.TotalClosed = itemidexissst.TotalClosed;
                                    }
                                    if (itemexists != null)
                                    {
                                        product.AvailableStock = itemexists.AvailableStock + product.TotalClosed;
                                        product.TotalClosed += itemexists.TotalClosed;
                                        product.StockOut = itemexists.StockOut;
                                        scopedcontext.Update(product);
                                    }
                                    else
                                    {
                                        // product.AvailableStock = product.Quantity;
                                        product.AvailableStock = product.TotalClosed;
                                        await scopedcontext.UploadPOFile.AddAsync(product);
                                    }


                                    if (product.AvailableStock > product.ReOrderLevel)
                                    {
                                        product.Status = "Good";
                                    }
                                    else if (product.AvailableStock < product.ReOrderLevel || product.AvailableStock > 0)
                                    {
                                        product.Status = "Low";
                                    }
                                    else
                                    {
                                        product.Status = "Out";
                                    }



                                    //await scopedcontext.SaveChangesAsync();



                                    //if (product.CategoryName == "Product")
                                    //{
                                    //    var new_numb = 0;

                                    //    while (new_numb < product.ActualQuantity)
                                    //    {
                                    //        new_numb++;
                                    //        var new_numbering = new ProductNumbering
                                    //        {
                                    //            NumberValue = new_numb,
                                    //            Reference_Number = product.Reference_Number,
                                    //            Type = "Product",
                                    //            Status = "UNASSIGNED",
                                    //        };


                                    //        await scopedcontext.ProductNumbering.AddAsync(new_numbering);
                                    //        products.Add(product);
                                    //    }

                                    //}

                                    //product.AjustedQuantity = product.Quantity;
                                    totalStock += product.AvailableStock;
                                    product.WarrantyEndDate = product.WarrantyStartDate.AddMonths(product.Warranty);
                                    product.TotalStockIn = totalStock;

                                    //if (product.CategoryName == "Accesory")
                                    //{
                                    //    product.ProductStatus = "Complete";
                                    //}
                                    //else
                                    //{
                                    //    product.ProductStatus = "Incomplete";
                                    //}
                                }

                                await scopedcontext.SaveChangesAsync(); // Save changes after all items are added/updated

                                return new StockResponse(true, "Uploaded successfully", products);
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }

            return new StockResponse(false, "Item Already Saved.", null);
        }


        public async Task<StockResponse> GetAllPOSDetails()
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allpos = await scopedcontext.UploadPOFile.ToListAsync();


                    if (allpos == null)
                    {
                        return new StockResponse(false, "PO doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allpos);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetItemsByPOS(string PONumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var itemexists = await scopedcontext.UploadPOFile.Where(y => y.PONumber == PONumber).ToListAsync();
                    if (itemexists == null)
                    {
                        return new StockResponse(false, "No items found", null);
                    }
                    return new StockResponse(true, "Queried successfully", itemexists);


                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetItemByPONumber(string PONumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    // Retrieve items from the first database
                    var itemsFromFirstDb = await scopedContext.UploadPOFile
                        .Where(y => y.PONumber == PONumber )
                        .ToListAsync();

                    if (itemsFromFirstDb == null || itemsFromFirstDb.Count == 0)
                    {
                        return new StockResponse(false, "No products found", null);
                    }

                    // Extract ItemIds from items in the first database
                    var itemIds = itemsFromFirstDb.Select(item => item.ID).ToList();

                    var serialNumbersFromSecondDb = await scopedContext.AddProductDetails
                        .Where(serial => itemIds.Contains(serial.ItemID))
                        .ToListAsync();

                    // Create a new list to hold items and their associated serial numbers
                    var itemsWithSerialNumbers = new List<ItemWithSerialNumbers>();

                    // Associate serial numbers with items
                    foreach (var item in itemsFromFirstDb)
                    {
                        var itemSerialNumbers = serialNumbersFromSecondDb
                            .Where(serial => serial.ItemID == item.ID)
                            .ToList();

                        itemsWithSerialNumbers.Add(new ItemWithSerialNumbers
                        {
                            Item = item,
                            SerialNumbers = itemSerialNumbers
                        });
                    }

                    return new StockResponse(true, "Queried successfully", itemsWithSerialNumbers);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        // Create a class to hold items and their associated serial numbers
        public class ItemWithSerialNumbers
        {
            public UploadPOFile Item { get; set; }
            public List<AddProductDetails> SerialNumbers { get; set; }
        }

        public async Task<StockResponse> AddPurchaseOrdersDetails(PurchaseOrderssvm purchaseOrderssvm)
        {
            try
            {
                if (purchaseOrderssvm.PONumber == "")
                {

                    return new StockResponse(false, "Kindly provide PO number to add Purchase order", null);
                }
                if (purchaseOrderssvm.PODate == null)
                {
                    return new StockResponse(false, "Kindly provide PO Date", null);

                }
                if (purchaseOrderssvm.Vendor == "")
                {
                    return new StockResponse(false, "Kindly provide vendor", null);

                }

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();



                    var poexistss = await scopedcontext.PurchaseOrderss.Where(x => x.PONumber == purchaseOrderssvm.PONumber).FirstOrDefaultAsync();

                    if (poexistss != null)
                    {
                        return new StockResponse(false, $" Purchase Order  '{purchaseOrderssvm.PONumber}' already exists", null);
                    }

                    var purchaseorderss = new PurchaseOrderss
                    {
                        PONumber = purchaseOrderssvm.PONumber,
                        PODate = purchaseOrderssvm.PODate,
                        Vendor = purchaseOrderssvm.Vendor,
                        DateCreated = DateTime.Now,


                    };

                    await scopedcontext.AddAsync(purchaseorderss);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, $"Purchase Order '{purchaseOrderssvm.PONumber}'  created successfully", null);

                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }

        }
        public async Task<StockResponse> GetAllPurchaseOrderDetails()
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allpos = await scopedcontext.PurchaseOrderss.OrderByDescending(u=>u.DateCreated).ToListAsync();


                    if (allpos == null)
                    {
                        return new StockResponse(false, "PO doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allpos);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> AdjustStock(AdjustStockvm adjustStockvm)
        {

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var lastupdate = await scopedcontext.UploadPOFile.Where(y => y.ID == adjustStockvm.ItemID)
                      .FirstOrDefaultAsync();

                    if (lastupdate == null)
                    {
                        return new StockResponse(false, "item not found", null);
                    }

                    var Items = new AdjustStock
                    {
                        QuantityDamaged = adjustStockvm.QuantityDamaged,
                        Description = adjustStockvm.Description,

                    };
                    Items.ItemID = lastupdate.ID;

                    lastupdate.AjustedQuantity -= Items.QuantityDamaged;
                    // Update the TotalDamages
                    lastupdate.TotalDamages += adjustStockvm.QuantityDamaged;
                    scopedcontext.Update(lastupdate);
                    await scopedcontext.AddAsync(Items);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, " updated successfully", Items);

                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetPOItemsByID(int ItemId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var itemexists = await scopedcontext.UploadPOFile.Where(u => u.ID == ItemId).FirstOrDefaultAsync();
                    if (itemexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", itemexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetAdjustedStockById(string batchNumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var itemexist = await scopedcontext.StockAdjustment.Where(u => u.BatchNumber == batchNumber).ToListAsync();
                    if (itemexist == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", itemexist);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetDnoteItemsByReference(string referenceNumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    // Get items by reference number
                    var items = await scopedcontext.ApplyRequisitionItem
                        .Where(u => u.ReferenceNumber == referenceNumber)
                        .ToListAsync();

                    if (items == null || items.Count == 0)
                    {
                        return new StockResponse(false, "No items found for this reference number.", null);
                    }

                    // Get serials by reference number
                    var serials = await scopedcontext.AddProductDetails
                        .Where(s => s.ReferenceNumber == referenceNumber)
                        .ToListAsync();

                    // Map the items with corresponding serials
                    var result = items.Select(item => new
                    {
                        item.ItemID,
                        item.ItemName,
                        item.BrandName,
                        item.Quantity,
                        item.UnitPrice,
                        item.TotalAmount,
                        item.DiscountNumerator,
                        item.DiscountDenominator,
                        item.RequisitionName,
                        item.Status,
                        item.DateEdited,
                        item.EditedBy,
                        item.ReferenceNumber,
                        item.Reason,
                        Serials = serials
                            .Where(s => s.ItemID.ToString() == item.ItemID || s.ItemName == item.ItemName)
                            .Select(s => new
                            {
                                s.SerialNumber,
                                s.IMEI1,
                                s.IMEI2
                            }).ToList()
                    }).ToList(); // Ensure to collect the results into a list

                    return new StockResponse(true, "Queried successfully", result);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }



        public async Task<StockResponse> GetAllItemStock()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var allstock = await scopedcontext.UploadPOFile
                        .OrderByDescending(x => x.DateAdded)
                        .ToListAsync();

                    var loggedinuserobject = await _extraServices.LoggedInUser();

                    if (loggedinuserobject == null)
                    {
                        return new StockResponse(false, "User not logged in. Please log in again.", null);
                    }

                    var userName = $"{loggedinuserobject.FirstName} {loggedinuserobject.LastName}";

                    if (allstock == null || allstock.Count == 0)
                    {
                        var currentTotalDelivered = await scopedcontext.UploadPOFile
                            .SumAsync(x => x.totalDeliveredForAllItems);

                        allstock.ForEach(item => item.totalDeliveredForAllItems = currentTotalDelivered);

                        return new StockResponse(true, "No stock records found. totalDeliveredForAllItems set.", allstock);
                    }

                    var alltotals = await scopedcontext.AddDeliveryNote
                        .OrderByDescending(y => y.DateCreated)
                        .Select(y => y.totalDeliveredForAllItems)
                        .FirstOrDefaultAsync();

                    var totaldamaged = await scopedcontext.StockAdjustment
                        .OrderByDescending(y => y.DateCreated)
                        .Select(y => y.TotalQuantityDamaged)
                        .FirstOrDefaultAsync();

                    var totalavailablestock = await scopedcontext.ApprovalBatch
                        .Select(y => y.ClosedQuantity)
                        .SumAsync();

                    var totalreturnedstock = await scopedcontext.ReturnedItem
                        .Select(y => y.ReturnedQuantity)
                        .SumAsync();

                    var totalStockOutForAllItems = await scopedcontext.SelectSerial
                  .Select(y => y.TotalQuantityDispatchedForItem)
                  .SumAsync();

                    var totalStockOutByMonth = await scopedcontext.SelectSerial
                        .GroupBy(y => new { y.DateIssued.Year, y.DateIssued.Month })
                        .OrderByDescending(g => g.Key.Year).ThenByDescending(g => g.Key.Month)
                        .Select(g => new
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            TotalStockOut = g.Sum(y => y.TotalStockOut)
                        })
                        .ToListAsync();

                    var alltotalsByMonth = await scopedcontext.AddDeliveryNote
                        .GroupBy(y => new { y.DateCreated.Year, y.DateCreated.Month })
                        .OrderByDescending(g => g.Key.Year).ThenByDescending(g => g.Key.Month)
                        .Select(g => new
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            TotalDelivered = g.Sum(y => y.totalDeliveredForAllItems)
                        })
                        .ToListAsync();

                    var TotalStockOutPerMonth = totalStockOutByMonth.ToDictionary(
                        x => $"{x.Year}-{x.Month}", x => x.TotalStockOut);

                    var TotalDeliveredPerMonth = alltotalsByMonth.ToDictionary(
                        x => $"{x.Year}-{x.Month}", x => x.TotalDelivered);

                    Dictionary<string, AllStockListItems> stockDictionary = new Dictionary<string, AllStockListItems>();

                    foreach (var stock in allstock)
                    {
                        if (!stockDictionary.TryGetValue(stock.ItemName, out var existingStock))
                        {
                            var newStockItem = new AllStockListItems
                            {
                                ItemName = stock.ItemName,
                                OpeningStock = stock.OpeningStock,
                                AvailableStock = stock.TotalClosed - stock.StockOut, // ✅ No double subtraction
                                DateAdded = DateTime.Now,
                                Quantity = stock.TotalClosed,
                                StockOut = stock.StockOut,
                                Status = stock.Status,
                                BrandName = stock.BrandName,
                                StockIn = stock.TotalStockIn,
                                UpdatedBy = userName,
                                CategoryName = stock.CategoryName,

                                totalDeliveredForAllItems = alltotals,
                                TotalQuantityDamaged = totaldamaged,
                                TotalAvailableStock = totalavailablestock - totalStockOutForAllItems + totalreturnedstock,
                                TotalStockOutForAllItems = totalStockOutForAllItems,
                                TotalDeliveredPerMonth = TotalDeliveredPerMonth,
                                TotalIssuedPerMonth = TotalStockOutPerMonth,
                                TotalStockReturned = totalreturnedstock
                            };

                            stockDictionary.Add(stock.ItemName, newStockItem);
                        }
                    }

                    List<AllStockListItems> stockList = stockDictionary.Values.ToList();

                    return new StockResponse(true, "Successfully queried.", stockList);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetMonthlyRecord(int Year)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var allstock = await scopedcontext.UploadPOFile.OrderByDescending(x => x.DateAdded).ToListAsync();
                    var loggedinuserobject = await _extraServices.LoggedInUser();

                    if (loggedinuserobject == null)
                    {
                        return new StockResponse(false, "User not logged in. Login again.", null);
                    }

                    var userName = loggedinuserobject.FirstName + ' ' + loggedinuserobject.LastName;

                    List<MonthlyRecordvm> monthlyRecords = new List<MonthlyRecordvm>();

                    // Query totalStockOutForAllItems from SelectSerial
                    var totalStockOutForAllItems = await scopedcontext.SelectSerial
                        .OrderByDescending(y => y.DateIssued)
                        .ToListAsync();
                    var totaldamaged = await scopedcontext.StockAdjustment
      .OrderByDescending(y => y.DateCreated).ToListAsync();

                    var totalDeliveredFirAllItems = await scopedcontext.AddDeliveryNote.OrderByDescending(y => y.DateCreated).ToListAsync();

                    // Create a dictionary to store the monthly records
                    var monthlyRecordDict = new Dictionary<(int Year, int Month), MonthlyRecordvm>();

                    // Initialize the monthly records
                    for (int month = 1; month <= 12; month++)
                    {
                        monthlyRecordDict[(Year, month)] = new MonthlyRecordvm
                        {
                            Year = Year,
                            Month = month,
                            TotalDelivered = 0, // Initialize with 0
                            TotalIssued = 0, // Initialize with 0
                            TotalDamaged=0

                        };
                    }

                    // Populate the monthly records for the specified year based on DateIssued
                    foreach (var stock in totalStockOutForAllItems)
                    {
                        // Get the year and month from DateIssued
                        int year = stock.DateIssued.Year;

                        if (year == Year) // Filter records for the specified year
                        {
                            int month = stock.DateIssued.Month;

                            // Update the monthly record with the stock data
                            var record = monthlyRecordDict[(Year, month)];
                            record.TotalIssued += stock.QuantityDispatched;
                        }
                    }

                    // Populate the monthly records for the specified year based on DateCreated
                    foreach (var stock in totalDeliveredFirAllItems)
                    {
                        // Get the year and month from DateCreated
                        int year = stock.DateCreated.Year;

                        if (year == Year) // Filter records for the specified year
                        {
                            int month = stock.DateCreated.Month;

                            // Update the monthly record with the stock data by adding BatchQuantity
                            var record = monthlyRecordDict[(Year, month)];
                            record.TotalDelivered += stock.BatchQuantity;
                        }
                    }

                    // Populate the monthly records for the specified year based on DateCreated
                    foreach (var stock in totaldamaged)
                    {
                        // Get the year and month from DateCreated
                        int year = stock.DateCreated.Year;

                        if (year == Year) // Filter records for the specified year
                        {
                            int month = stock.DateCreated.Month;

                            // Update the monthly record with the stock data by adding BatchQuantity
                            var record = monthlyRecordDict[(Year, month)];
                            record.TotalDamaged += stock.QuantityDamaged;
                        }
                    }

                    // Convert the dictionary values to a list
                    var monthlrecordlist = monthlyRecordDict.Values.ToList();

                    return new StockResponse(true, "Successfully queried", monthlrecordlist);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }





        public async Task<StockResponse> ReturnReview(ApprovalReturnedStockvm approvalProcessvm)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var loggedinuserobject = await _extraServices.LoggedInUser();
                    var userEmail = loggedinuserobject.FirstName + ' ' + loggedinuserobject.LastName;

                    if (loggedinuserobject == null)
                    {
                        return new StockResponse(false, "user not logged in. login again", null);
                    }

                    var applieditem = await scopedcontext.ReturnedItem
                        .Where(u => u.ReturnID == approvalProcessvm.Id)
                        .FirstOrDefaultAsync();

                    if (applieditem == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }

                    var Items = new ApprovalRetun
                    {
                        selectedOption = approvalProcessvm.selectedOption,
                        RejectedReason = approvalProcessvm.RejectedReason,
                        AprrovedDate = DateTime.Now,
                        ApprovedBy = userEmail,
                        ReturnedStatus = "Returned",
                    };

                    Items.SerialNumber = applieditem.SerialNumber;
                    Items.ReturnedQuantity = applieditem.ReturnedQuantity;
                    Items.BrandName = applieditem.BrandName;
                    Items.ItemName = applieditem.ItemName;

                    if (applieditem.CategoryName == "Product")
                    {
                        var serialNumberChecking = await scopedcontext.AddProductDetails
                            .Where(u => u.SerialNumber == Items.SerialNumber)
                            .FirstOrDefaultAsync();

                        if (serialNumberChecking != null)
                        {
                            if (Items.selectedOption == "Approve")
                            {
                                serialNumberChecking.SerialStatus = "Returned";

                                if (applieditem.ReturnedCondition == "faulty")
                                {
                                    serialNumberChecking.ItemStatus = "Faulty";
                                }
                                else
                                {
                                    serialNumberChecking.ItemStatus = "Okay";
                                }

                                if (applieditem.ReturnedCondition == "okay")
                                {
                                    var existingBatch = await scopedcontext.UploadPOFile
                                        .Where(x => x.ItemName == Items.ItemName && x.BrandName == Items.BrandName)
                                        .OrderByDescending(y => y.DateAdded)
                                        .FirstOrDefaultAsync();

                                    if (existingBatch != null)
                                    {
                                        existingBatch.AvailableStock += Items.ReturnedQuantity;
                                        existingBatch.TotalClosed += Items.ReturnedQuantity;

                                        if (existingBatch.AvailableStock > existingBatch.ReOrderLevel)
                                        {
                                            existingBatch.Status = "Good";
                                        }
                                        else if (existingBatch.AvailableStock < existingBatch.ReOrderLevel || existingBatch.AvailableStock > 0)
                                        {
                                            existingBatch.Status = "Low";
                                        }
                                        else
                                        {
                                            existingBatch.Status = "Out";
                                        }

                                        scopedcontext.Update(existingBatch);
                                    }
                                    else
                                    {
                                        return new StockResponse(false, "Item does not exist", null);
                                    }
                                }
                            }
                            else
                            {
                                serialNumberChecking.SerialStatus = "Issued";
                            }
                        }
                    }
                    else
                    {
                        if (applieditem.ReturnedCondition == "okay")
                        {
                            var existingBatch = await scopedcontext.UploadPOFile
                                .Where(x => x.ItemName == Items.ItemName && x.BrandName == Items.BrandName)
                                .OrderByDescending(y => y.DateAdded)
                                .FirstOrDefaultAsync();

                            if (existingBatch != null)
                            {
                                existingBatch.AvailableStock += Items.ReturnedQuantity;
                                existingBatch.TotalClosed += Items.ReturnedQuantity;

                                if (existingBatch.AvailableStock > existingBatch.ReOrderLevel)
                                {
                                    existingBatch.Status = "Good";
                                }
                                else if (existingBatch.AvailableStock < existingBatch.ReOrderLevel || existingBatch.AvailableStock > 0)
                                {
                                    existingBatch.Status = "Low";
                                }
                                else
                                {
                                    existingBatch.Status = "Out";
                                }

                                scopedcontext.Update(existingBatch);
                            }
                        }
                    }

                    await scopedcontext.AddAsync(Items);
                    await scopedcontext.SaveChangesAsync();

                    if (approvalProcessvm.selectedOption == "Approve")
                    {
                        approvalProcessvm.RejectedReason = "";
                    }

                    scopedcontext.Update(applieditem);
                    await scopedcontext.SaveChangesAsync();

                    return new StockResponse(true, "Successfully updated", applieditem);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<List<AddDeliveryNote>> GetDeliveredItemsByMonthAndYear(int year, int month)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>(); // Replace YourDbContext with your actual DbContext class

                    // Query the delivered items based on the provided year and month
                    var deliveredItems = await scopedcontext.AddDeliveryNote
                        .Where(item => item.DateCreated.Year == year && item.DateCreated.Month == month)
                        .ToListAsync();

                    return deliveredItems;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                throw ex;
            }
        }
        public async Task<List<StockAdjustment>> GetDamagedItemsByMonthAndYear(int year, int month)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>(); // Replace YourDbContext with your actual DbContext class

                    // Query the delivered items based on the provided year and month
                    var deliveredItems = await scopedcontext.StockAdjustment
                        .Where(item => item.DateCreated.Year == year && item.DateCreated.Month == month)
                        .ToListAsync();

                    return deliveredItems;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                throw ex;
            }
        }

        public async Task<List<SelectSerial>> GetIssuedItemsByMonthandYear(int year, int month)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>(); // Replace YourDbContext with your actual DbContext class

                    // Query the delivered items based on the provided year and month
                    var deliveredItems = await scopedcontext.SelectSerial
                        .Where(item => item.DateIssued.Year == year && item.DateIssued.Month == month)
                        .ToListAsync();

                    return deliveredItems;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                throw ex;
            }
        }
        public async Task<StockResponse> GetIssuedSerialNumber()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var deliveredItems = await scopedContext.selectedSetialNumber
                        .Where(item => item.SerialStatus == "Issued")
                        .ToListAsync();

                    return new StockResponse(true, "Queried successfully", null);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, "Not Queried successfully", null);
            }
        }



        public async Task<StockResponse> ApplyRequisition(AddRequisition addRequisition)
        {
            try
            {
                if (addRequisition.BrandName == "")
                {

                    return new StockResponse(false, "Kindly provide  brand name", null);
                }
                if (addRequisition.itemName == "")
                {

                    return new StockResponse(false, "Kindly provide  item name ", null);
                }
                if (addRequisition.Quantity == 0)
                {

                    return new StockResponse(false, "Kindly provide quantity", null);
                }
                if (addRequisition.clientName == "")
                {

                    return new StockResponse(false, "Kindly provide customer name ", null);
                }
                if (addRequisition.DeviceBeingRepaired == "")
                {
                    return new StockResponse(false, "Kindly provide device being repaired ", null);
                }
                if (addRequisition.Department == "")
                {
                    return new StockResponse(false, "Kindly provide department details", null);
                }
                if (addRequisition.clientName == "")
                {
                    return new StockResponse(false, "Kindly provide client details", null);
                }
                if (addRequisition.Requisitioner == "")
                {
                    return new StockResponse(false, "Kindly provide person requisitioning ", null);
                }

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var itemexists = await scopedcontext.RequisitionForm
                    .Where(y => y.BrandName == addRequisition.BrandName &&
                    y.itemName == addRequisition.itemName).FirstOrDefaultAsync();
                    if (itemexists == null)
                    {
                        return new StockResponse(false, "Does not exist", null);
                    }



                    var itemclass = new RequisitionForm
                    {
                        BrandName = addRequisition.BrandName,
                        itemName = addRequisition.itemName,
                        Quantity = addRequisition.Quantity,
                        clientName = addRequisition.clientName,
                        DeviceBeingRepaired = addRequisition.DeviceBeingRepaired,
                        IssuedByDate = DateTime.Now,
                        Department = addRequisition.Department,
                        Description = addRequisition.Description,
                        Requisitioner = addRequisition.Requisitioner,
                        ApprovedStatus = "Waiting For Approval",
                        stockNeed = addRequisition.stockNeed,
                        Purpose = addRequisition.Purpose,
                        ApprovedDate = DateTime.Now,






                    };


                    await scopedcontext.AddAsync(itemclass);
                    await scopedcontext.SaveChangesAsync();
                    return new StockResponse(true, "Requisition has been sent successfully", null);

                }


            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }

        }
        public async Task<StockResponse> ApplyRequisitionForm(ApplyRequistionvm addRequisition)
        {
            try
            {
                if (addRequisition.BrandName == "")
                {
                    return new StockResponse(false, "Kindly provide brand name", null);
                }

                if (addRequisition.itemName == "")
                {
                    return new StockResponse(false, "Kindly provide item name", null);
                }

                if (addRequisition.Quantity == 0)
                {
                    return new StockResponse(false, "Kindly provide quantity", null);
                }

                if (addRequisition.clientName == "")
                {
                    return new StockResponse(false, "Kindly provide customer name", null);
                }

                if (addRequisition.Department == "")
                {
                    return new StockResponse(false, "Kindly provide department details", null);
                }

                // ... (other validation checks)

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var itemsWithSameName = await scopedcontext.UploadPOFile
                        .Where(u => u.ItemName == addRequisition.itemName && u.BrandName == addRequisition.BrandName)
                        .OrderByDescending(y => y.DateAdded)
                        .FirstOrDefaultAsync();

                    if (itemsWithSameName == null)
                    {
                        return new StockResponse(false, "Item with such brand does not exist", null);
                    }

                    var loggedinuserobject = await _extraServices.LoggedInUser();

                    if (loggedinuserobject == null)
                    {
                        return new StockResponse(false, "User not logged in. Login again", null);
                    }

                    var useremails = loggedinuserobject.Email;
                    var userName = loggedinuserobject.FirstName + ' ' + loggedinuserobject.LastName;

                    var availableStockResponse = await GetAllItemStock();

                    if (!availableStockResponse.isTrue || !(availableStockResponse.Body is List<AllStockListItems> stockList))
                    {
                        return new StockResponse(false, "Error retrieving available stock information", null);
                    }

                    AllStockListItems itemExistsInStock = null;

                    foreach (var stock in stockList)
                    {
                        if (stock.ItemName == addRequisition.itemName)
                        {
                            itemExistsInStock = stock;
                            break;
                        }
                    }
                    string nameToUse = addRequisition.stockNeed == "Customer Order" ? addRequisition.clientName : userName;
                    Console.WriteLine($"nameToUse: {nameToUse}"); // Add this line to print nameToUse


                    if (itemExistsInStock == null)
                    {
                        return new StockResponse(false, "Item does not exist in stock", null);
                    }
                    string batchnumber;
                    if (addRequisition.stockNeed == "Customer Order")
                    {
                        batchnumber = await GetClientBatch(addRequisition.clientName, "", addRequisition.stockNeed);
                    }
                    else
                    {
                        batchnumber = await GetClientBatch("", userName, addRequisition.stockNeed);
                    }


                    var itemclass = new ApplyRequistionForm
                    {
                        BrandName = addRequisition.BrandName,
                        itemName = addRequisition.itemName,
                        Quantity = addRequisition.Quantity,
                        clientName = addRequisition.clientName,
                        DeviceBeingRepaired = addRequisition.DeviceBeingRepaired,
                        IssuedByDate = DateTime.Now,
                        Department = addRequisition.Department,
                        Description = addRequisition.Description,
                        Requisitioner = userName,
                        ApprovedStatus = "Pending",
                        stockNeed = addRequisition.stockNeed,
                        Purpose = addRequisition.Purpose,
                        ApprovedDate = DateTime.Now,
                        useremail = useremails,
                        OrderNumber = batchnumber,
                        DispatchStatus = "Incomplete",
                        NameToUse=nameToUse,
                        ApplicationStatus="Pending",

                    };

                    itemclass.CategoryName = itemsWithSameName.CategoryName;

                    if (itemExistsInStock.AvailableStock < itemclass.Quantity)
                    {
                        itemclass.ApprovedStatus = "Failed";
                        itemclass.RejectReason = $"Insufficient stock. Available stock: {itemExistsInStock.AvailableStock}";
                        return new StockResponse(false, $"Insufficient stock for the requisition. Available stock: {itemExistsInStock.AvailableStock}", itemExistsInStock);
                    }

                    scopedcontext.Update(itemclass);
                    await scopedcontext.AddAsync(itemclass);
                    await scopedcontext.SaveChangesAsync();

                    await _iemail_service.MakerEmail();
                    return new StockResponse(true, "Requisition has been sent successfully", itemclass);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        private Dictionary<string, string> orderCounters = new Dictionary<string, string>();

        public async Task<string> GetClientBatch(string clientName, string requisitioner, string stockNeed)
        {
            try
            {
                string nameToUse = stockNeed == "Customer Order" ? clientName : requisitioner;
                Console.WriteLine($"nameToUse: {nameToUse}"); // Add this line to print nameToUse

                string counter = "001"; // Initialize to "001" if no previous orders exist

                // Check if a counter exists for the client or requisitioner
                if (orderCounters.TryGetValue(nameToUse, out string existingCounter))
                {
                    counter = IncrementOrderNumber(existingCounter); // Increment the existing counter
                }
                else
                {
                    // If no counter exists, retrieve the maximum counter value for the current client or requisitioner
                    string maxCounter = await _dragonFlyContext.ApplyRequistionForm
                        .Where(o => o.OrderNumber.StartsWith(nameToUse + "-"))
                        .Select(o => o.OrderNumber)
                        .MaxAsync()
                        .ConfigureAwait(false);

                    // Extract the numeric part and increment it
                    if (!string.IsNullOrEmpty(maxCounter) && int.TryParse(new string(maxCounter.Where(char.IsDigit).ToArray()), out int maxCounterValue))
                    {
                        counter = (maxCounterValue + 1).ToString("D3");
                    }

                    // Store the updated counter in the dictionary
                    orderCounters[nameToUse] = counter;
                }



                return $"{nameToUse}-{counter}"; // Already formatted as "001"
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine($"Error in GetClientBatch: {ex.Message}");
                throw; // Rethrow the exception to propagate it up the call stack
            }
        }
    

        private string IncrementOrderNumber(string currentOrderNumber)
        {
            // Extract the numeric portion of the currentOrderNumber
            string numericPart = new string(currentOrderNumber.Where(char.IsDigit).ToArray());

            // Parse the extracted numeric portion and increment it
            if (int.TryParse(numericPart, out int orderNumber))
            {
                orderNumber++;
                // Combine the incremented numeric part with the non-numeric part (e.g., "KCB UGANDA -")
                string nonNumericPart = currentOrderNumber.Substring(0, currentOrderNumber.IndexOf(numericPart));
                return $"{nonNumericPart}{orderNumber:D3}";
            }

            // Return the original order number if parsing fails
            return currentOrderNumber;
        }


        public async Task<StockResponse> ApplicationStatus(ApprovalProcessvm approvalProcessvm)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var loggedinuserobject = await _extraServices.LoggedInUser();

                    var userEmail = loggedinuserobject.FirstName + ' ' + loggedinuserobject.LastName;

                    if (loggedinuserobject == null)
                    {

                        return new StockResponse(false, "user not logged in. login again", null);

                    }

                    var applieditem = await scopedcontext.RequisitionApplication.Where(u => u.ReferenceNumber == approvalProcessvm.id).FirstOrDefaultAsync();
                    if (applieditem == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }

                    if (approvalProcessvm.selectedOption == "Approve")
                    {
                        approvalProcessvm.RejectedReason = "";
                    }
                    approvalProcessvm.AprrovedDate = DateTime.Now;

                    applieditem.selectedOption = approvalProcessvm.selectedOption;
                    applieditem.DateApproved = approvalProcessvm.AprrovedDate;
                    applieditem.RejectReason = approvalProcessvm.RejectedReason;
                    applieditem.DeviceBeingRepaired = approvalProcessvm.DeviceBeingRepaired;
                    applieditem.ApprovedBy = userEmail;
                    if (approvalProcessvm.selectedOption == "Reject")
                    {
                        applieditem.ApprovedStatus = "Rejected";
                        applieditem.ApplicationStatus = "Rejected";

                    }
                    else
                    {
                        applieditem.ApprovedStatus = "Approved";
                        applieditem.ApplicationStatus = "Approved";
                    }

                    scopedcontext.Update(applieditem);
                    await scopedcontext.SaveChangesAsync();

                    if (approvalProcessvm.selectedOption == "Reject")
                    {
                        var new_mail = new emailbody
                        {
                            ToEmail = applieditem.useremail,
                            PayLoad = "Your request has been Rejected,Please check the reason why",
                            UserName = "N/A",
                        };
                        await _iemail_service.send_status_to_Requester(new_mail);
                    }
                    else if (approvalProcessvm.selectedOption == "Approve")
                    {
                        var new_mail = new emailbody
                        {
                            ToEmail = applieditem.useremail,
                            PayLoad = "Your request has been approved,Wait a minute for it to undergo issue process",
                            UserName = "N/A"
                        };
                        await _iemail_service.send_status_to_Requester(new_mail);
                        await _iemail_service.IssuerEmail();
                    }
                    else
                    {
                        var new_mail = new emailbody
                        {
                            ToEmail = applieditem.useremail,
                            PayLoad = "Your request has been Rejected",
                            UserName = "N/A",
                        };
                        await _iemail_service.send_status_to_Requester(new_mail);

                    }
                    return new StockResponse(true, "Successfully updated", applieditem);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetAllRequisitionApplication()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var result = await (
                        from app in scopedcontext.RequisitionApplication
                        join item in scopedcontext.ApplyRequisitionItem
                            on app.ReferenceNumber equals item.ReferenceNumber into itemGroup
                        select new
                        {
                            app.ReferenceNumber,
                            app.Requisitioner,
                            app.clientName,
                            app.Quantity,
                            app.stockNeed,
                            app.Purpose,
                            app.Department,
                            app.ApplicationStatus,
                            app.DispatchStatus,
                            app.DateRequisitioned,
                            app.ApprovedStatus,
                            // ... other app fields

                            Items = itemGroup.ToList()
                        }).ToListAsync();

                    if (result == null || !result.Any())
                    {
                        return new StockResponse(false, "Requisition doesn't exist", null);
                    }

                    return new StockResponse(true, "Successfully queried", result);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }


        public async Task<StockResponse> GetIssueReport()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var result = await (
                        from app in scopedContext.SelectSerial
                        join item in scopedContext.selectedSetialNumber
                            on app.OrderNumber equals item.ReferenceNumber into itemGroup
                        select new
                        {
                            app.OrderNumber,
                            app.Requisitioner,
                            app.QuantityOrdered,
                            app.QuantityDispatched,
                            app.BrandName,
                            app.ItemName,
                            app.CategoryName,
                            app.Comments,
                            app.DispatchStatus,
                            app.IssueStatus,
                            app.IssuedBy,
                            app.DateIssued,
                            app.StockNeed,
                            app.OutStandingBalance,
                            Items = itemGroup.DefaultIfEmpty().ToList() // Left join to include unmatched app records
                        }).ToListAsync();

                    if (result == null || !result.Any())
                    {
                        return new StockResponse(false, "Issue doesn't exist", null);
                    }

                    return new StockResponse(true, "Successfully queried", result);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, "An error occurred while retrieving the issue report.", null);
            }
        }

        public async Task<StockResponse> GetAllRequisitionApplicationbyClientName(string clientName)
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allbrands = await scopedcontext.ApplyRequistionForm.Where(x=>x.NameToUse==clientName).ToListAsync();

                    if (allbrands == null)
                    {
                        return new StockResponse(false, "Requisition doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allbrands);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetRequisitionByReferenceNumber(string referenceNumber)
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allbrands = await scopedcontext.RequisitionApplication.Where(x => x.ReferenceNumber == referenceNumber).FirstOrDefaultAsync();

                    if (allbrands == null)
                    {
                        return new StockResponse(false, "Requisition doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allbrands);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetCustomerByName(string clientName)
        {

            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var allbrands = await scopedcontext.Customer.Where(x => x.CompanyName == clientName).FirstOrDefaultAsync();

                    if (allbrands == null)
                    {
                        return new StockResponse(false, "Requisition doesn't exist", null);
                    }
                    return new StockResponse(true, "Successfully queried", allbrands);

                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetRequisitionbyId(int Id)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.ApplyRequistionForm.Where(u => u.ID == Id).FirstOrDefaultAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> IssueProcess(int id)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var applieditem = await scopedcontext.ApplyRequistionForm.Where(u => u.ID == id).FirstOrDefaultAsync();
                    var loggedinuserobject = await _extraServices.LoggedInUser();
                    var userEmail = loggedinuserobject.FirstName + ' ' + loggedinuserobject.LastName;

                    if (loggedinuserobject == null)
                    {
                        return new StockResponse(false, "User not logged in. Please log in again.", null);
                    }

                    if (applieditem == null)
                    {
                        return new StockResponse(false, "Item not found.", null);
                    }

                    if (applieditem.ApprovedStatus == "Rejected")
                    {
                        return new StockResponse(false, "Check reason for rejection and revise again.", null);
                    }


                    // Retrieve the items with the same itemName from UploadPOFile
                    var itemsWithSameName = await scopedcontext.UploadPOFile
                        .Where(u => u.ItemName == applieditem.itemName && u.BrandName == applieditem.BrandName)
                        .OrderByDescending(y => y.DateAdded)
                        .FirstOrDefaultAsync();
                    if (itemsWithSameName == null)
                    {
                        return new StockResponse(false, "Item with such brandname does not exist", null);
                    }

                    //var issueResponses = new List<StockResponse>();

                    //foreach (var item in itemsWithSameName)
                    //{
                    var itemclass = new IssueProcess
                    {
                        IssueStatus = "Issued",
                        IssuedBy = userEmail,
                        DateIssued = DateTime.Now,
                        BrandName=applieditem.BrandName,
                        ClientName=applieditem.clientName,
                        Requisitiioner=applieditem.Requisitioner,
                        StockNeed=applieditem.stockNeed,
                        


                    };
                    itemclass.CategoryName = itemsWithSameName.CategoryName;

                    itemclass.Quantity = applieditem.Quantity;
                    itemclass.ItemName = applieditem.itemName;
                  
                    itemclass.FormID = applieditem.ID;
                    applieditem.ApprovedStatus = itemclass.IssueStatus;

                    //itemsWithSameName.StockOut += itemclass.Quantity;

                    if (itemsWithSameName.AvailableStock < itemclass.Quantity)
                    {
                        applieditem.ApprovedStatus = "Failed";
                        applieditem.RejectReason = "Not available...Will be restocked soon";
                        scopedcontext.Update(applieditem);
                        return new StockResponse(false, $"Not enough available stock to issue {itemclass.Quantity} units for item {itemclass.ItemName}.", null);




                    }
                    else
                    {
                        // Update stock quantities
                        itemsWithSameName.AvailableStock = itemsWithSameName.AvailableStock - applieditem.Quantity;
                        scopedcontext.Update(itemsWithSameName);
                        await scopedcontext.SaveChangesAsync();

                        if (itemsWithSameName.AvailableStock > itemsWithSameName.ReOrderLevel)
                        {
                            itemsWithSameName.Status = "Good";
                        }
                        else if (itemsWithSameName.AvailableStock < itemsWithSameName.ReOrderLevel || itemsWithSameName.AvailableStock > 0)
                        {
                            itemsWithSameName.Status = "Low";
                        }
                        else if (itemsWithSameName.AvailableStock == 0)
                        {
                            itemsWithSameName.Status = "Out";
                        }
                        else
                        {
                            itemsWithSameName.Status = "Out";
                        }



                        // Save changes to the database
                        await scopedcontext.AddAsync(itemclass);
                        await scopedcontext.SaveChangesAsync();

                        new StockResponse(true, $"Successfully issued and updated for item {itemclass.ItemName}.", itemclass);
                    }
                }

                return new StockResponse(true, "Issue process completed.", null);

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> GetRequisitionByEmail()
        {
            try
            {

                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var loggedinuserobject = await _extraServices.LoggedInUser();
                    var useremailexists = await scopedcontext.RequisitionApplication.Where(u => u.useremail == loggedinuserobject.Email).ToListAsync();
                    if (useremailexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", useremailexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetFormByStatusPending()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.RequisitionApplication.Where(u => u.ApprovedStatus == "Pending").OrderByDescending(x => x.ApprovedDate).ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetAllItemToBeReturned()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var itemsToBeReturned = await scopedContext.ReturnedItem
                        .Where(x => x.ReturnedStatus != "Returned")
                        .OrderByDescending(x => x.DateReturned)
                        .ToListAsync();

                    if (itemsToBeReturned == null || itemsToBeReturned.Count == 0)
                    {
                        return new StockResponse(false, "Items not found", null);
                    }

                    return new StockResponse(true, "Query successful", itemsToBeReturned);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> GetFormStatusApproved()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.RequisitionApplication.Where(u => u.ApprovedStatus == "Approved"|| u.DispatchStatus=="Incomplete"&& u.ApprovedStatus=="Issued").OrderByDescending(x => x.ApprovedDate).ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> SelectSerialNumber(SelectSerialvm selectSerialvm)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var applieditem = await scopedcontext.ApplyRequistionForm
                        .Where(u => u.ID == selectSerialvm.IssueID)
                        .FirstOrDefaultAsync();
                    var loggedinuserobject = await _extraServices.LoggedInUser();
                    var userEmail = loggedinuserobject.FirstName + ' ' + loggedinuserobject.LastName;

                 // Use ToListAsync to get multiple results

                    if (applieditem == null)
                    {
                        return new StockResponse(false, "Issue item not found.", null);
                    }


                        var itemclass = new SelectSerial
                        {
                            SerialNumber = "None",
                            SerialStatus = "Not issued",
                            ItemName = applieditem.itemName,
                            BrandName = applieditem.BrandName,
                            clientName = applieditem.clientName,
                            StockNeed = applieditem.stockNeed,
                            Requisitioner = applieditem.Requisitioner,
                            IssueID = applieditem.ID,
                            CategoryName = applieditem.CategoryName,
                            IssuedBy = userEmail,
                            IssueStatus = "Issued",
                            DateIssued = DateTime.Now,
                            QuantityOrdered = applieditem.Quantity,
                            Comments = "Working OK",
                            QuantityDispatched = selectSerialvm.QuantityDispatched,
                            Reason = selectSerialvm.Reason,
                            DispatchStatus = "Incomplete",
                            TotalStockOut = 0,
                            OutStandingBalance = selectSerialvm.QuantityDispatched,
                            TotalQuantityDispatchedForItem = 0,
                            TotalQuantityDispatchedForAnId = 0,
                            OrderNumber=applieditem.OrderNumber,
                            QuantityDispatchStatus="Incomplete",
                            NameToUse=applieditem.NameToUse,
                        };

                   

                        if (applieditem.ApprovedStatus == "Rejected")
                        {
                            return new StockResponse(false, "Check reason for rejection and revise again.", null);
                        }
                    applieditem.DateIssued = itemclass.DateIssued;

                        var totalStockOutAllItems = await scopedcontext.SelectSerial
                            .SumAsync(y => y.QuantityDispatched);
                        var totalQuantityDispatched = await scopedcontext.SelectSerial
                            .Where(y => y.IssueID == itemclass.IssueID) // Add the condition here
                            .SumAsync(y => y.QuantityDispatched);
                        itemclass.TotalQuantityDispatchedForItem = totalQuantityDispatched + itemclass.QuantityDispatched;
                        applieditem.QuantityDispatched = itemclass.TotalQuantityDispatchedForItem;
                        itemclass.OutStandingBalance = itemclass.QuantityOrdered - itemclass.TotalQuantityDispatchedForItem;
                    // Check if the quantity dispatched exceeds the outstanding balance
                    if (itemclass.OutStandingBalance < 0 || itemclass.TotalQuantityDispatchedForItem > itemclass.QuantityOrdered)
                    {
                        return new StockResponse(false, "Quantity dispatched cannot exceed the outstanding balance.", null);
                    }

                    itemclass.TotalStockOut = totalStockOutAllItems + itemclass.QuantityDispatched;
                        applieditem.OutStandingBalance = itemclass.OutStandingBalance;
                        scopedcontext.Update(applieditem);

                        // Retrieve the items with the same itemName from UploadPOFile
                        var itemsWithSameName = await scopedcontext.UploadPOFile
                            .Where(u => u.ItemName == applieditem.itemName && u.BrandName == applieditem.BrandName)
                            .OrderByDescending(y => y.DateAdded)
                            .FirstOrDefaultAsync();
                        if (itemsWithSameName == null)
                        {
                            return new StockResponse(false, "Item with such brand name does not exist", null);
                        }

                        itemclass.CategoryName = itemsWithSameName.CategoryName;
                        applieditem.ApprovedStatus = itemclass.IssueStatus;

                        if (itemsWithSameName.AvailableStock < itemclass.QuantityDispatched)
                        {
                            applieditem.ApprovedStatus = "Failed";
                            applieditem.RejectReason = "Not available...Will be restocked soon";
                            scopedcontext.Update(applieditem);
                            return new StockResponse(false, $"Not enough available stock to issue {itemclass.QuantityDispatched} units for item {itemclass.ItemName}.", null);
                        }
                        else
                        {
                            // Update stock quantities
                            itemsWithSameName.StockOut += itemclass.QuantityDispatched;
                            itemsWithSameName.AvailableStock -= itemsWithSameName.StockOut;
                        

                            if (itemsWithSameName.AvailableStock > itemsWithSameName.ReOrderLevel)
                            {
                                itemsWithSameName.Status = "Good";
                            }
                            else if (itemsWithSameName.AvailableStock < itemsWithSameName.ReOrderLevel || itemsWithSameName.AvailableStock > 0)
                            {
                                itemsWithSameName.Status = "Low";
                            }
                            else if (itemsWithSameName.AvailableStock == 0)
                            {
                                itemsWithSameName.Status = "Out";
                            }
                            else
                            {
                                itemsWithSameName.Status = "Out";
                            }
                            if (itemclass.QuantityOrdered == itemclass.TotalQuantityDispatchedForItem)
                            {
                                applieditem.DispatchStatus = "Complete";
                                itemclass.DispatchStatus = "Complete";
                            }
                        await scopedcontext.AddAsync(itemclass);
                        await scopedcontext.SaveChangesAsync();
                    }
                    }

                    // Save all SelectSerial objects to the database
                  

                    return new StockResponse(true, $"Successfully issued and updated  items.",null);
                
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }



        public async Task<StockResponse> GetSelectedSerials(int issueID)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.SelectSerial.Where(u => u.IssueID == issueID).ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> GetSelectedSerialsByID(int ID)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.SelectSerial.Where(u => u.Id == ID).FirstOrDefaultAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> GetSelectedSerialsToIssueByID(int ID)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.selectedSetialNumber.Where(u => u.IssueID == ID && u.SerialStatus=="Issued").ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetSelectedSerialsByIssueNo(int issuedNo)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.selectedSetialNumber.Where(u => u.IssuedNo == issuedNo && u.SerialStatus == "Issued").ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetSerialByIssued()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.AddProductDetails.Where(u => u.SerialStatus == "Not Issued" && u.ItemStatus=="Okay").ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetFormIssuedByID(int Issueid)
        
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.ApplyRequistionForm.Where(u => u.ID == Issueid).FirstOrDefaultAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetFormByStatusIssued()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.ApplyRequistionForm.Where(u => u.ApprovedStatus == "Issued").OrderByDescending(x => x.ApprovedDate).ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetFormByStatusIssuedByClient(string ClientName)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.ApplyRequistionForm.Where(u => u.ApprovedStatus == "Issued" && u.NameToUse==ClientName).OrderByDescending(x => x.ApprovedDate).ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> AddDeliveryNote(AddDeliveryNotevm addDeliveryNotevm)
        {
            try
            {
                if (addDeliveryNotevm.DeliveryNumber == "")
                    return new StockResponse(false, "Kindly provide delivery number", null);

                if (addDeliveryNotevm.MeansOfDelivery == "")
                    return new StockResponse(false, "Kindly provide means of delivery", null);

                if (addDeliveryNotevm.BatchQuantity == 0)
                    return new StockResponse(false, "Kindly provide batch quantity", null);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var itemexists = await scopedcontext.UploadPOFile
                        .FirstOrDefaultAsync(x => x.ID == addDeliveryNotevm.ItemID);

                    if (itemexists == null)
                        return new StockResponse(false, $"Item '{addDeliveryNotevm.ItemID}' doesn't exist", null);

                    var invoice_no_obj = GetBatchNumber().Result;
                    var batchnumber = "BATCH-" + invoice_no_obj;

                    var itemidexists = await scopedcontext.AddDeliveryNote
                        .Where(x => x.ItemID == addDeliveryNotevm.ItemID)
                        .OrderByDescending(y => y.DateCreated)
                        .FirstOrDefaultAsync();

                    var CalculateTotalClosed = await scopedcontext.ApprovalBatch
                        .Where(u => u.itemID == addDeliveryNotevm.ItemID && u.selectedOption == "Approve")
                        .SumAsync(y => y.ClosedQuantity);

                    var TotalDelivered = await scopedcontext.UploadPOFile
                        .FirstOrDefaultAsync(x => x.ID == addDeliveryNotevm.ItemID);

                    var deliveryclass = new AddDeliveryNote
                    {
                        DeliveryNumber = addDeliveryNotevm.DeliveryNumber,
                        BatchQuantity = addDeliveryNotevm.BatchQuantity,
                        BatchNumber = batchnumber,
                        DeliveryDate = addDeliveryNotevm.DeliveryDate,
                        MeansOfDelivery = addDeliveryNotevm.MeansOfDelivery,
                        DateCreated = DateTime.Now,
                        AirWayBillNumber = addDeliveryNotevm.AirWayBillNumber,
                        ItemID = addDeliveryNotevm.ItemID,
                        Reference_Number = await GetGeneratedref(),
                        PONumber = addDeliveryNotevm.PONumber,
                        TotalClosed = CalculateTotalClosed,
                        ExcessQuantityInBatch = 0,
                        ItemName = itemexists.ItemName,
                        BrandName = itemexists.BrandName,
                        CategoryName = itemexists.CategoryName,
                    };

                    // Set Product Status
                    if (deliveryclass.CategoryName == "Accesory")
                    {
                        deliveryclass.ProductStatus = "Complete";
                    }
                    else if (deliveryclass.CategoryName == "Product" || deliveryclass.CategoryName == "Spare Parts")
                    {
                        deliveryclass.ProductStatus = "Incomplete";
                    }

                    // Calculate total delivered for all items
                    var totalDeliveredForAllItems = await scopedcontext.AddDeliveryNote
                        .SumAsync(y => y.BatchQuantity);

                    deliveryclass.totalDeliveredForAllItems = totalDeliveredForAllItems + deliveryclass.BatchQuantity;

                    // Check for excess quantity
                    var totalQuantityInBatch = await scopedcontext.AddDeliveryNote
                        .Where(x => x.ItemID == addDeliveryNotevm.ItemID)
                        .SumAsync(y => y.BatchQuantity);

                    if (totalQuantityInBatch + addDeliveryNotevm.BatchQuantity > TotalDelivered.Quantity)
                    {
                        var excessQuantity = totalQuantityInBatch + addDeliveryNotevm.BatchQuantity - TotalDelivered.Quantity;
                        deliveryclass.ExcessQuantityInBatch = excessQuantity;
                    }

                    // Check quantity damaged
                    var quantitydamaged = await scopedcontext.StockAdjustment
                        .FirstOrDefaultAsync(x => x.BatchNumber == deliveryclass.BatchNumber);

                    if (quantitydamaged != null)
                        deliveryclass.quantityDamaged += quantitydamaged.QuantityDamaged;

                    // Total Quantity Logic
                    if (itemidexists != null)
                    {
                        deliveryclass.TotalQuantity = itemidexists.TotalQuantity + deliveryclass.BatchQuantity;
                        scopedcontext.Update(deliveryclass);
                    }
                    else
                    {
                        deliveryclass.TotalQuantity = deliveryclass.BatchQuantity;
                    }

                    // ProductNumbering generation for Product or Spare Parts
                    if (deliveryclass.CategoryName == "Product" || deliveryclass.CategoryName == "Spare Parts")
                    {
                        var new_numb = 0;
                        while (new_numb < deliveryclass.BatchQuantity)
                        {
                            new_numb++;
                            var new_numbering = new ProductNumbering
                            {
                                NumberValue = new_numb,
                                Reference_Number = deliveryclass.Reference_Number,
                                Type = deliveryclass.CategoryName,
                                Status = "UNASSIGNED"
                            };
                            await scopedcontext.ProductNumbering.AddAsync(new_numbering);
                        }
                    }

                    // Update PO File with delivery totals
                    if (TotalDelivered != null)
                    {
                        TotalDelivered.TotalDelivered = deliveryclass.TotalQuantity;
                        TotalDelivered.OutstandingQuantity = TotalDelivered.Quantity - TotalDelivered.TotalDelivered;
                        TotalDelivered.totalDeliveredForAllItems = deliveryclass.totalDeliveredForAllItems;

                        if (TotalDelivered.TotalDelivered >= TotalDelivered.Quantity)
                            TotalDelivered.ProductStatus = "Complete";
                        else
                            TotalDelivered.ProductStatus = "Incomplete";

                        scopedcontext.Update(TotalDelivered);
                    }

                    await scopedcontext.AddAsync(deliveryclass);
                    await scopedcontext.SaveChangesAsync();

                    return new StockResponse(true, $"Batch with delivery note number: '{addDeliveryNotevm.DeliveryNumber}' created successfully", null);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<int> GetBatchNumber()
        {

            var last_number_obj = await _dragonFlyContext.BatchNumber
                .OrderByDescending(y => y.DateCreated).FirstOrDefaultAsync();


            if (last_number_obj == null)
            {
                var newvalue = new BatchNo();

                newvalue.BatchNumber = 1;
                await _dragonFlyContext.AddAsync(newvalue);
                await _dragonFlyContext.SaveChangesAsync();
                return newvalue.BatchNumber;
            }

            last_number_obj.BatchNumber = last_number_obj.BatchNumber + 1;
            _dragonFlyContext.Update(last_number_obj);
            await _dragonFlyContext.SaveChangesAsync();

            return last_number_obj.BatchNumber;
        }
      

        public async Task<StockResponse> GetBatchByPONumber(string poNumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.AddDeliveryNote.Where(u => u.PONumber == poNumber).ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetProductDetailsByBatchNumber(string BatchNumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.AddProductDetails.Where(u => u.BatchNumber == BatchNumber).ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetSerialNumberByBatch(string BatchNumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.AddProductDetails.Where(u => u.BatchNumber == BatchNumber && u.ItemStatus=="Faulty").ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> GetBatchByItems(int itemId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var product_exist = await scopedcontext.AddDeliveryNote.Where(u => u.ItemID == itemId).ToListAsync();
                    if (product_exist == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", product_exist);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetSerialByBrandAndItem(string brandName, string itemName)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var serialNumbers = await scopedContext.AddProductDetails
                        .Where(u => u.SerialStatus == "Not Issued" && u.ItemStatus == "Okay" &&
                                    u.BrandName == brandName && u.ItemName == itemName)
                        .ToListAsync();

                    if (serialNumbers == null || serialNumbers.Count == 0)
                    {
                        return new StockResponse(false, "Serial numbers not found for the specified brand and item", null);
                    }

                    return new StockResponse(true, "Serial numbers queried successfully", serialNumbers);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> GetPOLinesNByID(int itemId)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var product_exist = await scopedcontext.UploadPOFile.Where(u => u.ID == itemId).FirstOrDefaultAsync();
                    if (product_exist == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", product_exist);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetProductbyBatchid(string BatchNumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var product_exist = await scopedcontext.AddDeliveryNote.Where(u => u.BatchNumber == BatchNumber).FirstOrDefaultAsync();
                    if (product_exist == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", product_exist);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetSerialByReferenceNumberAndItemName(string referenceNumber, string brandName, string itemName)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var product = await scopedContext.AddProductDetails
                        .Where(u => u.ReferenceNumber == referenceNumber &&
                                    u.BrandName == brandName &&
                                    u.ItemName == itemName)
                        .FirstOrDefaultAsync();

                    if (product == null)
                    {
                        return new StockResponse(false, "Product not found", null);
                    }

                    return new StockResponse(true, "Queried successfully", product);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, $"An error occurred: {ex.Message}", null);
            }
        }
        public async Task<StockResponse> StockAdjustment([FromBody] StockAdjustvm adjustStockvm)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var lastupdate = await scopedcontext.AddDeliveryNote.Where(y => y.BatchNumber == adjustStockvm.BatchNumber)
                        .FirstOrDefaultAsync();

                    if (lastupdate == null)
                    {
                        return new StockResponse(false, "Item not found", null);
                    }

                    var Items = new StockAdjustment
                    {
                        QuantityDamaged = adjustStockvm.QuantityDamaged,
                        Description = adjustStockvm.Description,
                        ItemID = adjustStockvm.ItemID,
                        BatchNumber = adjustStockvm.BatchNumber,
                        SerialNumber = adjustStockvm.SerialNumber,
                        ConditionStatus = "Okay",
                        ItemName = lastupdate.ItemName,
                        brandName = lastupdate.BrandName,
                    };

                    if (lastupdate.CategoryName == "Product")
                    {
                        // If the category is "Product," record the serial number and set the status as "Faulty."
                        Items.ConditionStatus = "Faulty";
                        var serialNumbersToUpdate = Items.SerialNumber;

                        var serialStatusItems = await scopedcontext.AddProductDetails
                            .Where(x => x.BatchNumber == Items.BatchNumber && serialNumbersToUpdate.Contains(x.SerialNumber))
                            .ToListAsync();

                        foreach (var serialStatusItem in serialStatusItems)
                        {
                            serialStatusItem.ItemStatus = Items.ConditionStatus;
                            scopedcontext.Update(serialStatusItem);
                        }
                    }
                    else if (lastupdate.CategoryName == "Accessory")
                    {
                        // If the category is "Accessory," record the quantity damaged.
                        Items.QuantityDamaged = adjustStockvm.QuantityDamaged;
                    }

                    // Calculate the total delivered for all items from StockAdjustment
                    var totalQuantityDamaged = await scopedcontext.StockAdjustment.SumAsync(y => y.QuantityDamaged);

                    // Add the current Items.QuantityDamaged to the total
                    Items.TotalQuantityDamaged = totalQuantityDamaged + Items.QuantityDamaged;

                    Items.ItemID = lastupdate.ItemID;
                    Items.CategoryName = lastupdate.CategoryName;

                    var quantityDamagedItems = await scopedcontext.AddDeliveryNote
                        .Where(x => x.BatchNumber == Items.BatchNumber)
                        .FirstOrDefaultAsync();

                    if (quantityDamagedItems != null)
                    {
                        quantityDamagedItems.quantityDamaged += Items.QuantityDamaged;
                        scopedcontext.Update(quantityDamagedItems);
                    }

                    var okQuantity = await scopedcontext.UploadPOFile
                        .Where(x => x.ID == Items.ItemID)
                        .FirstOrDefaultAsync();

                    var itemIDExists = await scopedcontext.AddDeliveryNote
                        .Where(x => x.ItemID == Items.ItemID)
                        .OrderByDescending(y => y.DateCreated)
                        .LastOrDefaultAsync();

                    if (itemIDExists != null)
                    {
                        // Update the specific item
                        itemIDExists.TotalDamages += adjustStockvm.QuantityDamaged;
                        okQuantity.TotalDamages = itemIDExists.TotalDamages;
                        itemIDExists.OKQuantity = okQuantity.TotalDelivered - itemIDExists.TotalDamages;
                        scopedcontext.Update(itemIDExists);

                        // Update the UploadPOFile OKQuantity
                        if (okQuantity != null)
                        {
                            okQuantity.OKQuantity = itemIDExists.OKQuantity;
                            scopedcontext.Update(okQuantity);
                        }

                        // Update the item in the context
                        scopedcontext.Update(itemIDExists);
                    }

                    await scopedcontext.SaveChangesAsync(); // Save changes to the database

                    // ... rest of the code ...

                    await scopedcontext.AddAsync(Items);
                    await scopedcontext.SaveChangesAsync();
                }

                return new StockResponse(true, "Updated successfully", adjustStockvm);
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> ApprovalsReview(ApproveBatchvm approvalProcessvm)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var loggedinuserobject = await _extraServices.LoggedInUser();
                    var userEmail = loggedinuserobject.FirstName + ' ' + loggedinuserobject.LastName;

                    if (loggedinuserobject == null)
                    {
                        return new StockResponse(false, "user not logged in. login again", null);
                    }

                    var applieditem = await scopedcontext.AddDeliveryNote
                        .Where(u => u.BatchNumber == approvalProcessvm.BatchNumber)
                        .FirstOrDefaultAsync();

                    if (applieditem == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    if (approvalProcessvm.selectedOption == "Reject")
                    {
                        // If selected option is "Reject," update ProductStatus to "Incomplete" and save
                        applieditem.ProductStatus = "Complete";
                        scopedcontext.Update(applieditem);
                        await scopedcontext.SaveChangesAsync();

                        return new StockResponse(false, "Successfully updated (Rejected)", applieditem);
                    }

                    var Items = new ApprovalBatch
                    {
                        selectedOption = approvalProcessvm.selectedOption,
                        RejectedReason = approvalProcessvm.RejectedReason,
                    };

                    Items.itemID = applieditem.ItemID;
                    Items.BatchNumber = applieditem.BatchNumber;
                    Items.ClosedQuantity = applieditem.BatchQuantity - applieditem.quantityDamaged;
                    Items.BrandName = applieditem.BrandName;
                    Items.ItemName = applieditem.ItemName;

                    await scopedcontext.AddAsync(Items);
                    await scopedcontext.SaveChangesAsync();
                    var itemexists = await scopedcontext.UploadPOFile
                        .Where(x => x.ItemName == Items.ItemName && x.BrandName == Items.BrandName)
                        .OrderByDescending(y => y.DateAdded)
                        .FirstOrDefaultAsync();
                    var totalAvailableStock = await scopedcontext.ApprovalBatch.SumAsync(y => y.ClosedQuantity);
                    Items.TotalAvalialbeStock = totalAvailableStock;
  
                    if (itemexists != null)
                    {
                       // If this is a new ApprovalBatch entry, assign CalculateTotalClosed directly to AvailableStock
                            itemexists.AvailableStock +=Items.ClosedQuantity ;
                        itemexists.TotalClosed += Items.ClosedQuantity;
                        }
                        else
                        {
                            // If an existing batch already summed up the quantity, just set AvailableStock to CalculateTotalClosed
                            itemexists.AvailableStock = Items.ClosedQuantity;
                        itemexists.TotalClosed = Items.ClosedQuantity;
                    }
                    
                        // Update Status based on AvailableStock (if needed)
                        if (itemexists.AvailableStock > itemexists.ReOrderLevel)
                        {
                            itemexists.Status = "Good";
                        }
                        else if (itemexists.AvailableStock < itemexists.ReOrderLevel || itemexists.AvailableStock > 0)
                        {
                            itemexists.Status = "Low";
                        }
                        else
                        {
                            itemexists.Status = "Out";
                        }

                        scopedcontext.Update(itemexists);
                    



                    //applieditem.TotalClosed = CalculateTotalClosed;

                    scopedcontext.Update(applieditem);

                    if (approvalProcessvm.selectedOption == "Approve")
                    {
                        approvalProcessvm.RejectedReason = "";
                    }

                    applieditem.selectedOption = approvalProcessvm.selectedOption;
                    applieditem.AprrovedDate = DateTime.Now;
                    applieditem.RejectedReason = approvalProcessvm.RejectedReason;
                    applieditem.ApprovedBy = userEmail;

                    if (approvalProcessvm.selectedOption == "Reject")
                    {
                        applieditem.ProductStatus = "Complete";
                    }
                    else
                    {
                        applieditem.ProductStatus = "Closed";
                    }

                    scopedcontext.Update(applieditem);
                    await scopedcontext.SaveChangesAsync();

                    return new StockResponse(true, "Successfully updated", applieditem);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

            public async Task<StockResponse> GetBatchCompleteStatus()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.AddDeliveryNote.Where(u => u.ProductStatus == "Pending").OrderByDescending(x => x.AprrovedDate).ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> GetBatchByBatchNumber(string BatchNumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var product_exist = await scopedcontext.AddDeliveryNote.Where(u => u.BatchNumber == BatchNumber).FirstOrDefaultAsync();
                    if (product_exist == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", product_exist);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> AddPOItemLines(AddPOItemLinesvm addBatchDetailsvm)
        {
            try
            {
                if (addBatchDetailsvm.ItemName == "")
                {

                    return new StockResponse(false, "Kindly provide an item name ", null);
                }
                if (addBatchDetailsvm.CategoryName == "")
                {
                    return new StockResponse(false, "Kindly provide category", null);

                }

                if (addBatchDetailsvm.Currency == "")
                {
                    return new StockResponse(false, "Kindly provide currency", null);
                }
                if (addBatchDetailsvm.UnitPrice < 0)
                {
                    return new StockResponse(false, "Kindly provide unit price", null);
                }
                if (addBatchDetailsvm.Quantity < 0)
                {
                    return new StockResponse(false, "Kindly provide quantity", null);
                }
                if (addBatchDetailsvm.Warranty < 0)
                {
                    return new StockResponse(false, "Kindly provide warranty", null);
                }
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var invoiceexists = await scopedcontext.PurchaseOrderss
                 .Where(y => y.PONumber == addBatchDetailsvm.PONumber).OrderByDescending(y => y.PONumber).FirstOrDefaultAsync();
                    if (invoiceexists == null)
                    {
                        return new StockResponse(false, "PO Number does not exist", null);
                    }

                    var loggedinuserobject = await _extraServices.LoggedInUser();

                    var userEmail = loggedinuserobject.FirstName + ' ' + loggedinuserobject.LastName;

                    if (loggedinuserobject == null)
                    {

                        return new StockResponse(false, "user not logged in. login again", null);

                    }




                    var itemNameexists = await scopedcontext.AddItem
                   .Where(y => y.ItemName == addBatchDetailsvm.ItemName && y.BrandName==addBatchDetailsvm.BrandName ).FirstOrDefaultAsync();
                    if (itemNameexists == null)
                    {
                        return new StockResponse(false, "Item name does not exist", null);
                    }

                    var itemclass = new UploadPOFile
                    {
                        ItemName = addBatchDetailsvm.ItemName,
                        CategoryName = addBatchDetailsvm.CategoryName,
                        Currency = addBatchDetailsvm.Currency,
                        UnitPrice = addBatchDetailsvm.UnitPrice,
                        Warranty = addBatchDetailsvm.Warranty,
                        Quantity = addBatchDetailsvm.Quantity,
                        WarrantyStartDate = addBatchDetailsvm.WarrantyStartDate,
                        UpdatedBy = userEmail,
                        UpdatedOn = addBatchDetailsvm.UpdatedOn,
                        PONumber = addBatchDetailsvm.PONumber,
                        BrandName = addBatchDetailsvm.BrandName,
                        Amount = "Unknown",
                        ProductStatus = "Incomplete",
                        Status = "Out",







                    };
                    //var itemassigned = await scopedcontext.AddProductDetails.Where(x => x.ItemID == itemclass.InvoiceLineId).FirstOrDefaultAsync();

                    //if (itemassigned != null)
                    //{
                    //    itemassigned.ProductStatus = itemclass.Status;
                    //    return new StockResponse(false, "ITEM ALREADY COMPLETE", null);
                    //}
                    var itemexists = await scopedcontext.UploadPOFile.Where(x => x.ItemName == addBatchDetailsvm.ItemName && x.BrandName == addBatchDetailsvm.BrandName && x.PONumber == addBatchDetailsvm.PONumber && x.CategoryName == itemNameexists.Category).FirstOrDefaultAsync();
                    var itemalreadyexists = await scopedcontext.UploadPOFile
                         .Where(x => x.ItemName == addBatchDetailsvm.ItemName && x.BrandName == addBatchDetailsvm.BrandName)
                         .OrderByDescending(y => y.DateAdded)
                         .FirstOrDefaultAsync();

                    if (itemalreadyexists != null)
                    {
                        // Item already exists, update the existing item instead of adding a new one
                        itemalreadyexists.AvailableStock = itemalreadyexists.TotalClosed; // Update quantity
                        itemalreadyexists.TotalClosed = itemalreadyexists.TotalClosed;
                        itemclass.AvailableStock = itemalreadyexists.AvailableStock;
                        itemclass.TotalClosed = itemalreadyexists.TotalClosed;
                        itemclass.StockOut = itemalreadyexists.StockOut;
                        itemclass.Status = itemalreadyexists.Status;
                        // Add any other necessary updates here

                        scopedcontext.Update(itemalreadyexists);
                        await scopedcontext.SaveChangesAsync();
                    }

                        itemclass.CategoryName = itemNameexists.Category;
                    itemclass.ReOrderLevel = itemNameexists.ReOrderLevel;

                    if (itemexists != null)
                    {
                        return new StockResponse(false, $" Invoice {addBatchDetailsvm.PONumber} with'{addBatchDetailsvm.BrandName}-{addBatchDetailsvm.ItemName} already exists ", null);
                    }


                    itemclass.Reference_Number = await GetGeneratedref();
                    itemclass.TotalUnitPrice = itemclass.UnitPrice * itemclass.Quantity;

                    if (itemclass.CategoryName == "Accesory")
                    {
                        itemclass.ProductStatus = "Incomplete";
                    }
                    else
                    {
                        itemclass.ProductStatus = "Incomplete";
                    }

                    if (itemclass.Quantity == itemclass.Quantity) { 
                    }


                    itemclass.WarrantyEndDate = itemclass.WarrantyStartDate.AddMonths(itemclass.Warranty);
                    itemclass.WarrantyStatus = itemclass.WarrantyEndDate < DateTime.Now
    ? "Expired"
    : "Under Warranty";
                    await scopedcontext.AddAsync(itemclass);
                    await scopedcontext.SaveChangesAsync();
                    if (itemclass.CategoryName == "Product")
                    {
                        var new_numb = 0;



                        while (new_numb < itemclass.Quantity)
                        {
                            new_numb++;
                            var new_numbering = new ProductNumbering
                            {
                                NumberValue = new_numb,
                                Reference_Number = itemclass.Reference_Number,
                                Type = "Product",
                                Status = "UNASSIGNED"
                            };

                            await _dragonFlyContext.AddAsync(new_numbering);
                            await _dragonFlyContext.SaveChangesAsync();
                        }

                        // Update the item status based on AvailableStock and ReOrderLevel
                        if (itemclass.AvailableStock >= itemclass.ReOrderLevel)
                        {
                            itemclass.Status = "Good";
                        }
                        else if (itemclass.AvailableStock == 0)
                        {
                            itemclass.Status = "Out";
                        }
                        else
                        {
                            itemclass.Status = "Low";
                        }

                        scopedcontext.Update(itemclass);
                        await scopedcontext.SaveChangesAsync();


                    
                }

                    return new StockResponse(true, $"Item '{addBatchDetailsvm.BrandName}-{addBatchDetailsvm.ItemName}' in invoice{addBatchDetailsvm.PONumber}  created successfully", null);

                }

            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);

            }

        }
        public async Task<StockResponse> MarkPOComplete( string PONumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
               

                    var applieditem = await scopedcontext.PurchaseOrderss.Where(u => u.PONumber == PONumber).FirstOrDefaultAsync();
                    var uploadedpo = await scopedcontext.UploadPOFile.Where(u => u.PONumber == PONumber).FirstOrDefaultAsync();
                    if (applieditem == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    // Check if there are any items associated with the PO
                    var itemCount = await scopedcontext.UploadPOFile
                        .Where(item => item.PONumber == PONumber)
                        .CountAsync();

                    if (itemCount == 0)
                    {
                        return new StockResponse(false, "No items found for this Purchase Order....please fill in the items first", null);
                    }
                    if (applieditem.CaptureStatus == "Pending")
                    {
                        return new StockResponse(false, "Purchase Order is already under review", null);
                    }
                    if (uploadedpo != null)
                    {
                        uploadedpo.CaptureStatus = "Pending";
                        scopedcontext.Update(uploadedpo);
                    }
                   
                    applieditem.DateCreated = DateTime.Now;
                    applieditem.CaptureStatus = "Pending";

                   

                    scopedcontext.Update(applieditem);
                    await scopedcontext.SaveChangesAsync();

                    return new StockResponse(true, "Successfully updated ", applieditem);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> POApprovalReview(POApprovalvm pOApprovalvm)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var loggedinuserobject = await _extraServices.LoggedInUser();

                    var userEmail = loggedinuserobject.FirstName + ' ' + loggedinuserobject.LastName;

                    if (loggedinuserobject == null)
                    {

                        return new StockResponse(false, "user not logged in. login again", null);

                    }

                    var applieditem = await scopedcontext.PurchaseOrderss.Where(u => u.PONumber == pOApprovalvm.PONumber).FirstOrDefaultAsync();
                    var uploadedpoitem = await scopedcontext.UploadPOFile.Where(u => u.PONumber == pOApprovalvm.PONumber).FirstOrDefaultAsync();
                    if (applieditem == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    if (uploadedpoitem == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }

                    if (pOApprovalvm.selectedOption == "Approve")
                    {
                        pOApprovalvm.RejectedReason = "";
                    }
                    var poApproval =new POApproval
                    {
                        selectedOption=pOApprovalvm.selectedOption,
                        RejectedReason=pOApprovalvm.RejectedReason,
                        AprrovedDate = DateTime.Now,
                        PONumber=pOApprovalvm.PONumber,


                    };
                    applieditem.DateCreated = pOApprovalvm.AprrovedDate;

                    if (pOApprovalvm.selectedOption == "Reject")
                    {
                        applieditem.CaptureStatus = "Incomplete";
                        uploadedpoitem.CaptureStatus = "Incomplete";
                        poApproval.ApprovalStatus = "Incomplete";
                        scopedcontext.Update(applieditem);
                        scopedcontext.Update(uploadedpoitem);

                    }
                    else
                    {
                        applieditem.CaptureStatus = "Complete";
                        uploadedpoitem.CaptureStatus = "Complete";
                        
                        poApproval.ApprovalStatus = "Incomplete";
                        scopedcontext.Update(applieditem);
                        scopedcontext.Update(uploadedpoitem);
                    }


                    await scopedcontext.AddAsync(poApproval);
                
                    await scopedcontext.SaveChangesAsync();

                    
                    return new StockResponse(true, "Successfully updated", poApproval);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetPOByStatusPending()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.PurchaseOrderss.Where(u => u.CaptureStatus == "Pending").OrderByDescending(x => x.DateCreated).ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetAllPOSWithStatusComplete()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var allpos = await scopedcontext.PurchaseOrderss
                        .Where(x => x.CaptureStatus == "Complete" && x.DeliveryStatus == "Incomplete")
                        .ToListAsync();

                    if (allpos==null)  // Check for empty list using Count instead of null check
                    {
                        return new StockResponse(false, "No POs with the specified status combination", null);
                    }


                    return new StockResponse(true, "Successfully queried", allpos);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetPONumberbyNumber(string POnumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.PurchaseOrderss.Where(u => u.PONumber == POnumber).OrderByDescending(x => x.DateCreated).FirstOrDefaultAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
      public async Task<StockResponse> MarkBatchCompplete(string BatchNumber) // Corrected method name
{
    try
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

            var applieditem = await scopedcontext.AddDeliveryNote.Where(u => u.BatchNumber == BatchNumber).FirstOrDefaultAsync();
            var serialNumbers = await scopedcontext.AddProductDetails.Where(u => u.BatchNumber == BatchNumber).ToListAsync();

            if (applieditem == null)
            {
                return new StockResponse(false, "Batch not found", null);
            }
            else if (applieditem.ProductStatus == "Incomplete")
            {
                if (serialNumbers == null || serialNumbers.Count != applieditem.BatchQuantity)
                {
                    return new StockResponse(false, "Serial numbers missing or count does not match expected quantity", null);
                }
               
            }
            else if (applieditem.ProductStatus == "Pending")
            {
                return new StockResponse(false, "Batch is already on the approval process", null);
            }
                    else if (applieditem.ProductStatus == "Closed")
                    {
                        return new StockResponse(false, "Batch is already marked as complete", null);
                    }
                    else
                    {
                        // Update the product status to "Pending" since serial numbers match the expected quantity.
                        applieditem.ProductStatus = "Pending";
                    }


                    applieditem.DateCreated = DateTime.Now;

            scopedcontext.Update(applieditem);
            await scopedcontext.SaveChangesAsync();

            return new StockResponse(true, "Batch marked as complete successfully", applieditem);
        }
    }
    catch (Exception ex)
    {
        return new StockResponse(false, ex.Message, null);
    }
}
        public async Task<StockResponse> CreateRequisitionHeader(RequisitionDatavm header)
        {
            try
            {
                // Validate basic fields
                if (string.IsNullOrWhiteSpace(header.stockNeed) || string.IsNullOrWhiteSpace(header.Department))
                    return new StockResponse(false, "Stock need and department are required", null);

                var loggedInUser = await _extraServices.LoggedInUser();
                if (loggedInUser == null)
                    return new StockResponse(false, "User not logged in", null);

                string nameToUse = header.stockNeed == "Customer Order" ? header.clientName : $"{loggedInUser.FirstName} {loggedInUser.LastName}";
                string batchNumber = await GetClientBatch(header.clientName, nameToUse, header.stockNeed);

                var newRequisition = new RequisitionApplication
                {
                    stockNeed = header.stockNeed,
                    Department = header.Department,
                    clientName = header.clientName,
                    Purpose = header.Purpose,
                    Description = header.Description,
                    ReferenceNumber = header.ReferenceNumber,
                    Currency =header.Currency ,
                    Requisitioner = nameToUse,
                    OrderNumber = batchNumber,
                    IssuedByDate = DateTime.Now,
                    ApprovedStatus = "Incomplete",
                    DispatchStatus = "Incomplete",
                    ApplicationStatus = "Incomplete",
                    useremail = loggedInUser.Email,
                    NameToUse = nameToUse,
                    DateRequisitioned = DateTime.Now,
                };

                _dragonFlyContext.RequisitionApplication.Add(newRequisition);
                await _dragonFlyContext.SaveChangesAsync();

                return new StockResponse(true, "Requisition header created", newRequisition);
            }
            catch (Exception ex)
            {
                return new StockResponse(false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<StockResponse> AddRequisitionItem(AddRequisitionItemDto dto)
        {
            try
            {
                var header = await _dragonFlyContext.RequisitionApplication
                    .FirstOrDefaultAsync(r => r.Requisitioner == dto.RequisitionId);
                var headerData = await _dragonFlyContext.RequisitionApplication
                    .FirstOrDefaultAsync(r => r.ReferenceNumber == dto.ReferenceNumber);

                if (header == null)
                    return new StockResponse(false, "Requisition header not found", null);

                var currentDate = DateTime.Now;

                // ✅ Get active price for the item in the specified currency
                var activePrice = await _dragonFlyContext.PriceRecord
                    .Where(p => p.ItemId == dto.ItemID &&
                                !p.IsDeleted &&
                                p.Currency == header.Currency &&
                                p.Status == "Active" &&
                                p.EffectiveFrom <= currentDate &&
                                (p.EffectiveTo == null || p.EffectiveTo >= currentDate))
                    .OrderByDescending(p => p.EffectiveFrom)
                    .FirstOrDefaultAsync();

                if (activePrice == null)
                    return new StockResponse(false, "No active price found for this item in the selected currency.", null);

                // ✅ Check available stock before proceeding
                var availableStock = await _dragonFlyContext.UploadPOFile
                    .Where(s => s.ItemName == dto.ItemName && s.BrandName == dto.BrandName)
                    .Select(s => s.AvailableStock)
                    .FirstOrDefaultAsync();

                if (availableStock <= 0)
                    return new StockResponse(false, "Item is currently out of stock.", null);

                if (dto.Quantity > availableStock)
                    return new StockResponse(false, $"Requested quantity ({dto.Quantity}) exceeds available stock ({availableStock}).", null);

                // ✅ Calculate total amount with optional discount
                decimal totalAmount = activePrice.SellingPrice * dto.Quantity;

                if (dto.DiscountNumerator.HasValue && dto.DiscountDenominator.HasValue && dto.DiscountDenominator != 0)
                {
                    decimal discount = (decimal)dto.DiscountNumerator.Value / dto.DiscountDenominator.Value;
                    totalAmount *= (1 - discount);
                }
                if (!int.TryParse(dto.ItemID, out int itemIdInt))
                {
                    // Handle the invalid case here (maybe return an error response)
                    return new StockResponse(false, "Invalid Item ID", null);
                }

                var CategoryData = await _dragonFlyContext.AddItem
                    .FirstOrDefaultAsync(r => r.ItemID == itemIdInt);


                // ✅ Create and save the item
                var newItem = new ApplyRequisitionItem
                {
                    Requisitooner = dto.RequisitionId,
                    ItemName = dto.ItemName,
                    ItemID = dto.ItemID,
                    BrandName = dto.BrandName,
                    Quantity = dto.Quantity,
                    DateAdded = DateTime.Now,
                    Currency = header.Currency,
                    UnitPrice = activePrice.SellingPrice,
                    TotalAmount = totalAmount,
                    DiscountNumerator = dto.DiscountNumerator,
                    DiscountDenominator = dto.DiscountDenominator,
                    RequisitionName = header.Requisitioner,
                    RequisitionId = headerData.RequisitionID,
                    ReferenceNumber = dto.ReferenceNumber,
                    CategoryName = CategoryData.Category,
                    Status = "Incomplete",
                    DispatchComment ="None",
                };

                await _dragonFlyContext.ApplyRequisitionItem.AddAsync(newItem);
                await _dragonFlyContext.SaveChangesAsync();

                return new StockResponse(true, "Item successfully added to requisition.", newItem);
            }
            catch (Exception ex)
            {
                return new StockResponse(false, $"Error: {ex.Message}", null);
            }
        }
        public async Task<StockResponse> IssueRequisitionItemAsync(
       int requisitionItemId,
       int quantityToDispatch,
       string issuedBy,
       string comment = null,
       List<string> selectedSerialNumbers = null)
        {
            try
            {
                var item = await _dragonFlyContext.ApplyRequisitionItem
                    .FirstOrDefaultAsync(r => r.RequisitionItemID == requisitionItemId);

                if (item == null)
                    return new StockResponse(false, "Requisition item not found.", null);

                var poRecord = await _dragonFlyContext.UploadPOFile
                    .FirstOrDefaultAsync(s => s.ItemName == item.ItemName && s.BrandName == item.BrandName);

                if (poRecord == null || poRecord.AvailableStock < quantityToDispatch)
                    return new StockResponse(false, "Insufficient stock to dispatch.", null);

                if (!int.TryParse(item.ItemID, out int itemIdInt))
                    return new StockResponse(false, "Invalid Item ID.", null);

                var itemData = await _dragonFlyContext.AddItem
                    .FirstOrDefaultAsync(a => a.ItemID == itemIdInt);

                if (itemData == null)
                    return new StockResponse(false, "Item details not found in AddItem.", null);

                if (itemData.Category?.ToLower() == "product")
                {
                    if (selectedSerialNumbers == null || selectedSerialNumbers.Count != quantityToDispatch)
                        return new StockResponse(false, "Serial numbers must match dispatch quantity.", null);

                    var upperSelectedSerials = selectedSerialNumbers.Select(s => s.ToUpper()).ToList();
                    var potentialSerials = await _dragonFlyContext.AddProductDetails
                        .Where(s => s.ItemIDdetails == itemIdInt && s.SerialStatus == "Not Issued")
                        .ToListAsync();

                    var validSerials = potentialSerials
                        .Where(s => upperSelectedSerials.Contains(s.SerialNumber.ToUpper()))
                        .ToList();

                    var validSerialNumbers = validSerials.Select(s => s.SerialNumber.ToUpper()).ToList();
                    var missingSerials = upperSelectedSerials.Except(validSerialNumbers).ToList();

                    if (missingSerials.Any())
                        return new StockResponse(false, $"These serials are invalid or already issued: {string.Join(", ", missingSerials)}", null);

                    foreach (var serial in validSerials)
                    {
                        serial.IsIssued = true;
                        serial.DateIssued = DateTime.Now;
                        serial.SerialStatus = "Issued";
                        serial.ReferenceNumber = item.ReferenceNumber;
                        serial.IssuedBy = issuedBy;
                        serial.ClientName = item.RequisitionName;

                        await _dragonFlyContext.AddAsync(new selectedSetialNumber
                        {
                            SerialStatus = "Issued",
                            SerialNumber = serial.SerialNumber,
                            IMEII1 = serial.IMEI1,
                            IMEI2 = serial.IMEI2,
                            DateUpdated = DateTime.Now,
                            IssueID = item.RequisitionItemID,
                            IssuedNo = item.RequisitionItemID,
                            ClientName = item.RequisitionName,
                            ReferenceNumber = item.ReferenceNumber,
                        });
                    }
                }

                item.QuantityDispatched += quantityToDispatch;
                item.OutstandingBalance = item.Quantity - item.QuantityDispatched;
                item.Status = item.OutstandingBalance == 0 ? "Complete" : "Partially Issued";
                item.DispatchComment = comment ?? "Partial issue with no comment provided.";

                poRecord.AvailableStock -= quantityToDispatch;
                poRecord.StockOut += quantityToDispatch;

                if (poRecord.AvailableStock > poRecord.ReOrderLevel)
                    poRecord.Status = "Good";
                else if (poRecord.AvailableStock > 0)
                    poRecord.Status = "Low";
                else
                    poRecord.Status = "Out";

                // ✅ Set DispatchStatus = "Issued" at this stage
                var requisition = await _dragonFlyContext.RequisitionApplication
                    .FirstOrDefaultAsync(r => r.ReferenceNumber == item.ReferenceNumber);

                if (requisition != null)
                {
                    requisition.DispatchStatus = "Issued";
                    _dragonFlyContext.Entry(requisition).State = EntityState.Modified;
                }

                await _dragonFlyContext.SaveChangesAsync();

                return new StockResponse(true, "Item issued successfully. Awaiting requisitioner confirmation.", item);
            }
            catch (Exception ex)
            {
                return new StockResponse(false, $"Error while issuing item: {ex.Message}", null);
            }
        }

        public async Task<StockResponse> MarkRequisitionAsIssuedAsync(int requisitionItemId, string issuedBy)
        {
            try
            {
                var item = await _dragonFlyContext.ApplyRequisitionItem
                    .FirstOrDefaultAsync(r => r.RequisitionItemID == requisitionItemId);

                if (item == null)
                    return new StockResponse(false, "Requisition item not found.", null);

                var applicationData = await _dragonFlyContext.RequisitionApplication
                    .FirstOrDefaultAsync(r => r.RequisitionID == item.RequisitionId);

                if (applicationData == null)
                    return new StockResponse(false, "Related application not found.", null);

                // ✅ Ensure the person confirming is the original requisitioner
                if (!string.Equals(applicationData.Requisitioner, issuedBy, StringComparison.OrdinalIgnoreCase))
                    return new StockResponse(false, "Only the original requisitioner can confirm this issuance.", null);

                var previousStockOut = await _dragonFlyContext.SelectSerial
                    .Where(s => s.ItemName == item.ItemName && s.BrandName == item.BrandName)
                    .OrderByDescending(s => s.DateIssued)
                    .Select(s => s.TotalStockOut)
                    .FirstOrDefaultAsync();
                var previousItemStock = await _dragonFlyContext.UploadPOFile
                  .Where(s => s.ItemName == item.ItemName && s.BrandName == item.BrandName)
                  .FirstOrDefaultAsync();

                var itemData = await _dragonFlyContext.AddItem
                    .FirstOrDefaultAsync(a => a.ItemID == int.Parse(item.ItemID));

                if (itemData == null)
                    return new StockResponse(false, "Item details not found.", null);

                var newTotalStockOut = previousStockOut + item.QuantityDispatched;

                // Add to SelectSerial
                var serialRecord = new SelectSerial
                {
                    SerialNumber = itemData.Category?.ToLower() == "product" ? "Issued via confirmation" : "None",
                    SerialStatus = itemData.Category?.ToLower() == "product" ? "Issued" : "Not issued",
                    ItemName = item.ItemName,
                    BrandName = item.BrandName,
                    clientName = applicationData.clientName,
                    StockNeed = applicationData.stockNeed,
                    Requisitioner = applicationData.Requisitioner,
                    IssueID = item.RequisitionItemID,
                    CategoryName = itemData.Category,
                    IssuedBy = issuedBy,
                    IssueStatus = "Issued",
                    DateIssued = DateTime.Now,
                    QuantityOrdered = item.Quantity,
                    QuantityDispatched = item.QuantityDispatched,
                    Comments = item.DispatchComment ?? "Issued via requisitioner confirmation.",
                    Reason = "Issued through requisition (confirmed)",
                    DispatchStatus = "Closed", // ✅ This item is now closed
                    TotalStockOut = newTotalStockOut,
                    OutStandingBalance = item.OutstandingBalance,
                    TotalQuantityDispatchedForItem = item.QuantityDispatched,
                    TotalQuantityDispatchedForAnId = item.QuantityDispatched,
                    OrderNumber = item.ReferenceNumber,
                    QuantityDispatchStatus = "Closed",
                    NameToUse = applicationData.NameToUse
                };
                previousItemStock.StockOut = newTotalStockOut;
                _dragonFlyContext.Entry(previousItemStock).State = EntityState.Modified;


                await _dragonFlyContext.AddAsync(serialRecord);

                // ✅ Update item-level status to "Closed"
                item.Status = "Closed";
                _dragonFlyContext.Entry(item).State = EntityState.Modified;

                await _dragonFlyContext.SaveChangesAsync();

                // ✅ Check if all items for the same requisition are now marked "Closed"
                var allItemsClosed = await _dragonFlyContext.ApplyRequisitionItem
                    .Where(r => r.ReferenceNumber == item.ReferenceNumber)
                    .AllAsync(r => r.Status.ToLower() == "closed");

                if (allItemsClosed)
                {
                    var requisition = await _dragonFlyContext.RequisitionApplication
                        .FirstOrDefaultAsync(r => r.ReferenceNumber == item.ReferenceNumber);

                    if (requisition != null)
                    {
                        requisition.DispatchStatus = "Closed";
                        _dragonFlyContext.Entry(requisition).State = EntityState.Modified;
                        await _dragonFlyContext.SaveChangesAsync();
                    }
                }

                return new StockResponse(true, "Item marked as Closed. Requisition status updated if all items are closed.", null);
            }
            catch (Exception ex)
            {
                return new StockResponse(false, $"Error confirming requisition item: {ex.Message}", null);
            }
        }


        public async Task<StockResponse> RemoveDiscount(int requisitionItemId, string approver,string Reason)
        {
            try
            {
                // Get the requisition item to be updated
                var item = await _dragonFlyContext.ApplyRequisitionItem
                    .FirstOrDefaultAsync(i => i.RequisitionItemID == requisitionItemId);

                if (item == null)
                    return new StockResponse(false, "Requisition item not found.", null);

                // Remove the discount by setting both numerator and denominator to null
                item.DiscountNumerator = null;
                item.DiscountDenominator = null;
                item.EditedBy = approver; // Store the person who removed the discount
                item.DateEdited = DateTime.Now; // Store the person who approved the removal of the discount
                item.IsApprover = true;
                item.Reason =Reason;// Set IsApprover flag to true because approver is making the change

                // Calculate the total amount after removing the discount
                item.TotalAmount = item.UnitPrice * item.Quantity; // Recalculate total based on unit price and quantity

                // Update the item in the database
                _dragonFlyContext.ApplyRequisitionItem.Update(item);
                await _dragonFlyContext.SaveChangesAsync();

                return new StockResponse(true, "Discount successfully removed.", item);
            }
            catch (Exception ex)
            {
                return new StockResponse(false, $"Error: {ex.Message}", null);
            }
        }
        public async Task<StockResponse> AmendDiscount(int requisitionItemId, decimal newDiscountNumerator, decimal newDiscountDenominator, string amendedBy, string Reason)
        {
            try
            {
                // Get the requisition item to be updated
                var item = await _dragonFlyContext.ApplyRequisitionItem
                    .FirstOrDefaultAsync(i => i.RequisitionItemID == requisitionItemId);

                if (item == null)
                    return new StockResponse(false, "Requisition item not found.", null);

                // Validate the new discount values
                if (newDiscountDenominator == 0)
                    return new StockResponse(false, "Discount denominator cannot be zero.", null);

                if (newDiscountNumerator < 0 || newDiscountDenominator < 0)
                    return new StockResponse(false, "Discount values cannot be negative.", null);

                // Convert decimal to int (you can choose rounding, truncation, or rounding off)
                item.DiscountNumerator = (int)newDiscountNumerator;
                item.DiscountDenominator = (int)newDiscountDenominator;
                item.EditedBy = amendedBy; // Store the person who amended the discount
                item.DateEdited = DateTime.Now; // Store the date of the amendment
                item.IsApprover = true;
                item.Reason = Reason;// Set IsApprover flag to true because approver is making the amendment

                // Calculate the new total amount with the updated discount
                decimal discount = newDiscountNumerator / newDiscountDenominator;
                decimal newTotalAmount = item.UnitPrice * item.Quantity * (1 - discount);
                item.TotalAmount = newTotalAmount;

                // Update the item in the database
                _dragonFlyContext.ApplyRequisitionItem.Update(item);
                await _dragonFlyContext.SaveChangesAsync();

                return new StockResponse(true, "Discount successfully amended.", item);
            }
            catch (Exception ex)
            {
                return new StockResponse(false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<StockResponse> MarkRequisitionComplete(string referenceNumber)
        {
            try
            {
                // Fetch the requisition based on the reference number
                var requisition = await _dragonFlyContext.RequisitionApplication
                    .FirstOrDefaultAsync(r => r.ReferenceNumber == referenceNumber);

                if (requisition == null)
                    return new StockResponse(false, "Requisition not found", null);

                // Check if there are no requisition items
                var requisitionItems = await _dragonFlyContext.ApplyRequisitionItem
                    .Where(r => r.RequisitionId == requisition.RequisitionID)
                    .ToListAsync();

                if (requisitionItems.Count == 0)
                    return new StockResponse(false, "Cannot mark as complete. No items found for this requisition.", null);

                // If already marked as Pending, return error
                if (requisition.ApprovedStatus == "Pending" && requisition.ApplicationStatus == "Pending")
                    return new StockResponse(false, "This requisition is already marked as pending approval.", null);

                // Update statuses
                requisition.ApprovedStatus = "Pending";
                requisition.ApplicationStatus = "Pending";

                await _dragonFlyContext.SaveChangesAsync();

                return new StockResponse(true, "Requisition marked as complete and pending approval", requisition);
            }
            catch (Exception ex)
            {
                return new StockResponse(false, $"An error occurred: {ex.Message}", null);
            }
        }

        public async Task<StockResponse> EditRequisitionItem(EditRequisitionItemDto dto)
        {
            try
            {
                // Find the item to edit
                var item = await _dragonFlyContext.ApplyRequisitionItem
                    .FirstOrDefaultAsync(i => i.RequisitionItemID == dto.RequisitionItemID);

                if (item == null)
                    return new StockResponse(false, "Requisition item not found", null);

                // ✅ Get current price for the item
                var activePrice = await _dragonFlyContext.PriceRecord
                    .Where(p => p.ItemId == item.ItemID &&
                                !p.IsDeleted &&
                                p.Currency == item.Currency &&
                                p.Status == "Active" &&
                                p.EffectiveFrom <= DateTime.Now &&
                                (p.EffectiveTo == null || p.EffectiveTo >= DateTime.Now))
                    .OrderByDescending(p => p.EffectiveFrom)
                    .FirstOrDefaultAsync();

                if (activePrice == null)
                    return new StockResponse(false, "Active price not found for this item.", null);

                // ✅ Check available stock
                var availableStock = await _dragonFlyContext.UploadPOFile
                    .Where(s => s.ItemName == item.ItemName && s.BrandName == item.BrandName)
                    .Select(s => s.AvailableStock)
                    .FirstOrDefaultAsync();

                if (availableStock <= 0)
                    return new StockResponse(false, "Item is currently out of stock.", null);

                if (dto.Quantity > availableStock)
                    return new StockResponse(false, $"Requested quantity ({dto.Quantity}) exceeds available stock ({availableStock}).", null);

                // Create a list to track changes
                List<EditHistory> editHistoryList = new List<EditHistory>();

                // ✅ Track Quantity Change
                if (item.Quantity != dto.Quantity)
                {
                    editHistoryList.Add(new EditHistory
                    {
                        RequisitionItemID = item.RequisitionItemID,
                        FieldName = "Quantity",
                        OldValue = item.Quantity.ToString(),
                        NewValue = dto.Quantity.ToString(),
                        EditedBy = dto.EditedBy
                    });
                    item.Quantity = dto.Quantity;
                }

                // ✅ Track Discount Change
                if (item.DiscountNumerator != dto.DiscountNumerator || item.DiscountDenominator != dto.DiscountDenominator)
                {
                    editHistoryList.Add(new EditHistory
                    {
                        RequisitionItemID = item.RequisitionItemID,
                        FieldName = "Discount",
                        OldValue = $"{item.DiscountNumerator}/{item.DiscountDenominator}",
                        NewValue = $"{dto.DiscountNumerator}/{dto.DiscountDenominator}",
                        EditedBy = dto.EditedBy
                    });
                    item.DiscountNumerator = dto.DiscountNumerator;
                    item.DiscountDenominator = dto.DiscountDenominator;
                }

                // ✅ Recalculate total amount
                decimal totalAmount = activePrice.SellingPrice * dto.Quantity;

                if (dto.DiscountNumerator.HasValue && dto.DiscountDenominator.HasValue && dto.DiscountDenominator != 0)
                {
                    decimal discount = (decimal)dto.DiscountNumerator.Value / dto.DiscountDenominator.Value;
                    totalAmount *= (1 - discount);
                }

                // ✅ Track Total Amount Change
                if (item.TotalAmount != totalAmount)
                {
                    editHistoryList.Add(new EditHistory
                    {
                        RequisitionItemID = item.RequisitionItemID,
                        FieldName = "TotalAmount",
                        OldValue = item.TotalAmount.ToString(),
                        NewValue = totalAmount.ToString(),
                        EditedBy = dto.EditedBy
                    });
                    item.TotalAmount = totalAmount;
                }

                // ✅ Update metadata
                item.EditedBy = dto.EditedBy;
                item.DateEdited = DateTime.Now;

                // ✅ Save Edit History
                if (editHistoryList.Any())
                {
                    await _dragonFlyContext.EditHistory.AddRangeAsync(editHistoryList);
                }

                // ✅ Save Requisition Item changes
                _dragonFlyContext.ApplyRequisitionItem.Update(item);
                await _dragonFlyContext.SaveChangesAsync();

                return new StockResponse(true, "Requisition item updated successfully.", item);
            }
            catch (Exception ex)
            {
                return new StockResponse(false, $"Error: {ex.Message}", null);
            }
        }


        public async Task<StockResponse> MarkPOLinesAsComplete(string PONumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var applieditem = await scopedcontext.PurchaseOrderss
                        .Where(u => u.PONumber == PONumber)
                        .FirstOrDefaultAsync();

                    if (applieditem == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    // Check if the Purchase Order is already under review
                    if (applieditem.DeliveryStatus == "Pending")
                    {
                        return new StockResponse(false, "Purchase Order is already under review", null);
                    }

                    // Fetch all items associated with the given PO number
                    var poItems = await scopedcontext.UploadPOFile
                        .Where(item => item.PONumber == PONumber)
                        .ToListAsync();

                    // Check if all items have delivery status complete
                    bool allItemsComplete = poItems.All(item => item.ProductStatus == "Complete");

                    if (allItemsComplete)
                    {
                        // Mark the purchase order lines as complete
                        foreach (var item in poItems)
                        {
                            item.ProductStatus = "Complete";
                            applieditem.DeliveryStatus = "Pending";// Update the line status here
                        }
                        scopedcontext.Update(applieditem);
                        
                        await scopedcontext.SaveChangesAsync();

                        return new StockResponse(true, "PO lines marked as complete...Wait for approval", null);
                    }
                    else
                    {
                        return new StockResponse(false, "Not all items have delivery status complete", null);
                    }
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetAllPOSWithStatusPending()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var allpos = await scopedcontext.PurchaseOrderss
                        .Where(x => x.CaptureStatus == "Complete" && x.DeliveryStatus == "Pending")
                        .ToListAsync();

                    //if (allpos.Count == 0)  // Check for empty list using Count instead of null check
                    //{
                    //    return new StockResponse(false, "No POs with the specified status combination", null);
                    //}

                    return new StockResponse(true, "Successfully queried", allpos);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
    
        public async Task<StockResponse> PODeliveryReview(ApprovalPODeliveryvm pOApprovalvm)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var loggedinuserobject = await _extraServices.LoggedInUser();

                    var userEmail = loggedinuserobject.FirstName + ' ' + loggedinuserobject.LastName;

                    if (loggedinuserobject == null)
                    {

                        return new StockResponse(false, "user not logged in. login again", null);

                    }

                    var applieditem = await scopedcontext.PurchaseOrderss.Where(u => u.PONumber == pOApprovalvm.PONumber).FirstOrDefaultAsync();
                    if (applieditem == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }

                    if (pOApprovalvm.selectedOption == "Approve")
                    {
                        pOApprovalvm.RejectedReason = "";
                    }
                    var poApproval = new ApprovalPODelivery
                    {
                        selectedOption = pOApprovalvm.selectedOption,
                        RejectedReason = pOApprovalvm.RejectedReason,
                        AprrovedDate = DateTime.Now,
                        PONumber = pOApprovalvm.PONumber,


                    };
                    applieditem.DateCreated = pOApprovalvm.AprrovedDate;

                    if (pOApprovalvm.selectedOption == "Reject")
                    {
                        applieditem.DeliveryStatus = "Incomplete";
                        poApproval.ApprovalStatus = "Incomplete";
                        scopedcontext.Update(applieditem);

                    }
                    else
                    {
                        applieditem.DeliveryStatus = "Complete";
                        poApproval.ApprovalStatus = "Complete";
                        scopedcontext.Update(applieditem);
                    }
                    await scopedcontext.AddAsync(poApproval);

                    await scopedcontext.SaveChangesAsync();


                    return new StockResponse(true, "Successfully updated", poApproval);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> GetItemByClient(string OrderNumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.SelectSerial.Where(u => u.OrderNumber == OrderNumber).ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetItemsByRequiestItemID(int requisitionItemID)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var item = await scopedContext.ApplyRequisitionItem
                        .FirstOrDefaultAsync(u => u.RequisitionItemID == requisitionItemID);

                    if (item == null)
                    {
                        return new StockResponse(false, "Requisition item not found", null);
                    }

                    var editHistories = await scopedContext.EditHistory
                        .Where(e => e.RequisitionItemID == requisitionItemID)
                        .OrderByDescending(e => e.DateEdited)
                        .ToListAsync();

                    var resultList = new List<object>();

                    resultList.Add(item); // Add the main requisition item
                    resultList.AddRange(editHistories); // Add all edit history records

                    return new StockResponse(true, "Queried successfully", resultList);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }


        public async Task<BaseResponse> GetItemByReferenceNumber(string referenceNumber)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var items = await scopedcontext.ApplyRequisitionItem
                        .Where(u => u.ReferenceNumber == referenceNumber)
                        .ToListAsync();

                    if (items == null || items.Count == 0)
                    {
                        return new BaseResponse("404", "Ticket reference not found", null);
                    }

                    var totalAmountByCurrency = items
                        .GroupBy(x => x.Currency)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Sum(x => x.Quantity * x.UnitPrice)
                        );

                    var response = new
                    {
                        TotalAmount = totalAmountByCurrency,
                        SparePartsUsed = items
                    };

                    return new BaseResponse("200", "Queried successfully", response);
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse("500", $"An error occurred: {ex.Message}", null);
            }
        }

        public async Task<StockResponse> GetSerialByItemName(string brandName,string itemName)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.AddProductDetails.Where(u => u.BrandName == brandName && u.ItemName==itemName && u.ItemStatus=="Okay" && u.SerialStatus=="Not Issued").ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetIsusedSerialNumberData(string brandName, string itemName)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();
                    var supplierexists = await scopedcontext.AddProductDetails.Where(u => u.BrandName == brandName && u.ItemName == itemName && u.SerialStatus == "Issued").ToListAsync();
                    if (supplierexists == null)
                    {
                        return new StockResponse(false, "not found", null);
                    }
                    return new StockResponse(true, "Queried successfully", supplierexists);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetNameToUse()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    // Query for distinct customers by a property (e.g., customerName)
                    var distinctCustomers = await scopedcontext.SelectSerial
                        .GroupBy(c => c.NameToUse) // Group by the property you want to be distinct
                        .Select(group => group.First()) // Select the first record in each group
                        .ToListAsync();

                    if (distinctCustomers == null)
                    {
                        return new StockResponse(false, "Customer doesn't exist", null);
                    }

                    return new StockResponse(true, "Successfully queried", distinctCustomers);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }

        public async Task<StockResponse> SearchForPO(string search_query)
 {

            try
          {
                using (var scope = _serviceScopeFactory.CreateScope())
                {

                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var allstock = await scopedcontext.PurchaseOrderss.Where
                        (u => EF.Functions.Like(u.PONumber, $"%{search_query}%") ||
                        EF.Functions.Like(u.Vendor, $"%{search_query}%") ||
                        EF.Functions.Like(u.DeliveryStatus, $"%{search_query}%") ||
                        EF.Functions.Like(u.CaptureStatus, $"%{search_query}%")
                        ).ToListAsync();

                    if (allstock == null)
                        return new StockResponse(false, "", null);

                    return new StockResponse(true, "Successfully queried", allstock);


                }
            }
            catch (Exception ex)
            {

                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> GetAllItemsReorder()
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    var latestStockEntries = await scopedcontext.UploadPOFile
                        .GroupBy(x => x.ItemName)
                        .Select(group => group.OrderByDescending(x => x.DateAdded).First())
                        .ToListAsync();

                    var loggedinuserobject = await _extraServices.LoggedInUser();

                    if (loggedinuserobject == null)
                    {
                        return new StockResponse(false, "User not logged in. Login again.", null);
                    }

                    var userName = loggedinuserobject.FirstName + ' ' + loggedinuserobject.LastName;

                    if (latestStockEntries == null || latestStockEntries.Count == 0)
                    {
                        return new StockResponse(false, "Stock doesn't exist", null);
                    }

                    Dictionary<string, AllStockListItems> stockDictionary = new Dictionary<string, AllStockListItems>();

                    foreach (var stock in latestStockEntries)
                    {
                        string normalizedStatus = stock.Status.Trim(); // Normalize and trim status

                        if (normalizedStatus == "Good")
                        {
                            // Skip items with status "Good"
                            continue;
                        }
                        else
                        {
                            if (!stockDictionary.TryGetValue(stock.ItemName, out var existingStock))
                            {
                                var newStockItem = new AllStockListItems
                                {
                                    ItemName = stock.ItemName,
                                    OpeningStock = stock.OpeningStock,
                                    DateAdded = DateTime.Now,
                                    Quantity = stock.TotalClosed,
                                    AvailableStock = stock.TotalClosed - stock.StockOut,
                                    StockOut = stock.StockOut,
                                    Status = normalizedStatus, // Store normalized status
                                    BrandName = stock.BrandName,
                                    StockIn = stock.TotalStockIn,
                                    UpdatedBy = userName,
                                    CategoryName= stock.CategoryName,
                                };

                                stockDictionary.Add(stock.ItemName, newStockItem);
                            }
                            else
                            {
                                // Update the existing item if necessary (e.g., quantity, stockout, etc.)
                                existingStock.Quantity += stock.TotalClosed;
                                existingStock.StockOut += stock.StockOut;
                                // Update other properties as needed
                            }
                        }
                    }

                    // Convert the dictionary values to a list
                    List<AllStockListItems> stockList = stockDictionary.Values.ToList();

                    return new StockResponse(true, "Successfully queried", stockList);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }
        public async Task<StockResponse> SelectSerialNumber(SelectedSerialvm addBrandvm)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedcontext = scope.ServiceProvider.GetRequiredService<DragonFlyContext>();

                    // Fetch the applied item using the provided IssueID
                    var applieditem = await scopedcontext.SelectSerial
                        .Where(u => u.Id == addBrandvm.IssueID)
                        .FirstOrDefaultAsync();

                    // Ensure an item with the provided IssueID exists
                    if (applieditem == null)
                    {
                        return new StockResponse(false, $"Issue ID {addBrandvm.IssueID} not found", null);
                    }

                    var loggedinuserobject = await _extraServices.LoggedInUser();
                    var userEmail = loggedinuserobject.FirstName + ' ' + loggedinuserobject.LastName;

                    // Use the Contains method to filter by multiple serial numbers
                    var serialNumbers = addBrandvm.SerialNumbers;

                    if (applieditem.CategoryName == "Product")
                    {
                        // Fetch the count of serial numbers already saved in the database
                        int savedSerialCount = await scopedcontext.selectedSetialNumber
                            .Where(u => u.IssueID == applieditem.Id)
                            .CountAsync();

                        // Calculate the remaining quantity that can be added
                        int remainingQuantity = applieditem.QuantityDispatched - savedSerialCount;

                        if (serialNumbers.Count > remainingQuantity)
                        {
                            return new StockResponse(false, $"Cannot add more than {remainingQuantity} serial numbers to match the QuantityDispatched", null);
                        }

                        foreach (var serialNumber in serialNumbers)
                        {
                            // Fetch the matching IMEI record for the serial number
                            var matchingImei = await scopedcontext.AddProductDetails
                                .FirstOrDefaultAsync(u => u.SerialNumber == serialNumber);

                            if (matchingImei == null)
                            {
                                return new StockResponse(false, $"Serial number {serialNumber} doesn't exist", null);
                            }

                            // Create a separate SelectSerial object for each serial number
                            var serialItem = new selectedSetialNumber
                            {
                                SerialStatus = "Issued",
                                SerialNumber = matchingImei.SerialNumber,
                                IMEII1 = matchingImei.IMEI1,
                                IMEI2 = matchingImei.IMEI2,
                                DateUpdated = DateTime.Now,
                                IssueID = applieditem.Id,
                                IssuedNo=applieditem.IssueID,
                            };
                            matchingImei.SerialStatus = serialItem.SerialStatus;


                            await scopedcontext.AddAsync(serialItem);
                        }
                    }

                    // Save changes to the database
                    await scopedcontext.SaveChangesAsync();

                    return new StockResponse(true, "Serial numbers issued successfully", null);
                }
            }
            catch (Exception ex)
            {
                return new StockResponse(false, ex.Message, null);
            }
        }







    }
}
    


    



    


