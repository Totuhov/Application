using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient.Server;

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