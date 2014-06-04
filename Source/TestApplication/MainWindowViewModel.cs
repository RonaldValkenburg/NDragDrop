using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using NDragDrop.TestApplication.Annotations;

namespace NDragDrop.TestApplication
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _canDropApples;

        public MainWindowViewModel()
        {
            Fruits = new ObservableCollection<Fruit> { new Apple(), new Banana()};
            Apples = new ObservableCollection<Apple>();
            Bananas = new ObservableCollection<Banana>();
            CanDropApples = true;
        }

        public ObservableCollection<Fruit> Fruits { get; set; }
        public ObservableCollection<Apple> Apples { get; set; }
        public ObservableCollection<Banana> Bananas { get; set; }

        public ICommand DropFruit
        {
            get
            {
                return new DelegateCommand<DropEventArgs>(args =>
                {
                    var fruit = args.Context as Fruit;
                    if (fruit == null || Fruits.Contains(fruit)) return;
                    if (fruit is Apple)
                    {
                        var apple = (Apple) fruit;
                        Apples.Remove(apple);
                        apple.Name = "Apple";
                    }
                    if (fruit is Banana) Bananas.Remove((Banana) fruit);
                    Fruits.Add(fruit);
                }, CanExecuteDropFruit);
            }
        }

        public ICommand DropApple
        {
            get
            {
                return new DelegateCommand<DropEventArgs>(args =>
                    {
                        var apple = args.Context as Apple;
                        if (apple == null || Apples.Contains(apple)) return;
                        Fruits.Remove(apple);
                        apple.Name = String.Format("{0}- {1}", "Apple", args.Parameter);
                        Apples.Add(apple);
                    }, CanExecuteDropApples);
            }
        }

        public ICommand DropBanana
        {
            get
            {
                return new DelegateCommand<DropEventArgs>(args =>
                {
                    var banana = args.Context as Banana;
                    if (banana == null || Bananas.Contains(banana)) return;
                    Fruits.Remove(banana);
                    Bananas.Add(banana);
                }, CanExecuteDropBanana);
            }
        }

        public bool CanDropApples
        {
            get { return _canDropApples; }
            set
            {
                if (_canDropApples == value) return;
                _canDropApples = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool CanExecuteDropFruit(object args)
        {
            var dropEventArgs = args as DropEventArgs;
            return dropEventArgs != null && dropEventArgs.Context is Fruit;
        }

        private bool CanExecuteDropBanana(object args)
        {
            var dropEventArgs = args as DropEventArgs;
            return dropEventArgs != null && dropEventArgs.Context is Banana;
        }

        private bool CanExecuteDropApples(object args)
        {
            var dropEventArgs = args as DropEventArgs;
            return CanDropApples && dropEventArgs != null && dropEventArgs.Context is Apple;
        }
    }
}
