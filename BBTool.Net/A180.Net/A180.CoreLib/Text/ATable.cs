using A180.CoreLib.Text.Extensions;

namespace A180.CoreLib.Text;

public static class ATable
{
    public static void ShowTable(List<string> header, List<List<string>> rows, List<int>? minLens = null,
        AStrings.AlignOption option = AStrings.AlignOption.Left)
    {
        const string lsep = "| ";
        const string sep = " | ";
        const string rsep = " |";

        const int lsepRight = 1;
        const int sepLeft = 1;
        const int sepRight = 1;
        const int rsepLeft = 1;

        // 检查字段数是否一致
        var columns = header.Count;
        if (columns == 0)
        {
            return;
        }

        foreach (var row in rows)
        {
            if (row.Count != columns)
            {
                return;
            }
        }

        var lens = new List<int>();

        // 填充 0
        for (int i = 0; i < columns; ++i)
        {
            lens.Add(0);
        }

        // 统计最长长度
        for (int j = 0; j < columns; ++j)
        {
            lens[j] = Math.Max(lens[j], header[j].WideLength());
        }

        for (int i = 0; i < rows.Count; ++i)
        {
            for (int j = 0; j < columns; ++j)
            {
                lens[j] = Math.Max(lens[j], rows[i][j].WideLength());
            }
        }

        // 不小于最小长度
        if (minLens != null)
        {
            for (int j = 0; j < Math.Min(columns, minLens.Count); ++j)
            {
                lens[j] = Math.Max(lens[j], minLens[j]) * 2;
            }
        }

        // 计算横线
        var rowLine = "";
        {
            rowLine += "+" + new string('-', lsepRight + lens.First());
            for (int j = 1; j < columns; ++j)
            {
                rowLine += new string('-', sepLeft) + "+" + new string('-', lens[j] + sepRight);
            }

            rowLine += new string('-', rsepLeft) + "+";
        }

        var printRow = (List<string> row) =>
        {
            // 顶框
            Console.Write(lsep);

            // 内容
            for (int i = 0; i < row.Count - 1; ++i)
            {
                Console.Write(row[i].Align(lens[i], option));
                Console.Write(sep);
            }

            Console.Write(row.Last().Align(lens.Last(), option));

            // 底框
            Console.Write(rsep);

            Console.WriteLine();
        };

        var printOutline = () => { Console.WriteLine(rowLine); };

        // Top
        printOutline();

        // Title
        printRow(header);
        printOutline();

        foreach (var row in rows)
        {
            printRow(row);
            printOutline();
        }
    }
}