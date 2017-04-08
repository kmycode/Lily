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

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
