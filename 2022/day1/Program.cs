using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Initialize input:
List<int> caloriesPerElf = new();

// Parse input:
StreamReader reader = File.OpenText("input.txt");
string? line;
int counter = 0;
int calories = 0;
while ((line = reader.ReadLine()) != null)
{
    if (int.TryParse(line, out var itemCal))
    {
        calories += itemCal;
    }
    else
    {
        counter++;
        Console.WriteLine($"{counter}: {calories} Cal");
        caloriesPerElf.Add(calories);
        calories = 0;
    }
}
// We add the last elf:
caloriesPerElf.Add(calories);

// Get index of
var maxCal = caloriesPerElf.Max();
Console.WriteLine($"The elf with the most hoarded calories: {caloriesPerElf.IndexOf(caloriesPerElf.Max()) + 1} ({maxCal})");

// Elfs with the most calories:
var sortedCals = caloriesPerElf.OrderByDescending(cal => cal).ToList();
Console.WriteLine($"The three riches elves have a total of {sortedCals.Take(3).Sum()} calories");
