// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Attributes;

//var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
//Console.ReadKey();

BenchmarkRunner.Run(typeof(Program).Assembly);

// https://stackoverflow.com/questions/248603/natural-sort-order-in-c-sharp

public class ToDo {
    public int Id { get; set; }
    public int Order { get; set; }
}

public class Benchmarks
{
    private List<string?> keys1 = new();
    private List<string?> keys2 = new();
    private List<string?> keys3 = new();
    private List<string?> keys4 = new();


    //private readonly List<ToDo> things_ToDo1 = new();
    //private readonly List<ToDo> things_ToDo2 = new();


    //private readonly List<string> things1 = new();
    //private readonly List<string> things2 = new();
    //private readonly List<string> things2b = new();
    //private readonly List<string> things3 = new();
    //private readonly List<string> things4 = new();

    public class ToDoComparer : IComparer<ToDo>
    {
        public int Compare(ToDo? x, ToDo? y)
        {
            if (x == null) return -1;
            if (y == null) return 1;
            if (x.Order < y.Order) return -1;
            if (x.Order > y.Order) return 1;
            return 0; //Equal
        }
    }

    public Benchmarks()
    {
        var rand = new Random();


        var keys = Enumerable.Range(1, 10000).OrderBy(i => rand.Next()).Select(x => (x / 2 == 0 ? x.ToString() : null) ).ToList();
        keys1 = keys;
        keys2 = keys;
        keys3 = keys;
        keys4 = keys;



        //var todos = Enumerable.Range(1, 10000).OrderBy(i => rand.Next()).Select(x => new ToDo() { Id = x, Order = x / 10 }).ToList();
        //things_ToDo1 = todos;
        //things_ToDo2 = todos;



        //var numbers = Enumerable.Range(1, 10000).OrderBy(i => rand.Next()).Select(x => x.ToString()).ToList();
        //things1 = numbers;
        //things2 = numbers;
        //things2b = numbers;
        //things3 = numbers;
        //things4 = numbers;
    }




    [Benchmark]
    public void Test_keys1()
    {
        //List operation
        keys1.RemoveAll(key => key is null);
    }

    [Benchmark]
    public void Test_keys2()
    {
        //LINQ operation
        keys2 = keys2.Where(x => x is null).ToList();
    }

    //[Benchmark]
    //public void Test_keys3()
    //{
    //    //Foreach Loop + List operation
    //    foreach (var key in keys3)
    //        if (key is null)
    //            keys3.Remove(key);
    //}

    //[Benchmark]
    //public void Test_keys4()
    //{
    //    //Reverse for Loop + List operation
    //    for (int i = keys4.Count - 1; i > 0; i++)
    //        if (keys4[i] is null)
    //            keys4.RemoveAt(i);
    //}




    //[Benchmark]
    //public void Test_todo1()
    //{
    //    things_ToDo1.OrderBy(x => x.Order).ToList();
    //}

    //[Benchmark]
    //public void Test_todo2()
    //{
    //    things_ToDo2.Sort(new ToDoComparer());
    //}





    //[Benchmark]
    //public void Test1()
    //{
    //    things1.Sort(new AgeStringComparer());
    //}

    //[Benchmark]
    //public void Test2()
    //{
    //    things2.Sort(new AgeStringComparer_improvement_attempt_1());
    //}

    //[Benchmark]
    //public void Test2b()
    //{
    //    things2b.Sort(new AgeStringComparer_improvement_attempt_1b());
    //}

    /*
    [Benchmark]
    public void Test3()
    {
        things3.Sort(new AgeStringComparer_improvement_attempt_2());
    }

    [Benchmark]
    public void Test4()
    {
        things4.Sort(new AgeStringComparer_improvement_attempt_2());
    }
    */
}

/// <summary>
/// Sorts integer strings the same way as integers.
/// This is VERY fast.
/// By Niklas Nielsen?
/// <list type="table">
///   <listheader>
///     <term>Method</term>
///     <term>Mean</term>
///     <term>Error</term>
///     <term>StdDev</term>
///   </listheader>
///   <item>
///     <term>Test1</term>
///     <term>661.7 us</term>
///     <term>2.05 us</term>
///     <term>1.92 us</term>
///   </item>
/// </list>
/// </summary>
/// |  Test1 |   667.3 us |   4.53 us |   4.23 us |
public class AgeStringComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == null) return -1;
        if (y == null) return 1;
        //If either number has more digits that the other, its higher
        if (x.Length < y.Length) return -1;
        if (x.Length > y.Length) return 1;

        //Perfectly safe - if we got here x.Length == y.Length
        for (int i = 0; i < x.Length; i++)
        {
            int charX = x[i];
            int charY = y[i];
            if (charX < charY) return -1;
            if (charX > charY) return 1;
        }
        return 0; //Equal
    }
}

