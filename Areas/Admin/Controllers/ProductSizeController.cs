using CingHuTang.Models;
using CingHuTang.Models.DTOs;
using CingHuTang.Config;
using Microsoft.AspNetCore.Mvc;
using CingHuTang.Repository;

namespace CingHuTang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductSizeController : Controller
    {
        ProductSizeRepository _sizeRepo = new ProductSizeRepository();
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

        public JsonResult GetAllNoPage()
        {

            return Json(_sizeRepo.GetAll());
        } 
        public JsonResult GetAll(string request = "", int pageNumber = 1)
        {
            List<ProductSize> data = SQLHelper<ProductSize>.ProcedureToList("spGetAllProductSize", new string[] { "@PageNumber", "@Request" }, new object[] { pageNumber, request });
            PaginationDto totalCount = SQLHelper<PaginationDto>.ProcedureToModel("spGetAllTotalProductSize", new string[] { "@Request" }, new object[] { request });

            return Json(new { data, totalCount });
        }
        public JsonResult GetById(int Id)
        {
            return Json(_sizeRepo.GetByID(Id));
        }

        public JsonResult CreateOrUpdate([FromBody] ProductSize data )
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
            if(acc == null)
            {
                return Json(new { status = 1, statusText = "Bạn đã hết phiên đăng nhập! Vui lòng đăng nhập lại!" });
            }

            if (string.IsNullOrWhiteSpace(data.SizeName))
            {
                return Json(new { status = 1, statusText = "Vui lòng nhập Tên size sản phẩm!" });
            }

            if (string.IsNullOrWhiteSpace(data.SizeCode))
            {
                return Json(new { status = 1, statusText = "Vui lòng nhập Mã size sản phẩm!" });
            }

            ProductSize model = _sizeRepo.GetByID(data.Id) ?? new ProductSize();    
            model.SizeCode = data.SizeCode;
            model.SizeName = data.SizeName;
            model.Description = data.Description;
            model.IsDelete = false;
            
            if (model.Id > 0)
            {
                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = acc.FullName;
                _sizeRepo.Update(model);
            }
            else 
            {
                model.CreatedDate =  DateTime.Now;
                model.CreatedBy = acc.FullName;
                _sizeRepo.Create(model); 
            }

            return Json(new {status = 0, statusText = "success"});
        }

        public bool Delete(int Id)
        {
            _sizeRepo.Delete( Id );
            return true;
        }
    }
}
