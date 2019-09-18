using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

using Xunit;

namespace Industrious.Mvvm.Tests
{
	public class TranslatingObservableTests
	{
		private readonly ObservableCollection<Int32> _source;
		private readonly TranslatingObservable<Int32, String> _sut;


		public TranslatingObservableTests()
		{
			_source = new ObservableCollection<Int32>(new Int32[] { 1, 2, 3 });
			_sut = new TranslatingObservable<Int32, String>(_source, i => i.ToString());
		}


		[Fact]
		public void Constructor_InitializesCollection_FromSourceObservable()
		{
			Assert.Equal(new String[] { "1", "2", "3" }, _sut);
		}


		[Fact]
		public void Add_AddsItemToCollection()
		{
			_source.Add(4);
			Assert.Equal(new String[] { "1", "2", "3", "4" }, _sut);
		}


		[Fact]
		public void Add_RaisesCollectionChanged()
		{
			NotifyCollectionChangedEventArgs changeEvent = null;
			_sut.CollectionChanged += (sender, e) => changeEvent = e;

			_source.Add(4);

			Assert.Equal(NotifyCollectionChangedAction.Add, changeEvent.Action);
			Assert.Equal(new String[] { "4" }, changeEvent.NewItems);
			Assert.Equal(3, changeEvent.NewStartingIndex);
		}


		[Fact]
		public void Add_RaisesCountChanged()
		{
			PropertyChangedEventArgs changeEvent = null;
			_sut.PropertyChanged += (sender, e) => changeEvent = e;

			_source.Add(4);

			Assert.Equal("Count", changeEvent.PropertyName);
		}


		[Fact]
		public void Clear_CallsDisposer()
		{
			var isDisposeCalled = false;
			var sut = new TranslatingObservable<Int32, String>(_source, i => i.ToString(), i => isDisposeCalled = true);
			_source.Clear();
			Assert.True(isDisposeCalled);
		}


		[Fact]
		public void Clear_RemovesAllItemsFromCollection()
		{
			_source.Clear();
			Assert.Empty(_sut);
		}


		[Fact]
		public void Clear_RaisesCollectionChanged()
		{
			NotifyCollectionChangedEventArgs changeEvent = null;
			_sut.CollectionChanged += (sender, e) => changeEvent = e;

			_source.Clear();

			Assert.Equal(NotifyCollectionChangedAction.Reset, changeEvent.Action);
			Assert.Null(changeEvent.NewItems);
			Assert.Equal(-1, changeEvent.NewStartingIndex);
		}


		[Fact]
		public void Clear_RaisesCountChanged()
		{
			PropertyChangedEventArgs changeEvent = null;
			_sut.PropertyChanged += (sender, e) => changeEvent = e;

			_source.Clear();

			Assert.Equal("Count", changeEvent.PropertyName);
		}


		[Fact]
		public void GetTranslatedValue_CanRetrieveFromSourceItem()
		{
			Assert.Equal("2", _sut.GetTranslatedValue(2));
		}


		[Fact]
		public void GetTranslatedValueOrNull_ReturnsNull_OnIndexOutOfRange()
		{
			Assert.Null(_sut.GetTranslatedValueOrNull(99));
		}


		[Fact]
		public void Indexer_ReturnsTranslatedItemAtIndex()
		{
			Assert.Equal("3", _sut[2]);
		}


		[Fact]
		public void Insert_AddsItemToCollection()
		{
			_source.Insert(1, 4);
			Assert.Equal(new String[] { "1", "4", "2", "3" }, _sut);
		}


		[Fact]
		public void Insert_RaisesCollectionChanged()
		{
			NotifyCollectionChangedEventArgs changeEvent = null;
			_sut.CollectionChanged += (sender, e) => changeEvent = e;

			_source.Insert(1, 4);

			Assert.Equal(NotifyCollectionChangedAction.Add, changeEvent.Action);
			Assert.Equal(new String[] { "4" }, changeEvent.NewItems);
			Assert.Equal(1, changeEvent.NewStartingIndex);
		}


