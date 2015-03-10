DataAccess
==========
Repository and Unit Of Work implementation over a relational database.

Overview
-----------
This library provides an abstraction and implementation of data access to a relational database. It targets two main goals:

 - support for consistent development model for reading or modifying data in the entire application
 - enforce good separation of concerns by separating data access from business logic

The implementation is done with Entity Framework, but the client code should not have any direct dependencies (reference assemblies) to it. The client code depends only on the abstract interface provided by the library and does not know details about the underlying ORM.

Usage Patterns
------------------
**`IRepository`** is a generic interface for querying data for read-only purposes:
```csharp
private readonly IRepository rep; // injected w/ Dependency Injection
public IEnumerable<Order> GetAllLargeOrders(int amount)
{
	var orders = rep.GetEntities<Order>()
					.Where(o => o.OrderLines.Any(ol => ol.Ammount > amount)
	return orders.ToList();
}
```
Queries may be returned to be composed and executed by caller code:
```csharp
private readonly IRepository rep; // injected w/ Dependency Injection
private IQueriable<Order> GetAllLargeOrders(int amount)
{
	var orders = rep.GetEntities<Order>()
					.Where(o => o.OrderLines.Any(ol => ol.Ammount > amount)
	return orders;
}

public IEnumerable<OrderSummary> GetRecentLargeOrders(int amount)
{
   int thisYear = DateTime.UtcNow.Year;
   var orders = GetAllLargeOrders(amount)
				   .Where(o.Year == thisYear)
				   .Select(o => new OrderSummary
				                {
				                ...
				                });
	return orders;
}
```  

**`IUnitOfWork`** is a generic interface for creating a well defined scope for adding, modifying or deleting data:
```csharp
public void ReviewLargeAmountOrders(int amount, ReviewData data)
{
	using (IUnitOfWork uof = rep.CreateUnitOfWork())
	{
		IQueryable<Order> orders = uof.GetEntities<Order>()
					.Where(o => o.OrderLines.Any(ol => ol.Ammount > amount);
		foreach(var order in orders)
		{
			order.Status = Status.Reviewed;
			order.Cutomer.Name = data.CustomerNameUpdate;
			...
		}
		
		ReviewEvent re = new ReviewEvent {...}
		uof.Add(re)
		
		uof.SaveCanges();
	}
}
```

Getting Started
-----------------
Create a data model project which contains the POCOs that are mapped to the database model. This will also contain the Entity Framework `DbContext`.

Register into your Dependency Injection container the following mappings:

`IDbContextFactory` , `DbContextFactory<MyEntities>`  
`IRepository`       , `Repository`  
`IInterceptorsResolver` , `InterceptorsResolver`

Related Repository
-------------------------
[iQuarc.AppBoot](https://github.com/iQuarc/AppBoot)