using System.Collections;
using System.Drawing;
using System.Reflection;

using Console.Menus.lib.src.Utils;

namespace Console.Menus.lib.src.Main
{
    public class Menu : IMenu
    {
        /// <inheritdoc />
        public IMenuItem this[Index index]
        {
            get => _subItems[index] ?? throw new InvalidOperationException();
            set => _subItems[index] = value;
        }

        public IMenuItem this[int index]
        {
            get => _subItems[index] ?? throw new InvalidOperationException();
            set => _subItems[index] = value;
        }

        public IMenuItem this[object tag]
        {
            get => _subItems.First(item => item?.Tag == tag) ?? throw new InvalidOperationException();
            set
            {
                var index = _subItems.FindIndex(item => item?.Tag == tag);
                _subItems[index] = value;
            }
        }

        public List<object> Args { get; } = new ();

        /// <inheritdoc />
        public bool Running
        {
            get
            {
                if (_isRoot)
                    return _running;
                else if (Parent != null)
                    return Parent.Running;
                else
                    return _running;
            }
        }

        private bool _isRoot;
        /// <inheritdoc />
        public void Run()
        {
            _running = true;
            _isRoot = true;
            _positionZero = new Point(System.Console.CursorLeft, System.Console.CursorTop + 1);
            DisplayMenu(Items.Count);
            _isRoot = false;
        }

        /// <inheritdoc />
        public void Stop()
        {
            _running = false;

            if (!_isRoot && Parent != null)
            {
                Parent.Stop();
            }
        }

        /// <inheritdoc />
        public IMenu RootParent
        {
            get
            {
                IMenu result;
                for (result = this; result.Parent != null; result = result.Parent) { }
                return result;
            }
        }
        /// <inheritdoc />
        public IMenu? Parent { get; set; }
        /// <inheritdoc />
        public string Caption { get; set; }
        /// <inheritdoc />
        public object? Tag { get; set; }

        protected static Point _defaultConsoleLocation;
        protected int _actionPerformedEndRow = -1;
        private bool _running;
        protected static Point _positionZero = new (System.Console.CursorLeft, System.Console.CursorTop + 1);

        /// <inheritdoc />
        public bool RemoveItem(IMenuItem? item) => _subItems.Remove(item);

        /// <inheritdoc />
        public void Clear() => _subItems.Clear();

        /// <inheritdoc />
        public int SelectedIndex { get; private set; } = -1;

        /// <inheritdoc />
        public IMenuItem? SelectedItem => SelectedIndex < 0 || SelectedIndex >= _subItems.Count ? _subItems[SelectedIndex] : null;
        /// <inheritdoc />
        public event EventHandler<PerformActionEventArgs>? ActionPerformed;
        /// <inheritdoc />
        public event EventHandler<PerformActionEventArgs>? ActionPerforming;

        public Menu(string caption = "<insert caption>",
                    params IMenuItem?[] menuItems)
        {
            Caption = caption;

            foreach (var menuItem in menuItems)
            {
                AddItem(menuItem);
            }
        }
        /// <inheritdoc />
        public virtual void PerformAction(ConsoleKeyInfo userInput, params object[] args)
        {
            var actionArgs = new PerformActionEventArgs(userInput, args);
            ActionPerforming?.Invoke(this, actionArgs);
            DisplayMenu((int) args[0]);
            ActionPerformed?.Invoke(this, actionArgs);
        }
        /// <inheritdoc />
        public virtual bool IsValid(IEnumerable<IMenuItem?> menuToCheck) => true;
        /// <inheritdoc />
        public string GetDirectory() => string.Join(@"\", GetParentsNames());
        private IEnumerable<string> GetParentsNames()
        {
            List<string> directories = new();

            for (IMenuItem? current = this; current != null; current = current.Parent)
            {
                directories.Add(current.Caption);
            }

            directories.Reverse();
            return directories;
        }
        protected readonly List<IMenuItem?> _subItems = new();
        /// <inheritdoc />
        public IReadOnlyList<IMenuItem?> Items => _subItems.AsReadOnly();
        /// <inheritdoc />
        public virtual (DisplayMenuReturn, int) DisplayMenu(int previousLines = -1)
        {
            if (!Running) return (DisplayMenuReturn.Quit, -1);
            
            (DisplayMenuReturn, int) @return = default;

            do
            {
                ResetPosition();
                PrintFirstLines();
                PrintItems(new Point(System.Console.CursorLeft, System.Console.CursorTop), @return.Item2 + previousLines);
                var answer = System.Console.ReadKey(true);
                @return = TryPerformAction(answer);
            } while (@return.Item1 != DisplayMenuReturn.Quit && @return.Item1 != DisplayMenuReturn.Back);
            ClearPerformedLines();
            return @return;
        }
        /// <inheritdoc />
        public virtual int PerformAction(int numItem, ConsoleKeyInfo userInput, params object[] args)
        {
            ClearPerformedLines();
            System.Console.CursorVisible = true;
            var subItem = _subItems[numItem];   
            subItem?.PerformAction(userInput, Items.Count, args);
            System.Console.CursorVisible = false;
            _actionPerformedEndRow = System.Console.CursorTop;
            if (subItem is IMenu m) return m.Items.Count;
            return -1;

        }
        private void ClearPerformedLines()
        {
            if (_actionPerformedEndRow == -1) return;
            var curr = System.Console.CursorTop;
            for (var i = 0; i < _actionPerformedEndRow - curr + 1; i++)
            {
                ClearCurrentConsoleLine();
                System.Console.WriteLine();
            }
            System.Console.CursorTop = curr;
        }
        /// <inheritdoc />
        public bool AddMenu(string title)
        {
            var menu = new Menu(title) { Tag = title };
            return AddItem(menu);
        }
        /// <inheritdoc />
        public bool AddItem(IMenuItem? item)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (item == null || !item.IsValid(_subItems))
            {
                return false;
            }

            item.Parent = this;
            _subItems.Add(item);
            return true;
        }
        
