using Infrastructure.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace NewsLetterProvider.Functions
{
    public class CheckUser(ILogger<CheckUser> logger, DataContext datacontex)
    {
        private readonly ILogger<CheckUser> _logger = logger;
        private readonly DataContext _datacontex = datacontex;

        [Function("CheckUser")]
        public async Task <IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route ="newsletter/status")] HttpRequestData req, string email)
        {
            var subscritpion = await _datacontex.NewsLetter.FirstOrDefaultAsync(s => s.Email == email);
            if (subscritpion != null) 
            { 
             return new OkObjectResult(new {IsSubscribed = subscritpion.IsSubscribed});
            }
            return new NotFoundObjectResult("Subscritpion not found");
        }

    }
}
