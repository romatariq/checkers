namespace MenuSystem;

public class Menu
{
    public string Title { get; set; }
    public Dictionary<string, MenuItem> MenuItems = new();
    private EMenuLevel MenuLevel { get; set; }

    private const string BackShortcut = "X";
    private const string MainMenuShortcut = "M";
    private const string QuitShortcut = "Q";

    public Menu(string title, EMenuLevel menuLevel, List<MenuItem> menuItems)
    {
        Title = title;
        MenuLevel = menuLevel;
        AddMenuItems(menuItems);
        
        if (MenuLevel != EMenuLevel.Main)
        {
            AddMenuItem(new MenuItem( "Back", BackShortcut, null));
        }
        
        if (MenuLevel == EMenuLevel.Other)
        {
            AddMenuItem(new MenuItem("Main menu", MainMenuShortcut, null));
        }
        AddMenuItem(new MenuItem("Quit", QuitShortcut, null));
    }

    public void AddMenuItem(MenuItem item)
    {
        MenuItems[item.Shortcut] = item;
    }

    private void AddMenuItems(List<MenuItem> items)
    {
        foreach (var item in items)
        {
            AddMenuItem(item);
        }
    }

    public string RunMenu()
    {
        var menuIsRunning = true;
        var userChoice = "";
        
        while (menuIsRunning)
        {
            userChoice = PrintMenu();

            if (!MenuItems.ContainsKey(userChoice)) continue;
            
            string? methodToRunValue = null;

            if (GetMenuItem(userChoice).MethodToRun != null)
            {
                methodToRunValue = GetMenuItem(userChoice).MethodToRun!();
            }

            if (userChoice == BackShortcut)
            {
                menuIsRunning = false;
            }

            if (userChoice == QuitShortcut || methodToRunValue == QuitShortcut)
            {
                userChoice = methodToRunValue ?? userChoice;
                menuIsRunning = false;
            }

            if ((userChoice == MainMenuShortcut || methodToRunValue == MainMenuShortcut) && MenuLevel != EMenuLevel.Main)
            {
                userChoice = methodToRunValue ?? userChoice;
                menuIsRunning = false;
            }
        }

        return userChoice;
    }

    public MenuItem GetMenuItem(string shortcut) => MenuItems[shortcut];

    private List<MenuItem> GetItemsWithMainShortcutsInEnd()
    {
        var menuItemList = MenuItems.Values.ToList();

        var lastMenuItems = new List<string>() { BackShortcut, MainMenuShortcut, QuitShortcut };
        
        foreach (var shortcut in lastMenuItems)
        {
            if (!MenuItems.ContainsKey(shortcut))
            {
                continue;
            }

            var item = GetMenuItem(shortcut);
            menuItemList.Remove(item);
            menuItemList.Add(item);
        }
        return menuItemList;
    }

    private string PrintMenu()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(Title + "\n");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        var menuItemsList = GetItemsWithMainShortcutsInEnd();
        foreach (var item in menuItemsList)
        {
            Console.WriteLine(item);
        }
        
        Console.ResetColor();
        Console.WriteLine();
        Console.Write("");
        
        var originalPos = Console.CursorTop;
        var k = Console.ReadKey();
        var previousKeys = new Stack<ConsoleKeyInfo>();
        previousKeys.Push(k);
        var distanceFromOriginalPos = 2;
        const int menuNameSize = 2;

        while (true)
        {

            switch (k.Key)
            {
                case ConsoleKey.UpArrow:
                {
                    if (Console.CursorTop - distanceFromOriginalPos >= menuNameSize)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop - distanceFromOriginalPos);
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.WriteLine(menuItemsList[Console.CursorTop - menuNameSize]);
                        Console.ResetColor();
                        Console.ForegroundColor = ConsoleColor.DarkYellow;

                        if (Console.CursorTop != menuItemsList.Count + menuNameSize)
                        {
                            Console.WriteLine(menuItemsList[Console.CursorTop - menuNameSize]);
                        }
                        Console.ResetColor();

                        distanceFromOriginalPos++;
                    }

                    break;
                }
                case ConsoleKey.DownArrow:
                {
                    if (Console.CursorTop - distanceFromOriginalPos < menuItemsList.Count)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop - distanceFromOriginalPos + 1);
                        Console.ResetColor();
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine(menuItemsList[Console.CursorTop - menuNameSize]);

                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.WriteLine(menuItemsList[Console.CursorTop - menuNameSize]);
                        Console.ResetColor();
                        distanceFromOriginalPos--;
                    }

                    break;
                }
                case ConsoleKey.Enter when previousKeys.Peek().Key is ConsoleKey.DownArrow or ConsoleKey.UpArrow:
                    previousKeys.Pop();
                    var fieldIndex = Console.CursorTop - distanceFromOriginalPos + 1 - menuNameSize;
                    return menuItemsList[fieldIndex].Shortcut;
                
                case ConsoleKey.Enter when previousKeys.Peek().Key is ConsoleKey.Q or ConsoleKey.X or ConsoleKey.M:
                    return previousKeys.Pop().Key.ToString();
            }

            Console.SetCursorPosition(0, originalPos);
            previousKeys.Push(k);
            k = Console.ReadKey();
        }
        
    }
    
}