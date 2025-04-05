using CingHuTang.Config;
using CingHuTang.Models;
using CingHuTang.Models.DTOs;
using CingHuTang.Reposiory;
using CingHuTang.Repository;
using Humanizer;
using MesWeb.Models.CommonConfig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace CingHuTang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MaterialController : Controller
    {
        MaterialRepository _materialRepo = new MaterialRepository();
        AccountRepository _accRepo = new AccountRepository();
        UnitRepository _unitRepo  = new UnitRepository();
        SupplierRepository _supplier = new SupplierRepository();
        public IActionResult Index()
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
            if (acc == null || acc.Role == 3 || acc.Role == 1)
            {
                return Redirect("/Home/Index");
            }

            ViewBag.ListUnit = new SelectList(_unitRepo.GetAll().Where(x => x.IsDelete != true).ToList(), "Id", "UnitName");
            ViewBag.ListSupplier = new SelectList(_supplier.GetAll().Where(x => x.IsDelete != true).ToList(), "Id", "SupplierName");
            return View();
        }

        public async Task<JsonResult> GetAll(string request ="", int pageNumber = 1)
        {
            DataSet ds = LoadDataFromSP.GetDataSetSP("spGetAllMaterial", new string[] { "@PageNumber", "@Request"}
                                                                       , new object[] { pageNumber, request });
            var data = TextUtils.ConvertDataTable<MaterialDTO>(ds.Tables[0]);
            var totalCount = TextUtils.ConvertDataTable<PaginationDto>(ds.Tables[1]);
            var dataTotal = TextUtils.ConvertDataTable<MaterialDTO>(ds.Tables[2]);

            return Json(new { data, totalCount, dataTotal }, new System.Text.Json.JsonSerializerOptions());
        }

        public async Task<JsonResult> GetByID(int Id)
        {
            return Json(_materialRepo.GetByID(Id));
        }


        
        public async Task<JsonResult> CreateOrUpdate([FromBody] Material data)
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
            if (acc == null)
            {
                return Json(new { status = 0, statusText = "Bạn đã hết phiên đăng nhập! Vui lòng đăng nhập lại!" });
            }


            if (string.IsNullOrWhiteSpace(data.MaterialCode))
            {
                return Json(new { status = 0, statusText = "Vui lòng nhập Mã Nguyên vật liệu!" });
            }
            else if (string.IsNullOrWhiteSpace(data.MaterialName))
            {
                return Json(new { status = 0, statusText = "Vui lòng nhập Tên Nguyên vật liệu!" });
            }


            bool isCheck = _materialRepo.GetAll().Any(x => x.Id != data.Id && x.MaterialCode == data.MaterialCode && x.IsDelete != true);
            if (isCheck) return Json(new { status = 0, statusText = "Mã Nguyên liệu đã được sử dụng! vui lòng kiểm tra lại!", result = 0 });

            Material model = _materialRepo.GetByID(data.Id) ?? new Material();

            model.UnitId = data.UnitId;
            model.MaterialCode = TextUtils.ToString(data.MaterialCode);
            model.MaterialName = TextUtils.ToString(data.MaterialName);
            model.MinQuantity = data.MinQuantity;
            model.Decription = data.Decription;
            model.IsDelete = false;


            if (model.Id > 0)
            {
                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = acc.FullName;
                _materialRepo.Update(model);
            }
            else
            {
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = acc.FullName;
                await _materialRepo.CreateAsync(model);
            }
            return Json(new { status = 1, statusText = "", result = model });
        }
        public async Task<JsonResult> Delete(int Id)
        {
            Material model = _materialRepo.GetByID(Id) ?? new Material();
            if (model.Id <= 0) return Json(new { status = 0, message = "Không tìm thấy Phiếu nhập!" });

            model.IsDelete = true;
            _materialRepo.Update(model);
            return Json(new { status = 1, message = "" });
        }

        public JsonResult GetAllForView()
        {
            return Json(_materialRepo.GetAll(),new System.Text.Json.JsonSerializerOptions());
        }

        public async Task<FileResult> ExportExcel()
        {
            DataSet ds = LoadDataFromSP.GetDataSetSP("spGetAllMaterial", new string[] { "@PageNumber", "@Request" }
                                                                      , new object[] { 1, "" });
            var result = TextUtils.ConvertDataTable<MaterialDTO>(ds.Tables[2]);
            string[] colName = { "Mã nguyên liệu", "Tên nguyên liệu", "Số lượng", "Đơn vị", "Đơn giá", "Nhà cung cấp", "Ghi chú" };
            string[] colValue = { "MaterialCode", "MaterialName", "ToltalQuantity", "UnitName", "UnitPrice", "SupplierName", "Decription" };
            var (contentFile, contentType, fileName) = Excel.GenerateExcel("Material.xlsx", result.ToList(), colName, colValue);
            return File(contentFile,
                        contentType,
                        fileName);
        }
    }
}
