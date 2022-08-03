
# Console Menus
A simple, yet powerful implementation of console menus.

# Example
Here is an example of a demo app built using this package:
![The demo ](https://raw.githubusercontent.com/pallemry/Console.Menus/main/Resources/Windows-PowerShell-2022-08-03-02-30-51-_online-video-cutter.com_.gif)

> Note : **Everything** is controlled by the up/down arrow keys and the
> enter/esc key
# User usage
|Key| Usage |
|--|--   |
| ↑ Up arrow key | Move up in menu |
|↓ Down arrow key|Move down in menu|
|↳ Enter key|Select an item in the menu
|↰ Escape key|Go back to the previous menu (if exits)|
|Ctrl + C keys|Exit the menu by forcing it to stop

# Code Usage
You can start by creating a `MainMenu` object which will be responsible to run all of your other menus/items like this:
```csharp
MainMenu mainMenu = MainMenu.Create("My main menu");
```
Then you can start adding menus and/or items to the main menu
```csharp
mainMenu.AddMenu("My first sub menu");
IMenu subMenu = (mainMenu[0] as IMenu);
subMenu.AddItem("My first sub-sub item");
```
The casting is necessary because all the items a menu contains are by default of type `IMenuItem`. In order to access the `IMenu` members, we will need to cast the item to `IMenu`.

You can also add listeners to when the users selects the item like this:
```csharp
int count = 0;
var item = (subMenu[0] as MenuItem);
item.ActionToPerform += (sender, args) => {
    count++;
    System.Console.WriteLine("I have been selected " + count + " time(s)");
};
```

> The following code increments `count` by `1` each time the users selects the item - `item`. And then proceeds to print the count. (e.g. "I have been selected 1 time(s)")

In order to run the menus you'll need to call the `MainMenu.Run()` method.
```csharp
// In order to run
mainMenu.Run();
// In order to stop
mainMenu.Stop();
```
# Class Hierarchy
Here is a diagram explaining the hierarchy of the project to give you a better understanding of the project
![The project hierarchy](https://raw.githubusercontent.com/pallemry/Console.Menus/main/Resources/Hierarchy%20Diagram.png)
# Help
Any problems? questions? suggestions?
Contact me:
- **Mail** - yishai.israel8@gmail.com
- **Here**, write an issue or comment on this repository
