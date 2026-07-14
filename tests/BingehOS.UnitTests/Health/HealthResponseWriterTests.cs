using System.Text;
using System.Text.Json;
using BingehOS.Api.Health;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BingehOS.UnitTests.Health;

public class HealthResponseWriterTests
{
    [Fact]
    public async Task Write_Produces_Json_With_Status_EntryName_And_Tags()
    {
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["postgres"] = new HealthReportEntry(
                status: HealthStatus.Healthy,
                description: "ok",
                duration: TimeSpan.FromMilliseconds(2),
                exception: null,
                data: null,
                tags: new[] { "ready" })
        };
        var report = new HealthReport(entries, TimeSpan.FromMilliseconds(5));

        var context = new DefaultHttpContext();
        var stream = new MemoryStream();
        context.Response.Body = stream;

        await HealthResponseWriter.Write(context, report);

        stream.Position = 0;
        var json = Encoding.UTF8.GetString(stream.ToArray());

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal("Healthy", root.GetProperty("status").GetString());

        var entry = root.GetProperty("entries").EnumerateArray().First();
        Assert.Equal("postgres", entry.GetProperty("name").GetString());
        Assert.Equal("Healthy", entry.GetProperty("status").GetString());
        Assert.Equal("ok", entry.GetProperty("description").GetString());

        var tagsElement = entry.GetProperty("tags");
        if (tagsElement.ValueKind == JsonValueKind.Array)
            Assert.Contains(tagsElement.EnumerateArray(), e => e.GetString() == "ready");
        else
            Assert.Contains(tagsElement.EnumerateObject(), p => p.Name == "ready");

        Assert.Equal("application/json", context.Response.ContentType);
    }
}
