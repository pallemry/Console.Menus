using Console.Menus.lib.src.Utils;

namespace Console.Menus.lib.src.Main
{
    public sealed class MainMenu : Menu
    {
        public void Stop() { Running = false; }
        public bool Running { get; private set; } = false;

        public void Run()
        {
            _positionZero = new (System.Console.CursorLeft, System.Console.CursorTop + 1);
            Running = true;
            DisplayMenuReturn shouldContinue;

            do
            {
                shouldContinue = DisplayMenu(Items.Count).Item1;
            } while (Running && shouldContinue != DisplayMenuReturn.Quit);
        }

        /// <summary>
        /// Initializes a new <see cref="MainMenu"/> object.
        /// </summary>
        public static MainMenu Create(string caption, params IMenuItem?[] items) => new (null!, caption, items);

        /// <inheritdoc />
        private MainMenu(MainMenu? parentMainMenu, string caption = "<insert caption>",params IMenuItem?[] menuItems) 
            : base(parentMainMenu, null, caption, menuItems)
        {
            RootParent = this;
            Parent = null;
        }
    }
}
