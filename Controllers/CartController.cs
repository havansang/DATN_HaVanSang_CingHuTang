using CingHuTang.Models;
using CingHuTang.Models.DTOs;
using CingHuTang.Reposiory;
using CingHuTang.Config;
using Microsoft.AspNetCore.Mvc;
using CingHuTang.Repository;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CingHuTang.Controllers
{
    public class CartController : Controller
    {
        public CartRepository _repo = new CartRepository();
        public AccountRepository _accRepo = new AccountRepository();
        public CartToppingRepository _cartToppingRepo = new CartToppingRepository();
        public async Task<ActionResult> Index()
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0) ?? new Account();
            if (acc.Id <= 0)
            {
                return Redirect("/Shop/Index");
            }
            ViewBag.Account = acc;
            string add=acc.Address;
            string apiKey = "5b3ce3597851110001cf6248f3bcf7a07dad4dbda439a8a2f09e1072";
            string shopAddress = "80 Xuân Phương, Quận Nam Từ Liêm, Hà Nội"; // Địa chỉ cửa hàng

            try
            {
                double distance = await CalculateWalkingDistance(shopAddress, add, apiKey);
                decimal fee = (decimal)(distance * 1500 * 0.001);
                ViewBag.fee = fee;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.fee = "Đang tính...";
                return View();
            }
        }
        public JsonResult GetCartByAccountId()
        {
            Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0) ?? new Account();
            if (acc.Id <= 0)
            {
                return Json(new { status = 0, massage = "Đăng nhập để sử dụng tính năng!" });
            }
            List<CartDto> lst = SQLHelper<CartDto>.ProcedureToList("spGetCartByAccountId",
                                                                    new string[] { "@AccountId" },
                                                                    new object[] { acc.Id });
            foreach (CartDto item in lst)
            {
                item.lstToppings = SQLHelper<Topping>.SqlToList($"SELECT t.* FROM dbo.CartTopping AS pt LEFT JOIN dbo.Topping AS t ON pt.ToppingID = t.ID WHERE pt.CartID = {item.CartId}");
            }
            if (lst.Count == 0) return Json(new { status = 2, massage = "Bạn chưa có sản phẩm nào trong giỏ hàng!" });

            return Json(new { status = 1, massage = "", data = lst });

        }

        [HttpPost]
        public async Task<JsonResult> AddToCart([FromBody] AddToCartDTO data)
        {
            try
            {
                Account acc = _accRepo.GetByID(HttpContext.Session.GetInt32("AccountId") ?? 0) ?? new Account();

                if (data.AccountID <= 0 || acc.Id <= 0) return Json(new { status = 0, message = "Hãy đăng nhập để sử dụng tính năng này!" });
                if (data.ProductDetailID <= 0) return Json(new { status = 0, message = "Hãy chọn size sản phẩm!" });


                List<Cart> lst = SQLHelper<Cart>.SqlToList($"SELECT * FROM Cart WHERE AccountId = {data.AccountID}");

                // Tìm lại productDetail đã có trong giỏ hàng
                List<Cart> lstModel = lst.Where(x => x.ProductDetailId == data.ProductDetailID).ToList();
                List<CartDataDTO> lstData = new List<CartDataDTO>();
                foreach (Cart item in lstModel) 
                {
                    CartDataDTO dto = new CartDataDTO();
                    dto.Id = item.Id;
                    dto.AccountId = item.AccountId;
                    dto.ProductDetailId = item.ProductDetailId;
                    dto.Quantity = item.Quantity;
                    dto.CreatedDate = item.CreatedDate;
                    dto.CreatedBy = item.CreatedBy;
                    dto.UpdatedDate = item.UpdatedDate;
                    dto.UpdatedBy = item.UpdatedBy;
                    dto.lstTopping = SQLHelper<Topping>.SqlToList($"SELECT t.* FROM dbo.CartTopping AS pt LEFT JOIN dbo.Topping AS t ON pt.ToppingID = t.ID WHERE pt.CartID = {item.Id}");
                    lstData.Add(dto);
                }
                lstData = lstData.Where(p=> p.lstTopping.Count == data.ToppingIDs.Count).ToList();
                Cart model = new Cart();

                //Tìm cart productDetail có cx topping
                foreach (CartDataDTO item in lstData) 
                {
                    bool isValid = true;
                    for (int i = 0; i < data.ToppingIDs.Count;  i++)
                    {
                        isValid = item.lstTopping.Any(p=> p.Id == data.ToppingIDs[i]);
                        if (!isValid) break;
                    }
                    if (isValid) model = lstModel.FirstOrDefault(p=> p.Id == item.Id) ?? new Cart();
                }

                if (model.Id > 0)
                {
                    model.Quantity = model.Quantity + data.Quantity;
                    model.UpdatedBy = acc.FullName;
                    model.UpdatedDate = DateTime.Now;
                    _repo.Update(model);
                }
                else
                {
                    model.Id = 0;
                    model.ProductDetailId = data.ProductDetailID;
                    model.AccountId = data.AccountID;
                    model.Quantity = data.Quantity;
                    model.CreatedBy = acc.FullName;
                    model.CreatedDate = DateTime.Now;
                    await _repo.CreateAsync(model);
                }

                SQLHelper<CartTopping>.SqlToList($"DELETE FROM dbo.CartTopping WHERE CartID = {model.Id}");
                foreach (int toppingID in data.ToppingIDs)
                {
                    CartTopping newCartTopping = new CartTopping()
                    {
                        CartId = model.Id,
                        ToppingId = toppingID,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        CreatedBy = acc.FullName,
                        UpdatedBy = acc.FullName
                    };
                    _cartToppingRepo.Create(newCartTopping);
                }


                return Json(new { status = 1, message = "Đã thêm vào giỏ hàng!" });
            }
            catch (Exception ex)
            {

                return Json(new { status = 0, message = ex.Message });
            }

        }


        public async Task<JsonResult> RemoveToCart(int cartId)
        {
            try
            {
                Cart model = _repo.GetByID(cartId) ?? new Cart();
                if (model.Id > 0)
                {

                    _repo.Delete(model.Id);
                    SQLHelper<CartTopping>.SqlToList($"DELETE FROM dbo.CartTopping WHERE CartID = {model.Id}");
                }
                return Json(new { status = 1, message = "Cập nhật giỏ hàng thành công!" });
            }
            catch (Exception ex)
            {

                return Json(new { status = 0, message = ex.Message });
            }

        }
        public async Task<double> CalculateWalkingDistance(string originAddress, string destinationAddress, string apiKey)
        {
            try
            {
                var (destLng, destLat) = await GetCoordinates(destinationAddress, apiKey);

                // Debug tọa độ
                //System.Diagnostics.Debug.WriteLine($"Start: 105.733334,21.048424");
                //System.Diagnostics.Debug.WriteLine($"End: {destLng},{destLat}");

                using (HttpClient client = new HttpClient())
                {
                    // 1. THAY ĐỔI HEADER ACCEPT THEO YÊU CẦU CỦA API
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/geo+json"));
                    client.DefaultRequestHeaders.Add("Accept-Charset", "UTF-8");

                    // 2. THÊM API KEY VÀO HEADER (nếu cần)
                    client.DefaultRequestHeaders.Add("Authorization", apiKey);

                    string url = $"https://api.openrouteservice.org/v2/directions/foot-walking?start=105.733334,21.048424&end={destLng},{destLat}";

                    // 3. DEBUG URL TRƯỚC KHI GỬI
                    //System.Diagnostics.Debug.WriteLine($"Request URL: {url}");

                    HttpResponseMessage response = await client.GetAsync(url);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // 4. KIỂM TRA RESPONSE
                    if (!response.IsSuccessStatusCode)
                    {
                        //System.Diagnostics.Debug.WriteLine($"API Error: {response.StatusCode}");
                        //ystem.Diagnostics.Debug.WriteLine($"Response: {responseBody}");
                        throw new Exception($"API error: {responseBody}");
                    }

                    dynamic result = JsonConvert.DeserializeObject(responseBody);
                    return (double)result.features[0].properties.summary.distance;
                }
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine($"Lỗi: {ex.Message}");
                throw;
            }
        }

        // Hàm lấy tọa độ từ địa chỉ
        public async Task<(double lng, double lat)> GetCoordinates(string address, string apiKey)
        {
            using (HttpClient client = new HttpClient())
            {
                // Gọi Geocoding API
                string url = $"https://api.openrouteservice.org/geocode/search?api_key={apiKey}&text={address}";
                HttpResponseMessage response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(json);

                // Lấy tọa độ đầu tiên từ kết quả
                if (result.features.Count > 0)
                {
                    double lng = result.features[0].geometry.coordinates[0];
                    double lat = result.features[0].geometry.coordinates[1];
                    return (lng, lat);
                }
                else
                {
                    throw new Exception("Không tìm thấy tọa độ cho địa chỉ này.");
                }
            }
        }
    }
}
