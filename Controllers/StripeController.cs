using dotnetstripe.Data;
using dotnetstripe.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace dotnetstripe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private readonly AppDbContext _db;
        public StripeController(AppDbContext db)
        {
            _db = db;
            StripeConfiguration.ApiKey = "stripe_sk_test_key";
        }

        [HttpPost, Route("clientsecret"), Authorize()]
        public async Task<ActionResult<ClientSecret>> GetClientSecret(PlanDto planDto)
        {
            var plan = _db.Plans.FirstOrDefault(u => u.Id == int.Parse(planDto.planId));
            if (plan == null) return BadRequest("No Plan found with this id.");
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)plan.Price,
                Currency = "inr",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };
            var service = new PaymentIntentService();
            var paymentIntent = service.Create(options);

            return new ClientSecret { clientSecret = paymentIntent.ClientSecret };
        }
    }
}
