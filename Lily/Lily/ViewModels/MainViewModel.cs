﻿using Lily.Common;
using Lily.Models;
using Lily.Models.Connection;
using Lily.Models.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lily.ViewModels
{
	class MainViewModel : ViewModelBase
	{
		/// <summary>
		/// Javaとの接続
		/// </summary>
		private readonly JavaServerConnector connector = new JavaServerConnector();

		/// <summary>
		/// 音声
		/// </summary>
		private readonly SoundModel sound = new SoundModel();

		/// <summary>
		/// すべてのメッセージ
		/// </summary>
		public MessageCollection Messages { get; } = new MessageCollection();

		/// <summary>
		/// 音量
		/// </summary>
		public float Volume => this.sound.Volume;

		public MainViewModel()
		{
			// PropertyChangedをつなげる
			this.sound.PropertyChanged += this.RaisePropertyChanged;

			// ObservableとObserverをつなげる
			this.connector.MessageObservable.Subscribe(this.Messages);
			this.connector.SoundObservable.Subscribe(this.sound);

			// イベントをつなげる
			this.sound.RecognizeStartRequested += this.connector.Sender.Requested_StartRecognize;
			this.sound.RecognizeStopRequested += this.connector.Sender.Requested_StopRecognize;
		}

		/// <summary>
		/// 録音する・しないを切り替える
		/// </summary>
		public RelayCommand ToggleRecognizeCommand
		{
			get
			{
				return this._toggleRecognizeCommand = this._toggleRecognizeCommand ?? new RelayCommand(() =>
				{
					this.sound.ToggleRecognize();
				});
			}
		}
		private RelayCommand _toggleRecognizeCommand;

	}
}