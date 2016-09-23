using Krab.DataAccess.Dac;
using Krab.Web.Models.Response;

namespace Krab.Web.Controllers.Api
{
    public class HealthController : BaseController
    {
        private readonly IUserDac _userDac;

        public HealthController(IUserDac userDac)
        {
            _userDac = userDac;
        }

        public OkResponse<string> Get()
        {
            var userId = GetUserId();

            return new OkResponse<string>($"everything is good to go ({userId}).");
        }
    }
}