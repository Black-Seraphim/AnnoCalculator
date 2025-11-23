namespace AnnoCalculator.Models
{
    public class Building
    {
        public string Name { get; set; } = string.Empty;
        public string Belonging { get; set; } = string.Empty;
        public string Produces { get; set; } = string.Empty;
        public List<string> Requires { get; set; } = [];
        public int SecondsToProduce { get; set; }
    }
}
