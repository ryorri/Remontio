using Application.Interfaces.ServiceInterfaces;
using Application.Objects.DTOs.AlertsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        [Authorize]
        [HttpPost("create-alert")]
        public async Task<ActionResult<bool>> CreateAlert(CreateAlertDTO alert)
        {
            try
            {
                var result = await _alertService.CreateAlertAsync(alert);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-alert-by-id")]
        public async Task<ActionResult<AlertDataDTO>> GetAlertById(string alertId)
        {
            try
            {
                var result = await _alertService.GetAlertByIdAsync(alertId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-alert-list")]
        public async Task<ActionResult<List<AlertDataDTO>>> GetAlertList()
        {
            try
            {
                var result = await _alertService.GetAllAlertsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-alert-list-by-user-id")]
        public async Task<ActionResult<List<AlertDataDTO>>> GetAlertListByUserId(string userId)
        {
            try
            {
                var result = await _alertService.GetAlertsByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("delete-alert")]
        public async Task<ActionResult<bool>> DeleteAlert(string alertId)
        {
            try
            {
                var result = await _alertService.DeleteAlertAsync(alertId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("edit-alert")]
        public async Task<ActionResult<bool>> EditAlert(AlertDataDTO alertDTO)
        {
            try
            {
                var result = await _alertService.UpdateAlertAsync(alertDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("edit-alert-status")]
        public async Task<ActionResult<bool>> EditAlertStatus(string alertId, string status)
        {
            try
            {
                var result = await _alertService.ChangeStatusAsync(alertId, status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("edit-alert-priority")]
        public async Task<ActionResult<bool>> EditAlertPriority(string alertId, string priority)
        {
            try
            {
                var result = await _alertService.ChangePriorityAsync(alertId, priority);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
