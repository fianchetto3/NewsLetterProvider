using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace NewsLetterProvider.Functions
{
    public class Unsubscribe(ILogger<Unsubscribe> logger, DataContext datacontex)
    {
        private readonly ILogger<Unsubscribe> _logger = logger;
        private readonly DataContext _dataContex = datacontex;

        [Function("Unsubscribe")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route ="newsletter/unsubscribe")] HttpRequestData req, string email)
        {
            var subscription = await _dataContex.NewsLetter.FirstOrDefaultAsync(s => s.Email == email);
            if (subscription != null)
            {
                _dataContex.NewsLetter.Remove(subscription);
                await _dataContex.SaveChangesAsync();
                return new OkObjectResult("Unsubscribed from newsletter successfully");
            }

            return new NotFoundObjectResult("Subscription not found.");

        }
    }
}
