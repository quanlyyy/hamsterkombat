namespace Ekz.Models;

public class Zakaz
{
    public int IdZakaza { get; set; }
    public int KlId { get; set; }
    public int Summa { get; set; }
    public int PrId { get; set; }
    public string ProductName { get; set; } = string.Empty;
}