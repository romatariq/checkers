// See https://aka.ms/new-console-template for more information


using ConsoleUI;
using DAL;
using DAL.Db;
using DAL.FileSystem;
using Domain;
using GameBrain;
using MenuSystem;
using Microsoft.EntityFrameworkCore;


var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite($"Data Source={Constants.DatabasePath}")
    .Options;
var ctx = new AppDbContext(dbOptions);

IGameSettingsRepository gameSettingsRepositoryFs = new GameSettingsRepositoryFileSystem();
IGameRepository gameRepositoryFs = new GameRepositoryFileSystem();

IGameSettingsRepository gameSettingsRepositoryDb = new GameSettingsRepositoryDatabase(ctx);
IGameRepository gameRepositoryDb = new GameRepositoryDatabase(ctx);


IGameSettingsRepository gameSettingsRepository = gameSettingsRepositoryDb;
IGameRepository gameRepository = gameRepositoryDb;


InitializeGameModeSettings();

var settings = gameSettingsRepository.GetGameSettings(1)!;


var gameStateMenu = new Menu("Game states", EMenuLevel.Other, new List<MenuItem>());
var savedRulesMenu = new Menu("Saved rules", EMenuLevel.Other, new List<MenuItem>());
var savedGamesMenu = new Menu("All saved games", EMenuLevel.Other, new List<MenuItem>());
var customRulesMenu = new Menu("Edit rules", EMenuLevel.Other, new List<MenuItem>()
{
    new("Game mode name", "GMN", ChangeGameModeName),
    new("Board width", "CW", ChangeSettingsWidth),
    new("Board height", "CH", ChangeSettingsHeight),
    new("Have to eat", "CE", ChangeSettingsHasToEat),
    new("Can eat backwards", "CEB", ChangeSettingsCanEatBackwards),
    new("White starts", "WS", ChangeWhiteStarts),
    new("King step length", "CK", ChangeKingCanMoveOnlyOneStep),
    new("Capturing king", "CEK", ChangeAllButtonCanEatKing)
});
var optionMenu = new Menu("Options", EMenuLevel.Second, new List<MenuItem>()
{
    new("Load rules", "LR", SavedRulesMenuRunMenu),
    new("Create new game setting", "CNRAS", CreateNewGameMode),
    new("Delete rules", "DEL", DeleteGameMode)
});

Menu mainMenu = null!;
mainMenu = new Menu("Checkers", EMenuLevel.Main, new List<MenuItem>()
{
    new("New game", "GO", StartNewGame),
    new("Load game", "L", LoadGamesMenuRunMenu),
    new("Delete game", "D", DeleteGame),
    new("Options", "O", optionMenu.RunMenu),
    new("Using Database", "DB", ToggleSavingSystem)
});


mainMenu.RunMenu();



string SavedRulesMenuRunMenu()
{
    Console.WriteLine("Enter filter:");
    var filter = Console.ReadLine() ?? "";
    LoadGameModesToFixedRulesMenu(savedRulesMenu, filter == "" ? null : filter);
    return savedRulesMenu.RunMenu();
}

string LoadGamesMenuRunMenu()
{
    Console.WriteLine("Enter filter:");
    var filter = Console.ReadLine() ?? "";
    LoadSavedGamesToSavedGamesMenu(savedGamesMenu, filter == "" ? null : filter);
    return savedGamesMenu.RunMenu();
}


string ToggleSavingSystem()
{
    if (gameRepository == gameRepositoryDb)
    {
        gameRepository = gameRepositoryFs;
        gameSettingsRepository = gameSettingsRepositoryFs;
        mainMenu.GetMenuItem("DB").Title = "Using File System";
    }
    else
    {
        gameRepository = gameRepositoryDb;
        gameSettingsRepository = gameSettingsRepositoryDb;
        mainMenu.GetMenuItem("DB").Title = "Using Database";
    }
    return "-";
}

void InitializeGameModeSettings()
{
    var gameModeSettings = new GameSetting
    {
        Id = 1,
        Name = "classic"
    };
    gameSettingsRepositoryFs.SaveGameSettings(gameModeSettings);
    gameSettingsRepositoryDb.SaveGameSettings(gameModeSettings);

    
    gameModeSettings = new GameSetting
    {
        Id = 2,
        Name = "russian",
        BoardHeight = 10,
        BoardWidth = 10,
        KingCanMoveOnlyOneStep = false,
    };
    gameSettingsRepositoryFs.SaveGameSettings(gameModeSettings);
    gameSettingsRepositoryDb.SaveGameSettings(gameModeSettings);
}

