using CingHuTang.Config;
using CingHuTang.Models.DTOs;
using CingHuTang.Models;
using CingHuTang.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using CingHuTang.Reposiory;
using System.Xml;
using Humanizer;
using Microsoft.AspNetCore.Mvc.Rendering;
using MesWeb.Models.CommonConfig;

namespace CingHuTang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GoodsReceiptController : Controller
    {
        AccountRepository _accRepo = new AccountRepository();
        MaterialRepository _materialRepo = new MaterialRepository();
        GoodsReceiptRepository _receipttRepo = new GoodsReceiptRepository();
        GoodsReceiptDetailRepository _receiptDetail = new GoodsReceiptDetailRepository();
        SupplierRepository _supplierRepo = new SupplierRepository();
        GoodsReceiptFileRepository _fileRepo = new GoodsReceiptFileRepository();

        public IActionResult Index()
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
            if (acc == null || acc.Role == 3 || acc.Role == 1)
            {
                return Redirect("/Home/Index");
            }

            DateTime currentDate = DateTime.Now;
            DateTime dateStart = new DateTime(currentDate.Year, currentDate.Month, 1);
            ViewBag.DateStart = dateStart.ToString("yyyy-MM-dd");
            ViewBag.DateEnd = dateStart.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            ViewBag.CurrentDate = currentDate.ToString("yyyy-MM-dd");


            List<int> withoutRoleAccount = new List<int>() { 1, 2 };
            List<Account> lstAccounts = _accRepo.GetAll().ToList();
            ViewBag.LstAccount = new SelectList(lstAccounts.Where(x => x.IsDelete != true && !withoutRoleAccount.Contains(x.Role)).ToList(), "Id", "FullName");
            ViewBag.Account = acc;

            List<Supplier> lstSupplier = _supplierRepo.GetAll().ToList();
            ViewBag.ListSupplier = new SelectList(lstSupplier.Where(x => x.IsDelete != true).ToList(), "Id", "SupplierName"); ;


            return View();
        }


        [HttpPost]
        public JsonResult GetAll([FromBody] GoodReceiptRequestDTO dto)
        {
            dto.DateStart = new DateTime(dto.DateStart.Year, dto.DateStart.Month, dto.DateStart.Day, 0, 0, 0);
            dto.DateEnd = new DateTime(dto.DateEnd.Year, dto.DateEnd.Month, dto.DateEnd.Day, 23, 59, 59);


            DataSet ds = LoadDataFromSP.GetDataSetSP("spGetAllGoodsReceipt", new string[] { "@PageNumber", "@Request", "@DateStart", "@DateEnd", "@AccountID" }
                                                                           , new object[] { dto.PageNumber, TextUtils.ToString(dto.Request), dto.DateStart, dto.DateEnd, dto.AccountID });

            var data = TextUtils.ConvertDataTable<GoodReceiptResponeDTO>(ds.Tables[0]);
            var totalCount = TextUtils.ConvertDataTable<PaginationDto>(ds.Tables[1]);

            return Json(new { data, totalCount }, new System.Text.Json.JsonSerializerOptions());
        }
        public JsonResult GetById(int Id)
        {
            DataSet ds = LoadDataFromSP.GetDataSetSP("spGetGoodsReceiptByID", new string[] { "@GoodsReceiptID" }
                                                                          , new object[] { Id });

            var data = TextUtils.ConvertDataTable<GoodReceiptResponeDTO>(ds.Tables[0]);
            var details = TextUtils.ConvertDataTable<GoodsReceiptDetailsDTO>(ds.Tables[1]);
            var files = TextUtils.ConvertDataTable<GoodsReceiptFile>(ds.Tables[2]);
            foreach (var item in files)
            {
                item.FileUrl = $"{Config.Config.GoodsReceiptUrl()}{item.FileUrl}/{item.FileName}";
            }

            return Json(new { data, details, files }, new System.Text.Json.JsonSerializerOptions());
        }

        public async Task<JsonResult> CreateOrUpdate([FromBody] GoodsReceiptDTO data)
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
            if (acc == null)
            {
                return Json(new { status = 0, statusText = "Bạn đã hết phiên đăng nhập! Vui lòng đăng nhập lại!" });
            }

            for (int i = 0; i < data.LstDetails.Count; i++)
            {
                List<GoodsReceiptDetail> lstCheck = data.LstDetails.Where(p=> p.MaterialId == data.LstDetails[i].MaterialId).ToList();
                if(lstCheck.Count > 1) return Json(new { status = 0, statusText = "Nguyên vật liệu chỉ được chọn 1 lần! Vui lòng kiểm tra lại!" });
            }


            GoodsReceipt model = _receipttRepo.GetByID(data.Id) ?? new GoodsReceipt();

            model.AccoutId = acc.Id;
            model.SupplierId = data.SupplierId;
            model.ReceiptedDate = data.ReceiptedDate;
            model.Decription = data.Decription;
            model.IsDelete = false;


            if (model.Id > 0)
            {
                model.GoodsReceiptCode = data.GoodsReceiptCode;
                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = acc.FullName;
                _receipttRepo.Update(model);
            }
            else
            {
                model.GoodsReceiptCode = LoadGoodsReceiptCode();
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = acc.FullName;
                await _receipttRepo.CreateAsync(model);
            }



            SQLHelper<GoodsReceiptDetail>.SqlToModel($"DELETE FROM dbo.GoodsReceiptDetails WHERE GoodsReceiptID = {model.Id}");
            List<GoodsReceiptDetail> lstDetails = new List<GoodsReceiptDetail>();
            foreach (GoodsReceiptDetail item in data.LstDetails)
            {
                if (item.MaterialId <= 0) continue;
                GoodsReceiptDetail newDetails = new GoodsReceiptDetail()
                {
                    GoodsReceiptId = model.Id,
                    MaterialId = item.MaterialId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    CreatedDate = DateTime.Now,
                    CreatedBy = acc.FullName,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = acc.FullName,
                    IsDelete = false,
                };
                lstDetails.Add(newDetails);
            }
            if (lstDetails.Count > 0) 
            {
                _receiptDetail.CreateRange(lstDetails);
            }


            return Json(new { status = 1, statusText = "", result = model });
        }

        public async Task<JsonResult> Delete(int Id)
        {
            GoodsReceipt model = _receipttRepo.GetByID(Id) ?? new GoodsReceipt();
            if (model.Id <= 0) return Json(new { status = 0, message = "Không tìm thấy Phiếu nhập!" });

            model.IsDelete = true;
            _receipttRepo.Update(model);
            return Json(new { status = 1, message = "" });
        }


        [HttpPost]
        public async Task<IActionResult> UploadFile(GoodsReceipt goodsReceipt)
        {
            try
            {
                Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
                if (acc == null)
                {
                    return Json(new { status = 0, statusText = "Bạn đã hết phiên đăng nhập! Vui lòng đăng nhập lại!" });
                }
                goodsReceipt = _receipttRepo.GetByID(goodsReceipt.Id) ?? new GoodsReceipt();
                var files = Request.Form.Files;

                DateTime currentDate = DateTime.Now;
                List<GoodsReceiptFile> listFiles = new List<GoodsReceiptFile>();
                foreach (var file in files)
                {
                    if (file.Length <= 0) continue;
                    listFiles.Add(new GoodsReceiptFile()
                    {
                        FileUrl = $"{goodsReceipt.ReceiptedDate.Year}/{goodsReceipt.ReceiptedDate.Month}/{goodsReceipt.GoodsReceiptCode}_{goodsReceipt.ReceiptedDate.ToString("ddMMyyyy")}",
                        FileName = file.FileName,
                        GoodsReceiptId = goodsReceipt.Id,
                        CreatedDate = DateTime.Now,
                        IsDelete = false,
                        CreatedBy = acc.FullName
                    });
                    string pathUpload = Path.Combine(Directory.GetCurrentDirectory(), 
                                                    $"wwwroot\\GoodsReceipt\\{goodsReceipt.ReceiptedDate.Year}\\{goodsReceipt.ReceiptedDate.Month}\\{goodsReceipt.GoodsReceiptCode}_{goodsReceipt.ReceiptedDate.ToString("ddMMyyyy")}");
                    if (!Directory.Exists(pathUpload))
                    {
                        Directory.CreateDirectory(pathUpload);
                    }
                    string filePath = pathUpload + $"\\{file.FileName}";
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    using (FileStream stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                _fileRepo.CreateRange(listFiles);
                return Ok(new { status = 1, message = "Upload file thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 1, message = ex.Message });
            }
        }

        public JsonResult LoadCode()
        {
            string code = LoadGoodsReceiptCode();
            return Json(new { code });
        }
        public string LoadGoodsReceiptCode()
        {
            DateTime curentDate = DateTime.Now;
            List<GoodsReceipt> lstdata = _receipttRepo.GetAll().Where(p => p.IsDelete == false && p.ReceiptedDate.Year == curentDate.Year).ToList();
            string totalcount = (lstdata.Count + 1).ToString();
            while(totalcount.Length < 6)
            {
                totalcount = "0" + totalcount;
            }
            string goodsRecriptCode = $"PNH{curentDate.ToString("yyyyMMdd")}T{totalcount}";
            return goodsRecriptCode;
        }

        public async Task<FileResult> ExportExcel(DateTime dateStart, DateTime dateEnd)
        {
            dateStart = new DateTime(dateStart.Year, dateStart.Month, dateStart.Day, 0, 0, 0);
            dateEnd = new DateTime(dateEnd.Year, dateEnd.Month, dateEnd.Day, 23, 59, 59);
            DataSet ds = LoadDataFromSP.GetDataSetSP("spGetAllGoodsReceipt", new string[] { "@PageNumber", "@Request", "@DateStart", "@DateEnd", "@AccountID" }
                                                                          , new object[] { "1", "", dateStart, dateEnd, 0 });
            var result = TextUtils.ConvertDataTable<GoodReceiptResponeDTO>(ds.Tables[2]);
            string[] colName = { "Mã phiếu nhập", "Ngày nhập", "Tổng tiền", "Nhà cung cấp", "Người nhập", "Ghi chú" };
            string[] colValue = { "GoodsReceiptCode", "ReceiptedDate", "TotalMoney", "SupplierName", "FullName", "Decription" };
            var (contentFile, contentType, fileName) = Excel.GenerateExcel("GoodsReceipt.xlsx", result.ToList(), colName, colValue);
            return File(contentFile,
                        contentType,
                        fileName);
        }
    }
}
