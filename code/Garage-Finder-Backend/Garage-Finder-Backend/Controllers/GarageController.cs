﻿using DataAccess.DAO;
using DataAccess.DTO;
using DataAccess.DTO.RequestDTO.Category;
using DataAccess.DTO.RequestDTO.Garage;
using DataAccess.DTO.User.ResponeModels;
using GFData.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.DependencyResolver;
using Repositories.Interfaces;
using System.Security.Claims;

namespace Garage_Finder_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GarageController : Controller
    {
        private readonly IGarageRepository garageRepository;
        private readonly IGarageBrandRepository garageBrandRepository;
        private readonly ICategoryGarageRepository categoryGarageRepository;
        private readonly IImageGarageRepository imageGarageRepository;
        private readonly IGarageInforRepository inforRepository;
        public GarageController(IGarageRepository garageRepository, IGarageBrandRepository garageBrandRepository,
            ICategoryGarageRepository categoryGarageRepository, IImageGarageRepository imageGarageRepository,
            IGarageInforRepository inforRepository)
        {
            this.garageRepository = garageRepository;
            this.garageBrandRepository = garageBrandRepository;
            this.categoryGarageRepository = categoryGarageRepository;
            this.imageGarageRepository = imageGarageRepository;
            this.inforRepository = inforRepository;
        }
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(garageRepository.GetGarages());
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPost("Add")]
        [Authorize]
        public IActionResult Add([FromBody] AddGarageDTO addGarage)
        {
            try
            {
                var user = GetUserFromToken();
                var garageDTO = garageRepository.Add(addGarage);

                GarageInfoDTO garageInfoDTO = new GarageInfoDTO()
                {
                    GarageID = garageDTO.GarageID,
                    UserID = user.UserID,
                };
                inforRepository.Add(garageInfoDTO);

                var listGarageBrand = new List<GarageBrandDTO>();
                foreach (var brand in addGarage.BrandsID)
                {
                    listGarageBrand.Add(new GarageBrandDTO()
                    {
                        GarageID = garageDTO.GarageID,
                        BrandID = brand
                    });
                }
                listGarageBrand.ForEach(x => garageBrandRepository.Add(x));

                var listCategory = new List<CategoryGarageDTO>();
                foreach (var cate in addGarage.CategoriesID)
                {
                    listCategory.Add(new CategoryGarageDTO()
                    {
                        CategoryID = cate,
                        GarageID = garageDTO.GarageID
                    });
                }
                listCategory.ForEach(x => categoryGarageRepository.Add(x));

                var listImage = new List<ImageGarageDTO>();
                foreach (var image in addGarage.ImageLink)
                {
                    listImage.Add(new ImageGarageDTO()
                    {
                        GarageID = garageDTO.GarageID,
                        ImageLink = image
                    });
                }
                listImage.ForEach(x => imageGarageRepository.AddImageGarage(x));
                return Ok("SUCCESS");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPut("Update")]
        public IActionResult Update([FromBody] UpdateGarageDTO garageUpdate)
        {
            try
            {
                garageRepository.Update(garageUpdate);
                return Ok("SUCCESS");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                garageRepository.DeleteGarage(id);
                return Ok("SUCCESS");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }


        [HttpPost("GetByKeyWord")]
        public IActionResult SearchGarage([FromBody] SearchGarage searchGarage)
        {
            // Lấy danh sách garage từ nguồn dữ liệu
            var garages = garageRepository.GetGarages();

            // Áp dụng các bộ lọc
            if (!string.IsNullOrEmpty(searchGarage.keyword) && searchGarage.provinceID is null && searchGarage.districtsID is null && searchGarage.categoriesID is null)
            {
                garages = garages.Where(g => g.GarageName.Contains(searchGarage.keyword)).Skip((searchGarage.pageNumber - 1) * searchGarage.pageSize).Take(searchGarage.pageSize).ToList();
            }

            if (string.IsNullOrEmpty(searchGarage.keyword) && searchGarage.provinceID is not null && searchGarage.districtsID is not null && searchGarage.categoriesID is null)
            {
                garages = garages.Where(g => searchGarage.provinceID.Any(d => d == g.ProvinceID) && searchGarage.districtsID.Any(d => d == g.DistrictsID)).Skip((searchGarage.pageNumber - 1) * searchGarage.pageSize).Take(searchGarage.pageSize).ToList();
            }

            if (string.IsNullOrEmpty(searchGarage.keyword) && searchGarage.provinceID is null && searchGarage.districtsID is null && searchGarage.categoriesID is not null)
            {
                garages = garages.Where(g => g.CategoryGarages.Any(c => searchGarage.categoriesID.Any(x => x == c.CategoryID))).Skip((searchGarage.pageNumber - 1) * searchGarage.pageSize).Take(searchGarage.pageSize).ToList();
            }

            if (!string.IsNullOrEmpty(searchGarage.keyword) && searchGarage.provinceID is not null && searchGarage.districtsID is not null && searchGarage.categoriesID is null)
            {
                garages = garages.Where(g => g.GarageName.Contains(searchGarage.keyword) && searchGarage.provinceID.Any(d => d == g.ProvinceID) && searchGarage.districtsID.Any(d => d == g.DistrictsID)).Skip((searchGarage.pageNumber - 1) * searchGarage.pageSize).Take(searchGarage.pageSize).ToList();
            }

            if (!string.IsNullOrEmpty(searchGarage.keyword) && searchGarage.provinceID is null && searchGarage.districtsID is null && searchGarage.categoriesID is not null)
            {
                garages = garages.Where(g => g.GarageName.Contains(searchGarage.keyword) && g.CategoryGarages.Any(c => searchGarage.categoriesID.Any(x => x == c.CategoryID))).Skip((searchGarage.pageNumber - 1) * searchGarage.pageSize).Take(searchGarage.pageSize).ToList();
            }

            if (string.IsNullOrEmpty(searchGarage.keyword) && searchGarage.provinceID is not null && searchGarage.districtsID is not null && searchGarage.categoriesID is not null)
            {
                garages = garages.Where(g => searchGarage.provinceID.Any(d => d == g.ProvinceID) && searchGarage.districtsID.Any(d => d == g.DistrictsID) && g.CategoryGarages.Any(c => searchGarage.categoriesID.Any(x => x == c.CategoryID))).Skip((searchGarage.pageNumber - 1) * searchGarage.pageSize).Take(searchGarage.pageSize).ToList();
            }

            if (!string.IsNullOrEmpty(searchGarage.keyword) && searchGarage.provinceID is not null && searchGarage.districtsID is not null && searchGarage.categoriesID is not null)
            {
                garages = garages.Where(g => g.GarageName.Contains(searchGarage.keyword) && searchGarage.provinceID.Any(d => d == g.ProvinceID) && searchGarage.districtsID.Any(d => d == g.DistrictsID) && g.CategoryGarages.Any(c => searchGarage.categoriesID.Any(x => x == c.CategoryID))).Skip((searchGarage.pageNumber - 1) * searchGarage.pageSize).Take(searchGarage.pageSize).ToList();
            }

            if (string.IsNullOrEmpty(searchGarage.keyword) && searchGarage.provinceID is not null && searchGarage.districtsID is null && searchGarage.categoriesID is null)
            {
                garages = garages.Where(g => searchGarage.provinceID.Any(d => d == g.ProvinceID)).Skip((searchGarage.pageNumber - 1) * searchGarage.pageSize).Take(searchGarage.pageSize).ToList();
            }

            if (!string.IsNullOrEmpty(searchGarage.keyword) && searchGarage.provinceID is not null && searchGarage.districtsID is null && searchGarage.categoriesID is null)
            {
                garages = garages.Where(g => g.GarageName.Contains(searchGarage.keyword) && searchGarage.provinceID.Any(d => d == g.ProvinceID)).Skip((searchGarage.pageNumber - 1) * searchGarage.pageSize).Take(searchGarage.pageSize).ToList();
            }

            if (string.IsNullOrEmpty(searchGarage.keyword) && searchGarage.provinceID is not null && searchGarage.districtsID is null && searchGarage.categoriesID is not null)
            {
                garages = garages.Where(g => searchGarage.provinceID.Any(d => d == g.ProvinceID) && g.CategoryGarages.Any(c => searchGarage.categoriesID.Any(x => x == c.CategoryID))).Skip((searchGarage.pageNumber - 1) * searchGarage.pageSize).Take(searchGarage.pageSize).ToList();
            }

            // Trả về kết quả
            return Ok(garages);
        }

        [HttpPost("GetByPage")]
        public IActionResult GetByPage([FromBody] PageDTO p)
        {
            var garages = garageRepository.GetByPage(p);
            return Ok(garages);
        }

        [HttpGet("GetByUser")]
        [Authorize]
        public IActionResult GetByUser()
        {
            try
            {
                var user = GetUserFromToken();
                return Ok(garageRepository.GetGarageByUser(user.UserID));
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }
        #region Brand

        [HttpPost("addBrand")]
        [Authorize]
        public IActionResult AddBrandForGarage([FromBody] List<GarageBrandDTO> garageBrandsDTO)
        {
            try
            {
                foreach (var garage in garageBrandsDTO)
                {
                    garageBrandRepository.Add(garage);
                }
                return Ok("SUCCESS");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("removeBrand")]
        [Authorize]
        public IActionResult RemoveBrand([FromBody] List<int> garageBrandIds)
        {
            try
            {
                foreach (var id in garageBrandIds)
                {
                    garageBrandRepository.Delete(id);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
        #region Category

        [HttpPost("addCategoryForGarage")]
        [Authorize]
        public IActionResult AddCategoryForGarage([FromBody] List<AddCategoryGarage> addCategoryGarageDTO)
        {
            try
            {
                foreach (var cate in addCategoryGarageDTO)
                {
                    CategoryGarageDTO categoryGarageDTO = new CategoryGarageDTO()
                    {
                        GarageID = cate.GarageID,
                        CategoryID = cate.CategoryID
                    };
                    categoryGarageRepository.Add(categoryGarageDTO);
                }

                return Ok("SUCCESS");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("removeCategory")]
        [Authorize]
        public IActionResult RemoveCategory([FromBody] List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    categoryGarageRepository.Remove(id);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Image
        [HttpPost("addImage")]
        [Authorize]
        public IActionResult AddImage([FromBody] List<ImageGarageDTO> imagesDTO)
        {
            try
            {
                foreach (var image in imagesDTO)
                {
                    imageGarageRepository.AddImageGarage(image);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("removeImage")]
        [Authorize]
        public IActionResult RemoveImage([FromBody] List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    imageGarageRepository.RemoveImageGarage(id);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
        #region Search/Filter

        [HttpGet("GetByID/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                return Ok(garageRepository.GetGaragesByID(id));
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPost("GetByFilter")]
        public ActionResult GetGarages([FromBody] FilterGarage filterGarage)
        {
            List<GarageDTO> filteredGarages = garageRepository.GetGarages();
            List<GarageDTO> filteredByProvince = new List<GarageDTO>();
            List<GarageDTO> filteredByDistricts = new List<GarageDTO>();
            List<GarageDTO> filteredByCategory = new List<GarageDTO>();
            List<GarageDTO> filteredByBrand = new List<GarageDTO>();

            if (filterGarage.provinceID is not null)
            {
                if (filterGarage.provinceID.Count != 0)
                    filteredGarages = filteredGarages.Where(x => filterGarage.provinceID.Any(d => d == x.ProvinceID)).ToList();
            }

            if (filterGarage.districtsID is not null)
            {
                if (filterGarage.districtsID.Count != 0)
                    filteredGarages = filteredGarages.Where(x => filterGarage.districtsID.Any(d => d == x.DistrictsID)).ToList();
            }

            if (filterGarage.categoriesID is not null)
            {
                if (filterGarage.categoriesID.Count != 0)
                    filteredGarages = filteredGarages.Where(x => filterGarage.categoriesID.Any(i => x.CategoryGarages.Any(c => c.CategoryID == i))).ToList();
            }

            if (filterGarage.brandsID is not null)
            {
                if (filterGarage.brandsID.Count != 0)
                    filteredGarages = filteredGarages.Where(x => filterGarage.brandsID.Any(i => x.GarageBrands.Any(c => c.BrandID == i))).ToList();
            }

            if (filterGarage.provinceID is null && filterGarage.districtsID is null &&
                filterGarage.categoriesID is null && filterGarage.brandsID is null)
            {
                filteredGarages = garageRepository.GetGarages();
            }

            return Ok(filteredGarages);
        }

        //[HttpPost]
        //public IActionResult GetGarageAround([FromBody] FindGarageAroundDTO findGarageAroundDTO)
        //{
        //    try
        //    {
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        #endregion
        #region Private
        private UserInfor GetUserFromToken()
        {
            var jsonUser = User.FindFirstValue("user");
            var user = JsonConvert.DeserializeObject<UserInfor>(jsonUser);
            return user;
        }
        #endregion
    }
}