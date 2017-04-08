using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lily.Models.Connection
{
	/// <summary>
	/// 音声認識結果
	/// </summary>
	class RecognitionResult
	{
		/// <summary>
		/// メッセージのID
		/// </summary>
		[JsonProperty("id")]
		public int Id { get; set; }

		/// <summary>
		/// メッセージのテキスト
		/// </summary>
		[JsonProperty("text")]
		public string Text { get; set; }
	}
}
