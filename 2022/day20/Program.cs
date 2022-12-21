using Day20;
using Utils;
// Parse input:
var elements = File.OpenText("test_input.txt")
    .GetLines()
    .Select((line, id) => new Element
    {
        Value = int.Parse(line),
        Position = id,
        InitialPosition = id
    })
    .ToList();

// Perform mixing:
var mixed = Decryption.MixDecrypt(elements);

Console.WriteLine(
    string.Join(
        ", ",
        mixed.OrderBy(item => item.Position)
            .Select(element => element.Value)
    )
);