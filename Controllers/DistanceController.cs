using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace CingHuTang.Controllers
{
    public class DistanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CalculateShippingFee(string deliveryAddress)
        {
            string apiKey = "5b3ce3597851110001cf6248f3bcf7a07dad4dbda439a8a2f09e1072";
            string shopAddress = "80 Xuân Phương, Quận Nam Từ Liêm, Hà Nội"; // Địa chỉ cửa hàng

            try
            {
                double distance = await CalculateWalkingDistance(shopAddress, deliveryAddress, apiKey);
                decimal fee = (decimal)(distance * 1500*0.001); // Giả sử 5.000đ/km
                return Json(new { distance, fee });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
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
