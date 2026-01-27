namespace PeopleApp.Api.Options;

public class DemoSeedOptions
{
    public bool Enabled { get; set; }
    public int Purchases { get; set; } = 300;
    public int DaysBack { get; set; } = 90;
    public int MaxLinesPerPurchase { get; set; } = 5;
}
