using Microsoft.AspNetCore.Mvc;
using SmartMeterService.Services;


namespace SmartMeterService.Controllers
{
	[Route("api/dlms")]
	[ApiController]
	public class DlmsController : ControllerBase
	{
		private readonly DlmsTcpService _dlmsService;

		public DlmsController(DlmsTcpService dlmsService)
		{
			_dlmsService = dlmsService;
		}

		[HttpGet("connect")]
		public async Task<IActionResult> ConnectToMeter()
		{
			var result = await _dlmsService.ConnectToMeterAsync();
			return Ok(result);
		}
	}
}