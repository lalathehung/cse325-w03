using BlazingPizza.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazingPizza;

[Route("orders")]
[ApiController]
public class OrdersController : Controller
{
    private readonly PizzaStoreContext _db;

    public OrdersController(PizzaStoreContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderWithStatus>>> GetOrders()
    {
        var orders = await _db.Orders
            .Include(order => order.Pizzas)
                .ThenInclude(pizza => pizza.Special)
            .Include(order => order.Pizzas)
                .ThenInclude(pizza => pizza.Toppings)
                .ThenInclude(pizzaTopping => pizzaTopping.Topping)
            .OrderByDescending(order => order.CreatedTime)
            .ToListAsync();

        return orders
            .Select(OrderWithStatus.FromOrder)
            .ToList();
    }

    [HttpPost]
    public async Task<ActionResult<int>> PlaceOrder(Order order)
    {
        order.CreatedTime = DateTime.Now;

        foreach (var pizza in order.Pizzas)
        {
            pizza.SpecialId = pizza.Special.Id;
            pizza.Special = null!;
        }

        _db.Orders.Attach(order);
        await _db.SaveChangesAsync();

        return order.OrderId;
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderWithStatus>> GetOrderWithStatus(
        int orderId)
    {
        var order = await _db.Orders
            .Where(order => order.OrderId == orderId)
            .Include(order => order.Pizzas)
                .ThenInclude(pizza => pizza.Special)
            .Include(order => order.Pizzas)
                .ThenInclude(pizza => pizza.Toppings)
                .ThenInclude(pizzaTopping => pizzaTopping.Topping)
            .SingleOrDefaultAsync();

        if (order is null)
        {
            return NotFound();
        }

        return OrderWithStatus.FromOrder(order);
    }
}