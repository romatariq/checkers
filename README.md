# Checkers

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
