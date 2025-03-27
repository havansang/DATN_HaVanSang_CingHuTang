using CingHuTang.Models;
using CingHuTang.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using CingHuTang.Config;
using System.Security.Cryptography;
using CingHuTang.Repository;

namespace CingHuTang.Controllers
{
    public class AccountDetailsController : Controller
    {
        public AccountRepository _accRepo = new AccountRepository();
        public IActionResult Index()
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0) ?? new Account();
            if (acc.Id <= 0)
            {
                return Redirect("/Home/Index");
            }

            acc.AvatarUrl = string.IsNullOrWhiteSpace(acc.AvatarUrl) ? "/assets/images/user.png" : acc.AvatarUrl;
            ViewBag.Account = acc;

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> UpdateAccount([FromBody] Account account)
        {
            Account accSesion = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0) ?? new Account();
            if (accSesion.Id <= 0)
            {
                return  Json(new { status = 0, message = "Đã hết phiên đăng nhập! Vui lòng đăng nhập lại để sử dụng chức năng!" });
            }


            Account model = _accRepo.GetByID(account.Id) ?? new Account();
            if (model.Id <= 0) return Json(new { status = 0, message = "Không tìm thấy tài khoản!" });

            model.Address = account.Address;
            model.PhoneNumber = account.PhoneNumber;
            model.FullName = account.FullName;
            model.Gender = account.Gender;
            model.UpdatedDate = DateTime.Now;
            model.UpdatedBy = accSesion.FullName;
            _accRepo.Update(model);


            return Json(new { status = 1, message = "Cập nhật thành công!", data = model });
        }
        [HttpPost]
        public async Task<JsonResult> UploadFile(Account account)
        {
            try
            {
                account = _accRepo.GetByID(account.Id) ?? new Account();
                var files = Request.Form.Files;
                if(files == null || files.Count <= 0) return Json(new { status = 0, message = "Không nhận được file ảnh!" });

                var file = files[0];
                string pathUpload = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\Images\\Avatar\\Account{account.Id}");
                if (!Directory.Exists(pathUpload))
                {
                    Directory.CreateDirectory(pathUpload);
                }
                string imagePath = pathUpload + $"\\{file.FileName}";
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                using (FileStream stream = System.IO.File.Create(imagePath))
                {
                    await file.CopyToAsync(stream);
                }
                account.AvatarUrl = $"/Images/Avatar/Account{account.Id}/{file.FileName}";
                _accRepo.Update(account);

                return Json(new { status = 1, message = "Upload file thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<JsonResult> UpdatePassword([FromBody] PasswordDTO data)
        {
            Account account = _accRepo.GetByID(data.AccountID) ?? new Account();
            if(account.Id <= 0)
            {
                return Json(new { status = 0, message = "Không tìm thấy tài khoản!" });
            }
            else if (string.IsNullOrWhiteSpace(data.OldPassword))
            {
                return Json(new { status = 0, message = "Mật khẩu cũ không được để trống!" });
            }
            else if (string.IsNullOrWhiteSpace(data.NewPassword))
            {
                return Json(new { status = 0, message = "Mật khẩu mới không được để trống!" });
            }
            else if (string.IsNullOrWhiteSpace(data.ConfirmPassword))
            {
                return Json(new { status = 0, message = "Mật khẩu xác thực không được để trống!" });
            }
            else if (data.ConfirmPassword.Trim() != data.NewPassword)
            {
                return Json(new { status = 0, message = "Mật khẩu xác thực không chính xác! Vui lòng nhập lại!" });
            }
            else if (data.OldPassword.Trim() != MaHoaMD5.DecryptPassword(account.Password ?? "MQA="))
            {
                return Json(new { status = 0, message = "Mật khẩu cũ không chính xác! Vui lòng nhập lại!" });
            }

            account.Password = MaHoaMD5.EncryptPassword(data.NewPassword);
            _accRepo.Update(account);
            return Json(new { status = 1, message = "Cập nhật thành công!" });
        }
    }
}
