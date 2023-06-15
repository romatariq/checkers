namespace MenuSystem;

public class MenuItem
{
    public string Title { get; set; }
    public string Shortcut { get; set; }
    public Func<string>? MethodToRun { get; set; }

    public MenuItem(string title, string shortcut, Func<string>? methodToRun)
    {
        Title = title;
        Shortcut = shortcut;
        MethodToRun = methodToRun;
    }

    public override string ToString() => $"{Title}{(Shortcut is "Q" or "X" or "M" ? $" [{Shortcut}]" : "")}";
}