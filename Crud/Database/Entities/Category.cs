namespace Crud.Database.Entities;

public class Category
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    public List<Product> Products { get; set; } = [];
}