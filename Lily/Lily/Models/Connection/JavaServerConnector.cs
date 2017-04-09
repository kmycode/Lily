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
		private static readonly string PipeNameServer = "lily_speech_2";
		private static readonly string JarName = @".\target\JavaSpeechServer-1.0-SNAPSHOT-jar-with-dependencies.jar";
		private static readonly string JarCommandOptions = @"-Xbootclasspath/p:.\jars\alpn-boot-8.1.9.v20160720.jar " +
														   @"-javaagent:.\jars\jetty-alpn-agent-2.0.6.jar";

		private readonly Process process;
		private readonly JavaServerReceiver receiver;
		public JavaServerSender Sender { get; }

		public IObservable<IMessage> MessageObservable => this.receiver;
		public IObservable<SoundInformation> SoundObservable => this.receiver;

		public JavaServerConnector()
		{
			this.receiver = new JavaServerReceiver(PipeName);
			this.Sender = new JavaServerSender(PipeNameServer);

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
	}
}
