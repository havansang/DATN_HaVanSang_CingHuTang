using CingHuTang.Config;
using CingHuTang.Models;
using CingHuTang.Models.DTOs;
using CingHuTang.Reposiory;
using CingHuTang.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CingHuTang.Controllers
{
    public class ReviewController : Controller
    {
        public ReviewRepository _repo = new ReviewRepository();

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ReviewModel model)
        {
            try
            {
                var review = new Review
                {
                    ProductId = model.ProductId,
                    AccountId = model.AccountId,
                    Rating = model.Rating,
                    Comment = model.Comment,
                    ReviewDate = DateTime.Now
                };

                await _repo.CreateAsync(review);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        [HttpGet]
        public JsonResult GetProductReviews(int productId)
        {
            try
            {
                var reviews = SQLHelper<ReviewDTO>.ProcedureToList(
                        "spGetProductReviews",
                        new string[] { "@ProductId" },
                        new object[] { productId }
                    );
                return Json(new
                {
                    status = 1,
                    message = "Success",
                    data = reviews
                }, new System.Text.Json.JsonSerializerOptions());
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = 0,
                    message = ex.Message,
                    data = new List<ReviewDTO>()
                });
            }
        }

    }

    public class ReviewModel
    {
        public int ProductId { get; set; }
        public int AccountId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string ReviewDate { get; set; }
    }
}
