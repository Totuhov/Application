
namespace Application.UnitTests;

public class UserServiceTests
{
    private ApplicationDbContext _context;
    private IUserService _service;

    [OneTimeSetUp]
    public void TestInitialize()
    {
        List<ApplicationUser> users = new()
        {
            new ApplicationUser()
            {
                Id = "1",
                UserName = "guest",
                Email = "guest@guest.guest"
            }
        };

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryUserServiceDatabase")
            .Options;

        this._context = new ApplicationDbContext(options);
        this._context.Users.AddRange(users);
        this._context.SaveChanges();
        this._service = new UserService(this._context);
    }

    [Test]
    public async Task Test_IsUserExists_Succeed()
    {
        string username = "guest";

        bool rsult = await this._service.IsUserExists(username);

        Assert.IsTrue(rsult);
    }

    [Test]
    public async Task Test_IsUserExists_Fail()
    {
        string username = "notExist";

        bool rsult = await this._service.IsUserExists(username);

        Assert.IsFalse(rsult);
    }

    [Test]
    public async Task Test_GetUsernameByIdAsync()
    {
        string userId = "1";

        string username = await this._service.GetUsernameByIdAsync(userId);

        Assert.That(username, Is.EqualTo("guest"));
    }

    [Test]
    public async Task Test_GetUserEmailByUsernameAsync()
    {
        string username = "guest";

        string userEmail = await this._service.GetUserEmailByUsernameAsync(username);

        Assert.That(userEmail, Is.EqualTo("guest@guest.guest"));
    }
}
