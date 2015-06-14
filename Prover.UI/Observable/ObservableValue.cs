using System.Collections.Generic;
using System.ComponentModel;

namespace Prover.UI.Observable
{
	public class ObservableValue<T> : INotifyPropertyChanged
	{
		T _valueField;

		public T Value
		{
			get
			{
				return _valueField;
			}
			set
			{
				if (!EqualityComparer<T>.Default.Equals(_valueField, value))
				{
					_valueField = value;
					OnPropertyChanged("Value");
				}
			}
		}

		public ObservableValue(T value = default(T))
		{
			Value = value;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
