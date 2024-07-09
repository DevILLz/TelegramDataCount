using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using AngleSharp;
using System.Text.RegularExpressions;

var path = @"D:\\Downloads\\Telegram Desktop\\ChatExport_2024-07-09";

var statistics = new Dictionary<string, int>();

foreach (string file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
{
    using StreamReader reader = new(file);
    string text = await reader.ReadToEndAsync();

    IBrowsingContext context = BrowsingContext.New();
    IHtmlParser parser = context.GetService<IHtmlParser>();
    IDocument document = parser.ParseDocument(text);

    var node = document.Body?.Children[0]?.Children[1].Children[0];

    foreach (var div in document.Body?.Children[0]?.Children[1].Children[0].Children)
    {
        var divBody = div
            .Children.FirstOrDefault(c => c.ClassList.Contains("body"));

        if (divBody is null) continue;

        var date = divBody.Children.FirstOrDefault(c => c.ClassList.Contains("date"))?.OuterHtml;
        if (date is null) continue;

        var dateParsed = Regex.Match(date, @"title=""(\d{2}\.\d{2}\.\d{4})");
        var d = DateTime.Parse(dateParsed.Groups[1].Value);

        if (d.Year < 2024)
            continue;

        var name = divBody.Children.FirstOrDefault(c => c.ClassList.Contains("from_name"))?.TextContent.Trim();
        if (name is null) continue;

        var message = divBody.Children.FirstOrDefault(c => c.ClassList.Contains("text"))?.TextContent.Trim();
        if (message is null) continue;

        var isManExists = statistics.ContainsKey(name);

        if (isManExists)
            statistics[name]++;
        else 
            statistics[name] = 1;
    }
}

var sortedDictionary = statistics.OrderByDescending(x => x.Value)
    .ThenBy(x => x.Key)
    .ToDictionary(x => x.Key, x => x.Value);

foreach (var key in sortedDictionary.Keys)
{
    Console.WriteLine($"{key, 30} {sortedDictionary[key], 5}");
}

Console.ReadLine();
record People(string Name, int MessageCount);