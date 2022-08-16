
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
You can start by creating a `Menu` object which will be responsible to run all of your other menus/items like this:
```csharp
Menu mainMenu = new Menu("My first menu");
```
Then you can start adding menus and/or items to the main menu
```csharp
mainMenu.AddMenu("My first sub menu");
IMenu subMenu = ((IMenu) mainMenu[0]);
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

In order to run the menu you'll need to call the `IMenu.Run()` method. And `IMenu.Stop()` in order to stop.
```csharp
// In order to run
mainMenu.Run();
// In order to stop
mainMenu.Stop();
```
**Extra** - you can also create menus in this syntax - (which may be more readable in certain cases):
```csharp
var mm = new Menu("My main menu")
        {
            new Menu("My sub menu")
            {
                new Menu("My sub-sub menu")
                {
                    new MenuItem("Print Hello World", (sender, args) => {
                        System.Console.WriteLine("Hello World");
                    })
                },
                new MenuItem("Click me", (sender, args) => {
                    System.Console.WriteLine("I have been clicked :)");
                }),
                new MenuItem("Stop menu", (sender, args) => {
                    var thisItem = ((IMenuItem) sender);
                    thisItem.Parent.Stop();
                })
            }
        };
        mm.Run();
```


# Class Hierarchy
Here is a diagram explaining the hierarchy of the project to give you a better understanding of the project

![Project Hierarchy](https://raw.githubusercontent.com/pallemry/Console.Menus/main/Resources/Untitled%20Diagram.drawio.png)

# Help
Any problems? questions? suggestions?
Contact me:
- **Mail** - yishai.israel8@gmail.com
- **Here**, write an issue or comment on this repository
