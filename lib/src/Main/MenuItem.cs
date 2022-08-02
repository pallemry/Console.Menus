namespace Console.Menu.lib.src.Main
{
    public class MenuItem : IMenuItem
    {
        public MenuItem(MainMenu? rootParent, IMenu? parent, string caption,
                        EventHandler<PerformActionEventArgs>? actionToPerform = null)
        {
            RootParent = rootParent;
            Caption = caption;
            Parent = parent;
            if (actionToPerform != null)
                ActionToPerform += actionToPerform;
        }
        /// <inheritdoc />
        public MainMenu? RootParent { get; set; }
        /// <inheritdoc />
        public IMenu? Parent { get; set; }
        /// <inheritdoc />
        public string Caption { get; set; }
        /// <inheritdoc />
        public object? Tag { get; set; }
        public event EventHandler<PerformActionEventArgs>? ActionToPerform;
        /// <inheritdoc />
        public event EventHandler<PerformActionEventArgs>? ActionPerformed;
        /// <inheritdoc />
        public event EventHandler<PerformActionEventArgs>? ActionPerforming;

        /// <inheritdoc />
        public void PerformAction(ConsoleKeyInfo userInput, params object[] args)
        {
            var actionArgs = new PerformActionEventArgs(userInput, args);
            ActionPerforming?.Invoke(this, actionArgs);
            ActionToPerform?.Invoke(this, actionArgs);
            ActionPerformed?.Invoke(this, actionArgs);
        }

        /// <inheritdoc />
        public bool IsValid(IEnumerable<IMenuItem?> menuToCheck) => true;

        /// <inheritdoc />
        public string GetDirectory()
        {
            List<string> directories = new ();
            for (IMenuItem? current = this; current != null; current = current.Parent)
            {
                directories.Add(current.Caption);
            }

            directories.Reverse();
            return string.Join(@"\", directories) + ".item";
        }
    }
}
