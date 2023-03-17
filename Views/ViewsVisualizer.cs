using NAI.Projekt.KNN_ConsoleApp_s24759.Structures;
using Spectre.Console;

namespace NAI.Projekt.KNN_ConsoleApp_s24759.Views;

public class ViewsVisualizer
{
    public AppViews CurrentView { get; private set; }
    
    private string[] _args;
    
    private List<MainMenuChoice> _mainMenuChoices = new List<MainMenuChoice>(MainMenuChoice.GetMainMenuChoices());
    
    private bool _isKnnModelCreated = false;
    
    public ViewsVisualizer(string[] args)
    {
        _args = args;

        DisplayAppEntry();
        LoadView(AppViews.MainMenu);
    }

    
    public void LoadView(AppViews view)
    {
        CurrentView = view;
        AnsiConsole.Clear();
        switch (view)
        {
            case AppViews.MainMenu:
                VisualiseMainMenu();
                break;
            case AppViews.KnnModelCreation:
                VisualiseKnnInit();
                break;
            case AppViews.KnnModelTesting:
                break;
            case AppViews.Credits:
                VisualizeCredits();
                break;
            case AppViews.Exit:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(view), view, "Nieznany widok");
        }
    }
    
    private static void DisplayAppEntry()
    {
        AnsiConsole.Status()
            .Start("Przygotowywanie programu...", ctx => 
            {
                AnsiConsole.MarkupLine("Sprawdzanie poprawności kodu...");
                Thread.Sleep(1000);
        
                ctx.Status("Przetwarzanie danych...");
                ctx.Spinner(Spinner.Known.Star);
                ctx.SpinnerStyle(Style.Parse("green"));

                AnsiConsole.MarkupLine("Przygotowywanie GUI...");
                Thread.Sleep(2000);
            });
        
        AnsiConsole.MarkupLine("Zakończono.");
        Thread.Sleep(1000);
    }


    private void VisualiseMainMenu()
    {
        AnsiConsole.Write(new FigletText("K-NN").Centered().Color(Color.Red1));
        AnsiConsole.Write(new Markup("[italic underline]Aplikacja do klasyfikacji danych metodą k-NN[/]").Centered());
        AnsiConsole.Write(new Markup("[bold yellow]Autor[/]: Cyprian Gburek, PJATK (s24759)").Centered());

        if (!_isKnnModelCreated && Program.IsKnnModelCreated())
        {
            _mainMenuChoices[0].Name = "Nowy model k-NN";
            _mainMenuChoices[1].Name = "Testowanie modelu k-NN [bold green](gotowe do użycia)[/]";
            _isKnnModelCreated = true;
        }
        
        var userChoice = AnsiConsole.Prompt(
            new SelectionPrompt<MainMenuChoice>()
                .Title("Wybierz opcję [grey](Naciśnij [u]enter[/] aby wybrać daną opcję, używaj strzałek do poruszania się po wyborach)[/]:").PageSize(10)
                .AddChoices(_mainMenuChoices)
        );
        Console.WriteLine(userChoice);
        LoadView(userChoice.View);
    }

    private void VisualizeCredits()
    {
        AnsiConsole.Render(Align.Center(new Markup("[bold red]Autorzy[/]\n"), VerticalAlignment.Middle));
        Thread.Sleep(1000);
        AnsiConsole.Render(Align.Center(new Markup("[bold yellow]Cyprian Gburek[/]"), VerticalAlignment.Middle));
        Thread.Sleep(1000);
        AnsiConsole.Render(Align.Center(new Markup("[bold red]PJATK[/]"), VerticalAlignment.Middle));
        Thread.Sleep(1000);
        AnsiConsole.Render(Align.Center(new Markup("[bold green]s24759[/]"), VerticalAlignment.Middle));
        Thread.Sleep(1000);
        AnsiConsole.Render(Align.Center(new Markup("[bold link=https://github.com/SirCypkowskyy]Github[/]([underline]kliknij trzymając ctrl[/])"), VerticalAlignment.Middle));
        
        AnsiConsole.WriteLine("Naciśnij enter, aby wrócić do menu głównego");
        Console.ReadLine();
        LoadView(AppViews.MainMenu);
    }
    private void VisualiseKnnInit()
    {
        string inputDataPath;
        string outputDataFolderPath;
        int kValue;

        // Dla k-NN bez wprowadzonej ścieżki do pliku z danymi
        if (_args is null || _args.Length < 3)
        {
            inputDataPath = AnsiConsole.Ask<string>("Podaj absolutną ścieżkę do pliku z danymi iris: ");
            outputDataFolderPath = AnsiConsole.Ask<string>("Podaj absolutną ścieżkę do folderu, w którym zostaną zapisane dane: ");
            int parsedInt;
            while (!int.TryParse(AnsiConsole.Ask<string>("Podaj wartość k: "), out parsedInt))
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[red]Podana wartość nie jest liczbą![/]");
            }
            kValue = parsedInt;
        }
        else // Dla k-NN z podanymi argumentami do programu
        {
            inputDataPath = _args[0];
            outputDataFolderPath = _args[1];
            if (!int.TryParse(_args[2], out var parsedInt))
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine("[red]Podana wartość nie jest liczbą![/]");
                while (!int.TryParse(AnsiConsole.Ask<string>("Podaj wartość k: "), out parsedInt))
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[red]Podana wartość nie jest liczbą![/]");
                }

                kValue = parsedInt;
            }
            else
                kValue = parsedInt;
        }
        
        // Sprawdzanie poprawności ścieżki do pliku z danymi
        while (!File.Exists(inputDataPath))
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[red]Podana ścieżka jest niepoprawna[/]");
            inputDataPath = AnsiConsole.Ask<string>("Podaj ponownie absolutną ścieżkę do pliku z danymi iris: ");
        }
        
        // Sprawdzanie poprawności ścieżki do pliku z danymi
        while (!Directory.Exists(outputDataFolderPath))
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[red]Podana ścieżka jest niepoprawna[/]");
            outputDataFolderPath = AnsiConsole.Ask<string>("Podaj ponownie absolutną ścieżkę do folderu, w którym zostaną zapisane dane: ");
        }
        
        Thread.Sleep(1000);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[green]Poprawnie podana ścieżka do pliku z danymi: [/][bold]{0}[/]", inputDataPath);
        Thread.Sleep(1000);
        AnsiConsole.MarkupLine("[green]Poprawnie podana ścieżka do folderu wynikowego: [/][bold]{0}[/]", outputDataFolderPath);
        Thread.Sleep(1000);
        AnsiConsole.MarkupLine("[green]Poprawnie podana wartość k: [/][bold]{0}[/]", kValue);
        Thread.Sleep(2000);
        AnsiConsole.Clear();
        
        Program.InitKnnExecutor(inputDataPath, outputDataFolderPath, kValue);
        LoadView(AppViews.MainMenu);
    }
    
    
}