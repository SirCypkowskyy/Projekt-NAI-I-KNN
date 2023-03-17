using NAI.Projekt.KNN_ConsoleApp_s24759.Structures;

namespace NAI.Projekt.KNN_ConsoleApp_s24759.Views;
public class MainMenuChoice
{
    public string Name { get; set; }
    public AppViews View { get; set; }
    
    public MainMenuChoice(string name, AppViews view)
    {
        Name = name;
        View = view;
    }
    
    public override string ToString()
    {
        return Name;
    }
    
    public static IEnumerable<MainMenuChoice> GetMainMenuChoices()
    {
        return new[]
        {
            new MainMenuChoice("Tworzenie modelu k-NN [bold blue](<- od tego zacznij)[/]", AppViews.KnnModelCreation),
            new MainMenuChoice("Testowanie modelu k-NN [bold yellow](Wymaga stworzenia modelu)[/]", AppViews.KnnModelTesting),
            new MainMenuChoice("Wyświetl dane treningowe [bold yellow](Wymaga stworzenia modelu)[/]", AppViews.ShowData),
            new MainMenuChoice("Autorzy", AppViews.Credits),
            new MainMenuChoice("Wyjście", AppViews.Exit)
        };
    }
    
}