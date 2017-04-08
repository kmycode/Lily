using Lily.Models.Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lily.Models.Connection
{
	class JavaServerConnector : IDisposable
	{
		private static readonly string PipeName = "lily_speech";
		private static readonly string JarName = @".\target\JavaSpeechServer-1.0-SNAPSHOT-jar-with-dependencies.jar";
		private static readonly string JarCommandOptions = @"-Xbootclasspath/p:.\jars\alpn-boot-8.1.9.v20160720.jar " +
														   @"-javaagent:.\jars\jetty-alpn-agent-2.0.6.jar";

		private readonly Process process;
		private readonly JavaServerReceiver receiver;

		public IObservable<IMessage> MessageObservable => this.receiver;

		public JavaServerConnector()
		{
			this.receiver = new JavaServerReceiver(PipeName);

			var processInfo = new ProcessStartInfo
			{
				FileName = "java",
				Arguments = JarCommandOptions + " -cp " + JarName + " net.kmycode.javaspeechserver.JavaSpeechServer",
				WorkingDirectory = @"..\..\..\..\JavaSpeechServer",
			};
			this.process = Process.Start(processInfo);
		}

		public void Dispose()
		{
			this.receiver.Dispose();
			this.process.Kill();
			GC.SuppressFinalize(this);
		}

		~JavaServerConnector()
		{
			this.Dispose();
		}

		private void CreatePipe()
		{
			CreateNamedPipe(@"\\.\pipe\" + PipeName, 3, 0, 255, 4096, 4096, 30000, IntPtr.Zero);
		}

		[DllImport("kernel32")]
		static extern IntPtr CreateNamedPipe(
			string lpName,
			UInt32 dwOpenMode,
			UInt32 dwPipeMode,
			UInt32 nMaxInstances,
			UInt32 nOutBufferSize,
			UInt32 nInBufferSize,
			UInt32 nDefaultTimeOut,
			IntPtr lpSecurityAttributes
		);
	}
}
