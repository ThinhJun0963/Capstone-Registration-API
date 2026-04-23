using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CapstoneProjectRegistration.Services.Utilities;

public static class DocxToMarkdownConverter
{
    public static string Convert(Stream docxStream)
    {
        using var document = WordprocessingDocument.Open(docxStream, false);
        var body = document.MainDocumentPart?.Document?.Body;
        if (body == null)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        foreach (var element in body.Elements())
        {
            if (element is Paragraph paragraph)
            {
                AppendParagraphAsMarkdown(sb, paragraph);
            }
            else if (element is Table table)
            {
                AppendTableAsMarkdown(sb, table);
            }
        }

        return sb.ToString();
    }

    private static void AppendParagraphAsMarkdown(StringBuilder sb, Paragraph paragraph)
    {
        var text = GetParagraphInnerText(paragraph);
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        var styleVal = paragraph.ParagraphProperties?.ParagraphStyleId?.Val?.Value;
        if (IsHeadingStyle(styleVal, 1))
        {
            sb.AppendLine("# " + text.Trim());
        }
        else if (IsHeadingStyle(styleVal, 2))
        {
            sb.AppendLine("## " + text.Trim());
        }
        else
        {
            sb.AppendLine(text.Trim());
        }

        sb.AppendLine();
    }

    private static bool IsHeadingStyle(string? styleVal, int level)
    {
        if (string.IsNullOrEmpty(styleVal))
        {
            return false;
        }

        // English Word built-ins
        if (level == 1)
        {
            return styleVal is "Heading1" or "Title" || styleVal.Contains("heading 1", StringComparison.OrdinalIgnoreCase);
        }

        if (level == 2)
        {
            return styleVal is "Heading2" or "Subtitle" || styleVal.Contains("heading 2", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    private static string GetParagraphInnerText(Paragraph paragraph)
    {
        return string.Concat(paragraph.Descendants<Text>().Select(t => t.Text));
    }

    private static void AppendTableAsMarkdown(StringBuilder sb, Table table)
    {
        var rows = table.Elements<TableRow>().ToList();
        if (rows.Count == 0)
        {
            return;
        }

        var parsedRows = new List<List<string>>();
        foreach (var row in rows)
        {
            var cells = row.Elements<TableCell>().ToList();
            var cellTexts = new List<string>();
            foreach (var cell in cells)
            {
                var cellText = string.Concat(cell.Descendants<Text>().Select(t => t.Text));
                cellTexts.Add(EscapeMarkdownTableCell(cellText));
            }

            if (cellTexts.Count > 0)
            {
                parsedRows.Add(cellTexts);
            }
        }

        if (parsedRows.Count == 0)
        {
            return;
        }

        var columnCount = parsedRows.Max(r => r.Count);
        void PadRow(List<string> r)
        {
            while (r.Count < columnCount)
            {
                r.Add(string.Empty);
            }
        }

        foreach (var r in parsedRows)
        {
            PadRow(r);
        }

        var header = parsedRows[0];
        sb.AppendLine("| " + string.Join(" | ", header) + " |");
        sb.AppendLine("| " + string.Join(" | ", Enumerable.Repeat("---", columnCount)) + " |");
        for (var i = 1; i < parsedRows.Count; i++)
        {
            sb.AppendLine("| " + string.Join(" | ", parsedRows[i]) + " |");
        }

        sb.AppendLine();
    }

    private static string EscapeMarkdownTableCell(string value)
    {
        return value.Replace("|", "\\|", StringComparison.Ordinal).Replace("\r", " ", StringComparison.Ordinal)
            .Replace("\n", " ", StringComparison.Ordinal).Trim();
    }
}
