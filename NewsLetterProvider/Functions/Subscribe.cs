using Data.Requests;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SiliconBlazorFrontEnd.Data;
using System.Text.Json;

namespace NewsLetterProvider.Functions;

public class Subscribe(ILogger<Subscribe> logger, DataContext datacontex)
{
    private readonly ILogger<Subscribe> _logger = logger;
    private readonly DataContext _datacontex = datacontex;

    [Function("Subscribe")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "newsletter/subscribe")] HttpRequestData req)
    {
        try
        {
            // Deserialize the request body
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var subscriptionRequest = JsonSerializer.Deserialize<SubscriptionRequest>(requestBody);

            if (subscriptionRequest == null || string.IsNullOrEmpty(subscriptionRequest.Email))
            {
                return new BadRequestObjectResult("Invalid request payload");
            }

            // Check if the email is already subscribed
            var existingSubscription = await _datacontex.NewsLetter.FirstOrDefaultAsync(s => s.Email == subscriptionRequest.Email);
            if (existingSubscription != null)
            {
                return new ConflictObjectResult("Email is already subscribed");
            }

            // Create a new subscription
            var newSubscription = new NewsLetterEntity
            {
                Id = Guid.NewGuid().ToString(), // Generate a new GUID for the Id
                Email = subscriptionRequest.Email,
                IsSubscribed = true
            };

            // Add the new subscription to the database
            _datacontex.NewsLetter.Add(newSubscription);
            await _datacontex.SaveChangesAsync();

            return new OkObjectResult("Subscribed to newsletter successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while subscribing to newsletter");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}