        public bool AddItem(string caption = "<insert caption>", EventHandler<PerformActionEventArgs> actionToPerform = default, object? tag = default)
        {
            var menuItem = new MenuItem(caption, actionToPerform) { Tag = null };
            return AddItem(menuItem);
        }

        protected void PrintFirstLines()
        {
            ResetPosition();
            System.Console.ForegroundColor = ConsoleColor.Gray;
            CustomWriteLine(Globals.FirstLine);
            CustomWriteLine();
            PrintPrettyDir();
            CustomWriteLine();
            _defaultConsoleLocation = new Point( System.Console.CursorLeft, System.Console.CursorTop);
        }

        private void ResetPosition()
        {
            System.Console.CursorTop = _positionZero.Y;
            System.Console.CursorLeft = _positionZero.X;
        }

        private void CustomWriteLine(object? s = null)
        {
            ClearCurrentConsoleLine();
            System.Console.WriteLine(s);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="answer">The answer of the user</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        private (DisplayMenuReturn, int) TryPerformAction(ConsoleKeyInfo answer)
        {
            var @return = Globals.ProcessAnswer(answer);
            var @int = -1;
            switch (@return)
            {
                case DisplayMenuReturn.Enter when SelectedIndex != -1:
                    @int = PerformAction(SelectedIndex, answer, Args);
                    break;
                case DisplayMenuReturn.Quit:
                    Parent.Stop();
                    break;
                case DisplayMenuReturn.Up:
                    int max = Items.Count == 0 ? 0 : Items.Count - 1, min = 0;
                    if (SelectedIndex <= min) 
                        SelectedIndex = max;
                    else 
                        SelectedIndex--;
                    break;
                case DisplayMenuReturn.Down:
                    (max, min) = (Items.Count == 0 ? 0 : Items.Count - 1, 0);
                    if (SelectedIndex >= max)
                        SelectedIndex = min;
                    else
                        SelectedIndex++;
                    break;
                case DisplayMenuReturn.Back:
                case DisplayMenuReturn.Unknown:
                default:
                    break;
            }

            return (!Running ? DisplayMenuReturn.Quit : @return, @int);
        }
        protected void PrintPrettyDir()
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            CustomWriteLine("Current directory: ");
            System.Console.ForegroundColor = ConsoleColor.White;
            var parents = GetParentsNames();
            ClearCurrentConsoleLine();
            foreach (var parent in parents)
            {
                if (parent == RootParent?.Caption)
                {
                    System.Console.BackgroundColor = ConsoleColor.Green;
                    System.Console.Write(parent);
                }
                else
                {
                    System.Console.BackgroundColor = ConsoleColor.Blue;
                    System.Console.Write(parent);
                }

                System.Console.ResetColor();
                System.Console.Write(@"\");
            }

            System.Console.Write(">");
            System.Console.WriteLine();
        }
        protected void PrintItems(Point consolePosition, int previousLines)
        {
            System.Console.CursorVisible = false;
            System.Console.SetCursorPosition(0, _defaultConsoleLocation.Y);
            var max = previousLines > _subItems.Count ? previousLines : _subItems.Count;
            const string format = "> ";
            for (var i = 0; i < max; i++)
            {
                ClearCurrentConsoleLine();
                if (i == SelectedIndex)
                {
                    System.Console.BackgroundColor = ConsoleColor.White;
                    System.Console.ForegroundColor = ConsoleColor.Black;
                    System.Console.Write(format);
                }
                if (i < _subItems.Count)
                {
                    System.Console.Write($"(#{i + 1}) - {_subItems[i].Caption} ");
                    if (i != SelectedIndex)
                        System.Console.Write($"{string.Join("", Enumerable.Repeat(" ", format.Length))}");
                    System.Console.WriteLine();
                }
                else
                {
                    System.Console.WriteLine();
                }

                System.Console.ResetColor();
            }
        }
        public static void ClearCurrentConsoleLine()
        {
            var currentLineCursor = System.Console.CursorTop;
            System.Console.SetCursorPosition(0, System.Console.CursorTop);
            System.Console.Write(new string(' ', System.Console.WindowWidth));
            System.Console.SetCursorPosition(0, currentLineCursor);
        }

        /// <inheritdoc />
        public IEnumerator<IMenuItem> GetEnumerator() => _subItems.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(IMenuItem menuItem) => AddItem(menuItem);
    }

}