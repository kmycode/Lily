using Lily.Models.Connection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lily.Models
{
	class SoundModel : INotifyPropertyChanged, IObserver<SoundInformation>
	{
		/// <summary>
		/// 音量
		/// </summary>
		public float Volume
		{
			get
			{
				return this._volume;
			}
			set
			{
				if (this._volume != value)
				{
					this._volume = value;
					this.OnPropertyChanged();
				}
			}
		}
		private float _volume;

		/// <summary>
		/// 音声認識するかどうか
		/// </summary>
		public bool IsRecognize
		{
			get
			{
				return this._isRecognize;
			}
			set
			{
				if (this._isRecognize != value)
				{
					this._isRecognize = value;
					this.OnPropertyChanged();
				}
			}
		}
		private bool _isRecognize;

		/// <summary>
		/// 音声認識するかどうかを切り替える
		/// </summary>
		public void ToggleRecognize()
		{
			if (this.IsRecognize)
			{
				this.IsRecognize = false;
				this.RecognizeStopRequested?.Invoke(this, new EventArgs());
			}
			else
			{
				this.IsRecognize = true;
				this.RecognizeStartRequested?.Invoke(this, new EventArgs());
			}
		}
		
		#region IObserver

		public void OnNext(SoundInformation value)
		{
			this.Volume = value.Volume;
		}

		public void OnError(Exception error)
		{
			throw new NotImplementedException();
		}

		public void OnCompleted()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region イベント

		public event EventHandler RecognizeStartRequested;
		public event EventHandler RecognizeStopRequested;

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
