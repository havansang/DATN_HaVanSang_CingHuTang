using CingHuTang.Config;
using CingHuTang.Models;
using CingHuTang.Models.DTOs;
using CingHuTang.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CingHuTang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        public AccountRepository _accRepo = new AccountRepository();
        public IActionResult Index()
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0) ?? new Account();
            if (acc == null || acc.Role < 2)
            {
                return Redirect("/Shop/Index");
            }
            ViewBag.Account = acc;
            return View();
        }

        public IActionResult actionResult()
        {
            ViewBag.data = _accRepo.GetAll();
            return View();
        }
        public JsonResult GetAllNoPage()
        {

            return Json(_accRepo.GetAll());
        }
        [HttpGet]
        public JsonResult GetAll(string request = "", int pageNumber = 1)
        {
            DataSet dsCus = LoadDataFromSP.GetDataSetSP("spGetAllAccount", new string[] { "@PageNumber", "@Request", "@RoleID" }
                                                                      , new object[] { pageNumber, request, 1 });
            DataSet dsEmp = LoadDataFromSP.GetDataSetSP("spGetAllAccount", new string[] { "@PageNumber", "@Request", "@RoleID", "@PageSize" }
                                                                      , new object[] { pageNumber, request, 3, 9999999 });
            DataSet dsManager = LoadDataFromSP.GetDataSetSP("spGetAllAccount", new string[] { "@PageNumber", "@Request", "@RoleID", "@PageSize" }
                                                                      , new object[] { pageNumber, request, 4, 9999999 });
            object customer = new
            {
                data = TextUtils.ConvertDataTable<AccountDto>(dsCus.Tables[0]),
                totalCount = TextUtils.ConvertDataTable<PaginationDto>(dsCus.Tables[1])
            };

            object employee = new
            {
                data = TextUtils.ConvertDataTable<AccountDto>(dsEmp.Tables[0]),
                totalCount = TextUtils.ConvertDataTable<PaginationDto>(dsEmp.Tables[1])
            };

            object manager = new
            {
                data = TextUtils.ConvertDataTable<AccountDto>(dsManager.Tables[0]),
                totalCount = TextUtils.ConvertDataTable<PaginationDto>(dsManager.Tables[1])
            };

            return Json(new { customer, employee, manager });
        }
        public JsonResult GetById(int Id)
        {
            return Json(_accRepo.GetByID(Id));
        }

        public JsonResult CreateOrUpdate([FromBody] Account data)
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
            if (acc == null)
            {
                return Json(new { status = 0, message = "Bạn đã hết phiên đăng nhập! Vui lòng đăng nhập lại!" });
            }
            bool isAdmin = acc.Role == 2 || acc.Role == 4;// 2: admin 4: quản lý
            if (!isAdmin)
            {
                return Json(new { status = 0, message = "Bạn KHÔNG được phép sử dụng tính năng này!" });
            }
            bool isCheck = _accRepo.GetAll().Any(p => p.Id != data.Id && p.Email.ToLower().Equals(data.Email.ToLower()));
            if (isCheck)
            {
                return Json(new { status = 0, message = "Email đã bị trùng! Hãy tạo lại email!" });
            }

            Account model = _accRepo.GetByID(data.Id) ?? new Account();

            model.Email = data.Email;
            model.Role = data.Role;
            model.FullName = data.FullName;
            model.Gender = data.Gender;
            model.PhoneNumber = data.PhoneNumber;
            model.Address = data.Address;
            model.IsActive = data.IsActive;
            model.IsDelete = false;

            if (model.Id > 0)
            {
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = acc.FullName;
                _accRepo.Update(model);
            }
            else
            {
                model.Password = MaHoaMD5.EncryptPassword("123456");
                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = acc.FullName;
                _accRepo.Create(model);
            }

            return Json(new { status = 1, message = "Thành công!" });
        }

        public bool ResetPassword(int Id)
        {
            Account acc = _accRepo.GetByID(Id);
            acc.Password = MaHoaMD5.EncryptPassword("1");
            _accRepo.Update(acc);
            return true;
        }

    }
}