using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Industrious.Mvvm
{
	/// <summary>
	///  Make it a little easier to create <see cref="INotifyPropertyChanged"/>-enabled classes.
	/// </summary>
	/// <example>
	///  <code>
	///   public class MyClass : NotifyPropertyChanged
	///   {
	///       private Int32 _someValue;
	///
	///       public Int32 SomeValue
	///       {
	///           get => _someValue;
	///           set => SetAndRaiseIfChanged(ref _someValue, value);
	///       }
	///   }
	///  </code>
	/// </example>
	public class NotifyPropertyChanged : INotifyPropertyChanging, INotifyPropertyChanged
	{
		public event PropertyChangingEventHandler PropertyChanging;

		public event PropertyChangedEventHandler PropertyChanged;


		protected void RaisePropertyChanging([CallerMemberName] String propertyName = "")
		{
			PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
		}


		protected void RaisePropertyChanged([CallerMemberName] String propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}


		protected void SetAndRaiseIfChanged<T>(ref T currentValue, T newValue, [CallerMemberName] String propertyName = "")
		{
			if (!EqualityComparer<T>.Default.Equals(currentValue, newValue))
			{
				RaisePropertyChanging(propertyName);
				currentValue = newValue;
				RaisePropertyChanged(propertyName);
			}
		}
	}
}
