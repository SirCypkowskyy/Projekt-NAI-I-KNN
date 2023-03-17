using NAI.Projekt.KNN_ConsoleApp_s24759.Algorithms;
using NAI.Projekt.KNN_ConsoleApp_s24759.Structures;
using Spectre.Console;

namespace NAI.Projekt.KNN_ConsoleApp_s24759;

public class KnnExecutor
{
    public KNN Knn { get; set; }
    
    public KnnExecutor(KNN knn)
    {
        Knn = knn;
    }

    public void ShowInputData()
    {
        Knn.ShowKnnModelLive();
        AnsiConsole.MarkupLine("[bold yellow]Dane treningowe zostały wyświetlone. Naciśnij \"enter\" aby wrócić do menu głównego[/]");
        Console.ReadLine();
    }

    
}