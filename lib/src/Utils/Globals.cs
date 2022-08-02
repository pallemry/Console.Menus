namespace Console.Menu.lib.src.Utils
{
    internal static class Globals
    {
        public const string FirstLine =
            "Type \"BACK\" to go to the previous menu, \"Quit\" to quit the app or the number of action";

        public const ConsoleKey Quit = ConsoleKey.C;
        public const ConsoleKey Back = ConsoleKey.Escape;
        public const ConsoleKey Dir = ConsoleKey.D;
        public const ConsoleKey Enter = ConsoleKey.Enter;
        public const ConsoleKey Down = ConsoleKey.DownArrow;
        public const ConsoleKey Up = ConsoleKey.UpArrow;
        public static DisplayMenuReturn ProcessAnswer(ConsoleKeyInfo answer)
        {
            return answer.Key switch
            { Back                                                   => DisplayMenuReturn.Back,
              Dir when answer.Modifiers == ConsoleModifiers.Control  => DisplayMenuReturn.Dir,
              Quit when answer.Modifiers == ConsoleModifiers.Control => DisplayMenuReturn.Quit,
              Enter                                                  => DisplayMenuReturn.Enter,
              Down                                                   => DisplayMenuReturn.Down,
              Up                                                     => DisplayMenuReturn.Up,
              _                                                      => DisplayMenuReturn.Unknown };
        }
    }
}

