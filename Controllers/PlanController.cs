using dotnetstripe.Data;
using dotnetstripe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnetstripe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlansController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PlansController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet, Authorize()]
        public async Task<ActionResult<List<Plan>>> GetPlans()
        {
            var plans = _db.Plans.ToList();
            return plans;
        }
    }
}
