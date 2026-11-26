using AutoMapper;
using Mango.CouponAPI.Data;
using Mango.CouponAPI.Models;
using Mango.CouponAPI.Models.Dto;
using Mango.Services.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/coupons")]
[ApiController]
public class CouponAPIController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<CouponAPIController> _logger;

    public CouponAPIController(ApplicationDbContext context, IMapper mapper, ILogger<CouponAPIController> logger)
    {
        _db = context;
        _mapper = mapper;
        _logger = logger;
    }

    // GET: api/coupons
    [HttpGet]
    public async Task<IActionResult> GetAllCoupons()
    {
        var response = new ResponseDto();

        try
        {
            var coupons = await _db.Coupons.ToListAsync();
            response.Result = _mapper.Map<IEnumerable<CouponDto>>(coupons);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving coupons.");
            response.IsSuccess = false;
            response.ErrorMessage = "An error occurred while retrieving coupons.";
            return StatusCode(500, response);
        }
    }

    // GET: api/coupons/5
    [HttpGet("{Id:int}")]
    public async Task<IActionResult> GetCouponById(int Id)
    {
        var response = new ResponseDto();

        try
        {
            var coupon = await _db.Coupons.FindAsync(Id);
            if (coupon == null)
            {
                response.IsSuccess = false;
                response.ErrorMessage = "Coupon not found.";

                return NotFound(response);
            }
            response.Result = _mapper.Map<CouponDto>(coupon);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving coupon with ID {Id}.", Id);
            response.IsSuccess = false;
            response.ErrorMessage = "An error occurred while retrieving the coupon.";
            return StatusCode(500, response);
        }
    }

    // POST: api/coupons
    [HttpPost]
    public async Task<IActionResult> CreateCoupon([FromBody] CouponDto couponDto)
    {
        var response = new ResponseDto();

        try
        {
            if (couponDto.CouponId != 0)
            {
                response.IsSuccess = false;
                response.ErrorMessage = "CouponId must be zero when creating a coupon.";
                return BadRequest(response);
            }

            var coupon = _mapper.Map<Coupon>(couponDto);
            _db.Coupons.Add(coupon);
            await _db.SaveChangesAsync();
            response.Result = _mapper.Map<CouponDto>(coupon);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating coupon.");
            response.IsSuccess = false;
            response.ErrorMessage = "An error occurred while creating the coupon.";
            return StatusCode(500, response);
        }
    }

    // PUT: api/coupons
    [HttpPut]
    public async Task<IActionResult> UpdateCoupon(
        [FromBody] CouponDto couponDto)
    {
        var response = new ResponseDto();

        try
        {
            if (couponDto.CouponId <= 0)
            {
                response.IsSuccess = false;
                response.ErrorMessage = "Valid CouponId is required for update.";

                return BadRequest(response);
            }

            var existingCoupon = await _db.Coupons.FindAsync(couponDto.CouponId);

            if (existingCoupon == null)
            {
                response.IsSuccess = false;
                response.ErrorMessage = "Coupon not found.";
                return NotFound(response);
            }

            _mapper.Map(couponDto, existingCoupon);
            await _db.SaveChangesAsync();
            response.Result = _mapper.Map<CouponDto>(existingCoupon);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating coupon with ID {Id}.", couponDto.CouponId);
            response.IsSuccess = false;
            response.ErrorMessage = "An error occurred while updating the coupon.";
            return StatusCode(500, response);
        }
    }

    // DELETE: api/coupons/5
    [HttpDelete("{Id:int}")]
    public async Task<IActionResult> DeleteCoupon(int Id)
    {
        var response = new ResponseDto();

        try
        {
            var coupon = await _db.Coupons.FindAsync(Id);

            if (coupon == null)
            {
                response.IsSuccess = false;
                response.ErrorMessage = "Coupon not found.";
                return NotFound(response);
            }

            _db.Coupons.Remove(coupon);
            await _db.SaveChangesAsync();
            response.Result = "Coupon deleted successfully.";
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting coupon with ID {Id}.", Id);
            response.IsSuccess = false;
            response.ErrorMessage = "An error occurred while deleting the coupon.";
            return StatusCode(500, response);
        }
    }
}