		[Fact]
		public void Insert_RaisesCountChanged()
		{
			PropertyChangedEventArgs changeEvent = null;
			_sut.PropertyChanged += (sender, e) => changeEvent = e;

			_source.Insert(1, 4);

			Assert.Equal("Count", changeEvent.PropertyName);
		}


		[Fact]
		public void Move_MovesItemInCollection()
		{
			_source.Move(0, 1);
			Assert.Equal(new String[] { "2", "1", "3" }, _sut);
		}


		[Fact]
		public void Move_RaisesCollectionChanged()
		{
			NotifyCollectionChangedEventArgs changeEvent = null;
			_sut.CollectionChanged += (sender, e) => changeEvent = e;

			_source.Move(0, 1);

			Assert.Equal(NotifyCollectionChangedAction.Move, changeEvent.Action);
			Assert.Equal(new String[] { "1" }, changeEvent.NewItems);
			Assert.Equal(new String[] { "1" }, changeEvent.OldItems);
			Assert.Equal(0, changeEvent.OldStartingIndex);
			Assert.Equal(1, changeEvent.NewStartingIndex);
		}


		[Fact]
		public void Move_DoesNotRaiseCountChanged()
		{
			PropertyChangedEventArgs changeEvent = null;
			_sut.PropertyChanged += (sender, e) => changeEvent = e;

			_source.Move(0, 1);

			Assert.Null(changeEvent);
		}


		[Fact]
		public void Remove_CallsDisposer()
		{
			var isDisposeCalled = false;
			var sut = new TranslatingObservable<Int32, String>(_source, i => i.ToString(), i => isDisposeCalled = true);
			_source.Remove(2);
			Assert.True(isDisposeCalled);
		}


		[Fact]
		public void Remove_RemovesItemFromCollection()
		{
			_source.Remove(2);
			Assert.Equal(new String[] { "1", "3" }, _sut);
		}


		[Fact]
		public void Remove_RaisesCollectionChanged()
		{
			NotifyCollectionChangedEventArgs changeEvent = null;
			_sut.CollectionChanged += (sender, e) => changeEvent = e;

			_source.Remove(2);

			Assert.Equal(NotifyCollectionChangedAction.Remove, changeEvent.Action);
			Assert.Equal(new String[] { "2" }, changeEvent.OldItems);
			Assert.Equal(1, changeEvent.OldStartingIndex);
		}


		[Fact]
		public void Remove_RaisesCountChanged()
		{
			PropertyChangedEventArgs changeEvent = null;
			_sut.PropertyChanged += (sender, e) => changeEvent = e;

			_source.Remove(2);

			Assert.Equal("Count", changeEvent.PropertyName);
		}


		[Fact]
		public void Replace_CallsDisposer()
		{
			var isDisposeCalled = false;
			var sut = new TranslatingObservable<Int32, String>(_source, i => i.ToString(), i => isDisposeCalled = true);
			_source[1] = 9;
			Assert.True(isDisposeCalled);
		}


		[Fact]
		public void Replace_ReplacesItemInCollection()
		{
			_source[1] = 9;
			Assert.Equal(new String[] { "1", "9", "3" }, _sut);
		}


		[Fact]
		public void Replace_RaisesCollectionChanged()
		{
			NotifyCollectionChangedEventArgs changeEvent = null;
			_sut.CollectionChanged += (sender, e) => changeEvent = e;

			_source[1] = 9;

			Assert.Equal(NotifyCollectionChangedAction.Replace, changeEvent.Action);
			Assert.Equal(new String[] { "9" }, changeEvent.NewItems);
			Assert.Equal(new String[] { "2" }, changeEvent.OldItems);
			Assert.Equal(1, changeEvent.OldStartingIndex);
			Assert.Equal(1, changeEvent.NewStartingIndex);
		}


		[Fact]
		public void Replace_DoesNotRaiseCountChanged()
		{
			PropertyChangedEventArgs changeEvent = null;
			_sut.PropertyChanged += (sender, e) => changeEvent = e;

			_source[1] = 9;

			Assert.Null(changeEvent);
		}
	}
}
