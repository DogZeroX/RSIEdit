using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Editor.ViewModels;
using Editor.Views.Events;
using ReactiveUI;

namespace Editor.Views
{
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class RsiItemView : ReactiveUserControl<RsiItemViewModel>
    {
        public RsiItemView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                var vm = ViewModel!;

                AddDisposables(d, vm);
                d.Add(this.WhenAnyValue(x => x.ViewModel)
                    .Subscribe(new AnonymousObserver<RsiItemViewModel?>(newVm =>
                    {
                        if (newVm != null)
                        {
                            AddDisposables(d, newVm);
                        }
                    })));
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void AddDisposables(CompositeDisposable d, RsiItemViewModel vm)
        {
            d.Add(vm.ImportPngInteraction.RegisterHandler(ImportPng));
            d.Add(vm.ErrorDialog.RegisterHandler(ShowError));
            d.Add(vm.CloseInteraction.RegisterHandler(Close));
            d.Add(vm.States.Subscribe(new AnonymousObserver<RsiStateViewModel>(s => d.Add(s.Image.Bitmap))));
            d.Add(vm);
        }

        private void ShowError(InteractionContext<ErrorWindowViewModel, Unit> interaction)
        {
            var args = new ShowErrorEvent(interaction.Input) {RoutedEvent = MainWindow.ShowErrorEvent};
            RaiseEvent(args);

            interaction.SetOutput(Unit.Default);
        }

        private void ImportPng(InteractionContext<Unit, string> interaction)
        {
            var dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>
                {
                    new()
                    {
                        Name = "PNG Files",
                        Extensions = new List<string> {"png"}
                    }
                }
            };
            var args = new OpenFileDialogEvent(dialog) {RoutedEvent = MainWindow.OpenFileEvent};
            RaiseEvent(args);

            interaction.SetOutput(args.Files.FirstOrDefault() ?? string.Empty);
        }

        private void Close(InteractionContext<RsiItemViewModel, Unit> interaction)
        {
            var args = new CloseRsiEvent(interaction.Input) {RoutedEvent = MainWindow.CloseRsiEvent};
            RaiseEvent(args);

            interaction.SetOutput(Unit.Default);
        }
    }
}
