using ModelSkyPath.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using Testing;

await checkcurrency();

static void TestAnnouncement()
{
    Announcement announcement = new Announcement();
    announcement.Announcement_Id = "1";
    announcement.Announcement_Date = "17/10/2025";
    announcement.Title = "Title";
    announcement.Content = "content";
    announcement.Admin_Id = "010";
    if(announcement.HasErrors==true)
    {
        foreach(KeyValuePair<string,List<string>> keyValuePair in announcement.AllErrors())
        {
            Console.WriteLine(keyValuePair.Key);
            foreach(string str in keyValuePair.Value)
            {
                Console.WriteLine($"        {str}"); 
            }
            Console.WriteLine("--------------------------------------------");
        }
    }
    else { Console.WriteLine("there were no errors"); }
}
static async Task checkcurrency()
{
    Console.WriteLine(">>>Insert Currency from");
    string from = Console.ReadLine();
    Console.WriteLine(">>>Insert Currency to");
    string to = Console.ReadLine();
    Console.WriteLine(">>>Insert Amount");
    string amount = Console.ReadLine();

    var client = new HttpClient();
    var request = new HttpRequestMessage
    {
        Method = HttpMethod.Get,
        RequestUri = new Uri($"https://currency-conversion-and-exchange-rates.p.rapidapi.com/convert?from={from}&to={to}&amount={amount}"),
        Headers =
    {
        { "x-rapidapi-key", "2d214fc222msh367adbbf02e79b3p1d60dbjsn41a4d1788257" },
        { "x-rapidapi-host", "currency-conversion-and-exchange-rates.p.rapidapi.com" },
    },
    };
    using (var response = await client.SendAsync(request))
    {
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Currency carr = JsonSerializer.Deserialize<Currency>(body);

        Console.WriteLine($"{carr.query.amount}{carr.query.from} = {carr.result}{carr.query.to}");
    }
}
