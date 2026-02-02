using CreditCardManagementApp.DTOS;
using CreditCardManagementApp.Services;
using CreditCardManagementApp.Helpers;
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

        [HttpGet("generate")]
        public IActionResult GenerateCardNumber([FromQuery] string cardType = "Visa")
        {
            try
            {
                // Valider le type de carte
                if (!new[] { "Visa", "Mastercard", "Amex" }.Contains(cardType))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid card type. Must be Visa, Mastercard, or Amex."
                    });
                }

                // Générer un numéro valide
                string cardNumber = CardNumberGenerator.GenerateCardNumber(cardType);

                return Ok(new
                {
                    success = true,
                    cardNumber = cardNumber,
                    cardType = cardType,
                    message = "Card number generated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"An error occurred while generating card number: {ex.Message}"
                });
            }
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
                        Message = "This card number is already in use. Please use a different card number.",
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

                var response = await _creditCardService.UpdateCreditCardAsync(id, dto, userId);

                if (response == null)
                {
                    return BadRequest(new ApiResponse<CreditCardResponseDTO>
                    {
                        Success = false,
                        Message = "Credit card not found or update failed.",
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

        [HttpGet]
        public async Task<IActionResult> GetCreditCards([FromQuery] bool? isActive = null, [FromQuery] int? userId = null)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int authenticatedUserId))
                {
                    return Unauthorized(new ApiResponse<CreditCardListDTO>
                    {
                        Success = false,
                        Message = "Invalid user authentication",
                        Data = null
                    });
                }

                if (userId.HasValue && userId.Value != authenticatedUserId)
                {
                    return BadRequest(new ApiResponse<CreditCardListDTO>
                    {
                        Success = false,
                        Message = "Invalid userId filter. You can only retrieve your own credit cards.",
                        Data = null
                    });
                }

                var response = await _creditCardService.GetCreditCardsAsync(authenticatedUserId, isActive);

                return Ok(new ApiResponse<CreditCardListDTO>
                {
                    Success = true,
                    Message = response.Message,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CreditCardListDTO>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving the credit cards: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCreditCards([FromQuery] string? cardType = null, [FromQuery] string? cardNumber = null)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new ApiResponse<CreditCardListDTO>
                    {
                        Success = false,
                        Message = "Invalid user authentication",
                        Data = null
                    });
                }

                if (string.IsNullOrWhiteSpace(cardType) && string.IsNullOrWhiteSpace(cardNumber))
                {
                    return BadRequest(new ApiResponse<CreditCardListDTO>
                    {
                        Success = false,
                        Message = "Please provide at least one search criteria (cardType or cardNumber).",
                        Data = null
                    });
                }

                var response = await _creditCardService.SearchCreditCardsAsync(userId, cardType, cardNumber);

                return Ok(new ApiResponse<CreditCardListDTO>
                {
                    Success = true,
                    Message = response.Message,
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CreditCardListDTO>
                {
                    Success = false,
                    Message = $"An error occurred while searching credit cards: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCreditCardById(int id)
        {
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

                var response = await _creditCardService.GetCreditCardByIdAsync(id, userId);

                if (response == null)
                {
                    return BadRequest(new ApiResponse<CreditCardResponseDTO>
                    {
                        Success = false,
                        Message = "Credit card not found or you don't have access to it.",
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
                    Message = $"An error occurred while retrieving the credit card: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCreditCard(int id, [FromBody] DeleteCreditCardDTO dto)
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

                if (id != dto.Id)
                {
                    return BadRequest(new ApiResponse<CreditCardResponseDTO>
                    {
                        Success = false,
                        Message = "ID mismatch between URL and request body.",
                        Data = null
                    });
                }

                var response = await _creditCardService.DeleteCreditCardAsync(dto, userId);

                if (response == null)
                {
                    return BadRequest(new ApiResponse<CreditCardResponseDTO>
                    {
                        Success = false,
                        Message = "Credit card delete failed. Ensure the card exists, belongs to you, and has a current balance of 0.",
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
                    Message = $"An error occurred while deleting the credit card: {ex.Message}",
                    Data = null
                });
            }
        }
    }
}
