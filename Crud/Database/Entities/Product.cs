using System.ComponentModel.DataAnnotations.Schema;

namespace Crud.Database.Entities;

public class Product
{
    public int Id { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public string Title { get; set; } = string.Empty;

    [Column(TypeName = "decimal(16,4)")]
    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string? Description { get; set; }
}