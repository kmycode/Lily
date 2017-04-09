using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lily.Models.Connection
{
	class JavaServerSender
	{
		private readonly string pipeName;
		private readonly NamedPipeServerStream client;

		public JavaServerSender(string pipeName)
		{
			this.pipeName = pipeName;
			this.client = new NamedPipeServerStream(this.pipeName, PipeDirection.InOut, 32, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
			this.client.BeginWaitForConnection(this.client.EndWaitForConnection, null);
		}

		public void StartRecognize()
		{
			byte[] data = Encoding.Default.GetBytes("V{}\n");
			this.client.BeginWrite(data, 0, data.Length, this.client.EndWrite, null);
		}

		public void Requested_StartRecognize(object sender, EventArgs e)
		{
			this.StartRecognize();
		}

		public void StopRecognize()
		{
			byte[] data = Encoding.Default.GetBytes("v{}\n");
			this.client.BeginWrite(data, 0, data.Length, this.client.EndWrite, null);
		}

		public void Requested_StopRecognize(object sender, EventArgs e)
		{
			this.StopRecognize();
		}
	}
}