string CreateNewGameMode()
{
    settings = new GameSetting
    {
        Name = "Creating in process"
    };
    var response = customRulesMenu.RunMenu();
    gameSettingsRepository.SaveGameSettings(settings);
    return response;
}

string DeleteGameMode()
{
    Console.WriteLine("Enter gamemode you would like to delete! Setting cannot be deleted if it is used by any game if using database! [q to quit]");
    var fileName = Console.ReadLine() ?? "";
    if (fileName.ToLower() == "q" || fileName == settings.Name)
    {
        return "-";
    }
    
    var settingsList = new List<GameSetting>();
    var page = 1;
    var pageCount = gameSettingsRepository.GetPageCount(fileName);
    while (pageCount >= page)
    {
        settingsList.AddRange(gameSettingsRepository.GetGameSettingsList(page, fileName));
        
        if (settingsList.Select(x => x.Name).Contains(fileName)) break;
        page++;
    }

    var settingsId = settingsList.Find(x => x.Name == fileName)?.Id;
    if (settingsId == null) return "-";
    gameSettingsRepository.DeleteGameSettings(settingsId.Value);
    return "-";
}

string DeleteGame()
{
    Console.WriteLine("Enter game you would like to delete! [q to quit]");
    var fileName = Console.ReadLine() ?? "";;
    if (fileName.ToLower() == "q")
    {
        return "-";
    }

    var gamesList = new List<Game>();
    var page = 1;
    var pageCount = gameRepository.GetPageCount(fileName);
    while (pageCount >= page)
    {
        gamesList.AddRange(gameRepository.GetSavedGamesList(page, fileName));
        
        if (gamesList.Select(x => x.Name).Contains(fileName)) break;
        page++;
    }

    var gameId = gamesList.Find(x => x.Name == fileName)?.Id;
    if (gameId == null) return "-";
    
    gameRepository.DeleteSavedGame(gameId.Value);
    return "-";
}

string StartNewGame()
{
    var game = new Game()
    {
        Name = "New Game",
        Player1Name = "You",
        Player2Name = "Opponent",
        GameSetting = settings,
    };
    
    var gameSetting = gameSettingsRepository.GetGameSettings(settings.Id);
    
    
    if (gameSetting != null)
    {
        game.GameSetting = gameSetting;
    }

    var ui = new GameUI(new GameRunner(game, gameRepository));
    ui.Run();
    return "-";
}

void LoadGameModesToFixedRulesMenu(Menu targetMenu, string? filter, int page = 1, int? pageCount = null)
{
    var newMenu = new Dictionary<string, MenuItem>();

    pageCount ??= gameSettingsRepository.GetPageCount(filter);
    var settingsList = gameSettingsRepository.GetGameSettingsList(page, filter);

    foreach (var gameSetting in settingsList)
    {
        var name = gameSetting.Name;
        string MethodToRun()
        {
            settings = gameSetting;
            return "-";
        }

        newMenu[name + gameSetting.Id] = new MenuItem(name, name + gameSetting.Id, MethodToRun);
    }
    
    if (pageCount > page)
    {
        var nextPageMenu = new Menu("Saved rules", EMenuLevel.Other, new List<MenuItem>());
        LoadGameModesToFixedRulesMenu(nextPageMenu, filter, page + 1, pageCount);
        newMenu["NEXT"] = new MenuItem("Next", "NEXT", nextPageMenu.RunMenu);
    }
    
    newMenu["X"] = targetMenu.MenuItems["X"];
    newMenu["M"] = targetMenu.MenuItems["M"];
    newMenu["Q"] = targetMenu.MenuItems["Q"];
    targetMenu.MenuItems = newMenu;
}

