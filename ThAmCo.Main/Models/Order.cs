namespace ThAmCo.Main.Models;

public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; }
    public DateTime OrderDate { get; set; }
}
