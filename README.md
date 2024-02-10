# Checkers

[Demo video](https://www.youtube.com/watch?v=S6A5avreAQk)  

Users can toggle between used data storages.
Can create, update, delete games and it's settings.
In game settings board size, legal moves and capturing rules can be configured.
There are different player types: human and AI with difficulty level (very easy, easy, medium, hard).
All player type combinations are allowed (AI vs AI included) and users can change player type mid game.
However there isn't authentication and therefore anyone can play anyones game.

## Implementation

- Written in C#.
- Web pages are created with Razor.
- 2 UIs: console and web app.
- 2 data storing methods: SQLite and file system.
- AI uses minimax method to choose the best move, difficulty determines the depth. 
An exception is very easy AI, which chooses moves randomly.

## How to run
1. Make sure you have at least .NET 7 with `dotnet --version`.
2. Clone the repository.
3. Navigate to the project's root folder.
4. Install Entity Framework Core tools.
   1. `dotnet tool install --global dotnet-ef`
   2. `dotnet tool update --global dotnet-ef`
5. Run `dotnet ef database update --project DAL.Db --startup-project WebApp --context AppDbContext` to create the database.
6. Run `dotnet run --project WebApp` to start the web app or `dotnet run --project ConsoleApp` to start the console app.

~~~bash
git clone git@github.com:romatariq/checkers.git
cd checkers
dotnet ef database update --project DAL.Db --startup-project WebApp --context AppDbContext
dotnet run --project WebApp
~~~
