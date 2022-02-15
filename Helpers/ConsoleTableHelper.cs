using System.Text;

namespace TeslaLightShow.Helpers;

// TODO: Simplify
public class ConsoleTableHelper
{
    private const string TopLeftJoint = "┌";
    private const string TopRightJoint = "┐";
    private const string BottomLeftJoint = "└";
    private const string BottomRightJoint = "┘";
    private const string TopJoint = "┬";
    private const string BottomJoint = "┴";
    private const string LeftJoint = "├";
    private const string MiddleJoint = "┼";
    private const string RightJoint = "┤";
    private const char HorizontalLine = '─';
    private const string VerticalLine = "│";
    private string[] headers = Array.Empty<string>();
    private readonly List<string[]> rows = new();

    private int Padding { get; set; } = 1;
    private bool HeaderTextAlignRight { get; set; } = false;
    private bool RowTextAlignRight { get; set; } = false;

    public void SetHeaders(params string[] newHeaders)
    {
        this.headers = newHeaders;
    }

    public void AddRow(params string[] row)
    {
        this.rows.Add(row);
    }

    public void ClearRows()
    {
        this.rows.Clear();
    }

    private int[] GetMaxCellWidths(List<string[]> table)
    {
        int maximumColumns = table.Select(row => row.Length).Prepend(0).Max();

        int[] maximumCellWidths = new int[maximumColumns];
        for (int i = 0; i < maximumCellWidths.Count(); i++)
        {
            maximumCellWidths[i] = 0;
        }

        int paddingCount = 0;
        if (this.Padding > 0)
        {
            //Padding is left and right
            paddingCount = this.Padding * 2;
        }

        foreach (string[] row in table)
        {
            for (int i = 0; i < row.Length; i++)
            {
                int maxWidth = row[i].Length + paddingCount;

                if (maxWidth > maximumCellWidths[i])
                {
                    maximumCellWidths[i] = maxWidth;
                }
            }
        }

        return maximumCellWidths;
    }

    private StringBuilder CreateTopLine(IReadOnlyList<int> maximumCellWidths, int rowColumnCount, StringBuilder formattedTable)
    {
        for (int i = 0; i < rowColumnCount; i++)
        {
            switch (i)
            {
                case 0 when i == rowColumnCount - 1:
                    formattedTable.AppendLine(
                        $"{TopLeftJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}{TopRightJoint}");
                    break;
                case 0:
                    formattedTable.Append(
                        $"{TopLeftJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}");
                    break;
                default:
                {
                    if (i == rowColumnCount - 1)
                    {
                        formattedTable.AppendLine(
                            $"{TopJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}{TopRightJoint}");
                    }
                    else
                    {
                        formattedTable.Append(
                            $"{TopJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}");
                    }

                    break;
                }
            }
        }

        return formattedTable;
    }

    private StringBuilder CreateBottomLine(IReadOnlyList<int> maximumCellWidths, int rowColumnCount, StringBuilder formattedTable)
    {
        for (int i = 0; i < rowColumnCount; i++)
        {
            switch (i)
            {
                case 0 when i == rowColumnCount - 1:
                    formattedTable.AppendLine($"{BottomLeftJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}{BottomRightJoint}");
                    break;
                case 0:
                    formattedTable.Append($"{BottomLeftJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}");
                    break;
                default:
                {
                    if (i == rowColumnCount - 1)
                    {
                        formattedTable.AppendLine($"{BottomJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}{BottomRightJoint}");
                    }
                    else
                    {
                        formattedTable.Append($"{BottomJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}");
                    }

                    break;
                }
            }
        }

        return formattedTable;
    }

