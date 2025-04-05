using CingHuTang.Config;
using CingHuTang.Models;
using CingHuTang.Models.DTOs;
using CingHuTang.Reposiory;
using CingHuTang.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CingHuTang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UnitController : Controller
    {
        UnitRepository _unitRepo = new UnitRepository();
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
            DataSet ds = LoadDataFromSP.GetDataSetSP("spGetAllUnit", new string[] { "@PageNumber", "@Request" }, new object[] { pageNumber, request });

            var data = TextUtils.ConvertDataTable<Unit>(ds.Tables[0]);
            var totalCount = TextUtils.ConvertDataTable<PaginationDto>(ds.Tables[1]);

            return Json(new { data, totalCount }, new System.Text.Json.JsonSerializerOptions());
        }
        public JsonResult GetById(int Id)
        {
            return Json(_unitRepo.GetByID(Id));
        }

        public async Task<JsonResult> CreateOrUpdate([FromBody] Unit data)
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
            if (acc == null)
            {
                return Json(new { status = 0, statusText = "Bạn đã hết phiên đăng nhập! Vui lòng đăng nhập lại!" });
            }


            if (string.IsNullOrWhiteSpace(data.UnitCode))
            {
                return Json(new { status = 0, statusText = "Vui lòng nhập Mã topping!" });
            }
            else if (string.IsNullOrWhiteSpace(data.UnitName))
            {
                return Json(new { status = 0, statusText = "Vui lòng nhập Tên topping!" });
            }
           

            bool isCheck = _unitRepo.GetAll().Any(x => x.Id != data.Id && x.UnitCode == data.UnitCode && x.IsDelete != true);
            if (isCheck) return Json(new { status = 0, statusText = "Mã đơn vị đã được sử dụng! Hãy kiểm tra lại!", result = 0 });

            Unit model = _unitRepo.GetByID(data.Id) ?? new Unit();

            model.UnitCode = TextUtils.ToString(data.UnitCode);
            model.UnitName = TextUtils.ToString(data.UnitName);
            model.Note = TextUtils.ToString(data.Note);
            model.IsDelete = false;


            if (model.Id > 0)
            {
                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = acc.FullName;
                _unitRepo.Update(model);
            }
            else
            {
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = acc.FullName;
                await _unitRepo.CreateAsync(model);
            }
            return Json(new { status = 1, statusText = "", result = model });
        }

        public async Task<JsonResult> Delete(int Id)
        {

            bool isCheck = _materialRepo.GetAll().Any(x => x.UnitId == Id);
            if (isCheck) return Json(new { status = 0, message = "Đơn vị đang được sử dụng! Không thể xóa!" });

            Unit model = _unitRepo.GetByID(Id) ?? new Unit();
            if (model.Id <= 0) return Json(new { status = 0, message = "Không tìm thấy Đơn vị!" });

            model.IsDelete = true;
            _unitRepo.Update(model);
            return Json(new { status = 1, message = "" });
        }

        public JsonResult GetAllForView()
        {
            return Json(_unitRepo.GetAll(), new System.Text.Json.JsonSerializerOptions());
        }
    }
}
