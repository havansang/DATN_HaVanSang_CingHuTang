using CingHuTang.Config;
using CingHuTang.Models;
using CingHuTang.Models.DTOs;
using CingHuTang.Reposiory;
using CingHuTang.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using System.Configuration.Internal;
using System.Data;
using System.IO;

namespace CingHuTang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        ProductTypeRepository _proType = new ProductTypeRepository();
        ProductRepository _pro = new ProductRepository();
        ProductDetailsRepository _detailRepo = new ProductDetailsRepository();
        ProductImageRepository fileRepo = new ProductImageRepository();
        AccountRepository _accRepo = new AccountRepository();
        ToppingRepository _toppingRepo = new ToppingRepository();
        ProductToppingRepository _proTopping = new ProductToppingRepository();
        public IActionResult Index()
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
            if (acc == null || acc.Role == 3 || acc.Role == 1)
            {
                return Redirect("/Home/Index");
            }
            List<ProductType> lstType = _proType.GetAll().Where(p=> p.IsDelete == false).ToList();
            ViewBag.ListType = new SelectList(lstType, "Id", "TypeName");

            List<Topping> lstTopping = _toppingRepo.GetAll().ToList();
            List<object> lstDataToping = new List<object>();
            foreach (Topping item in lstTopping)
            {
                lstDataToping.Add(new
                {
                    Id = item.Id,
                    ToppingCode = item.ToppingCode ,
                    ToppingName = item.ToppingName ,
                    ToppingPrice = item.ToppingPrice ,
                    ToppingPriceStr = TextUtils.ToDecimal(item.ToppingPrice).ToString("N0")

            });
            }
            ViewBag.Topping = lstDataToping;

            return View();
        }


        // ======================= Product=========================================================================

        public JsonResult GetAll(string request = "", int pageNumber = 1, int productTypeId = 0)
        {
            DataSet ds = LoadDataFromSP.GetDataSetSP("spGetAllProduct", new string[] { "@PageNumber", "@Request", "@ProductTypeID" }, new object[] { pageNumber, TextUtils.ToString(request), productTypeId });
            List <ProductDto> data = TextUtils.ConvertDataTable<ProductDto>(ds.Tables[0]);
            var totalCount = TextUtils.ConvertDataTable<PaginationDto>(ds.Tables[1]);

            return Json(new { data, totalCount });
        }
        public async Task<JsonResult> CreateOrUpdate([FromBody] ProductDto data)
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
            if (acc == null)
            {
                return Json(new { status = 1, statusText = "Bạn đã hết phiên đăng nhập! Vui lòng đăng nhập lại!" });
            }

            bool isCheck = _pro.GetAll().Any(p => p.Id != data.Id && p.ProductCode == data.ProductCode);
            if (isCheck) return Json(new { status = 1, message = "Mã sản phẩm đã bị trùng!", result = 0 });

            //Lưu sản phẩm
            Product newPro = _pro.GetByID(data.Id) ?? new Product();

            newPro.Id = data.Id;
            newPro.ProductCode = data.ProductCode;
            newPro.ProductName = data.ProductName;
            newPro.IsActive = data.IsActive;
            newPro.Description = data.Description;
            newPro.ProductTypeId = data.ProductTypeId;
            newPro.IsDeleted = false;


            if (newPro.Id > 0)
            {
                newPro.UpdatedDate = DateTime.Now;
                newPro.UpdatedBy = acc.FullName;
                _pro.Update(newPro);
            }
            else
            {
                newPro.CreatedDate = DateTime.Now;
                newPro.CreatedBy = acc.FullName;
                _pro.Create(newPro);
            }

            //Lưu giá sản phẩm
            SQLHelper<ProductDetail>.SqlToList($"DELETE ProductDetails WHERE ProductId = {newPro.Id}");

            foreach (ProductDetail item in data.ListDetails)
            {
                ProductDetail newDetail = new ProductDetail()
                {
                    Id = item.Id,
                    ProductId = newPro.Id,
                    ProductSizeId = item.ProductSizeId,
                    Price = item.Price,
                    CreatedDate = DateTime.Now,
                    CreatedBy = acc.FullName,
                    IsDelete = false
                };

                if (newDetail.Id > 0) _detailRepo.Update(newDetail);
                else _detailRepo.Create(newDetail);
            }

            //Lưu topping
            SQLHelper<ProductDetail>.SqlToList($"DELETE ProductTopping WHERE ProductId = {newPro.Id}");

            foreach (int toppingID in data.ListTopping)
            {
                ProductTopping newDetail = new ProductTopping()
                {
                    Id = 0,
                    ProductId = newPro.Id,
                    ToppingId = toppingID,
                    CreatedDate = DateTime.Now,
                    CreatedBy = acc.FullName
                };
                _proTopping.Create(newDetail);
            }

            if (data.ListFileIDs.Count > 0)
            {
                bool isDeleted = await fileRepo.DeleteProductImages(data.ListFileIDs);
                string lstFileIDs = string.Join(",", data.ListFileIDs);
                SQLHelper<ProductDetail>.SqlToList($"DELETE ProductImage WHERE ID IN ({lstFileIDs})");
            }
            return Json(new { status = 0, message = "Thành công!", result = newPro }); ;
        }


        public JsonResult GetById(int Id)
        {
            Product data = _pro.GetByID(Id);
            List<ProductDetail> details = SQLHelper<ProductDetail>.SqlToList($"Select * from ProductDetails where ProductId = {Id}");
            List<ProductImage> images = SQLHelper<ProductImage>.SqlToList($"Select * from ProductImage where ProductId = {Id}");
            List<ProductTopping> toppings = SQLHelper<ProductTopping>.SqlToList($"Select * from ProductTopping where ProductId = {Id}");
            foreach (ProductImage item in images)
            {
                item.ImageUrl = Config.Config.ProductImageUrl() + item.ImageUrl;
            }
            return Json(new { data, details, images, toppings });
        }

        public bool Delete(int Id)
        {
            Product data = _pro.GetByID(Id) ?? new Product();
            if (data.Id <= 0) return false;

            data.IsDeleted = true;
            _pro.Update(data);

            return true;
        }

        // ======================= ProductType =========================================================================

        public IActionResult ProductType()
        {
            return View();
        }
        public JsonResult GetAllProductTypeNoPage()
        {
            return Json(_proType.GetAll().ToList());
        }
        [HttpGet]
        public JsonResult GetAllProductType(string request = "", int pageNumber = 1)
        {
            List<ProductType> data = SQLHelper<ProductType>.ProcedureToList("spGetAllProductType", new string[] { "@PageNumber", "@Request" }, new object[] { pageNumber, request });
            PaginationDto totalCount = SQLHelper<PaginationDto>.ProcedureToModel("spGetTotalCountProductType", new string[] { "@Request" }, new object[] { request });

            return Json(new { data, totalCount });
        }
        public JsonResult GetByIdProductType(int Id)
        {
            return Json(_proType.GetByID(Id));
        }
        public async Task<JsonResult> CreateProductType([FromBody] ProductType data)
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
            if (acc == null)
            {
                return Json(new { status = 1, statusText = "Bạn đã hết phiên đăng nhập! Vui lòng đăng nhập lại!" });
            }


            if (string.IsNullOrWhiteSpace(data.TypeCode))
            {
                return Json(new { status = 1, statusText = "Vui lòng nhập Tên size sản phẩm!" });
            }

            if (string.IsNullOrWhiteSpace(data.TypeName))
            {
                return Json(new { status = 1, statusText = "Vui lòng nhập Mã size sản phẩm!" });
            }

            bool isCheck = _proType.GetAll().Any(x => x.Id != data.Id && x.TypeCode == data.TypeCode);
            if (isCheck) return Json(new { status = 0, message = "Mã loại sản phẩm đã bị trùng! Hãy kiểm tra lại!", result = 0 });

            ProductType model = _proType.GetByID(data.Id) ?? new ProductType();

            model.TypeCode = data.TypeCode;
            model.TypeName = data.TypeName;
            model.GroupTypeId = data.GroupTypeId;
            model.Description = data.Description;
            model.IsDelete = false;


            if (model.Id > 0)
            {
                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = acc.FullName;
                _proType.Update(model);
            }
            else
            {
                model.CreatedDate = DateTime.Now;
                model.CreatedBy = acc.FullName;
                _proType.Create(model);
            }


            return Json(new { status = 0, message = "", result = model });
        }

        public async Task<JsonResult> DeleteProductType(int Id)
        {

            bool isCheck = _pro.GetAll().Any(x => x.ProductTypeId == Id);
            if (isCheck) return Json(new { status = 0, message = "Mã loại sản phẩm đã được sử dụng! Không thể xóa!" });
            _proType.Delete(Id);
            return Json(new { status = 1, message = "" });
        }

        // ===================================================================  END =========================================================================

        [HttpPost]
        public async Task<IActionResult> UploadFile(Product product)
        {
            try
            {
                Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0);
                if (acc == null)
                {
                    return Json(new { status = 0, statusText = "Bạn đã hết phiên đăng nhập! Vui lòng đăng nhập lại!" });
                }
                product = _pro.GetByID(product.Id) ?? new Product();
                ProductType productType = _proType.GetByID(TextUtils.ToInt(product.ProductTypeId)) ?? new ProductType();
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                var files = Request.Form.Files;
                List<ProductImage> listFiles = new List<ProductImage>();
                foreach (var file in files)
                {
                    if (file.Length <= 0) continue;
                    listFiles.Add(new ProductImage()
                    {
                        ImageUrl = $"{productType.TypeCode}/{product.ProductCode}/{file.FileName}",
                        ImageName = file.FileName,
                        ProductId = product.Id,
                        CreatedDate = DateTime.Now,
                        IsDelete = false,
                        CreatedBy = acc.FullName
                    });
                    string pathUpload = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\Images\\Product\\{productType.TypeCode}\\{product.ProductCode}");
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
                }
                fileRepo.CreateRange(listFiles);
                return Ok(new { status = 1, message = "Upload file thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 1, message = ex.Message });
            }
        }

    }
}
