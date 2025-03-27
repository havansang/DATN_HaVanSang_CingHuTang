using CingHuTang.Models.DTOs;
using CingHuTang.Models;
using Microsoft.AspNetCore.Mvc;
using CingHuTang.Config;
using CingHuTang.Repository;

namespace CingHuTang.Controllers
{
    public class AccountController : Controller
    {
        public AccountRepository _accRepo = new AccountRepository();
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email = "", string password = "", string confirmPassword = "", string fullname = "")
        {
            bool isCheck = _accRepo.GetAll().Any(x => x.Email == email);
            if (confirmPassword != password)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View();
            }
            else if (isCheck)
            {
                ViewBag.Error = "Email đã được sử dụng!";
                return View();
            }

            Account newAcc = new Account()
            {
                Email = email,
                Password = MaHoaMD5.EncryptPassword(password),
                FullName = fullname,
                Address = "",
                Gender = 0,
                PhoneNumber = "",
                Role = 1,
                IsActive = 1,
                IsDelete = false,
                CreatedDate = DateTime.Now,
                CreatedBy = fullname,
                UpdatedDate = DateTime.Now,
                UpdatedBy = fullname
            };
            await _accRepo.CreateAsync(newAcc);
            HttpContext.Session.SetInt32("AccountId", newAcc.Id);
            HttpContext.Session.SetInt32("AccountRole", newAcc.Role);
            HttpContext.Session.SetString("FullName", newAcc.FullName ?? "");
            if (newAcc.Id > 0) return RedirectToAction("Index", "Home");

            return View();
        }
    }
}
