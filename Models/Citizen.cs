namespace AnnoCalculator.Models
{
    public class Citizen
    {
        public string Name { get; set; } = string.Empty;
        public string Belonging { get; set; } = string.Empty;
        public Dictionary<string, double> Needs { get; set; } = [];
    }
}
