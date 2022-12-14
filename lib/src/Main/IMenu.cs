using System.Collections;

using Console.Menus.lib.src.Utils;

namespace Console.Menus.lib.src.Main;

public interface IMenu : IMenuItem, IEnumerable<IMenuItem>
{
    IMenuItem this[Index index] { get; set; }
    IMenuItem this[int index] { get; set; }
    IMenuItem this[object tag] { get; set; }
    IReadOnlyList<IMenuItem?> Items { get; }
    (DisplayMenuReturn, int) DisplayMenu(int previousLines = -1);
    int PerformAction(int numItem, ConsoleKeyInfo userInput, params object[] args);
    /// <summary>
    /// Adds a menu to the <see cref="title"/> given
    /// </summary>
    /// <param name="title">The title and tag to give to the new menu</param>
    /// <returns>True if the addition was successful, otherwise false</returns>
    bool AddMenu(string title);
    /// <summary>
    /// Adds an item to the <seealso cref="Items"/> list
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <returns>True if the addition was successful, otherwise false</returns>
    bool AddItem(IMenuItem? item);

    /// <summary>
    /// Adds an item to the <seealso cref="Items"/> list
    /// </summary>
    /// <param name="caption">The caption of the item</param>
    /// <param name="actionToPerform">A method that will be called once the item is selected e.g. <see cref="IMenuItem.PerformAction"/></param>
    /// <param name="tag">An object that will be associated with this item</param>
    /// <returns>True if the addition was successful, otherwise false</returns>
    bool AddItem(string caption, EventHandler<PerformActionEventArgs> actionToPerform = default, object? tag = default);
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
    bool Running { get; }
    void Run();
    void Stop();
}