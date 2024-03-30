using System;
using System.ComponentModel.DataAnnotations;

public class TokenInfo
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal TotalSupply { get; set; }
    public decimal CirculatingSupply { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
