using System.Drawing;
using System.Reflection;

using Console.Menu.lib.src.Utils;

namespace Console.Menu.lib.src.Main
{
    public class Menu : IMenu
    {
        /// <inheritdoc />
        public IMenuItem? this[Index index]
        {
            get => _subItems[index];
            set => _subItems[index] = value;
        }

        public IMenuItem? this[int index]
        {
            get => _subItems[index];
            set => _subItems[index] = value;
        }

        public IMenuItem? this[object tag]
        {
            get => _subItems.First(item => item?.Tag == tag);
            set
            {
                var index = _subItems.FindIndex(item => item?.Tag == tag);
                _subItems[index] = value;
            }
        }

        public List<object> Args { get; } = new ();
        /// <inheritdoc />
        public MainMenu? RootParent { get; set; }
        /// <inheritdoc />
        public IMenu? Parent { get; set; }
        /// <inheritdoc />
        public string Caption { get; set; }
        /// <inheritdoc />
        public object? Tag { get; set; }
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

        public Menu(MainMenu? parentMainMenu, IMenu? parent, string caption = "<insert caption>",
                    params IMenuItem?[] menuItems)
        {
            if (Assembly.GetExecutingAssembly().FullName != GetType().Assembly.FullName)
            {
                if (parentMainMenu == null) throw new ArgumentNullException(nameof(parentMainMenu));
            }

            RootParent = parentMainMenu;
            Parent = parent;
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
            if (!RootParent.Running) return (DisplayMenuReturn.Quit, -1);
            
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
            for (int i = 0; i < _actionPerformedEndRow - curr + 1; i++)
            {
                ClearCurrentConsoleLine();
                System.Console.WriteLine();
            }
            System.Console.CursorTop = curr;
        }
        /// <inheritdoc />
        public bool AddMenu(string title)
        {
            var menu = new Menu(RootParent, this, title) { Tag = title };
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

            item.RootParent = RootParent;
            _subItems.Add(item);
            return true;
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

        protected static Point _defaultConsoleLocation;
        protected int _actionPerformedEndRow = -1;
        protected static Point _positionZero = new (System.Console.CursorLeft, System.Console.CursorTop + 1);

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
                    RootParent.Stop();
                    break;
                case DisplayMenuReturn.Dir:
                    System.Console.WriteLine();
                    PrintPrettyDir();
                    System.Console.WriteLine();
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

            return (!RootParent.Running ? DisplayMenuReturn.Quit : @return, @int);
        }
        private void PrintPrettyDir()
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
            int currentLineCursor = System.Console.CursorTop;
            System.Console.SetCursorPosition(0, System.Console.CursorTop);
            System.Console.Write(new string(' ', System.Console.WindowWidth));
            System.Console.SetCursorPosition(0, currentLineCursor);
        }
    }

}