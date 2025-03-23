using CingHuTang.Models;
using ManagementCourse.Reposiory;

namespace CingHuTang.Repository
{
    public class ProductImageRepository : GenericRepository<ProductImage>
    {
        public async Task<bool> DeleteProductImages(List<int> lstFileIDs)
        {
            try
            {
                foreach (int productFileID in lstFileIDs)
                {
                    try
                    {
                        ProductImage productImage = GetByID(productFileID);
                        if (productImage == null) continue;

                        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\Images\\Product\\{productImage.ImageUrl}");
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    catch 
                    {
                        continue;
                    }
                }

            }
            catch (Exception err)
            {
                return false;
            }

            return true;
        }


    }
}
