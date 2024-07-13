using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using SmartChord.ChordSheets.Pdf;
using SmartChord.Parser.Models;
using SmartChord.Transpose;
using Xceed.Words.NET;
using System.Windows.Media.TextFormatting;
using System.Windows;

namespace SmartChord.ChordSheets.Commands
{
    public static class CreatePdfFromText
    {
        public class Command : IRequest<Result>
        {
            public string SongText { get; set; }
            public string NewKey { get; set; }
            public string OriginalKey { get; set; }
            public string DestinationFilename { get; set; }

        }

        public class Result
        {
            public string OutputFilename { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                string chordsheet = string.Empty;
                var finalDestination = Path.Combine(Path.GetDirectoryName(request.DestinationFilename), $"{Path.GetFileNameWithoutExtension(request.DestinationFilename)}.pdf");
                var songTitle = GetFirstLine(request.SongText);

                // Remove the first line from the original string
                Regex regex = new Regex(@"^.*?(\r?\n|$)");
                request.SongText = regex.Replace(request.SongText, string.Empty, 1);

                if (!string.IsNullOrEmpty(request.NewKey))
                {
                    var transposer = new Transposer();
                    chordsheet = await transposer.ChangeKey(request.SongText, request.NewKey, request.OriginalKey);
                }


                var reader = new StringReader(chordsheet);
                var lines = new List<string>();
                var leftSongLines = new Queue<PdfSongLine>();
                var rightSongLines = new Queue<PdfSongLine>();
                var currentSongLines = leftSongLines;
                var nextSongLines = rightSongLines;
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null) break;

                    lines.Add(line);
                }

                lines = NormalizeLongLines(lines);


                var totalIndex = 0;
                var index = 0;

                Action IncreaseIndex = () => { totalIndex++; index++; };

                foreach (var line in lines)
                {
                    if(IsSectionTitle(line.Trim()))
                    {
                        if(index > 28 && (lines.Count - totalIndex) < 57)
                        {
                            var addLineCount = 58 - index;
                            for (var i = 0; i < addLineCount; i++)
                            {
                                currentSongLines.Enqueue(new PdfSongLine { Line = string.Empty });
                                IncreaseIndex();
                            }

                        }
                    }

                    else if(index == 57 && (PdfHelper.IsLineValidGuitarChords(line) || IsSectionTitle(line)))
                    {
                        currentSongLines.Enqueue(new PdfSongLine { Line = string.Empty});
                        IncreaseIndex();
                    }

                    if (index == 58)
                    {
                        index = 0;
                        var tmp = currentSongLines;
                        currentSongLines = nextSongLines;
                        nextSongLines = tmp;
                    }

                    var songLine = new PdfSongLine();

                    songLine.IsChordLine = PdfHelper.IsLineValidGuitarChords(line);
                    songLine.Line = line;

                    currentSongLines.Enqueue(songLine);

                    IncreaseIndex();

                }




                Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(.5f, QuestPDF.Infrastructure.Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Consolas));

                        page.Header()
                            .Text(songTitle)
                            .SemiBold().FontSize(14);

