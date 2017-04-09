using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Lily.Common
{
	class RelayCommand : ICommand
	{
		private Action action;

		public RelayCommand(Action action)
		{
			this.action = action;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter) => true;

		public void Execute(object parameter)
		{
			this.action();
		}
	}
}
