using System;

using Xunit;

namespace Industrious.Mvvm.Tests
{
	public class NotifyPropertyChangedTests
	{
		class TestObject : NotifyPropertyChanged
		{
			Int32 _value;

			public TestObject(Int32 value)
			{
				_value = value;
			}

			public Int32 Value
			{
				get => _value;
				set => SetAndRaiseIfChanged(ref _value, value);
			}
		}


		[Fact]
		public void SetAndRaiseIfChanged_RaisesPropertyChanging_OnValueChanged()
		{
			var sut = new TestObject(0);

			var eventWasCalled = false;
			sut.PropertyChanging += (sender, e) => eventWasCalled = true;

			sut.Value = 8;

			Assert.True(eventWasCalled, "PropertyChanging event was never called");
		}


		[Fact]
		public void SetAndRaiseIfChanged_RaisesPropertyChanged_OnValueChanged()
		{
			var sut = new TestObject(0);

			var eventWasCalled = false;
			sut.PropertyChanged += (sender, e) => eventWasCalled = true;

			sut.Value = 8;

			Assert.True(eventWasCalled, "PropertyChanged event was never called");
		}


		[Fact]
		public void SetAndRaiseIfChanged_DoesNotRaisePropertyChanged_OnValueNotChanged()
		{
			var sut = new TestObject(0);

			var eventWasCalled = false;
			sut.PropertyChanged += (sender, e) => eventWasCalled = true;

			sut.Value = 0;

			Assert.False(eventWasCalled, "PropertyChanged event should not have been called");
		}


		[Fact]
		public void SetAndRaiseIfChanged_DoesNotRaisePropertyChanging_OnValueNotChanged()
		{
			var sut = new TestObject(0);

			var eventWasCalled = false;
			sut.PropertyChanging += (sender, e) => eventWasCalled = true;

			sut.Value = 0;

			Assert.False(eventWasCalled, "PropertyChanging event should not have been called");
		}
	}
}
