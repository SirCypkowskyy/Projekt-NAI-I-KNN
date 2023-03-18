using NAI.Projekt.KNN_ConsoleApp_s24759.Structures;
using Spectre.Console;

namespace NAI.Projekt.KNN_ConsoleApp_s24759.Controllers;

public class AppController
{
    private string[] _args;
    
    private List<MainMenuChoice> _mainMenuChoices = new List<MainMenuChoice>(MainMenuChoice.GetMainMenuChoices());
    
    private bool _isKnnModelCreated;
    
    public AppController(string[] args)
    {
        _args = args;

        DisplayAppEntry();
        LoadView(AppViews.MainMenu);
    }

    
    public void LoadView(AppViews view)
    {
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
                VisualiseKnnModelTesting();
                break;
            case AppViews.Credits:
                VisualizeCredits();
                break;
            case AppViews.Exit:
                break;
            case AppViews.ShowData:
                VisualizeGeneratedData();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(view), view, "Nieznany widok");
        }
    }
    private void VisualizeGeneratedData()
    {
        if (!_isKnnModelCreated)
        {
            AnsiConsole.MarkupLine("[bold red]Nie można wyświetlić danych treningowych, ponieważ nie został stworzony model k-NN[/]");
            AnsiConsole.MarkupLine("[bold yellow]Naciśnij enter, aby wrócić do menu głównego[/]");
            Console.ReadLine();
            LoadView(AppViews.MainMenu);
            return;
        }

        AnsiConsole.MarkupLine("[bold green]Wyświetlanie danych treningowych[/]");
        Program.KnnAlgorithm.ShowKnnTrainingData();
        Console.WriteLine();
        AnsiConsole.MarkupLine("[bold yellow]Naciśnij enter, aby wrócić do menu głównego[/]");
        
        Console.ReadLine();
        LoadView(AppViews.MainMenu);
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
            _mainMenuChoices[0].Name = "Zmień/Edytuj model k-NN";
            _mainMenuChoices[1].Name = "Testowanie modelu k-NN [bold green](gotowe do użycia)[/]";
            _mainMenuChoices[2].Name = "Wyświetl dane treningowe [bold green](gotowe do użycia)[/]";
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
        AnsiConsole.Write(Align.Center(new Markup("[bold red]Autorzy[/]\n"), VerticalAlignment.Middle));
        Thread.Sleep(1000);
        AnsiConsole.Write(Align.Center(new Markup("[bold yellow]Cyprian Gburek[/]"), VerticalAlignment.Middle));
        Thread.Sleep(1000);
        AnsiConsole.Write(Align.Center(new Markup("[bold red]PJATK[/]"), VerticalAlignment.Middle));
        Thread.Sleep(1000);
        AnsiConsole.Write(Align.Center(new Markup("[bold green]s24759, 17c[/]"), VerticalAlignment.Middle));
        Thread.Sleep(1000);
        AnsiConsole.Write(Align.Center(new Markup("[bold link=https://github.com/SirCypkowskyy]Github[/]([underline]kliknij trzymając ctrl[/])"), VerticalAlignment.Middle));
        
        AnsiConsole.WriteLine("Naciśnij enter, aby wrócić do menu głównego");
        Console.ReadLine();
        LoadView(AppViews.MainMenu);
    }
    private void VisualiseKnnInit()
    {
        AnsiConsole.MarkupLine("Witaj w programie do klasyfikacji danych metodą k-NN");
        Thread.Sleep(1000);
       
        string userChoice;
        var userWantsToResetKnnModel = false;

        // Jeśli model k-NN został już stworzony, to wyświetl możliwości związane z nim
        if (_isKnnModelCreated)
        {
            AnsiConsole.Clear();
            Thread.Sleep(1000);
            AnsiConsole.MarkupLine("[bold yellow]Model k-NN został już stworzony[/]");
            Thread.Sleep(300);
            AnsiConsole.MarkupLine("Wybierz możliwe opcje:");
            Thread.Sleep(300);
            userChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Wybierz opcję [grey](Naciśnij [u]enter[/] aby wybrać daną opcję, używaj strzałek do poruszania się po wyborach)[/]:")
                    .AddChoices(new[] {
                        "Wróć do menu głównego",
                        "Zresetuj model k-NN"
                    })
            );
            
            if(userChoice == "Wróć do menu głównego")
            {
                LoadView(AppViews.MainMenu);
                return;
            }
            _isKnnModelCreated = false;
        }
        
        var userChoices = new List<string>();
        
        if(_args is not null && _args.Length >= 3)
            userChoices.Add("Wczytaj dane z argumentów programu");

        userChoices.Add("Podaj dane wejściowe programu przez konsolę");
        
        AnsiConsole.Clear();
        
        AnsiConsole.MarkupLine("Wybierz sposób wprowadzania danych wejściowych programu:");
        userChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Wybierz opcję [grey](Naciśnij [u]enter[/] aby wybrać daną opcję, używaj strzałek do poruszania się po wyborach)[/]:")
                .AddChoices(userChoices)
        );
        
        string inputDataPath;
        string outputDataFolderPath;
        int kValue;
        int parsedInt;
        
        switch (userChoices.IndexOf(userChoice))
        {
            case 0:
                if (_args is null || _args.Length < 3)
                {
                    AnsiConsole.MarkupLine("[bold red]Nie można wczytać danych z argumentów programu, ponieważ nie zostały podane[/]");
                    AnsiConsole.Clear();
                    goto case 1;
                }
                
                inputDataPath = _args[0];
                outputDataFolderPath = _args[1];
                if (!int.TryParse(_args[2], out parsedInt))
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
                break;
            case 1:
                inputDataPath = AnsiConsole.Ask<string>("Podaj absolutną ścieżkę do pliku z danymi iris: ");
                outputDataFolderPath = AnsiConsole.Ask<string>("Podaj absolutną ścieżkę do folderu, w którym zostaną zapisane dane: ");
                while (!int.TryParse(AnsiConsole.Ask<string>("Podaj wartość k: "), out parsedInt))
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[red]Podana wartość nie jest liczbą![/]");
                }
                kValue = parsedInt;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(userChoice), userChoice, "Nieznana opcja");
        }
        
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("Sprawdzanie poprawności ścieżek...");
        Thread.Sleep(1000);
        
        
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
        
        Thread.Sleep(500);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[green]Poprawnie podana ścieżka do pliku z danymi: [/][bold]{0}[/]", inputDataPath);
        Thread.Sleep(500);
        AnsiConsole.MarkupLine("[green]Poprawnie podana ścieżka do folderu wynikowego: [/][bold]{0}[/]", outputDataFolderPath);
        Thread.Sleep(500);
        AnsiConsole.MarkupLine("[green]Poprawnie podana wartość k: [/][bold]{0}[/]", kValue);
        Thread.Sleep(2000);
        AnsiConsole.Clear();
        
        Program.InitKnnStartingResources(inputDataPath, outputDataFolderPath, kValue);
        LoadView(AppViews.MainMenu);
    }

    private void LoadKnnTestWithFilePath(string path, bool checkIntegrityWithAssignedClass)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[green]Poprawnie podana ścieżka do pliku z danymi do przetestowania: [/][bold]{0}[/]", path);
        Thread.Sleep(2000);
        AnsiConsole.Clear();
        Program.InitKnnTesting(path, checkIntegrityWithAssignedClass);
    }

    private void LoadKnnTestWithInputData(IEnumerable<KnnVector<double>> inputTestData, bool checkIntegrityWithAssignedClass)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[green]Poprawnie podane dane do przetestowania[/]");
        Thread.Sleep(2000);
        AnsiConsole.Clear();
        Program.InitKnnTesting(inputTestData, checkIntegrityWithAssignedClass);
    }
    
    
    
    private void VisualiseKnnModelTesting()
    {
        if (!Program.IsKnnModelCreated())
        {
            AnsiConsole.MarkupLine("[red]Nie można wykonać testowania modelu k-NN, ponieważ nie został on wcześniej utworzony[/]");
            Thread.Sleep(2000);
            LoadView(AppViews.MainMenu);
            return;
        }
            
        var userChoices = new List<string>()
        {
            "Wczytaj dane do testowania z pliku ([bold yellow]ręczne wprowadzenie ścieżki[/])",
            "Wczytaj dane do testowania z konsoli"
        };
        
        var isTestDataPathProvided = _args.Length >= 4;
        if (isTestDataPathProvided)
            userChoices.Add("Wczytaj dane do testowania z pliku (argument programu)");

        string testDataPath;
        
        var userInputChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Wybierz opcję [grey](Naciśnij [u]enter[/] aby wybrać daną opcję, używaj strzałek do poruszania się po wyborach)[/]:").PageSize(10)
                .AddChoices(userChoices)
                .HighlightStyle(Style.Parse("bold red")
                )
        );

        bool shouldCompareWithDecisiveClass;
        switch (userInputChoice)
        {
            case "Wczytaj dane do testowania z pliku ([bold yellow]ręczne wprowadzenie ścieżki[/])":
                testDataPath = AnsiConsole.Ask<string>("Podaj absolutną ścieżkę do pliku z danymi do przetestowania: ");
                while (!File.Exists(testDataPath))
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[red]Podana ścieżka jest niepoprawna![/]");
                    testDataPath = AnsiConsole.Ask<string>("Podaj ponownie absolutną ścieżkę do pliku z danymi do przetestowania: ");
                }
                
                shouldCompareWithDecisiveClass = AnsiConsole.Confirm("Czy chcesz porównać wyniki z atrybutem decyzyjnym?");
                
                LoadKnnTestWithFilePath(testDataPath, shouldCompareWithDecisiveClass);
                break;
            case "Wczytaj dane do testowania z konsoli":
                AnsiConsole.Clear();
                var numberOfTestVectors = AnsiConsole.Ask<int>("Podaj ilość próbek do przetestowania: ");
                while (numberOfTestVectors <= 1)
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[red]Podana wartość nie jest liczbą dodatnią lub nie jest równa 1![/]");
                    numberOfTestVectors = AnsiConsole.Ask<int>("Podaj ilość próbek do przetestowania: ");
                }
                
                shouldCompareWithDecisiveClass = AnsiConsole.Confirm("Czy chcesz porównać wyniki z decyzyjną klasą? ");
                AnsiConsole.Clear();
                
                var testVectors = new List<KnnVector<double>>();

                for (var i = 0; i < numberOfTestVectors; i++)
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[green]Podaj dane próbki nr {0}[/]", i + 1);

                    var checkInputValidity = delegate(double x, string valueName) {
                        while (x <= 0)
                        {
                            AnsiConsole.Clear();
                            AnsiConsole.MarkupLine($"[red]Podana wartość {valueName} nie jest liczbą dodatnią![/]");
                            x = AnsiConsole.Ask<double>("Podaj ponownie wartość: ");
                        }
                    };
                    
                    var sepalLength = AnsiConsole.Ask<double>("Podaj długość działki (x): ");
                    checkInputValidity(sepalLength, "długości działki (x)");
                    var sepalWidth = AnsiConsole.Ask<double>("Podaj szerokość działki (y): ");
                    checkInputValidity(sepalWidth, "szerokości działki (y)");
                    var petalLength = AnsiConsole.Ask<double>("Podaj długość płatka (z): ");
                    checkInputValidity(petalLength, "długości płatka (z)");
                    var petalWidth = AnsiConsole.Ask<double>("Podaj szerokość płatka (a): ");
                    checkInputValidity(petalWidth, "szerokości płatka (a)");
                    var decisiveClass = string.Empty;
                    var writtenDownDecisiveClass = false;
                    while (shouldCompareWithDecisiveClass && !writtenDownDecisiveClass)
                    {
                        decisiveClass = AnsiConsole.Ask<string>("Podaj decyzyjną klasę próbki: ");
                        while (string.IsNullOrEmpty(decisiveClass))
                        {
                            AnsiConsole.Clear();
                            AnsiConsole.MarkupLine("[red]Podana wartość nie jest poprawną klasą![/]");
                            decisiveClass = AnsiConsole.Ask<string>("Podaj ponownie decyzyjną klasę próbki: ");
                        }
                        AnsiConsole.MarkupLine("[green]Podana decyzyjna klasa: [/][bold]{0}[/]", decisiveClass);
                        writtenDownDecisiveClass = AnsiConsole.Confirm("Czy chcesz potwierdzić decyzyjną klasę?");
                    }
                    
                    testVectors.Add(new KnnVector<double>(new[] {sepalLength, sepalWidth, petalLength, petalWidth}, decisiveClass));
                }
                
                LoadKnnTestWithInputData(testVectors, shouldCompareWithDecisiveClass);

                break;
            case "Wczytaj dane do testowania z pliku (argument programu)":
                // Sprawdzanie poprawności ścieżki do pliku z danymi
                testDataPath = _args[3];
                while (!File.Exists(testDataPath))
                {
                    AnsiConsole.Clear();
                    AnsiConsole.MarkupLine("[red]Podana ścieżka jest niepoprawna[/]");
                    testDataPath = AnsiConsole.Ask<string>("Podaj ponownie absolutną ścieżkę do pliku z danymi do przetestowania: ");
                }
                shouldCompareWithDecisiveClass = AnsiConsole.Confirm("Czy chcesz porównać wyniki z decyzyjną klasą? Klikmij [u]enter[/] aby potwierdzić, [u]spację[/] aby anulować");
                
                LoadKnnTestWithFilePath(testDataPath, shouldCompareWithDecisiveClass);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        LoadView(AppViews.MainMenu);

    }
    
    
}