void LoadSavedGamesToSavedGamesMenu(Menu targetMenu, string? filter, int page = 1, int? pageCount = null)
{
    var newMenu = new Dictionary<string, MenuItem>();

    pageCount ??= gameRepository.GetPageCount(filter);
    var list = gameRepository.GetSavedGamesList(page, filter);
    
    
    foreach (var game in list)
    {
        var name = game.Name;
        string MethodToRun()
        {
            gameStateMenu.Title = name + " states";
            AddStatesToMenu(game, gameStateMenu);
            return gameStateMenu.RunMenu();
        }
    
        newMenu[name + game.Id] = new MenuItem(name, name + game.Id, MethodToRun);

    }

    if (pageCount > page)
    {
        var nextPageMenu = new Menu("All saved games", EMenuLevel.Other, new List<MenuItem>());
        LoadSavedGamesToSavedGamesMenu(nextPageMenu, filter, page + 1, pageCount);
        newMenu["NEXT"] = new MenuItem("Next", "NEXT", nextPageMenu.RunMenu);
    }
    newMenu["M"] = targetMenu.MenuItems["M"];
    newMenu["Q"] = targetMenu.MenuItems["Q"];
    newMenu["X"] = targetMenu.MenuItems["X"];
    targetMenu.MenuItems = newMenu;
}

void AddStatesToMenu(Game game, Menu targetMenu)
{
    var states = game.GameStates;
    if (states == null)
    {
        return;
    }
    
    var newMenu = new Dictionary<string, MenuItem>();

    foreach (var state in states)
    {
        string MethodToRun()
        {
            var ui = new GameUI(new GameRunner(game, gameRepository, state.Id));
            ui.Run();
            return "-";
        }
        var date = $"{state.CreatedAt.ToLongTimeString()} {state.CreatedAt.ToShortDateString()}";
        newMenu[date] = new MenuItem(date, date, MethodToRun);
    }
    newMenu["M"] = targetMenu.MenuItems["M"];
    newMenu["Q"] = targetMenu.MenuItems["Q"];
    newMenu["X"] = targetMenu.MenuItems["X"];
    targetMenu.MenuItems = newMenu;
}

string ChangeGameModeName()
{
    Console.WriteLine("What would you like to name your settings?");
    settings.Name = Console.ReadLine() ?? "";
    return "-";
}


string ChangeSettingsWidth() {
    // should be even. Otherwise if height is not odd, white opponent will have one more button.
     var val = GetUserIntValue("Enter board width, has to be even! [4 - 26]", i => i >= 4 & i <= 26 & i % 2 == 0);
     settings.BoardWidth = val;
     return "-";
}
string ChangeSettingsHeight() {
    var val = GetUserIntValue("Enter board height, even is recommended! [4 - 99]", i => i >= 4 & i <= 99);
    settings.BoardHeight = val;
    return "-";
}
string ChangeSettingsHasToEat() {
    var val = GetUserBoolValue("Do you have to eat? [true or false]");
    settings.HasToCapture = val;
    return "-";
}
string ChangeSettingsCanEatBackwards() {
    var val = GetUserBoolValue("Can you capture backwards? [true or false]");
    settings.CanEatBackwards = val;
    return "-";
}
string ChangeWhiteStarts()
{
    var val = GetUserBoolValue("Does white start? [true or false]");
    settings.WhiteStarts = val;
    return "-";
}
string ChangeKingCanMoveOnlyOneStep()
{
    var val = GetUserBoolValue("Can king move only move 1 step at once? [true or false]");
    settings.KingCanMoveOnlyOneStep = val;
    return "-";
}
string ChangeAllButtonCanEatKing()
{
    var val = GetUserBoolValue("Can normal buttons capture king? [true or false]");
    settings.AllButtonCanEatKing = val;
    return "-";
}

int GetUserIntValue(string str, Func<int, bool>isValid)
{
    while (true)
    {
        Console.WriteLine(str);
        Console.WriteLine("Your input:");
        var userInput = Console.ReadLine() ?? "";

        if (int.TryParse(userInput, out _))
        {
            var val = int.Parse(userInput);
            if (isValid(val))
            {
                return val;
            }
        }
    }
}

bool GetUserBoolValue(string str)
{
    while (true)
    {
        Console.WriteLine(str);
        Console.WriteLine("Your input:");
        var userInput = Console.ReadLine() ?? "";

        if (bool.TryParse(userInput, out _))
        {
            var val = bool.Parse(userInput);
            return val;
        }
    }
}
