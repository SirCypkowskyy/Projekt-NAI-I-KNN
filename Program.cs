using System.Globalization;
using System.Text.RegularExpressions;
using NAI.Projekt.KNN_ConsoleApp_s24759.Algorithms;
using NAI.Projekt.KNN_ConsoleApp_s24759.Controllers;
using NAI.Projekt.KNN_ConsoleApp_s24759.Structures;
using Spectre.Console;

namespace NAI.Projekt.KNN_ConsoleApp_s24759;

static class Program   
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
                Regex.Replace(rowValues.Last(), "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled)
                )).ToList();
        

        KnnAlgorithm = new KNN(k, irisData, new List<KnnVector<double>>());
        OutputFolderPath = outputFolderPath;
        
        KnnAlgorithm.ShowKnnTrainingData();
        AnsiConsole.MarkupLine("[bold yellow]Dane treningowe zostały wyświetlone. Naciśnij \"enter\" aby wrócić do menu głównego[/]");
        Console.ReadLine();
    }

    public static void InitKnnTesting(string testedDataFilePath)
    {
        var txtIrisData = File.ReadAllText(testedDataFilePath);
        var irisDataRows = txtIrisData.Split('\n');
        
        var irisDataToTest = (from row in irisDataRows
                select row.Split(',')
                into rowValues
                let rowPointValuesDouble = rowValues.Take(rowValues.Length - 1)
                    .Select(element => double.Parse(element, NumberStyles.Any, CultureInfo.InvariantCulture))
                let decisiveAttribute = rowValues.Last() != rowPointValuesDouble.Last().ToString()
                    ? rowValues.Last()
                    : null
                select (decisiveAttribute is null
                        ? new KnnVector<double>(rowPointValuesDouble, isTrainingVector: true)
                        : new KnnVector<double>(rowPointValuesDouble, decisiveAttribute, isTrainingVector: true)
                    )
            );

        AnsiConsole.MarkupLine("Testowanie modelu k-NN dla zbioru treningowego...");
        KnnAlgorithm.TestData(in irisDataToTest, checkIntegrityWithAssignedClass: true);
        AnsiConsole.MarkupLine("[bold yellow]Testowanie modelu k-NN dla zbioru treningowego zakończone. Naciśnij \"enter\" aby wrócić do menu głównego[/]");
        Console.ReadLine();
    }
    
    
    public static bool IsKnnModelCreated()
    {
        return KnnAlgorithm is not null && KnnAlgorithm.TrainSet is not null && KnnAlgorithm.TrainSet.Count > 0;
    }
    
}