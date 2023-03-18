using System.Globalization;
using NAI.Projekt.KNN_ConsoleApp_s24759.Algorithms;
using NAI.Projekt.KNN_ConsoleApp_s24759.Controllers;
using NAI.Projekt.KNN_ConsoleApp_s24759.Structures;
using Spectre.Console;

namespace NAI.Projekt.KNN_ConsoleApp_s24759;

internal static class Program   
{
    public static KNN KnnAlgorithm { get; set; }
    
    public static string OutputFolderPath { get; set; }
    
    static void Main(string[] args)
    {
        new AppController(args);
    }

    public static void InitKnnStartingResources(string inputFilePath, string outputFolderPath, int k)
    {
        var txtIrisData = File.ReadAllText(inputFilePath);
        var irisDataRows = txtIrisData.Split('\n');


        var irisData = (from row in irisDataRows
            select row.Split(',')
            into rowValues
            let rowPointValuesDouble = rowValues.Take(rowValues.Length - 1)
                .Select(element => double.Parse(element, NumberStyles.Any, CultureInfo.InvariantCulture))
            select new KnnVector<double>(
                rowPointValuesDouble, 
                ClearStringFromInvisibleCharacters(rowValues.Last())
                )).ToList();
        

        KnnAlgorithm = new KNN(k, irisData);
        OutputFolderPath = outputFolderPath;
        AnsiConsole.MarkupLine("[bold green]Zasoby zostały zainicjowane[/]");
        Thread.Sleep(500);
        var dataShow = AnsiConsole.Confirm("Czy chcesz wyświetlić dane treningowe?");

        AnsiConsole.Clear();
        if (dataShow)
        {
            KnnAlgorithm.ShowKnnTrainingData();
            AnsiConsole.MarkupLine("[bold green]Dane treningowe zostały wyświetlone[/]");
        }
        AnsiConsole.MarkupLine("[bold yellow]Naciśnij \"enter\" aby wrócić do menu głównego[/]");
        Console.ReadLine();
    }

    public static void InitKnnTesting(string testedDataFilePath, bool checkIntegrityWithAssignedClass)
    {
        var txtIrisData = File.ReadAllText(testedDataFilePath);
        var irisDataRows = txtIrisData.Split('\n');
        
        var irisDataToTest = (from row in irisDataRows
                select row.Split(',')
                into rowValues
                let rowPointValuesDouble = rowValues.Take(rowValues.Length - 1)
                    .Select(element => double.Parse(element, NumberStyles.Any, CultureInfo.InvariantCulture))
                let decisiveAttribute = rowValues.Last() != rowPointValuesDouble.Last().ToString()
                    ? ClearStringFromInvisibleCharacters(rowValues.Last())
                    : null
                select (decisiveAttribute is null
                        ? new KnnVector<double>(rowPointValuesDouble)
                        : new KnnVector<double>(rowPointValuesDouble, decisiveAttribute)
                    )
            );

        AnsiConsole.MarkupLine("Testowanie modelu k-NN dla zbioru treningowego...");
        KnnAlgorithm.TestData(in irisDataToTest, OutputFolderPath, checkIntegrityWithAssignedClass: checkIntegrityWithAssignedClass);
        AnsiConsole.MarkupLine("[bold yellow]Testowanie modelu k-NN dla zbioru treningowego zakończone. Naciśnij \"enter\" aby wrócić do menu głównego[/]");
        Console.ReadLine();
    }
    
    public static void InitKnnTesting(IEnumerable<KnnVector<double>> irisDataToTest, bool checkIntegrityWithAssignedClass)
    {
        AnsiConsole.MarkupLine("Testowanie modelu k-NN dla zbioru treningowego...");
        KnnAlgorithm.TestData(in irisDataToTest, OutputFolderPath, checkIntegrityWithAssignedClass: checkIntegrityWithAssignedClass);
        AnsiConsole.MarkupLine("[bold yellow]Testowanie modelu k-NN dla zbioru treningowego zakończone. Naciśnij \"enter\" aby wrócić do menu głównego[/]");
        Console.ReadLine();
    }
    
    private static string ClearStringFromInvisibleCharacters(string input)
    {
        return new string(input.Where(c => !char.IsControl(c)).ToArray());
    }
    
    public static bool IsKnnModelCreated()
    {
        return KnnAlgorithm is not null && KnnAlgorithm.TrainSet is not null && KnnAlgorithm.TrainSet.Count > 0;
    }
    
}