using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace NewsLetterProvider.Functions
{
    public class GetSubscribedUsers(ILogger<GetSubscribedUsers> logger, DataContext dataContext)
    {
        private readonly ILogger<GetSubscribedUsers> _logger = logger;
        private readonly DataContext _dataContext = dataContext;

        [Function("GetSubscribedUsers")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "newsletter/subscribers")] HttpRequestData req)
        {
            _logger.LogInformation("HTTP trigger function 'GetSubscribedUsers' processed a request.");

            try
            {
                var subscribedEmails = await _dataContext.NewsLetter
                    .Where(s => s.IsSubscribed)
                    .Select(s => s.Email)
                    .ToListAsync();

                return new OkObjectResult(subscribedEmails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving subscribed users");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }



    }
}
