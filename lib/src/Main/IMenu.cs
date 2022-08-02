using Console.Menu.lib.src.Utils;

namespace Console.Menu.lib.src.Main;

public interface IMenu : IMenuItem
{
    IMenuItem? this[Index index] { get; set; }
    IMenuItem? this[int index] { get; set; }
    IMenuItem? this[object tag] { get; set; }
    IReadOnlyList<IMenuItem?> Items { get; }
    (DisplayMenuReturn, int) DisplayMenu(int previousLines = -1);
    int PerformAction(int numItem, ConsoleKeyInfo userInput, params object[] args);
    /// <summary>
    /// Adds a menu to the <see cref="title"/> given
    /// </summary>
    /// <param name="title">The title and tag to give to the new menu</param>
    /// <returns>True if the addition was successful, otherwise false</returns>
    bool AddMenu(string title);
    bool AddItem(IMenuItem? item);
    /// <summary>
    /// Removes an item from the <see cref="Items"/> list. Returns true if the removal was successful, otherwise false
    /// </summary>
    /// <param name="item">The item to remove</param>
    /// <returns>True if the removal was successful, otherwise false</returns>
    bool RemoveItem(IMenuItem? item);
    /// <summary>
    /// Removes all the elements from the <see cref="Items"/> list
    /// </summary>
    void Clear();
    /// <summary>
    /// The currently index of the selected item by the user, if nothing is selected this will be -1
    /// </summary>
    int SelectedIndex { get; }
    /// <summary>
    /// The currently selected item by the user, if nothing is selected this will be null
    /// </summary>
    IMenuItem? SelectedItem { get; }
    /// <summary>
    /// The args that will be supplied in <see cref="PerformAction"/> args
    /// </summary>
    List<object> Args { get; }
}