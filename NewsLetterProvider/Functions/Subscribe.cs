using Infrastructure.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SiliconBlazorFrontEnd.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

public class SubscribeFunction(DataContext context)
{
    private readonly DataContext _context = context;

    [Function("Subscribe")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post", Route = "newsletter/subscribe")] HttpRequestData req,
        FunctionContext context)
    {
        var logger = context.GetLogger("SubscribeFunction");

        try
        {
            // Läs innehållet på frontend sidan 
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            logger.LogInformation($"Request Body: {requestBody}");

            // Läs som string
            var subscriptionRequest = JsonSerializer.Deserialize<SubscriptionRequest>(requestBody);

            if (subscriptionRequest == null || string.IsNullOrEmpty(subscriptionRequest.Email))
            {
                logger.LogWarning("Email is null or empty in the request body.");
                return new BadRequestObjectResult("Invalid request body.");
            }


            var entity = new NewsLetterEntity
            {
                Id = Guid.NewGuid().ToString(),
                Email = subscriptionRequest.Email,
                IsSubscribed = true
            };

            // Kontrollera om email redan finns i databasen
            if (await _context.NewsLetter.AnyAsync(x => x.Email == entity.Email))
            {
                logger.LogInformation($"Email '{entity.Email}' already exists in database.");
                return new ConflictResult(); 
            }

            // Lägger till användaren som subscriber
            _context.NewsLetter.Add(entity);
            await _context.SaveChangesAsync();

            logger.LogInformation($"New email '{entity.Email}' subscribed successfully.");
            return new OkResult(); 
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while subscribing to newsletter.");
            return new BadRequestResult(); 
        }
    }
}


// JsonPropertyName, viktig för att allt ska funka. Annars klagar den på att JSON object / system.string inte går ihop. 
public class SubscriptionRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; }
}
