using NAI.Projekt.KNN_ConsoleApp_s24759.Algorithms;
using NAI.Projekt.KNN_ConsoleApp_s24759.Structures;
using Spectre.Console;

namespace NAI.Projekt.KNN_ConsoleApp_s24759;

public class KnnExecutor
{
    public KNN Knn { get; set; }
    
    private string _outputFolderPath;
    
    public KnnExecutor(KNN knn, string outputFolderPath)
    {
        Knn = knn;
        _outputFolderPath = outputFolderPath;
    }

    public void ShowInputData()
    {
        Knn.ShowKnnModelLive();
        AnsiConsole.MarkupLine("[bold yellow]Dane treningowe zostały wyświetlone. Naciśnij \"enter\" aby wrócić do menu głównego[/]");
        Console.ReadLine();
    }

    public void TestData(in IEnumerable<KnnVector<double>> trainingData)
    {
        AnsiConsole.MarkupLine("Testowanie modelu k-NN dla zbioru treningowego...");
        Knn.TestData(in trainingData, checkIntegrityWithAssignedClass: true);
        AnsiConsole.MarkupLine("[bold yellow]Testowanie modelu k-NN dla zbioru treningowego zakończone. Naciśnij \"enter\" aby wrócić do menu głównego[/]");
        Console.ReadLine();
    }
    
}