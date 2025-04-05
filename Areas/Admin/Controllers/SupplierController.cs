using CingHuTang.Config;
using CingHuTang.Models.DTOs;
using CingHuTang.Models;
using CingHuTang.Reposiory;
using CingHuTang.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CingHuTang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SupplierController : Controller
    {
        SupplierRepository _supplierRepo = new SupplierRepository();
        MaterialRepository _materialRepo = new MaterialRepository();
        AccountRepository _accRepo = new AccountRepository();
        public IActionResult Index()
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
            if (acc == null || acc.Role == 3 || acc.Role == 1)
            {
                return Redirect("/Home/Index");
            }
            return View();
        }

        public JsonResult GetAll(string request = "", int pageNumber = 1)
        {
            DataSet ds = LoadDataFromSP.GetDataSetSP("spGetAllSupplier", new string[] { "@PageNumber", "@Request" }, new object[] { pageNumber, request });

            var data = TextUtils.ConvertDataTable<Supplier>(ds.Tables[0]);
            var totalCount = TextUtils.ConvertDataTable<PaginationDto>(ds.Tables[1]);

            return Json(new { data, totalCount }, new System.Text.Json.JsonSerializerOptions());
        }
        public JsonResult GetById(int Id)
        {
            return Json(_supplierRepo.GetByID(Id));
        }

        public async Task<JsonResult> CreateOrUpdate([FromBody] Supplier data)
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
            if (acc == null)
            {
                return Json(new { status = 0, statusText = "Bạn đã hết phiên đăng nhập! Vui lòng đăng nhập lại!" });
            }


            if (string.IsNullOrWhiteSpace(data.SupplierCode))
            {
                return Json(new { status = 0, statusText = "Vui lòng nhập Mã nhà cung cấp!" });
            }
            else if (string.IsNullOrWhiteSpace(data.SupplierName))
            {
                return Json(new { status = 0, statusText = "Vui lòng nhập Tên nhà cung cấp!" });
            }


            bool isCheck = _supplierRepo.GetAll().Any(x => x.Id != data.Id && x.SupplierCode == data.SupplierCode && x.IsDelete != true);
            if (isCheck) return Json(new { status = 0, statusText = "Mã nhà cung cấp đã được sử dụng! Hãy kiểm tra lại!", result = 0 });

            Supplier model = _supplierRepo.GetByID(data.Id) ?? new Supplier();

            model.SupplierCode = TextUtils.ToString(data.SupplierCode);
            model.SupplierName = TextUtils.ToString(data.SupplierName);
            model.PhoneNumber = TextUtils.ToString(data.PhoneNumber);
            model.Decription = TextUtils.ToString(data.Decription);
            model.IsDelete = false;


            if (model.Id > 0)
            {
                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = acc.FullName;
                _supplierRepo.Update(model);
            }
            else
            {
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = acc.FullName;
                await _supplierRepo.CreateAsync(model);
            }
            return Json(new { status = 1, statusText = "", result = model });
        }

        public async Task<JsonResult> Delete(int Id)
        {
            Supplier model = _supplierRepo.GetByID(Id) ?? new Supplier();
            if (model.Id <= 0) return Json(new { status = 0, message = "Không tìm thấy Nhà cung cấp!" });
            model.IsDelete = true;
            _supplierRepo.Update(model);
            return Json(new { status = 1, message = "" });
        }
    }
}
