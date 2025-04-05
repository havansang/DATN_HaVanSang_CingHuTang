using CingHuTang.Models.DTOs;
using CingHuTang.Models;
using Microsoft.AspNetCore.Mvc;
using CingHuTang.Repository;
using System.Data;
using CingHuTang.Config;
using CingHuTang.Reposiory;

namespace CingHuTang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ToppingController : Controller
    {
        AccountRepository _accRepo = new AccountRepository();
        ToppingRepository _toppingRepo = new ToppingRepository();
        ProductToppingRepository _productToping = new ProductToppingRepository();
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
            DataSet ds = LoadDataFromSP.GetDataSetSP("spGetAllTopping", new string[] { "@PageNumber", "@Request" }, new object[] { pageNumber, request });

            var data = TextUtils.ConvertDataTable<Topping>(ds.Tables[0]);
            var totalCount = TextUtils.ConvertDataTable<PaginationDto>(ds.Tables[1]);

            return Json(new { data, totalCount }, new System.Text.Json.JsonSerializerOptions());
        }
        public JsonResult GetById(int Id)
        {
            return Json(_toppingRepo.GetByID(Id));
        }

        public async Task<JsonResult> CreateOrUpdate([FromBody] Topping data)
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
            if (acc == null)
            {
                return Json(new { status = 0, statusText = "Bạn đã hết phiên đăng nhập! Vui lòng đăng nhập lại!" });
            }


            if (string.IsNullOrWhiteSpace(data.ToppingName))
            {
                return Json(new { status = 0, statusText = "Vui lòng nhập Tên topping!" });
            }
            else if (string.IsNullOrWhiteSpace(data.ToppingCode))
            {
                return Json(new { status = 0, statusText = "Vui lòng nhập Mã topping!" });
            }
            else if (TextUtils.ToDecimal(data.ToppingPrice) <= 0)
            {
                return Json(new { status = 0, statusText = "Vui lòng nhập giá cho Topping!" });
            }

            bool isCheck = _toppingRepo.GetAll().Any(x => x.Id != data.Id && x.ToppingCode == data.ToppingCode && x.IsDelete != true);
            if (isCheck) return Json(new { status = 0, statusText = "Mã loại sản phẩm đã bị trùng! Hãy kiểm tra lại!", result = 0 });

            Topping model = _toppingRepo.GetByID(data.Id) ?? new Topping();

            model.ToppingCode = data.ToppingCode.Trim();
            model.ToppingName = data.ToppingName.Trim();
            model.ToppingPrice = TextUtils.ToDecimal(data.ToppingPrice);
            model.IsDelete = false;


            if (model.Id > 0)
            {
                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = acc.FullName;
                _toppingRepo.Update(model);
            }
            else
            {
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = acc.FullName;
                await _toppingRepo.CreateAsync(model);
            }


            return Json(new { status = 1, statusText = "", result = model });
        }

        public async Task<JsonResult> Delete(int Id)
        {

            bool isCheck = _productToping.GetAll().Any(x => x.ToppingId == Id);
            if (isCheck) return Json(new { status = 0, message = "Topping đang được sử dụng! Không thể xóa!" });

            Topping model = _toppingRepo.GetByID(Id) ?? new Topping();
            if (model.Id <= 0) return Json(new { status = 0, message = "Không tìm thấy topping!" });

            model.IsDelete = true;
            _toppingRepo.Update(model);
            return Json(new { status = 1, message = "" });
        }
    }
}
