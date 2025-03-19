using Microsoft.AspNetCore.Mvc;
using SmartMeterService.Services;
using System;
using System.Net.Sockets;
using Gurux.DLMS;
using Gurux.DLMS.Objects;
using Gurux.DLMS.Enums;
using Gurux.Net;

//namespace SmartMeterService.Controllers
//{
//	[Route("api/dlms")]
//	[ApiController]
//	public class DlmsController : ControllerBase
//	{
//		private readonly DlmsTcpService _dlmsService;
//		//private object _client;

//		public DlmsController(DlmsTcpService dlmsService)
//		{
//			_dlmsService = dlmsService;
//		}


//		[HttpGet("connect")]
//		public async Task<IActionResult> ConnectToMeter()
//		{
//			var result =  _dlmsService.ConnedtTrail();
//			return Ok(new{ message=result });
//		}

//		//[HttpGet("read")]
//		//public async Task<IActionResult> ReadMeterData()
//		//{
//		//	var result = _dlmsService.ReadClock();
//		//	return Ok(result);
//		//}



//	}
//}





namespace SmartMeterAPI.Controllers
{
	[ApiController]
	[Route("api/dlms")]
	public class DlmsController : ControllerBase
	{
		private string meterIp = "82.129.132.26"; 
		private int meterPort = 4059; // DLMS default TCP port
		private GXDLMSClient dlmsClient;
		private TcpClient tcpClient;

		public DlmsController()
		{
			dlmsClient = new GXDLMSClient(true, 11, 1, Authentication.Low, "00000000", InterfaceType.WRAPPER);
			//	true,                 // Logical Name referencing
			//	InterfaceType.HDLC,   // HDLC for TCP
			//	Authentication.None,  // Change authentication as needed
			//	null                  // Password if required
			//);
		}

		// CONNECT TO THE METER
		[HttpPost("connect")]
		public IActionResult Connect()
		{
			try
			{
				GXNet tcp = new GXNet(NetworkType.Tcp, "82.129.132.26", 4059); 
				tcp.Open();

				//tcpClient = new TcpClient(meterIp, meterPort);
				//dlmsClient.InitializeConnection();
				return Ok("Connected to DLMS meter successfully.");
			}
			catch (Exception ex)
			{
				return BadRequest($"Error connecting to meter: {ex.Message}");
			}
		}

		////LIST ALL AVAILABLE OBIS CODES
		//[HttpGet("list-obis")]
		//public IActionResult ListObisCodes()
		//{
		//	try
		//	{
		//		if (tcpClient == null)
		//		{
		//			return BadRequest("Error: Not connected to the meter. Please connect first.");
		//		}

		//		List<object> obisList = new List<object>();

		//		var objects = dlmsClient.GetObjectsRequest();

		//		foreach (var obj in objects)
		//		{
		//			obisList.Add(obj);
		//			//{
						
		//			//	OBIS = obj.LogicalName,
		//			//	Type = obj.ObjectType.ToString()
		//			//});
		//		}

		//		return Ok(new { Message = "Available OBIS Codes", OBIS_Codes = obisList });
		//	}
		//	catch (Exception ex)
		//	{
		//		return BadRequest($"Error retrieving OBIS codes: {ex.Message}");
		//	}
		//}




		// READ ACTIVE ENERGY
		[HttpGet("active-energy")]
		public IActionResult GetActiveEnergy()
		{
			return ReadObis("1.0.1.8.1.255", "Active Energy"); //1-0:1.8.0.255  "1.0.1.8.1.255" voltage:"1.0.32.7.0.225"
		}

		//READ REACTIVE ENERGY 
		[HttpGet("reactive-energy")]
		public IActionResult GetReactiveEnergy()
		{
			return ReadObis("1.0.3.8.0.255", "Reactive Energy");
		}

		//READ METER DATE/TIME
		[HttpGet("meter-datetime")]
		public IActionResult GetMeterDateTime()
		{
			return ReadObis("0.0.1.0.0.255", "Meter Date/Time"); //'0-0:1.0.0.255
		}

		// DISCONNECT FROM METER
		[HttpPost("disconnect")]
		public IActionResult Disconnect()
		{
			try
			{
				tcpClient?.Close();
				tcpClient?.Dispose();
				return Ok("Disconnected from DLMS meter.");
			}
			catch (Exception ex)
			{
				return BadRequest($"Error disconnecting: {ex.Message}");
			}
		}

		// CUSTOME METHOD TO READ OBIS DATA
		private IActionResult ReadObis(string obisCode, string description)
		{
			try
			{



				GXDLMSObject obj = new GXDLMSRegister(obisCode);
				Console.WriteLine("nothing is here" + obj);

				//GXDLMSObject obj = dlmsClient.Objects.FindByLN(0, obisCode);
				Console.WriteLine("nothing is here"+obj);
				if (obj == null)
					return NotFound($"{description} (OBIS {obisCode}) not found.");


				object value = dlmsClient.Read(obj, 2); // Attribute 2 = value field
				return Ok(new { Description = description, OBIS = obisCode, Value = value });
			}
			catch (Exception ex)
			{
				return BadRequest($"Error reading {description}: {ex.Message}");
			}
		}
	}
}
