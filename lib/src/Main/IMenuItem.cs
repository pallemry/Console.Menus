namespace Console.Menus.lib.src.Main;
/// <summary>
/// This is the base interface of all items and menus in the menu
/// </summary>
public interface IMenuItem
{
    /// <summary>
    /// This is the main menu that is linked to this item. Every item has to have a main menu embedded to it.
    /// </summary>
    MainMenu? RootParent { get; set; }
    /// <summary>
    /// The Parent of this object
    /// </summary>
    IMenu? Parent { get; set; }
    /// <summary>
    /// The caption or summary of the item. It will be used when showing the item in a menu
    /// </summary>
    string Caption { get; set; }
    /// <summary>
    /// The object associated with the MenuItem
    /// </summary>
    object? Tag { get; set; }
    /// <summary>
    /// Occurs before the <see cref="PerformAction"/> method is called
    /// </summary>
    event EventHandler<PerformActionEventArgs> ActionPerforming;
    /// <summary>
    /// Occurs after the <see cref="PerformAction"/> method is called
    /// </summary>
    event EventHandler<PerformActionEventArgs> ActionPerformed;
    /// <summary>
    /// The action to perform once the item is selected in the menu.
    /// </summary>
    /// <remarks>
    /// Note: this is called after the <see cref="ActionPerforming"/> is raised and before the <see cref="ActionPerformed"/>
    /// is raised
    /// </remarks>
    /// <param name="userInput">The user input given in the console to perform this action (e.g. user selected menu item 5)</param>
    /// <param name="args">Additional arguments</param>
    void PerformAction(ConsoleKeyInfo userInput, params object[] args);
    /// <summary>
    /// Used to validate an object when you enter it
    /// </summary>
    /// <param name="menuToCheck"></param>
    /// <returns></returns>
    bool IsValid(IEnumerable<IMenuItem?> menuToCheck);
    /// <summary>
    /// Gets the directory of this item all the way to the root menu
    /// </summary>
    /// <returns></returns>
    string GetDirectory();
    
}