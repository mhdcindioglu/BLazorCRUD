using Crud.Database;
using Crud.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Crud.Extensions;

public static class DbSeedExtensions
{
    public static async Task SeedDataAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
        await using var db = await dbFactory.CreateDbContextAsync();

        // Ensure database exists
        await db.Database.EnsureCreatedAsync();

        // If categories already exist, assume database is seeded
        if (await db.Categories.AnyAsync())
        {
            return;
        }

        string[] categoryNames =
        [
            "Electronics",
            "Books",
            "Clothing",
            "Home & Kitchen",
            "Sports & Outdoors",
            "Toys & Games",
            "Beauty & Personal Care",
            "Automotive",
            "Grocery",
            "Garden & Tools"
        ];

        var categories = new List<Category>();

        foreach (var catName in categoryNames)
        {
            var category = new Category
            {
                Title = catName,
                Products = []
            };

            // 5 real-like products per category with unique titles (global uniqueness enforced by index on Title)
            var products = GetProductsForCategory(catName);
            foreach (var p in products)
            {
                category.Products.Add(new Product
                {
                    Category = category,
                    Title = StripCategoryPrefix(p.Title),
                    Price = p.Price,
                    Stock = p.Stock,
                    Description = p.Description
                });
            }

            categories.Add(category);
        }

        await db.Categories.AddRangeAsync(categories);
        await db.SaveChangesAsync();
    }

    private static string StripCategoryPrefix(string title)
    {
        const string sep = " - ";
        int idx = title.IndexOf(sep, StringComparison.Ordinal);
        return idx >= 0 ? title[(idx + sep.Length)..] : title;
    }

    private static List<(string Title, decimal Price, int Stock, string Description)> GetProductsForCategory(string category)
    {
        return category switch
        {
            "Electronics" =>
            [
                ("55\" 4K Smart TV", 599.99m, 35, "55-inch 4K UHD HDR smart television with built-in apps."),
                ("Wireless Noise-Cancelling Headphones", 199.99m, 120, "Over-ear Bluetooth headphones with ANC and 30h battery."),
                ("14\" Ultrabook Laptop", 1099.00m, 18, "Lightweight 14-inch laptop with SSD and 16GB RAM."),
                ("Bluetooth Speaker", 79.90m, 85, "Portable speaker with deep bass and IPX7 water resistance."),
                ("Smartphone 128GB", 699.00m, 42, "6.1-inch display, dual camera, 5G, 128GB storage.")
            ],
            "Books" =>
            [
                ("Clean Code", 34.99m, 64, "A handbook of agile software craftsmanship."),
                ("The Pragmatic Programmer", 39.99m, 52, "Journey to mastery for modern developers."),
                ("Design Patterns", 49.99m, 40, "Elements of reusable object-oriented software."),
                ("Refactoring", 44.99m, 33, "Improving the design of existing code."),
                ("Domain-Driven Design", 59.99m, 28, "Tackling complexity in the heart of software.")
            ],
            "Clothing" =>
            [
                ("Men's Cotton T-Shirt", 14.99m, 200, "100% cotton classic fit tee."),
                ("Women's Denim Jacket", 49.90m, 75, "Stylish denim jacket with pockets."),
                ("Running Shoes", 89.00m, 60, "Breathable running shoes for everyday training."),
                ("Wool Sweater", 59.50m, 40, "Soft merino wool crew-neck sweater."),
                ("Baseball Cap", 19.99m, 150, "Adjustable cap with curved brim.")
            ],
            "Home & Kitchen" =>
            [
                ("Air Fryer 5 Qt", 99.99m, 55, "Oil-less air fryer with digital controls."),
                ("Stainless Steel Cookware Set", 129.99m, 22, "10-piece pots and pans set for all cooktops."),
                ("Memory Foam Pillow", 29.99m, 110, "Ergonomic pillow with washable cover."),
                ("Robot Vacuum", 229.00m, 18, "Self-charging vacuum with app control."),
                ("Electric Kettle 1.7L", 34.90m, 90, "Fast-boil kettle with auto shut-off.")
            ],
            "Sports & Outdoors" =>
            [
                ("Yoga Mat", 24.99m, 130, "Non-slip 6mm thick exercise mat."),
                ("Mountain Bike Helmet", 69.99m, 48, "Lightweight helmet with MIPS-like protection."),
                ("Adjustable Dumbbells 2x25lb", 179.00m, 26, "Space-saving adjustable free weights."),
                ("Camping Tent 2-Person", 119.00m, 34, "Water-resistant tent with vestibule."),
                ("Fitness Tracker", 59.99m, 77, "Activity tracker with heart rate and sleep monitor.")
            ],
            "Toys & Games" =>
            [
                ("Building Blocks Set 500 pcs", 39.99m, 80, "Creative building set compatible with major brands."),
                ("Remote Control Car", 49.99m, 65, "Fast RC car with rechargeable battery."),
                ("Board Game Strategy", 29.99m, 70, "Competitive strategy game for 2-4 players."),
                ("Plush Teddy Bear", 19.99m, 95, "Soft plush bear, 30 cm tall."),
                ("Puzzle 1000 Pieces", 16.99m, 120, "High-quality jigsaw puzzle with poster.")
            ],
            "Beauty & Personal Care" =>
            [
                ("Facial Cleanser 200ml", 12.99m, 140, "Gentle daily face wash for all skin types."),
                ("Moisturizing Cream 50ml", 17.99m, 90, "Hydrating cream with hyaluronic acid."),
                ("Shampoo 400ml", 8.49m, 160, "Sulfate-free shampoo for shiny hair."),
                ("Electric Toothbrush", 39.99m, 55, "Rechargeable sonic toothbrush with timer."),
                ("Hair Dryer 1800W", 29.99m, 60, "Compact dryer with ionic technology.")
            ],
            "Automotive" =>
            [
                ("Car Phone Mount", 14.99m, 150, "Dashboard and windshield compatible mount."),
                ("All-Weather Floor Mats", 69.99m, 30, "Heavy-duty mats for most vehicles."),
                ("Portable Jump Starter", 79.99m, 24, "Compact 1000A jump starter with USB-C."),
                ("LED Headlight Bulbs", 49.99m, 42, "Bright, energy-efficient replacement bulbs."),
                ("Tire Inflator", 39.99m, 58, "Portable air compressor with pressure gauge.")
            ],
            "Grocery" =>
            [
                ("Organic Arabica Coffee Beans 1kg", 19.99m, 100, "Fresh medium roast whole beans."),
                ("Extra Virgin Olive Oil 1L", 15.99m, 85, "Cold-pressed olive oil, first harvest."),
                ("Whole Grain Pasta 500g", 2.49m, 200, "Durum wheat pasta, al dente texture."),
                ("Almond Butter 340g", 8.99m, 70, "No added sugar or palm oil."),
                ("Protein Bars Pack of 12", 21.99m, 60, "12g protein per bar, assorted flavors.")
            ],
            "Garden & Tools" =>
            [
                ("Cordless Drill 20V", 89.99m, 33, "Compact drill with 2 batteries and charger."),
                ("Gardening Tool Set 10 pcs", 39.99m, 44, "Hand tools set with tote bag."),
                ("Pruning Shears", 16.99m, 120, "Bypass pruner with sharp stainless blade."),
                ("Lawn Sprinkler", 24.99m, 65, "Adjustable oscillating sprinkler."),
                ("LED Work Light", 29.99m, 52, "Rechargeable flood light with stand.")
            ],
            _ => []
        };
    }
}