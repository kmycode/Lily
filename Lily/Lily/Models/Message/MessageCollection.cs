using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Lily.Models.Message
{
	class MessageCollection : IReadOnlyCollection<IMessage>, INotifyCollectionChanged, IObserver<IMessage>
	{
		private readonly ObservableCollection<IMessage> messages = new ObservableCollection<IMessage>();
		private readonly Subject<IMessage> observer = new Subject<IMessage>();

		public MessageCollection()
		{
			BindingOperations.EnableCollectionSynchronization(this, new object());
		}

		/// <summary>
		/// メッセージを追加。同一IDのメッセージが存在する場合は上書き
		/// </summary>
		/// <param name="message">追加するメッセージ</param>
		public void Save(IMessage message)
		{
			var existingMessage = this.messages.SingleOrDefault(m => m.Id == message.Id);

			if (existingMessage == null)
			{
				this.messages.Add(message);
			}
			else
			{
				var index = this.messages.IndexOf(existingMessage);
				this.messages[index] = message;
			}
		}

		#region IObserver

		public void OnNext(IMessage value)
		{
			this.Save(value);
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

		#region INotifyCollectionChanged

		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add
			{
				this.messages.CollectionChanged += value;
			}
			remove
			{
				this.messages.CollectionChanged -= value;
			}
		}

		#endregion

		#region IReadOnlyCollection

		public int Count => this.messages.Count;

		public IEnumerator<IMessage> GetEnumerator()
		{
			return this.messages.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.messages.GetEnumerator();
		}

		#endregion
	}
}
