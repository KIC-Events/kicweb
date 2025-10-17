using System;
using System.Collections.Generic;
using System.Linq;
using KiCData.Models;
using KiCData.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using Org.BouncyCastle.Asn1.Ocsp;
using Square;
using Square.Authentication;
using Square.Exceptions;
using Square.Models;
using Event = KiCData.Models.Event;
using Order = KiCData.Models.Order;

namespace KiCWeb.Controllers;

[ApiController]
[Route("Setup")]
public class SetupController
{
    private IConfigurationRoot _config;
    private IKiCLogger _logger;
    private KiCdbContext _db;
    private SquareClient _client;
    private IWebHostEnvironment _env;
    private readonly InventoryService _inventoryService;
    
    public SetupController(IConfigurationRoot configuration, IKiCLogger logger, KiCdbContext context, IWebHostEnvironment appenv, InventoryService inventoryService)
    {
        Square.Environment env = Square.Environment.Production;

        if (configuration["Square:Environment"] == "Sandbox")
        {
            env = Square.Environment.Sandbox;
        }
        var token =  configuration["Square:Token"];

        _client = new SquareClient.Builder().BearerAuthCredentials
            (
                new BearerAuthModel.Builder(token)
                    .Build()
            )
            .Environment(env)
            .Build();
                
        _logger = logger;
        _db = context;
        _config = configuration;
        _env =  appenv;
        _inventoryService = inventoryService;
    }

    [HttpPost]
    [Route("/GenerateOrders")]
    public IActionResult GenerateOrders([FromHeader(Name = "X-Api-Key")] string apiKey)
    {
        var user = _db.User.FirstOrDefault(x => x.Token == apiKey);
        if (user == null)
        {
            return new UnauthorizedResult();
        }

        var attendees = _db.Attendees.Where(x=>x.OrderID != null).ToList();
        var orderIds =  attendees.Select(x => x.OrderID).Distinct().ToList();

        List<Square.Models.Order> orderData = [];
        foreach (var orderBatch in orderIds.Chunk(100))
        {
            var orderResponse = _client.OrdersApi.BatchRetrieveOrders(new BatchRetrieveOrdersRequest(orderBatch));
            if (orderResponse != null && orderResponse.Orders != null)
            {
                orderData.AddRange(orderResponse.Orders);
            }
        }
        var newOrders = 0;

        foreach (var orderId in orderIds)
        {
            Square.Models.Order? squareOrder;
            try
            {
                squareOrder = orderData.First(x => x.Id == orderId);
            }
            catch (InvalidOperationException e)
            {
                squareOrder = null;
            }
            var orderDate = DateTime.MinValue;
            var attendee = _db.Attendees.Include(attendee => attendee.Ticket).First(x => x.OrderID == orderId);
            var orderTotal = 0;
            if (attendee.Ticket != null)
            {
                orderDate = attendee.Ticket.DatePurchased.GetValueOrDefault().ToDateTime(TimeOnly.MinValue);
                orderTotal = (int) _db.Attendees.Where(x => x.OrderID == orderId && x.Ticket != null).Sum(x => x.Ticket!.Price) * 100;
            }

            int taxesTotal = (int)Math.Round(orderTotal * 0.8);
            int subTotal = orderTotal;
            int grandTotal;
            int paymentsTotal = 0;
            int refundsTotal = 0;
            if (squareOrder != null)
            {
                orderDate = DateTime.Parse(squareOrder.CreatedAt);
                grandTotal = (int)squareOrder.TotalMoney.Amount.GetValueOrDefault();
                paymentsTotal = (int)squareOrder.Tenders.Sum(x => x.AmountMoney?.Amount ?? 0);
                var paymentIds = squareOrder.Tenders.Select(x => x.PaymentId).ToList();
                foreach (var paymentId in paymentIds)
                {
                    var result = _client.PaymentsApi.GetPayment(paymentId);
                    if (result.Payment.RefundedMoney != null)
                    {
                        refundsTotal += (int)result.Payment.RefundedMoney.Amount.GetValueOrDefault();
                    }
                }
                
                if (squareOrder.Refunds != null)
                {
                    refundsTotal = (int) squareOrder.Refunds.Sum(x => x.AmountMoney.Amount.GetValueOrDefault());
                }
            }
            else
            {
                grandTotal = orderTotal;
            }
            var existingOrder = _db.Orders.FirstOrDefault(x => x.SquareOrderId == orderId);
            if (existingOrder == null)
            {
                var o = new Order
                {
                    SquareOrderId = orderId,
                    ItemsTotal = orderTotal,
                    OrderDate = orderDate,
                    SubTotal = subTotal,
                    Taxes = taxesTotal,
                    GrandTotal = grandTotal,
                    PaymentsTotal = paymentsTotal,
                    RefundsTotal = refundsTotal,
                };
                _db.Orders.Add(o);
                newOrders++;
            }
            else
            {
                existingOrder.ItemsTotal = orderTotal;
                existingOrder.OrderDate = orderDate;
                existingOrder.SubTotal = subTotal;
                existingOrder.Taxes = taxesTotal;
                existingOrder.GrandTotal = grandTotal;
                existingOrder.PaymentsTotal = paymentsTotal;
                existingOrder.RefundsTotal = refundsTotal;
                _db.Orders.Update(existingOrder);
            }
        }
        _db.SaveChanges();
        return new OkResult();
    }
    
