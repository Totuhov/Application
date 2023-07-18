using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient.Server;
using Microsoft.Extensions.Primitives;

namespace Application.IntegrationTests
{
    public class MockControllerContext : ControllerContext
    {
        public MockControllerContext(ClaimsPrincipal user)
        {
            HttpContext = new DefaultHttpContext
            {
                User = user                
            };
        }
    }
}