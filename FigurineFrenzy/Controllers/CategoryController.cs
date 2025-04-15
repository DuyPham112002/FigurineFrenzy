using DBAccess.Entites;
using FigurineFrenzeyViewModel.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.CategoryService;
using Service.TokenService;
using System.Runtime.CompilerServices;

namespace FigurineFrenzy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ITokenService _token;
        private readonly ICategoryService _category;

        public CategoryController(ITokenService token, ICategoryService category)
        {
            _token = token;
            _category = category;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CategoryViewModel categoryView)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "Admin")
                    {
                        if (ModelState.IsValid)
                        {
                            var createCate = await _category.CreateAsync(categoryView);
                            if (createCate == Service.Enum.RESPONSECODE.OK)
                            {
                                return Ok("Create Category Success");
                            }
                            else return StatusCode(500);
                        }
                        else return BadRequest("Model is Invalid, Please check again");
                    }
                    else return Unauthorized();
                }
                return Unauthorized();
            }
            return Unauthorized();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetById")]
        public async Task<IActionResult> Get(string categoryId)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "Admin")
                    {
                        var cateInfo = await _category.GetAsync(categoryId);
                        if (cateInfo != null)
                        {
                            return Ok(cateInfo);
                        }
                        else return StatusCode(500);
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "Admin")
                    {
                        List<Category> listCate = await _category.GetAllAsync();
                        if (listCate != null)
                        {
                            return Ok(listCate);
                        }
                        else return StatusCode(500);
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }



        [Authorize(Roles = "User")]
        [HttpGet("Activation")]
        public async Task<IActionResult> GetAllActive()
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "User")
                    {
                        List<Category> listCate = await _category.GetAllActiveAsync();
                        if (listCate != null)
                        {
                            return Ok(listCate);
                        }
                        else return StatusCode(500);
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Dissability")]
        public async Task<IActionResult> Disability(string Id)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "Admin")
                    {
                        var isExistCategory = await _category.GetAsync(Id);
                        if (isExistCategory != null)
                        {
                            var deleteCategory = await _category.DissableAsync(Id);
                            if (deleteCategory == Service.Enum.RESPONSECODE.OK)
                            {
                                return Ok("Delete Success");
                            }
                            else return StatusCode(500);
                        }
                        else return StatusCode(500);
                    }
                    return Unauthorized();
                }
                return Unauthorized();
            }
            return Unauthorized();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Active")]
        public async Task<IActionResult> Active(string Id)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "Admin")
                    {
                        var isExistCategory = await _category.GetAsync(Id);
                        if (isExistCategory != null)
                        {
                            var active = await _category.ActivateAsync(Id);
                            if (active == Service.Enum.RESPONSECODE.OK)
                            {
                                return Ok("Active Success");
                            }
                            else return StatusCode(500);
                        }
                        else return StatusCode(500);
                    }
                    return Unauthorized();
                }
                return Unauthorized();
            }
            return Unauthorized();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Update")]
        public async Task<IActionResult> Update(string Id, CategoryViewModel categoryView)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "Admin")
                    {
                        var isExistCategory = await _category.GetAsync(Id);
                        if (isExistCategory != null)
                        {
                            var updateCategory = await _category.UpdateAsync(Id, categoryView);
                            if (updateCategory == Service.Enum.RESPONSECODE.OK)
                            {
                                return Ok(updateCategory);
                            }
                            else return StatusCode(500);
                        }
                        else return StatusCode(500);
                    }
                    else return Unauthorized();
                }
                else Unauthorized();
            }
            return Unauthorized();
        }
    }
}
