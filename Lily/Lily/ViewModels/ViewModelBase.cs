using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lily.ViewModels
{
	abstract class ViewModelBase : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		protected void RaisePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName);
		}

		#endregion
	}
}
