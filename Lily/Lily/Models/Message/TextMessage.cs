using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lily.Models.Message
{
	class TextMessage : MessageBase<TextMessage>
	{
		/// <summary>
		/// メッセージのテキスト
		/// </summary>
		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				if (this._text != value)
				{
					this._text = value;
					this.OnPropertyChanged();
				}
			}
		}
		private string _text;

		public TextMessage(int id) : base(id)
		{
		}

		public override void CopyFrom(TextMessage other)
		{
			this.Text = other.Text;
		}
	}
}
