using Lily.Models.Message;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Lily.Models.Connection
{
	/// <summary>
	/// Javaからのパイプ通信を受け取るクラス
	/// </summary>
	class JavaServerReceiver : IObservable<IMessage>, IDisposable
	{
		private readonly string pipeName;
		private readonly NamedPipeServerStream client;
		private readonly Subject<IMessage> messageSubject = new Subject<IMessage>();
		private bool isStopReceiveLoop;

		public JavaServerReceiver(string pipeName)
		{
			this.pipeName = pipeName;
			this.client = new NamedPipeServerStream(this.pipeName, PipeDirection.InOut, 32, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
			this.client.BeginWaitForConnection((r) => this.ReceiveLoopAsync(r), null);
		}

		public void Dispose()
		{
			this.isStopReceiveLoop = true;
			this.client.Dispose();
			GC.SuppressFinalize(this);
		}

		~JavaServerReceiver()
		{
			this.Dispose();
		}

		/// <summary>
		/// Javaからデータの受信を行うメインループを回す
		/// </summary>
		/// <returns></returns>
		private async Task ReceiveLoopAsync(IAsyncResult asyncResult)
		{
			this.client.EndWaitForConnection(asyncResult);

			this.isStopReceiveLoop = false;
			byte[] buffer = new byte[4096];

			string data;

			while (!this.isStopReceiveLoop)
			{
				try
				{
					var readBytes = this.client.Read(buffer, 0, buffer.Length);
					if (readBytes > 0)
					{
						data = Encoding.Default.GetString(buffer);
						this.ReadObject(data);

						int i = 0;
						while (buffer[i] != 0)
						{
							buffer[i] = 0;
							i++;
						}
					}
				}
				catch (Exception e)
				{

				}

				await Task.Delay(10);
			}
		}

		/// <summary>
		/// データからオブジェクトを読んで処理する
		/// </summary>
		/// <param name="data">データ。最初の１文字に識別子、それ以降はJSONである必要がある</param>
		private void ReadObject(string data)
		{
			var signature = data[0];
			var json = data.Substring(1);

			switch (signature)
			{
				case 'r':
					this.ReadRecognitionResult(json);
					break;
			}
		}

		/// <summary>
		/// データを、音声認識結果とみなして処理する
		/// </summary>
		/// <param name="json">JSON文字列</param>
		private void ReadRecognitionResult(string json)
		{
			var obj = JsonConvert.DeserializeObject<RecognitionResult>(json);
			var message = new TextMessage(obj.Id)
			{
				Text = obj.Text,
			};
			this.messageSubject.OnNext(message);
		}

		public IDisposable Subscribe(IObserver<IMessage> observer)
		{
			return ((IObservable<IMessage>)this.messageSubject).Subscribe(observer);
		}
	}
}