                        page.Content()
                             .Row(row =>
                             {

                                 row.RelativeItem()
                                     .Padding(10)
                                     .Column(column =>
                                     {

                                         foreach (var line in leftSongLines)
                                         {
                                             var _ = line.IsChordLine
                                             ? column.Item().Text(line.Line).SemiBold()
                                             : column.Item().Text(line.Line);
                                         }

                                     });


                                 row.RelativeItem()
                                     .Padding(10)
                                     .Column(column =>
                                     {

                                         foreach (var line in rightSongLines)
                                         {
                                             var _ = line.IsChordLine
                                             ? column.Item().Text(line.Line).SemiBold()
                                             : column.Item().Text(line.Line);
                                         }

                                     });
                             });

                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                            });
                    });
                }).GeneratePdf(finalDestination);

                return new Result {OutputFilename = finalDestination };
            }

        }

        public static string FormatString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Remove special characters
            var sb = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                {
                    sb.Append(c);
                }
            }

            // Convert to string after removing special characters
            string result = sb.ToString();

            // Trim ending numeric characters
            result = Regex.Replace(result, @"\d+$", string.Empty);

            // Convert to proper case
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            result = textInfo.ToTitleCase(result.ToLower());

            return result;
        }

        public static string GetFirstLine(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Find the index of the first newline character
            int newlineIndex = input.IndexOf(Environment.NewLine);

            if (newlineIndex == -1)
            {
                // If there is no newline character, return the whole string
                return input;
            }
            else
            {
                // Return the substring from the start to the newline character
                return input.Substring(0, newlineIndex);
            }
        }


        public static bool IsSectionTitle(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            // Check if string starts with [ and ends with ]
            if (input.StartsWith("[") && input.EndsWith("]"))
                return true;

            // Regular expression to match the specified keywords followed by :
            Regex regex = new Regex(@"^(Chorus|Verse|Bridge|Instrumental|Intro|Outro).*\:$", RegexOptions.IgnoreCase);

            return regex.IsMatch(input);
        }

        public static List<string> NormalizeLongLines(List<string> lines)
        {
            var index = 0;
            var result = new List<string>();
            while (index < lines.Count)
            {
                var line = lines[index].TrimEnd();
                if (!PdfHelper.IsLineValidGuitarChords(line))
                {
                    result.Add(line);
                    index++;
                    continue;
                }
                var cutIndex = 43;

                if (lines[index].Count() > 44)
                {
                    if (lines[index][cutIndex + 1] != ' ')
                    {
                        var newCutLength = 0;
                        while (true)
                        {
                            if (line[cutIndex - newCutLength] == ' ')
                            {
                                cutIndex = cutIndex - newCutLength;
                                break;
                            }

                            newCutLength++;
                        }

                        var chordLine1 = line.Substring(0, cutIndex);
                        var chordLine2 = line.Substring(cutIndex + 1);

                        var nextLine = lines[index + 1];
                        var nextCutIndex = 43;
                        if (nextLine.Count() > 44)
                        {
                            if (nextLine[nextCutIndex + 1] != ' ')
                            {
                                var nextCutLength = 0;
                                while (true)
                                {
                                    if (nextLine[nextCutIndex - nextCutLength] == ' ')
                                    {
                                        nextCutIndex = nextCutIndex - nextCutLength;
                                        break;
                                    }

                                    nextCutLength++;
                                }

                                var nextLine1 = nextLine.Substring(0, nextCutIndex);
                                var nextLine2 = nextLine.Substring(nextCutIndex + 1);

                                var chordPadding = string.Concat(Enumerable.Repeat(" ", nextCutLength));

                                result.Add(chordLine1);
                                result.Add(nextLine1);
                                result.Add(chordPadding + chordLine2);
                                result.Add(nextLine2);

                                index += 2;
                                continue;
                            }
                        }
                        else
                        {
                            result.Add(chordLine1);
                            result.Add(nextLine);
                            result.Add(chordLine2);
                            index += 2;
                            continue;
                        }
                    }
                    else
                    {
                        var chordLine1 = line.Substring(0, cutIndex);
                        var chordLine2 = line.Substring(cutIndex + 1);

                        var nextLine = lines[index + 1];
                        var nextCutIndex = 43;
                        if (nextLine.Count() > 44)
                        {
                            if (nextLine[nextCutIndex + 1] != ' ')
                            {
                                var nextCutLength = 0;
                                while (true)
                                {
                                    if (nextLine[nextCutIndex - nextCutLength] == ' ')
                                    {
                                        nextCutIndex = nextCutIndex - nextCutLength;
                                        break;
                                    }

                                    nextCutLength++;
                                }

                                var nextLine1 = nextLine.Substring(0, nextCutIndex);
                                var nextLine2 = nextLine.Substring(nextCutIndex + 1);
                                var chordPadding = string.Concat(Enumerable.Repeat(" ", nextCutLength));
                                result.Add(chordLine1);
                                result.Add(nextLine1);
                                result.Add(chordPadding + chordLine2);
                                result.Add(nextLine2);

                                index += 2;
                                continue;
                            }
                        }
                        else
                        {
                            result.Add(chordLine1);
                            result.Add(nextLine);
                            result.Add(chordLine2);
                            index += 2;
                            continue;
                        }
                    }
                }

                result.Add(line);
                index++;

            }

            return result;

        }
    }
}