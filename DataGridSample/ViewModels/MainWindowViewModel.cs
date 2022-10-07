using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Bogus;
using ReactiveUI;
using Zafiro.Core.Mixins;
using Zafiro.Core.Trees;

namespace DataGridSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly CompositeDisposable disposables = new();

        public MainWindowViewModel()
        {
            var faker = new Faker();
            var items = new Model("Root", GenerateChildren(2));

            var textColumn = new TextColumn<Model, string>("adf", x => x.Name);

            Source = new HierarchicalTreeDataGridSource<Model>(items)
            {
                Columns =
                {
                    new HierarchicalExpanderColumn<Model>(textColumn, model => model.Children),
                }
            }.DisposeWith(disposables);

            ScrollToRandom = ReactiveCommand.Create(() =>
            {
                var randomModel = faker.Random
                    .ListItem(Source.Items.ToTreeNodes(x => x.Children)
                    .Flatten(x => x.Children).ToList());

                var index = randomModel.Path;
                Source.RowSelection!.SelectedIndex = new IndexPath(index);
            });
        }

        private static List<Model> GenerateChildren(int level)
        {
            if (level == 0)
            {
                return new List<Model>();
            }

            return Enumerable
                .Range(0, 100)
                .Select(i => new Model(i.ToString(), GenerateChildren(level - 1))).ToList();
        }

        public ReactiveCommand<Unit, Unit> ScrollToRandom { get; set; }

        public HierarchicalTreeDataGridSource<Model> Source { get; set; }
    }

    public class Model
    {
        public Model(string name, IEnumerable<Model> children)
        {
            Name = name;
            Children = children;
        }

        public string Name { get; }
        public IEnumerable<Model> Children { get; }
    }
}