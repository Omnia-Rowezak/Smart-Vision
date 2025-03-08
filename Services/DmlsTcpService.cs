using System;
using System.Net.Sockets;
using Gurux.DLMS;
using Gurux.Net;

namespace SmartMeterService.Services
{
	public class DlmsTcpService
	{
		private readonly string _host;
		private readonly int _port;

		/*
			Represents the DLMS/COSEM protocol handler for communication with the smart meter.
			Handles authentication, encryption, and data interpretation.
			Used to send and receive DLMS commands to interact with the meter.

			private GXDLMSClient _client;

		 */

		public DlmsTcpService(string host, int port)
		{
			_host = host;
			_port = port;

		}

		//need await
		public async Task<string> ConnectToMeterAsync()
		{
			try
			{
				using (GXNet connection = new GXNet(NetworkType.Tcp, _host, _port))
				{
					connection.Open();
					Console.WriteLine("TCP connection established.");

					// Perform DLMS communication here

					connection.Close();
					Console.WriteLine("Connection closed.");
				}

				return "Connection successful";
			}
			catch (Exception ex)
			{
				return $"Error: {ex.Message}";
			}
		}
	}
}