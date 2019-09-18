using System;
using System.Windows.Input;

namespace Industrious.Mvvm
{
	/// <summary>
	///  A general purpose, platform agnostic implementation of <see cref="ICommand"/>.
	/// </summary>
	/// <example>
	///  <code>
	///   public class Example
	///   {
	///       public Example()
	///       {
	///           MyCommand = new Command&lt;String&gt;(value =&gt;
	///           {
	///               Console.WriteLine($"MyCommand fired, value is {value}");
	///           }
	///       }
	///
	///       // Call like `example.MyCommand.Execute("value")`
	///       public ICommand MyCommand { get; }
	///   }
	///  </code>
	/// </example>
	public class Command : ICommand
	{
		readonly Action _execute;
		readonly Func<Boolean> _canExecute;


		public Command(Action execute)
		{
			_execute = execute ?? throw new ArgumentNullException(nameof(execute));
		}


		public Command(Action execute, Func<Boolean> canExecute)
			: this(execute)
		{
			_canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
		}


		public Boolean CanExecute(Object parameter)
		{
			return (_canExecute?.Invoke() ?? true);
		}


		public event EventHandler CanExecuteChanged;


		public void Execute(Object parameter)
		{
			_execute();
		}


		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}



	public class Command<T> : ICommand
	{
		readonly Func<T, Boolean> _canExecute;
		readonly Action<T> _execute;


		public Command(Action<T> execute)
		{
			_execute = execute ?? throw new ArgumentException(nameof(execute));
		}


		public Command(Action<T> execute, Func<T, Boolean> canExecute)
			: this(execute)
		{
			_canExecute = canExecute ?? throw new ArgumentException(nameof(canExecute));
		}


		public Boolean CanExecute(Object parameter)
		{
			return (_canExecute == null) || _canExecute((T)parameter);
		}


		public event EventHandler CanExecuteChanged;


		public void Execute(Object parameter)
		{
			_execute((T)parameter);
		}


		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