    [HttpPost]
    public string SetupCureProducts()
    {
        if (!_env.IsDevelopment())
        {
            return "This endpoint is only callable in development mode";
        }

        var result = "";
        
        try
        {
            if (!itemExists("CURE 2026 Ticket"))
            {
                _logger.LogText("Creating and stocking 'CURE 2026 Ticket' in catalog.");
                result += "Creating and stocking 'CURE 2026 Ticket' in catalog.\n";
                var cureId = CreateCURECatalogObject();
            }

            if (!itemExists("CURE 2026 Addon"))
            {
                _logger.LogText("Creating and stocking 'CURE 2026 Addon' in catalog.");
                result += "Creating and stocking 'CURE 2026 Addon' in catalog.\n";
                var addonId = CreateAddonCatalogObject();
            }
        }
        catch (ApiException e)
        {
            Console.WriteLine(e);
            throw;
        }
        return result;
    }

    [HttpPost]
    [Route("setupcureevent")]
    public string SetupCureEvent()
    {
        if (!_env.IsDevelopment())
        {
            return "This endpoint is only callable in development mode";
        }
        
        SetupCureProducts();

        Event CureEvent = new Event
        {
            Name = "CURE 2026",
            Description = "CURE 2026 Conference and party",
            StartDate = new DateOnly(2026, 1, 9),
            EndDate = new DateOnly(2026, 1, 11)
        };
        var result = _db.Events.Add(CureEvent);
        _db.SaveChanges();
        return result.Entity.Id.ToString() ?? "null";
    }

    private bool itemExists(string itemName)
    {
        var itemCount = _db.InventoryItems.Count(i => i.Type == itemName);
        return itemCount > 0;
    }

    private string CreateCURECatalogObject()
    {
        var cureTicketType = "CURE 2026 Ticket";
        
        InventoryItem standard = new InventoryItem
        {
            Description = "CURE 2026 Standard Ticket",
            Name = "Standard",
            IsActive = true,
            PriceInCents = 15000,
            Stock = 100,
            Type = cureTicketType
        };
        
        InventoryItem silver = new InventoryItem
        {
            Description = "CURE 2026 Silver Ticket",
            Name = "Silver",
            IsActive = true,
            PriceInCents = 27500,
            Stock = 100,
            Type = cureTicketType
        };
        
        InventoryItem gold = new InventoryItem
        {
            Description = "CURE 2026 Gold Ticket",
            Name = "Gold",
            IsActive = true,
            PriceInCents = 40000,
            Stock = 100,
            Type = cureTicketType
        };
        
        InventoryItem suiteItsASuite = new InventoryItem
        {
            Description = "CURE 2026 Suite it's a Suite Ticket",
            Name = "Suite it's a Suite",
            IsActive = true,
            PriceInCents = 25000,
            Stock = 100,
            Type = cureTicketType
        };
        
        var standardResult = _db.InventoryItems.Add(standard);
        var silverResult = _db.InventoryItems.Add(silver);
        var goldResult = _db.InventoryItems.Add(gold);
        var suiteResult = _db.InventoryItems.Add(suiteItsASuite);
        _db.SaveChanges();
        return standardResult.Entity.Id.ToString() ?? "null";
    }

    private string CreateAddonCatalogObject()
    {
        InventoryItem addon = new InventoryItem
        {
            Description = "Decadent Delights addon",
            Name = "Decadent Delights",
            IsActive = true,
            PriceInCents = 7500,
            Stock = 100,
            Type = "CURE 2026 Addon"
        };
        
        var addonResult = _db.InventoryItems.Add(addon);
        _db.SaveChanges();
        return addonResult.Entity.Id.ToString() ?? "null";
    }
}
