<p align="center">
	<a href="https://opensource.org/licenses/MIT" target="_blank">
        <img src="https://img.shields.io/github/license/industriousone/industrious-mvvm" alt="MIT" />
    </a>
    <a href="https://twitter.com/industrious" target="_blank">
        <img src="https://img.shields.io/twitter/follow/industrious.svg?style=social&label=Follow">
    </a>
</p>

# Industrious.Mvvm

A small collection of general purpose MVVM helpers, for any platform or view framework, in a .NET Standard 2.0 package.

See [Industrious.ToDo][todo] for usage examples in something resembling a real application.

## Contents

- [Command](#command)
- [ReactiveObject](#reactiveobject)
- [TranslatingObservable](#translatingobservable)

### Command

A general implementation of the [ICommand][icommand] interface, not tied to any particular view framework.

```c#
public class MyViewModel
{
    private String _value;

    public MyViewModel
    {
        ChangeValueCommand = new Command<String>(value =>
        {
            _value = value;
        });
    }

    public ICommand ChangeValueCommand { get; }
}

var viewModel = new MyViewModel();
viewModel.ChangeValueCommand.Execute("New value");
```

See [Industrious.ToDo.ViewModels.ItemEditorViewModel.cs][ievm] for a usage example.


### NotifyPropertyChanged

Helper base class which makes working with [INotifyPropertyChanged][inpc] a little easier. Calling `SetAndRaiseIfChanged()` will update the provided field, and raise the `PropertyChanging` and `PropertyChanged` events, if the value has changed.

```c#
public class MyViewModel : NotifyPropertyChanged
{
    private Int32 _myValue;

    public Int32 MyValue
    {
        get => _myValue;
        set => SetAndRaiseIfChanged(ref _myValue, value);
    }
}
```

See [Industrious.ToDo.ViewModels.ItemEditorViewModel.cs][ievm] for a usage example.

### TranslatingObservable

Given an [ObservableCollection][obscoll] containing one type of data, translate it to a new `ObservableCollection` of a different data type. Useful for turning a collection of model objects into a collection of view-models, which can then be bound to a list or collection view.

```c#
public class Item
{}

public class ItemViewModel
{
    public ItemViewModel(Item item)
    {}
}

public class ItemListViewModel
{
    public class ItemListViewModel(ObservableCollection<Item> items)
    {
        Items = new TranslatingObservable<Item, ItemViewModel>(
            items,
            item => new ItemViewModel(item)
        );
    }

    public TranslatingObservable<Item, ItemViewModel> Items { get; }
}
```

The translated collection is "live": any changes published by the source `ObservableCollection` are mirrored by the translated collection, and the corresponding change events are raised.

If the target data type (`ItemViewModel` in this example) implements [IDisposable][idispose], it's `Dispose()` method will be called automatically whenever an instance is removed from the translated list.

See [Industrious.ToDo.ViewModels.ItemListViewModel.cs][ilvm] for a usage example.

[icommand]: https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.icommand?view=netframework-4.8
[idispose]: https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
[ievm]: https://github.com/industriousone/industrious-todo/blob/master/Industrious.ToDo.ViewModels/ItemEditorViewModel.cs
[ilvm]: https://github.com/industriousone/industrious-todo/blob/master/Industrious.ToDo.ViewModels/ItemListViewModel.cs
[inpc]: https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged?view=netframework-4.8
[obscoll]: https://docs.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.observablecollection-1?view=netframework-4.8
[todo]: https://github.com/industriousone/industrious-todo.git

## Stay in touch

* Website - https://industriousone.com
* Twitter - [@industrious](https://twitter.com/industrious)

## License

[MIT](https://opensource.org/licenses/MIT)