/// <summary>
/// Sorts integer strings the same way as integers.
/// This is VERY fast.
/// Made by Niklas Nielsen ?
/// Improvement attempt 1 by Allan Rolschau.
/// <list type="table">
///   <listheader>
///     <term>Method</term>
///     <term>Mean</term>
///     <term>Error</term>
///     <term>StdDev</term>
///   </listheader>
///   <item>
///     <term>Test2</term>
///     <term>660.5 us</term>
///     <term>2.64 us</term>
///     <term>2.46 us</term>
///   </item>
/// </list>
/// </summary>
/// |  Test2 |   656.3 us |   2.58 us |   2.41 us |
public sealed class AgeStringComparer_improvement_attempt_1 : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == null) return -1;
        if (y == null) return 1;
        //If either number has more digits that the other, its higher
        if (x.Length < y.Length) return -1;
        if (x.Length > y.Length) return 1;

        //Perfectly safe - if we got here x.Length == y.Length
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] < y[i]) return -1;
            if (x[i] > y[i]) return 1;
        }
        return 0; //Equal
    }
}

public sealed class AgeStringComparer_improvement_attempt_1b : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == null) return -1;
        if (y == null) return 1;
        //If either number has more digits that the other, its higher
        if (x.Length < y.Length) return -1;
        if (x.Length > y.Length) return 1;

        //Perfectly safe - if we got here x.Length == y.Length
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] < y[i]) return -1;
            if (x[i] > y[i]) return 1;
        }
        return 0; //Equal
    }
}

/// <summary>
/// Sorts integer strings the same way as integers.
/// Improvement attempt 2 by Allan Rolschau.
/// <list type="table">
///   <listheader>
///     <term>Method</term>
///     <term>Mean</term>
///     <term>Error</term>
///     <term>StdDev</term>
///   </listheader>
///   <item>
///     <term>Test3</term>
///     <term>7,660.3 us</term>
///     <term>132.23 us</term>
///     <term>117.22 us</term>
///   </item>
/// </list>
/// </summary>
/// |  Test3 | 7,552.6 us | 112.90 us | 100.08 us |
public sealed class AgeStringComparer_improvement_attempt_2 : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == null) return -1;
        if (y == null) return 1;
        //If either number has more digits that the other, its higher
        if (x.Length < y.Length) return -1;
        if (x.Length > y.Length) return 1;

        ReadOnlySpan<char> spanX = x.AsSpan();
        ReadOnlySpan<char> spanY = y.AsSpan();
        //Perfectly safe - if we got here x.Length == y.Length
        for (int i = 0; i < x.Length; i++)
        {
            int charX = int.Parse(spanX.Slice(i, 1));
            int charY = int.Parse(spanY.Slice(i, 1));
            if (charX < charY) return -1;
            if (charX > charY) return 1;
        }
        return 0; //Equal
    }
}

/// <summary>
/// Sorts integer strings the same way as integers.
/// Improvement attempt 3 by Allan Rolschau.
/// <list type="table">
///   <listheader>
///     <term>Method</term>
///     <term>Mean</term>
///     <term>Error</term>
///     <term>StdDev</term>
///   </listheader>
///   <item>
///     <term>Test4</term>
///     <term>7,472.4 us</term>
///     <term>51.09 us</term>
///     <term>47.79 us</term>
///   </item>
/// </list>
/// </summary>
/// |  Test4 | 7,842.6 us | 156.50 us | 146.39 us |
public sealed class AgeStringComparer_improvement_attempt_3 : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == null) return -1;
        if (y == null) return 1;
        //If either number has more digits that the other, its higher
        if (x.Length < y.Length) return -1;
        if (x.Length > y.Length) return 1;

        //Perfectly safe - if we got here x.Length == y.Length
        for (int i = 0; i < x.Length; i++)
        {
            int charX = x.AsSpan()[i];
            int charY = y.AsSpan()[i];
            if (charX < charY) return -1;
            if (charX > charY) return 1;
        }
        return 0; //Equal
    }
}