using CreditCardManagementApp.DTOS;
using CreditCardManagementApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CreditCardManagementApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CreditCardController : ControllerBase
    {
        private readonly ICreditCardService _creditCardService;

        public CreditCardController(ICreditCardService creditCardService)
        {
            _creditCardService = creditCardService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCreditCard([FromBody] CreateCreditCardDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<CreditCardResponseDTO>
                {
                    Success = false,
                    Message = "Invalid input data. Please check your input fields.",
                    Data = null
                });
            }
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new ApiResponse<CreditCardResponseDTO>
                    {
                        Success = false,
                        Message = "Invalid user authentication",
                        Data = null
                    });
                }

                var response = await _creditCardService.CreateCreditCardAsync(dto, userId);

                if (response == null)
                {
                    return BadRequest(new ApiResponse<CreditCardResponseDTO>
                    {
                        Success = false,
                        Message = "Credit card creation failed. Please check: card number uniqueness, Luhn validation, expiration date (future, max 10 years), credit limit > 0, balance within limit, card type/prefix match, active card limit (max 5), total card limit (max 10), and ensure card number is not blacklisted.",
                        Data = null
                    });
                }

                return CreatedAtAction(
                    nameof(CreateCreditCard),
                    new { id = response.Id },
                    new ApiResponse<CreditCardResponseDTO>
                    {
                        Success = true,
                        Message = response.Message,
                        Data = response
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CreditCardResponseDTO>
                {
                    Success = false,
                    Message = $"An error occurred while creating the credit card: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCreditCard(int id, [FromBody] UpdateCreditCardDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<CreditCardResponseDTO>
                {
                    Success = false,
                    Message = "Invalid input data. Please check your input fields.",
                    Data = null
                });
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new ApiResponse<CreditCardResponseDTO>
                    {
                        Success = false,
                        Message = "Invalid user authentication",
                        Data = null
                    });
                }

                // Ensure the ID in the URL matches the ID in the DTO
                if (id != dto.Id)
                {
                    return BadRequest(new ApiResponse<CreditCardResponseDTO>
                    {
                        Success = false,
                        Message = "ID mismatch between URL and request body.",
                        Data = null
                    });
                }

                var response = await _creditCardService.UpdateCreditCardAsync(dto, userId);

                if (response == null)
                {
                    return BadRequest(new ApiResponse<CreditCardResponseDTO>
                    {
                        Success = false,
                        Message = "Credit card update failed. Please check: card ownership, card number uniqueness (except current record), Luhn validation, expiration date (future, max 10 years), credit limit > 0, balance within limit, card type/prefix match, and ensure card number is not blacklisted.",
                        Data = null
                    });
                }

                return Ok(new ApiResponse<CreditCardResponseDTO>
                {
                    Success = true,
                    Message = response.Message,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CreditCardResponseDTO>
                {
                    Success = false,
                    Message = $"An error occurred while updating the credit card: {ex.Message}",
                    Data = null
                });
            }
        }
    }
}
