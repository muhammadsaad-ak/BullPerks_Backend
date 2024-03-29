using System.ComponentModel.DataAnnotations;

public class TokenInfo
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal TotalSupply { get; set; }
    public decimal CirculatingSupply { get; set; }
}
