using System.Text;
using System.Text.RegularExpressions;
using CapstoneProjectRegistration.Services.DTOs.Topic;
using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.Utilities;

namespace CapstoneProjectRegistration.Services.Implements;

public class TopicImportService : ITopicImportService
{
    public async Task<TopicImportResultDto> ImportFromWordAsync(Stream fileStream)
    {
        ArgumentNullException.ThrowIfNull(fileStream);
        await using var mem = new MemoryStream();
        await fileStream.CopyToAsync(mem).ConfigureAwait(false);
        mem.Position = 0;
        var rawMarkdown = DocxToMarkdownConverter.Convert(mem);
        return ParseFromMarkdown(rawMarkdown);
    }

    private static TopicImportResultDto ParseFromMarkdown(string rawMarkdown)
    {
        var dto = new TopicImportResultDto { RawMarkdown = rawMarkdown };
        if (string.IsNullOrWhiteSpace(rawMarkdown))
        {
            return dto;
        }

        var normalized = rawMarkdown.Replace("\r\n", "\n", StringComparison.Ordinal);
        dto.EnglishName = ExtractLabeledField(normalized,
            new[] { "EnglishName", "English", "Tên tiếng Anh" },
            new[] { @"^\s*EnglishName\s*[:=]\s*(.+)$", @"^\s*English\s*[:=]\s*(.+)$" });

        if (string.IsNullOrEmpty(dto.EnglishName))
        {
            dto.EnglishName = ExtractBlockAfterHeading(normalized, "English", maxLines: 5);
        }

        dto.VietnameseName = ExtractLabeledField(normalized,
            new[] { "VietnameseName", "Vietnamese", "Tên tiếng Việt" },
            new[] { @"^\s*VietnameseName\s*[:=]\s*(.+)$", @"^\s*Vietnamese\s*[:=]\s*(.+)$" });

        if (string.IsNullOrEmpty(dto.VietnameseName))
        {
            dto.VietnameseName = ExtractBlockAfterHeading(normalized, "Vietnamese", maxLines: 5);
        }

        dto.Description = ExtractLabeledField(normalized,
            new[] { "Context", "Mô tả", "Description" },
            new[] { @"^\s*Context\s*[:=]\s*(.+)$", @"^\s*Description\s*[:=]\s*(.+)$" }, allowMultiline: true);

        if (string.IsNullOrEmpty(dto.Description))
        {
            dto.Description = ExtractBlockAfterHeading(normalized, "Context", maxLines: 50);
        }

        if (string.IsNullOrEmpty(dto.Description))
        {
            dto.Description = ExtractBlockAfterHeading(normalized, "Description", maxLines: 50);
        }

        var tables = ParseAllTables(normalized);
        dto.SupervisorName = ExtractSupervisorName(normalized, tables);
        dto.StudentCodes = ExtractStudentCodes(tables, normalized);
        return dto;
    }

    private static string ExtractLabeledField(
        string text,
        string[] keys,
        string[] linePatterns,
        bool allowMultiline = false)
    {
        foreach (var pattern in linePatterns)
        {
            var options = RegexOptions.Multiline | RegexOptions.IgnoreCase;
            if (allowMultiline)
            {
                options |= RegexOptions.Singleline;
            }

            var m = Regex.Match(text, pattern, options);
            if (m.Success && m.Groups.Count > 1)
            {
                return m.Groups[1].Value.Trim();
            }
        }

        foreach (var key in keys)
        {
            var block = ExtractBlockAfterHeadingLine(text, key, maxLines: allowMultiline ? 200 : 10);
            if (!string.IsNullOrEmpty(block))
            {
                return block;
            }
        }

        return string.Empty;
    }

    private static string ExtractBlockAfterHeadingLine(string fullText, string matchWord, int maxLines)
    {
        var lines = fullText.Split('\n', StringSplitOptions.None);
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (!line.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            if (!line.Contains(matchWord, StringComparison.OrdinalIgnoreCase) || line.Length > 200)
            {
                continue;
            }

            var sb = new StringBuilder();
            for (var j = i + 1; j < lines.Length && j < i + 1 + maxLines; j++)
            {
                var l = lines[j].Trim();
                if (l.StartsWith("#", StringComparison.Ordinal) && l.Length < 120)
                {
                    break;
                }

                if (l.StartsWith("|", StringComparison.Ordinal))
                {
                    break;
                }

                if (!string.IsNullOrWhiteSpace(l))
                {
                    sb.AppendLine(l);
                }
            }

            var s = sb.ToString().Trim();
            if (s.Length > 0)
            {
                return s;
            }
        }

        return string.Empty;
    }

