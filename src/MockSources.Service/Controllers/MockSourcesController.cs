using Microsoft.AspNetCore.Mvc;
using MockSources.Service.Data;

namespace MockSources.Service.Controllers;

// Exposes mock data endpoints for bank feed, card provider, and EFT transactions.

[ApiController]
[Route("mock")]

public class MockSourcesController : ControllerBase
{
    [HttpGet("bank-feed")]
    public IActionResult GetBankFeed()
    {
        var data = MockDataGenerator.GenerateBankFeed();
        return Ok(data);
    }

    [HttpGet("card-provider")]
    public IActionResult GetCardProvider()
    {
        var data = MockDataGenerator.GenerateCardProvider();
        return Ok(data);
    }

    [HttpGet("eft-transfers")]
    public IActionResult GetEftTransfers()
    {
        var data = MockDataGenerator.GenerateEft();
        return Ok(data);
    }
}