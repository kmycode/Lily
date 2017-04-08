using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lily.Models.Connection
{
	/// <summary>
	/// 音声の情報
	/// </summary>
	class SoundInformation
	{
		[JsonProperty("volume")]
		public float Volume { get; set; }
	}
}
