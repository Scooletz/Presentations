// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello");

await Task.Yield();

await File.WriteAllTextAsync("test.txt", "test");
Console.WriteLine("File written");

await Task.Yield();

File.Delete("test.txt");
Console.WriteLine("File deleted");
