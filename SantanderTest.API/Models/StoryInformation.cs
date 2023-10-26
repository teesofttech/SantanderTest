namespace SantanderTest.API.Models
{
    public class StoryInformation
    {
        public string Id { get; set; }
        public string By { get; set; }

        public string Descendants { get; set; }

        public int Score { get; set; }

        public string Time { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }

        public List<string> Kids { get; set; }
    }
}
