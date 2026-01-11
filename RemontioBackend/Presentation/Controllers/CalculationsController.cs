using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.CalculationsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalculationsController : ControllerBase
    {
        private readonly ICalculatorService _calculatorService;

        public CalculationsController(ICalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
        }

        [Authorize]
        [HttpPost("create-calculation")]
        public async Task<ActionResult<bool>> CreateCalculation(CreateCalculationDTO calculationDTO, string roomId)
        {
            try
            {
                var result = await _calculatorService.CreateCalculationAsync(calculationDTO, roomId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-calculation-by-id")]
        public async Task<ActionResult<CalculationDataDTO>> GetCalculationById(string calculationId)
        {
            try
            {
                var result = await _calculatorService.GetCalculationAsync(calculationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-calculation-list")]
        public async Task<ActionResult<List<CalculationDataDTO>>> GetCalculationList()
        {
            try
            {
                var result = await _calculatorService.GetAllCalculationsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-calculation-list-by-user-id")]
        public async Task<ActionResult<List<CalculationDataDTO>>> GetCalculationListByUserId(string userId)
        {
            try
            {
                var result = await _calculatorService.GetAllCalculationsByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-calculation-list-by-project-id")]
        public async Task<ActionResult<List<CalculationDataDTO>>> GetCalculationListByProjectId(string projectId)
        {
            try
            {
                var result = await _calculatorService.GetAllCalculationsByProjectIdAsync(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-calculation-list-by-room-id")]
        public async Task<ActionResult<List<CalculationDataDTO>>> GetCalculationListByRoomId(string roomId)
        {
            try
            {
                var result = await _calculatorService.GetAllCalculationsByRoomIdAsync(roomId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("delete-calculation")]
        public async Task<ActionResult<bool>> DeleteCalculation(string calculationId)
        {
            try
            {
                var result = await _calculatorService.DeleteCalculationAsync(calculationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("edit-calculation")]
        public async Task<ActionResult<bool>> EditCalculation(CalculationDataDTO calculationDTO)
        {
            try
            {
                var result = await _calculatorService.UpdateCalculationAsync(calculationDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("edit-calculation-type")]
        public async Task<ActionResult<bool>> EditCalculationType(string calculationId, string type)
        {
            try
            {
                var result = await _calculatorService.ChangeCalculationTypeAsync(calculationId, type);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
