using System.Web;

namespace Krab.Global.Extensions
{
    public static class HttpContextExtensions
    {
        private const string UserId = "UserId";

        public static int GetUserId(this HttpContext context)
        {
            return (int)context.Items[UserId];
        }

        public static void SetUserId(this HttpContext context, int userId)
        {
            context.Items.Add(UserId, userId);
        }
    }
}