    private static string ExtractBlockAfterHeading(string fullText, string word, int maxLines) =>
        ExtractBlockAfterHeadingLine(fullText, word, maxLines);

    private static List<List<string[]>> ParseAllTables(string markdown)
    {
        var all = new List<List<string[]>>();
        var lines = markdown.Split('\n', StringSplitOptions.None);
        var i = 0;
        while (i < lines.Length)
        {
            if (!lines[i].TrimStart().StartsWith("|", StringComparison.Ordinal))
            {
                i++;
                continue;
            }

            var table = new List<string[]>();
            while (i < lines.Length && lines[i].TrimStart().StartsWith("|", StringComparison.Ordinal))
            {
                var t = lines[i].Trim();
                if (IsMarkdownSeparatorRow(t))
                {
                    i++;
                    continue;
                }

                table.Add(ParseTableRow(t));
                i++;
            }

            if (table.Count > 0)
            {
                all.Add(table);
            }
        }

        return all;
    }

    private static bool IsMarkdownSeparatorRow(string line) =>
        Regex.IsMatch(line, @"^\|\s*:?-{2,}(:?\s*\|\s*:?-{2,})+\s*\|?\s*$");

    private static string[] ParseTableRow(string line)
    {
        var trimmed = line.Trim();
        if (trimmed.StartsWith("|", StringComparison.Ordinal))
        {
            trimmed = trimmed[1..];
        }

        if (trimmed.EndsWith("|", StringComparison.Ordinal))
        {
            trimmed = trimmed[..^1];
        }

        return trimmed.Split('|', StringSplitOptions.None).Select(c => c.Trim()).ToArray();
    }

    private static string ExtractSupervisorName(string fullText, List<List<string[]>> tables)
    {
        foreach (var table in tables)
        {
            for (var r = 0; r < table.Count; r++)
            {
                var row = table[r];
                for (var c = 0; c < row.Length; c++)
                {
                    var cell = row[c] ?? string.Empty;
                    if (cell.Contains("Supervisor", StringComparison.OrdinalIgnoreCase) ||
                        cell.Contains("GVHD", StringComparison.OrdinalIgnoreCase) ||
                        cell.Contains("Instructor", StringComparison.OrdinalIgnoreCase))
                    {
                        if (c + 1 < row.Length && !string.IsNullOrWhiteSpace(row[c + 1]) && row[c + 1].Length > 1)
                        {
                            return row[c + 1].Trim();
                        }

                        for (var cc = 0; cc < row.Length; cc++)
                        {
                            if (cc == c)
                            {
                                continue;
                            }

                            if (Regex.IsMatch(row[cc] ?? string.Empty, @"[A-Za-zÀ-ỹ][A-Za-zÀ-ỹ\s.]{2,}"))
                            {
                                return (row[cc] ?? string.Empty).Trim();
                            }
                        }
                    }
                }
            }

            if (table.Count == 0)
            {
                continue;
            }

            var header = table[0].Select(x => x.ToLowerInvariant()).ToArray();
            var col = Array.FindIndex(header, h =>
                h.Contains("supervisor", StringComparison.Ordinal) ||
                h.Contains("gvhd", StringComparison.Ordinal) ||
                h.Contains("instructor", StringComparison.Ordinal));
            if (col >= 0 && table.Count > 1)
            {
                return table[1][col].Trim();
            }
        }

        var m = Regex.Match(fullText, @"(?im)Supervisor\s*Name\s*[:=]\s*(.+)$", RegexOptions.Multiline);
        return m.Success ? m.Groups[1].Value.Trim() : string.Empty;
    }

    private static List<string> ExtractStudentCodes(List<List<string[]>> tables, string fullText)
    {
        var codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var codePattern = new Regex(@"\b[A-Z]{2,3}\d{5,9}\b", RegexOptions.Compiled);
        foreach (var table in tables)
        {
            foreach (var row in table)
            {
                foreach (var cell in row)
                {
                    foreach (Match m in codePattern.Matches(cell))
                    {
                        codes.Add(m.Value);
                    }
                }
            }
        }

        foreach (Match m in codePattern.Matches(fullText))
        {
            codes.Add(m.Value);
        }

        return codes.OrderBy(x => x).ToList();
    }
}
