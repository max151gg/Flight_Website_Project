using ModelSkyPath.Models;
using SkyPath_Models.Models;
using SkyPath_Models.ViewModel;
using SkyPathWS.ORM;
using SkyPathWSClient;
using System.Data;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
using Testing;
namespace Testing
{
    class Program
    {
        public static void Main(string[] args)
        {
            //CheckInsert();
            //TestAnnouncement();
            //CheckCreator();
            Console.WriteLine(GenerateSalt(16));
            for (int i = 1; i <= 5; i++)
            {
                Console.WriteLine("inert password to hash:");
                string password = Console.ReadLine();
                string salt = GenerateSalt(GetRandomNumber());
                string hash = GetHash(password, salt);
                Console.WriteLine($"salt: {salt}");
                Console.WriteLine($"Hash: {hash}");
            }

            //Console.ReadLine();
            //TestSkyPathClient();
            //Console.ReadLine();
        }
        static void TestSkyPathClient()
        {
            ApiClient<TicketViewModel> apiClient = new ApiClient<TicketViewModel>();
            apiClient.Scheme = "http";
            apiClient.Host = "localhost";
            apiClient.Port = 5125;
            apiClient.Path = "api/User/GetTicketByUserId";
            apiClient.SetQueryParameter("user_id", "1");
            TicketViewModel ticketVm = apiClient.GetAsync().Result;
            foreach(Ticket t in ticketVm.tickets)
              Console.WriteLine($"{t.Ticket_Id} - {t.Flight_Id} - {t.Purchase_Date}");
        }
        static int GetRandomNumber()
        {
            Random random = new Random();
            return random.Next(8, 16);
        }
        static string GenerateSalt(int length)
        {
            byte[] bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }
        static string GetHash(string password, string salt)
        {
            string combine = password + salt;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(combine);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
        static void CheckCreator()
        {
            string sql = "Select * from Arrival_City where Arrival_Id=3";
            DbHelperOleDb dbHelperOleDb = new DbHelperOleDb();
            dbHelperOleDb.OpenConnection();
            IDataReader dataReader = dbHelperOleDb.Select(sql);
            dataReader.Read();
            ModelCreators modelCreators = new ModelCreators();
            Arrival_City arrival_City = modelCreators.Arrival_CityCreator.CreateModel(dataReader);
            dbHelperOleDb.CloseConnection();
            Console.WriteLine($"{arrival_City.City_Name}");
        }
        static void CheckInsert()
        {
            Console.WriteLine("Insert City name");
            string city = Console.ReadLine();
            DbHelperOleDb dbHelperOleDb = new DbHelperOleDb();
            string sql = $"Insert into Arrival_City(City_Name) Values('{city}')";
            dbHelperOleDb.OpenConnection();
            dbHelperOleDb.Insert(sql);
            dbHelperOleDb.CloseConnection();
        }



        static void TestAnnouncement()
        {
            Announcement announcement = new Announcement();
            announcement.Announcement_Id = "1";
            announcement.Announcement_Date = "17/10/2025";
            announcement.Title = "Title";
            announcement.Content = "content";
            announcement.Admin_Id = "010";
            if (announcement.HasErrors == true)
            {
                foreach (KeyValuePair<string, List<string>> keyValuePair in announcement.AllErrors())
                {
                    Console.WriteLine(keyValuePair.Key);
                    foreach (string str in keyValuePair.Value)
                    {
                        Console.WriteLine($"        {str}");
                    }
                    Console.WriteLine("--------------------------------------------");
                }
            }
            else { Console.WriteLine("there were no errors"); }
        }




        //-------------------------------------------------------------------------------------------------------------------------
        //מיועד להמרת מחירי טיסות מדולר לכסף הרצוי למשתמש
        //-------------------------------------------------------------------------------------------------------------------------
        //await checkcurrency();

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
    }
}
