using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Industrious.Mvvm
{
	/// <summary>
	///  Translate the contents of an <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>
	///  to a different data type. Useful for converting a collection of model objects to a corresponding
	///  collection of view models.
	/// </summary>
	/// <typeparam name="Tin">
	///  The data type contained by the source collection.
	/// </typeparam>
	/// <typeparam name="Tout">
	///  The target data type of the translation.
	/// </typeparam>
	/// <example>
	///  <code>
	///   public class ToDoItem
	///   {
	///       /* ... */
	///   }
	///
	///   public class ItemViewModel
	///   {
	///       public ItemViewModel(ToDoItem item) { /* ... */ }
	///   }
	///   
	///   public class ItemCollectionViewModel
	///   {
	///       public ItemCollectionViewModel(ObservableCollection&lt;ToDoItem&gt; items)
	///       {
	///		      Items = new TranslatingObservable&lt;ToDoItem, ItemViewModel&gt;(items, item => new ItemViewModel(item));
	///       }
	///       
	///       public TranslatingObservable&lt;ToDoItem, ItemViewModel&gt; Items { get; }
	///  </code>
	/// </example>
	public class TranslatingObservable<Tin, Tout> : IReadOnlyList<Tout>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private List<Tout> _translatedItems;


		public TranslatingObservable(IList<Tin> source, Func<Tin, Tout> translate, Action<Tout> dispose = null)
		{
			Source = source;
			Translate = translate ?? throw new ArgumentNullException(nameof(translate));
			Dispose = dispose ?? DefaultDispose;

			_translatedItems = new List<Tout>();
			OnResetItems();

			// TODO: Find a way to enforce this in the method signature
			var observable = (source as INotifyCollectionChanged) ?? throw new ArgumentException("Observable must implement INotifyCollectionChanged");
			observable.CollectionChanged += OnCollectionChanged;
		}


		public Action<Tout> Dispose { get; }


		public Tout this[Int32 index] => _translatedItems[index];


		public Tout this[Tin index] => GetTranslatedValue(index);


		public int Count => _translatedItems.Count;


		public IList<Tin> Source { get; }


		public Func<Tin, Tout> Translate { get; }


		public event NotifyCollectionChangedEventHandler CollectionChanged;


		public event PropertyChangedEventHandler PropertyChanged;


		public IEnumerator<Tout> GetEnumerator()
		{
			return (_translatedItems.GetEnumerator());
		}


		IEnumerator IEnumerable.GetEnumerator()
		{
			return (_translatedItems.GetEnumerator());
		}


		/// <summary>
		///  Given an item from the source collection, return the corresponding item
		///  from the translated collection.
		/// </summary>
		/// <exception cref="KeyNotFoundException">
		///  The specified item was not found in the source collection.
		/// </exception>
		public Tout GetTranslatedValue(Tin index)
		{
			Int32 i = Source.IndexOf(index);

			if (i < 0)
				throw new KeyNotFoundException();

			return this[i];
		}


		/// <summary>
		///  Given an item from the source collection, return the corresponding item
		///  from the translated collection. If the item cannot be found in the source
		///  collection, return <c>null</c> instead.
		/// </summary>
		public Tout GetTranslatedValueOrNull(Tin index)
		{
			if (typeof(Tout).IsValueType)
				throw new ArgumentException("Tout must not be a value type");

			Int32 i = Source.IndexOf(index);

			if (i < 0)
				return default(Tout);

			return this[i];
		}


		private void OnCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
			case NotifyCollectionChangedAction.Add:
				OnAddItems(e.NewItems, e.NewStartingIndex);
				break;

			case NotifyCollectionChangedAction.Move:
				OnMoveItems(e.OldItems, e.OldStartingIndex, e.NewStartingIndex);
				break;

			case NotifyCollectionChangedAction.Remove:
				OnRemoveItems(e.OldItems, e.OldStartingIndex);
				break;

			case NotifyCollectionChangedAction.Replace:
				OnReplaceItems(e.NewItems, e.OldStartingIndex);
				break;

			case NotifyCollectionChangedAction.Reset:
				OnResetItems();
				break;
			}
		}


		private void DefaultDispose(Tout item)
		{
			(item as IDisposable)?.Dispose();
		}


		private void OnAddItems(IEnumerable items, Int32 startingIndex)
		{
			var itemsToAdd = AddItems(items, startingIndex);

			var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, itemsToAdd, startingIndex);
			CollectionChanged?.Invoke(this, e);
			PropertyChanged?.Invoke(this, EventArgsCache.CountPropertyChanged);
		}


		private List<Tout> AddItems(IEnumerable items, Int32 startingIndex)
		{
			var itemsToAdd = items
				.Cast<Tin>()
				.Select(item => Translate(item))
				.ToList<Tout>();

			_translatedItems.InsertRange(startingIndex, itemsToAdd);

			return (itemsToAdd);
		}


		private void OnMoveItems(IList items, Int32 oldIndex, Int32 newIndex)
		{
			var itemsToMove = _translatedItems.GetRange(oldIndex, items.Count);

			_translatedItems.RemoveRange(oldIndex, items.Count);
			_translatedItems.InsertRange(newIndex, itemsToMove);

			var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, itemsToMove, newIndex, oldIndex);
			CollectionChanged?.Invoke(this, e);
		}


		private void OnRemoveItems(IList items, Int32 startingIndex)
		{
			var itemsToRemove = RemoveItems(startingIndex, items.Count);

			var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, itemsToRemove, startingIndex);
			CollectionChanged?.Invoke(this, e);
			PropertyChanged?.Invoke(this, EventArgsCache.CountPropertyChanged);
		}


		private List<Tout> RemoveItems(Int32 startingIndex, Int32 count)
		{
			var itemsToRemove = _translatedItems.GetRange(startingIndex, count);

			foreach (var item in itemsToRemove)
				Dispose(item);

			_translatedItems.RemoveRange(startingIndex, count);

			return (itemsToRemove);
		}


		private void OnReplaceItems(IList newItems, Int32 startingIndex)
		{
			var itemsToRemove = RemoveItems(startingIndex, newItems.Count);
			var itemsToAdd = AddItems(newItems, startingIndex);

			var e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, itemsToAdd, itemsToRemove, startingIndex);
			CollectionChanged?.Invoke(this, e);
		}


		private void OnResetItems()
		{
			RemoveItems(0, _translatedItems.Count);
			AddItems(Source, 0);

			CollectionChanged?.Invoke(this, EventArgsCache.ResetCollectionChanged);
			PropertyChanged?.Invoke(this, EventArgsCache.CountPropertyChanged);
		}
	}


	internal static class EventArgsCache
	{
		internal static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs("Count");
		internal static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
	}
}
