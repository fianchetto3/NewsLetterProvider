using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Unsubscribe(ILogger<Unsubscribe> logger, DataContext datacontex)
{
    private readonly ILogger<Unsubscribe> _logger = logger;
    private readonly DataContext _datacontex = datacontex;

    [Function("Unsubscribe")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "newsletter/unsubscribe")] HttpRequestData req)
    {
        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var subscriptionRequest = JsonSerializer.Deserialize<SubscriptionRequest>(requestBody);

            if (subscriptionRequest == null || string.IsNullOrEmpty(subscriptionRequest.Email))
            {
                return new BadRequestObjectResult("Invalid request payload");
            }

            var existingSubscription = await _datacontex.NewsLetter.FirstOrDefaultAsync(s => s.Email == subscriptionRequest.Email);
            if (existingSubscription == null)
            {
                return new NotFoundObjectResult("Email not found");
            }

            _datacontex.NewsLetter.Remove(existingSubscription);
            await _datacontex.SaveChangesAsync();

            return new OkObjectResult("Unsubscribed from newsletter successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while unsubscribing from newsletter");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}

