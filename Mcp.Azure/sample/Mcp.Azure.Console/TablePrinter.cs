using static System.Console;

namespace Mcp.Azure.Console;

public class TablePrinter<T>
{
    private readonly IReadOnlyList<ColumnDefinition<T>> _columns;
    private readonly string _title;

    public TablePrinter(string title, IReadOnlyList<ColumnDefinition<T>> columns)
    {
        _title = title;
        _columns = columns;
    }

    public void Print(IEnumerable<T> items)
    {
        WriteLine($"\n{_title}:");

        // Calculate column widths based on data
        var columnWidths = CalculateColumnWidths(items);

        // Print header
        var header = "┌" + string.Join("┬", columnWidths.Select(w => new string('─', w))) + "┐";
        var headerRow = "│" + string.Join("│", _columns.Zip(columnWidths, (c, w) => c.Title.PadRight(w))) + "│";
        var separator = "├" + string.Join("┼", columnWidths.Select(w => new string('─', w))) + "┤";
        var footer = "└" + string.Join("┴", columnWidths.Select(w => new string('─', w))) + "┘";

        WriteLine(header);
        WriteLine(headerRow);
        WriteLine(separator);

        // Print rows
        foreach (var item in items)
        {
            var row = "│" + string.Join("│", _columns.Zip(columnWidths, (c, w) => 
                PadRight(c.GetValue(item), w))) + "│";
            WriteLine(row);
        }

        WriteLine(footer);
    }

    private int[] CalculateColumnWidths(IEnumerable<T> items)
    {
        var widths = new int[_columns.Count];
        
        // Initialize with header widths
        for (int i = 0; i < _columns.Count; i++)
        {
            widths[i] = _columns[i].Title.Length;
        }

        // Update with data widths
        foreach (var item in items)
        {
            for (int i = 0; i < _columns.Count; i++)
            {
                var value = _columns[i].GetValue(item);
                widths[i] = Math.Max(widths[i], value?.Length ?? 0);
            }
        }

        // Add padding
        for (int i = 0; i < widths.Length; i++)
        {
            widths[i] += 2; // Add padding on both sides
        }

        return widths;
    }

    private string PadRight(string? value, int length) => 
        (value ?? string.Empty).PadRight(length).Substring(0, length);
}

public class ColumnDefinition<T>
{
    public string Title { get; }
    public Func<T, string?> GetValue { get; }

    public ColumnDefinition(string title, Func<T, string?> getValue)
    {
        Title = title;
        GetValue = getValue;
    }
} 