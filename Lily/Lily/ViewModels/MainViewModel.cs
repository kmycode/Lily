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
		/// すべてのメッセージ
		/// </summary>
		public MessageCollection Messages { get; } = new MessageCollection();

		public MainViewModel()
		{
			// ObservableとObserverをつなげる
			this.connector.MessageObservable.Subscribe(this.Messages);
		}
	}
}
