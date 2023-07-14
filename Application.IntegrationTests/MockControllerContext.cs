namespace Application.IntegrationTests
{
    public class MockControllerContext : ControllerContext
    {
        public MockControllerContext(ClaimsPrincipal user)
        {
            HttpContext = new DefaultHttpContext
            {
                User = user,
            };
        }
    }
}