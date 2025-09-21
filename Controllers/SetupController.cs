using KiCData.Models;
using KiCData.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using Square;
using Square.Authentication;
using Square.Exceptions;
using Square.Models;
using Event = KiCData.Models.Event;

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
    
    public SetupController(IConfigurationRoot configuration, IKiCLogger logger, KiCdbContext context, IWebHostEnvironment appenv)
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
    }
    
    [HttpPost]
    public string SetupCureProducts()
    {
        if (!_env.IsDevelopment())
        {
            return "This endpoint is only callable in development mode";
        }
        
        try
        {
            if (!itemExists("CURE 2026"))
            {
                _logger.LogText("Creating and stocking 'CURE 2026' in square catalog.");
                var cureId = CreateCURECatalogObject();
                RestockVariants(cureId, 100);
            }

            if (!itemExists("Decadent Delights"))
            {
                _logger.LogText("Creating and stocking 'Decadent Delights' in square catalog.");
                var addonId = CreateAddonCatalogObject();
                RestockVariants(addonId, 100);
            }
        }
        catch (ApiException e)
        {
            Console.WriteLine(e);
            throw;
        }
        return "OK";
    }

    [HttpPost]
    [Route("setupcureevent")]
    public string SetupCureEvent()
    {
        if (!_env.IsDevelopment())
        {
            return "This endpoint is only callable in development mode";
        }

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
        var response = _client.CatalogApi.SearchCatalogObjects(new SearchCatalogObjectsRequest(query: new CatalogQuery(exactQuery: new CatalogQueryExact("name", itemName))));
        return !(response.Objects == null || response.Objects.Count == 0);
    }

    private void RestockVariants(string itemId, int amount)
    {
        var response = _client.CatalogApi.RetrieveCatalogObject(itemId);
        List<InventoryChange> inventoryChanges = new List<InventoryChange>();
        var locationId = _client.LocationsApi.RetrieveLocation("main").Location.Id;
        foreach (var variant in response.MObject.ItemData.Variations)
        {
            inventoryChanges.Add(
                new InventoryChange("ADJUSTMENT", adjustment: new InventoryAdjustment(
                    catalogObjectId: variant.Id,
                    fromState: "NONE",
                    toState: "IN_STOCK",
                    quantity: amount.ToString(),
                    locationId: locationId,
                    occurredAt:  DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.ffffK")
                    ))
                );
        }
        var idempotencyKey = Guid.NewGuid().ToString("N");
        _client.InventoryApi.BatchChangeInventory(new BatchChangeInventoryRequest(idempotencyKey, inventoryChanges));
    }

    private string CreateCURECatalogObject()
    {
        var idempotencyKey = Guid.NewGuid().ToString("N");

        List<CatalogObject> cureVariations =
        [
            new CatalogObject(
                type: "ITEM_VARIATION",
                id: "#2",
                itemVariationData: new CatalogItemVariation(
                    inventoryAlertType: "NONE",
                    itemId: "#1",
                    priceMoney: new Money(10000, "USD"),
                    name: "Standard",
                    sku: "cure-standard",
                    trackInventory: true,
                    stockable: true
                )
            ),
            new CatalogObject(
                type: "ITEM_VARIATION",
                id: "#3",
                itemVariationData: new CatalogItemVariation(
                    inventoryAlertType: "NONE",
                    itemId: "#1",
                    priceMoney: new Money(10000, "USD"),
                    name: "Silver",
                    sku: "cure-silver",
                    trackInventory: true,
                    stockable: true
                )
            ),
            new CatalogObject(
                type: "ITEM_VARIATION",
                id: "#4",
                itemVariationData: new CatalogItemVariation(
                    inventoryAlertType: "NONE",
                    itemId: "#1",
                    priceMoney: new Money(10000, "USD"),
                    name: "Gold",
                    sku: "cure-gold",
                    trackInventory: true,
                    stockable: true
                )
            ),
            new CatalogObject(
                type: "ITEM_VARIATION",
                id: "#5",
                itemVariationData: new CatalogItemVariation(
                    inventoryAlertType: "NONE",
                    itemId: "#1",
                    priceMoney: new Money(10000, "USD"),
                    name: "Sweet It's a Suite",
                    sku: "cure-suite",
                    trackInventory: true,
                    stockable: true
                )
            )
        ];
        var cureCatalogObject = new CatalogObject(
            "ITEM",
            "#1",
            presentAtAllLocations: true,
            itemData: new CatalogItem("CURE 2026", productType: "EVENT", variations: cureVariations)
        );
        try
        {
            var response = _client.CatalogApi.UpsertCatalogObject(new UpsertCatalogObjectRequest(idempotencyKey, cureCatalogObject));
            return response.CatalogObject.Id;
        }
        catch (ApiException e)
        {
            Console.WriteLine(
                $"An error occured while updating addon catalog. API returned status code {e.ResponseCode}");
            foreach (var err in e.Errors)
            {
                Console.WriteLine($"[{err.Category}] {err.Code}: {err.Detail} Field: {err.Field}");
            }

            throw;
        }
    }

    private string CreateAddonCatalogObject()
    {
        var idempotencyKey = Guid.NewGuid().ToString("N");

        List<CatalogObject> addonVariations =
        [
            new CatalogObject(
                type: "ITEM_VARIATION",
                id: "#4",
                itemVariationData: new CatalogItemVariation(
                    inventoryAlertType: "NONE",
                    itemId: "#1",
                    priceMoney: new Money(5000, "USD"),
                    name: "Standard",
                    sku: "cure-addon-decadentdelight",
                    trackInventory: true,
                    stockable: true
                )
            )
        ];

        var addonCatalogObject = new CatalogObject
        (
            "ITEM",
            "#1",
            presentAtAllLocations: true,
            itemData: new CatalogItem("Decadent Delights", productType: "REGULAR", variations: addonVariations)
        );

        try
        {
            var response = _client.CatalogApi.UpsertCatalogObject(new UpsertCatalogObjectRequest(idempotencyKey, addonCatalogObject));
            return response.CatalogObject.Id;
        }
        catch (ApiException e)
        {
            Console.WriteLine($"An error occured while updating addon catalog. API returned status code {e.ResponseCode}");
            foreach (var err in e.Errors)
            {
                Console.WriteLine($"[{err.Category}] {err.Code}: {err.Detail} Field: {err.Field}");
            }
            throw;
        }
    }
}