using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lily.Models.Message
{
	abstract class MessageBase<MT> : IMessage, INotifyPropertyChanged where MT : MessageBase<MT>
	{
		/// <summary>
		/// メッセージのID
		/// </summary>
		public int Id { get; }

		public MessageBase(int id)
		{
			this.Id = id;
		}

		/// <summary>
		/// 指定したオブジェクトからデータをコピーする
		/// </summary>
		/// <param name="other">コピー元のメッセージ</param>
		public abstract void CopyFrom(MT other);

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
