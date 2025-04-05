using CingHuTang.Config;
using CingHuTang.Models.DTOs;
using CingHuTang.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using CingHuTang.Reposiory;
using CingHuTang.Repository;
using MesWeb.Models.CommonConfig;

namespace CingHuTang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GoodsIssueController : Controller
    {
        AccountRepository _accRepo = new AccountRepository();
        MaterialRepository _materialRepo = new MaterialRepository();
        GoodsIssueRepository _issueRepo= new GoodsIssueRepository();
        GoodsIssueDetailsRepository _issueDetailRepo= new GoodsIssueDetailsRepository();
        GoodsIssueFileRepository _fileRepo = new GoodsIssueFileRepository();
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
            return View();
        }

        [HttpPost]
        public JsonResult GetAll([FromBody] GoodReceiptRequestDTO dto)
        {
            dto.DateStart = new DateTime(dto.DateStart.Year, dto.DateStart.Month, dto.DateStart.Day, 0, 0, 0);
            dto.DateEnd = new DateTime(dto.DateEnd.Year, dto.DateEnd.Month, dto.DateEnd.Day, 23, 59, 59);


            DataSet ds = LoadDataFromSP.GetDataSetSP("spGetAllGoodsIssue", new string[] { "@PageNumber", "@Request", "@DateStart", "@DateEnd", "@AccountID" }
                                                                           , new object[] { dto.PageNumber, TextUtils.ToString(dto.Request), dto.DateStart, dto.DateEnd, dto.AccountID });

            var data = TextUtils.ConvertDataTable<GoodsIssueDTO>(ds.Tables[0]);
            var totalCount = TextUtils.ConvertDataTable<PaginationDto>(ds.Tables[1]);

            return Json(new { data, totalCount }, new System.Text.Json.JsonSerializerOptions());
        }
        
        public JsonResult GetById(int Id)
        {
            DataSet ds = LoadDataFromSP.GetDataSetSP("spGetGoodsIssuesByID", new string[] { "@GoodsIssueID" }
                                                                          , new object[] { Id });

            var data = TextUtils.ConvertDataTable<GoodsIssueDTO>(ds.Tables[0]);
            var details = TextUtils.ConvertDataTable<GoodsIsssueDetailsDTO>(ds.Tables[1]);
            var files = TextUtils.ConvertDataTable<GoodsIssueFile>(ds.Tables[2]);
            foreach (var item in files)
            {
                item.FileUrl = $"{Config.Config.GoodsIssuetUrl()}{item.FileUrl}/{item.FileName}";
            }

            return Json(new { data, details, files }, new System.Text.Json.JsonSerializerOptions());
        }

        public async Task<JsonResult> CreateOrUpdate([FromBody] GoodsIssueDTO data)
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
            if (acc == null)
            {
                return Json(new { status = 0, statusText = "Bạn đã hết phiên đăng nhập! Vui lòng đăng nhập lại!" });
            }

            if (!data.IssueDate.HasValue)
            {
                return Json(new { status = 0, statusText = "Vui lòng nhập Ngày xuất kho!" });
            }

            for (int i = 0; i < data.LstDetails.Count; i++)
            {
                List<GoodsIssueDetail> lstCheck = data.LstDetails.Where(p => p.MaterialId == data.LstDetails[i].MaterialId).ToList();
                if (lstCheck.Count > 1) return Json(new { status = 0, statusText = "Nguyên vật liệu chỉ được chọn 1 lần! Vui lòng kiểm tra lại!" });
            }


            GoodsIssue model = _issueRepo.GetByID(data.Id) ?? new GoodsIssue();

            model.AccountId = acc.Id;
            model.IssueDate = data.IssueDate;
            model.Decription = data.Decription;
            model.IsDelete = false;


            if (model.Id > 0)
            {
                model.GoodIssueCode = data.GoodIssueCode;
                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = acc.FullName;
                _issueRepo.Update(model);
            }
            else
            {
                model.GoodIssueCode = LoadGoodsIssueCode();
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = acc.FullName;
                await _issueRepo.CreateAsync(model);
            }



            SQLHelper<GoodsIssueDetail>.SqlToModel($"DELETE FROM dbo.GoodsIssueDetails WHERE GoodIssueID = {model.Id}");
            List<GoodsIssueDetail> lstDetails = new List<GoodsIssueDetail>();
            foreach (GoodsIssueDetail item in data.LstDetails)
            {
                if (item.MaterialId <= 0) continue;
                GoodsIssueDetail newDetails = new GoodsIssueDetail()
                {
                    GoodIssueId = model.Id,
                    MaterialId = item.MaterialId,
                    Quantity = item.Quantity,
                    Decription = item.Decription,
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
                _issueDetailRepo.CreateRange(lstDetails);
            }


            return Json(new { status = 1, statusText = "", result = model });
        }

        public async Task<JsonResult> Delete(int Id)
        {
            GoodsIssue model = _issueRepo.GetByID(Id) ?? new GoodsIssue();
            if (model.Id <= 0) return Json(new { status = 0, message = "Không tìm thấy Phiếu nhập!" });

            model.IsDelete = true;
            _issueRepo.Update(model);
            return Json(new { status = 1, message = "" });
        }


        [HttpPost]
        public async Task<IActionResult> UploadFile(GoodsIssue goodsIssue)
        {
            try
            {
                Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
                if (acc == null)
                {
                    return Json(new { status = 0, statusText = "Bạn đã hết phiên đăng nhập! Vui lòng đăng nhập lại!" });
                }
                goodsIssue = _issueRepo.GetByID(goodsIssue.Id) ?? new GoodsIssue();
                if (goodsIssue.Id <= 0) return BadRequest(new { status = 1, message = "Không tìm thấy phiếu xuất" }); ;
                var files = Request.Form.Files;

                DateTime currentDate = DateTime.Now;
                List<GoodsIssueFile> listFiles = new List<GoodsIssueFile>();
                foreach (var file in files)
                {
                    if (file.Length <= 0) continue;
                    listFiles.Add(new GoodsIssueFile()
                    {
                        FileUrl = $"{goodsIssue.IssueDate.Value.Year}/{goodsIssue.IssueDate.Value.Month}/{goodsIssue.GoodIssueCode}_{goodsIssue.IssueDate.Value.ToString("ddMMyyyy")}",
                        FileName = file.FileName,
                        GoodsIssueId = goodsIssue.Id,
                        CreatedDate = DateTime.Now,
                        IsDelete = false,
                        CreatedBy = acc.FullName
                    });
                    string pathUpload = Path.Combine(Directory.GetCurrentDirectory(),
                                                    $"wwwroot\\GoodsIssues\\{goodsIssue.IssueDate.Value.Year}\\{goodsIssue.IssueDate.Value.Month}\\{goodsIssue.GoodIssueCode}_{goodsIssue.IssueDate.Value.ToString("ddMMyyyy")}");
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
            string code = LoadGoodsIssueCode();
            return Json(new { code });
        }
        public string LoadGoodsIssueCode()
        {
            DateTime curentDate = DateTime.Now;
            List<GoodsIssue> lstdata = _issueRepo.GetAll().Where(p => p.IsDelete == false && p.IssueDate.Value.Year == curentDate.Year).ToList();
            string totalcount = (lstdata.Count + 1).ToString();
            while (totalcount.Length < 6)
            {
                totalcount = "0" + totalcount;
            }
            string goodsRecriptCode = $"PXH{curentDate.ToString("yyyyMMdd")}T{totalcount}";
            return goodsRecriptCode;
        }

    }
}
