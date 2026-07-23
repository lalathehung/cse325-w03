using BlazingPizza;
using Microsoft.EntityFrameworkCore;

namespace BlazingPizza.Data;

public class PizzaStoreContext : DbContext
{
    public PizzaStoreContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; } = default!;

    public DbSet<Pizza> Pizzas { get; set; } = default!;

    public DbSet<PizzaSpecial> Specials { get; set; } = default!;

    public DbSet<Topping> Toppings { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PizzaTopping>()
            .HasKey(pst => new
            {
                pst.PizzaId,
                pst.ToppingId
            });

        modelBuilder.Entity<PizzaTopping>()
            .HasOne<Pizza>()
            .WithMany(pizza => pizza.Toppings);

        modelBuilder.Entity<PizzaTopping>()
            .HasOne(pizzaTopping => pizzaTopping.Topping)
            .WithMany();
    }
}