    private StringBuilder CreateValueLine(IReadOnlyList<int> maximumCellWidths, string[] row, bool alignRight, StringBuilder formattedTable)
    {
        int cellIndex = 0;
        int lastCellIndex = row.Length - 1;

        string paddingString = string.Empty;
        if (this.Padding > 0)
        {
            paddingString = string.Concat(Enumerable.Repeat(' ', this.Padding));
        }

        foreach (string column in row)
        {
            int restWidth = maximumCellWidths[cellIndex];
            if (this.Padding > 0)
            {
                restWidth -= this.Padding * 2;
            }

            string cellValue = alignRight ? column.PadLeft(restWidth, ' ') : column.PadRight(restWidth, ' ');

            switch (cellIndex)
            {
                case 0 when cellIndex == lastCellIndex:
                    formattedTable.AppendLine($"{VerticalLine}{paddingString}{cellValue}{paddingString}{VerticalLine}");
                    break;
                case 0:
                    formattedTable.Append($"{VerticalLine}{paddingString}{cellValue}{paddingString}");
                    break;
                default:
                {
                    if (cellIndex == lastCellIndex)
                    {
                        formattedTable.AppendLine($"{VerticalLine}{paddingString}{cellValue}{paddingString}{VerticalLine}");
                    }
                    else
                    {
                        formattedTable.Append($"{VerticalLine}{paddingString}{cellValue}{paddingString}");
                    }

                    break;
                }
            }

            cellIndex++;
        }

        return formattedTable;
    }

    private StringBuilder CreateSeparatorLine(IReadOnlyList<int> maximumCellWidths, int previousRowColumnCount, int rowColumnCount, StringBuilder formattedTable)
    {
        int maximumCells = Math.Max(previousRowColumnCount, rowColumnCount);

        for (int i = 0; i < maximumCells; i++)
        {
            switch (i)
            {
                case 0 when i == maximumCells - 1:
                    formattedTable.AppendLine($"{LeftJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}{RightJoint}");
                    break;
                case 0:
                    formattedTable.Append(
                        $"{LeftJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}");
                    break;
                default:
                {
                    if (i == maximumCells - 1)
                    {
                        if (i > previousRowColumnCount)
                        {
                            formattedTable.AppendLine($"{TopJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}{TopRightJoint}");
                        }
                        else if (i > rowColumnCount)
                        {
                            formattedTable.AppendLine($"{BottomJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}{BottomRightJoint}");
                        }
                        else if (i > previousRowColumnCount - 1)
                        {
                            formattedTable.AppendLine($"{MiddleJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}{TopRightJoint}");
                        }
                        else if (i > rowColumnCount - 1)
                        {
                            formattedTable.AppendLine($"{MiddleJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}{BottomRightJoint}");
                        }
                        else
                        {
                            formattedTable.AppendLine($"{MiddleJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}{RightJoint}");
                        }
                    }
                    else
                    {
                        if (i > previousRowColumnCount)
                        {
                            formattedTable.Append($"{TopJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}");
                        }
                        else if (i > rowColumnCount)
                        {
                            formattedTable.Append($"{BottomJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}");
                        }
                        else
                        {
                            formattedTable.Append($"{MiddleJoint}{string.Empty.PadLeft(maximumCellWidths[i], HorizontalLine)}");
                        }
                    }

                    break;
                }
            }
        }

        return formattedTable;
    }

    public override string ToString()
    {
        List<string[]> table = new();

        bool firstRowIsHeader = false;
        if (this.headers?.Any() == true)
        {
            table.Add(this.headers);
            firstRowIsHeader = true;
        }

        if (this.rows?.Any() == true)
        {
            table.AddRange(this.rows);
        }

        if (!table.Any())
        {
            return string.Empty;
        }

        StringBuilder formattedTable = new();

        string[] previousRow = table.FirstOrDefault() ?? Array.Empty<string>();
        string[] nextRow = table.FirstOrDefault() ?? Array.Empty<string>();

        int[] maximumCellWidths = this.GetMaxCellWidths(table);

        formattedTable = this.CreateTopLine(maximumCellWidths, nextRow?.Length ?? 0, formattedTable);

        int rowIndex = 0;
        int lastRowIndex = table.Count - 1;

        for (int i = 0; i < table.Count; i++)
        {
            string[] row = table[i];

            bool align = this.RowTextAlignRight;
            if (i == 0 && firstRowIsHeader)
            {
                align = this.HeaderTextAlignRight;
            }

            formattedTable = this.CreateValueLine(maximumCellWidths, row, align, formattedTable);

            previousRow = row;

            if (rowIndex != lastRowIndex)
            {
                nextRow = table[rowIndex + 1];
                formattedTable = this.CreateSeparatorLine(maximumCellWidths, previousRow.Count(), nextRow.Count(),
                    formattedTable);
            }

            rowIndex++;
        }

        formattedTable = this.CreateBottomLine(maximumCellWidths, previousRow?.Count() ?? 0, formattedTable);

        return formattedTable.ToString();
    }
}
