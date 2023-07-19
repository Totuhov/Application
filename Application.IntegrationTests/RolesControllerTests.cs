
namespace Application.IntegrationTests
{
    public class RolesControllerTests
    {
        private Mock<RoleManager<IdentityRole>> roleManagerMock;
        private Mock<UserManager<ApplicationUser>> userManagerMock;
        private MockControllerContext mockControllerContext;
        private RolesController controller;
        private ClaimsPrincipal user;

        [SetUp]
        public void Initialize()
        {
            this.roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

            this.userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            this.controller = new RolesController(roleManagerMock.Object, userManagerMock.Object);

            this.user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "guest"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            mockControllerContext = new MockControllerContext(this.user);

            this.controller.ControllerContext = mockControllerContext;
            controller.TempData = new TempDataDictionary(mockControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
        }

        [Test]
        public void Index_ReturnsView()
        {
            var result = controller.Index() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.Not.Null);
            Assert.That(result.Model, Is.InstanceOf<IQueryable<IdentityRole>>());
        }

        [Test]
        public void Create_ReturnsView()
        {
            var result = controller.Create() as ViewResult;

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            var roleName = "TestRole";

            roleManagerMock
                .Setup(m => m.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            { { "name", roleName } });
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            var result = await controller.Create(roleName) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task Create_InvalidModel_ReturnsView()
        {
            var roleName = "";
            controller.ModelState.AddModelError("roleName", "");

            var result = await controller.Create(roleName) as ViewResult;

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Create_RoleCreationFails_ReturnsViewWithErrors()
        {
            var roleName = "TestRole";

            var errorMessage = "Role creation failed";
            roleManagerMock
                .Setup(m => m.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = errorMessage }));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            { { "name", roleName } });
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            var result = await controller.Create(roleName);
        }

        [Test]
        public async Task Delete_RoleFound_RedirectsToIndex()
        {
            var roleId = "TestId";
            var role = new IdentityRole { Id = roleId, Name = "TestRole" };
            roleManagerMock
                .Setup(m => m.FindByIdAsync(roleId))
                .ReturnsAsync(role);
            roleManagerMock
                .Setup(m => m.DeleteAsync(role))
                .ReturnsAsync(IdentityResult.Success);

            var result = await controller.Delete(roleId) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task Delete_RoleNotFound_ReturnsViewWithModelError()
        {
            var roleId = "TestId";
            roleManagerMock.Setup(m => m.FindByIdAsync(roleId)).ReturnsAsync((IdentityRole)null);

            var result = await controller.Delete(roleId) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Model, Is.Not.Null); // Check if the model is not null
                Assert.That(controller.ModelState.IsValid, Is.False); // Check if the ModelState is invalid
                Assert.That(controller.ModelState.ContainsKey(""), Is.True); // Check if there is an error associated with the model
                Assert.That(controller.ModelState[""].Errors[0].ErrorMessage, Is.EqualTo("No role found")); // Check the error message
            });
        }

        [Test]
        public async Task Delete_RoleDeletionFails_ReturnsViewWithErrors()
        {
            var roleId = "TestId";
            var role = new IdentityRole { Id = roleId, Name = "TestRole" };
            roleManagerMock
                .Setup(m => m.FindByIdAsync(roleId))
                .ReturnsAsync(role);
            var errorMessage = "Role deletion failed";
            roleManagerMock
                .Setup(m => m.DeleteAsync(role))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = errorMessage }));

            var result = await controller.Delete(roleId) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Model, Is.Not.Null);
                Assert.That(controller.ModelState.IsValid, Is.False);
                Assert.That(controller.ModelState.ContainsKey(""), Is.True);
                Assert.That(controller.ModelState[""].Errors[0].ErrorMessage, Is.EqualTo(errorMessage));
            });
        }

        [Test]
        public async Task Update_ReturnsCorrectViewData()
        {
            string roleId = "someRoleId";
            var role = new IdentityRole { Id = roleId, Name = "RoleName" };
            var members = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "memberId1", UserName = "Member1" },
                new ApplicationUser { Id = "memberId2", UserName = "Member2" }
            };
            var nonMembers = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "nonMemberId1", UserName = "NonMember1" },
                new ApplicationUser { Id = "nonMemberId2", UserName = "NonMember2" }
            };

            roleManagerMock.Setup(rm => rm.FindByIdAsync(roleId)).ReturnsAsync(role);
            userManagerMock.Setup(um => um.Users).Returns(members.Concat(nonMembers).AsQueryable());
            userManagerMock.Setup(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), role.Name)).ReturnsAsync((ApplicationUser user, string roleName) => members.Contains(user));

            var result = await controller.Update(roleId) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.InstanceOf<EditRoleModel>());

            var model = result.Model as EditRoleModel;
            Assert.Multiple(() =>
            {
                Assert.That(model.Role, Is.EqualTo(role));
                Assert.That(model.Members, Is.EqualTo(members));
                Assert.That(model.NonMembers, Is.EqualTo(nonMembers));
            });
        }

        [Test]
        public async Task Update_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            var model = new ModificationRoleModel
            {
                RoleName = "RoleName",
                RoleId = "someRoleId",
                AddIds = new string[] { "userId1", "userId2" },
                DeleteIds = new string[] { "userId3", "userId4" }
            };

            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "userId1" },
                new ApplicationUser { Id = "userId2" },
                new ApplicationUser { Id = "userId3" },
                new ApplicationUser { Id = "userId4" }
            };

            userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((string userId) => users.FirstOrDefault(u => u.Id == userId));

            userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock.Setup(um => um.RemoveFromRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await controller.Update(model) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public async Task Update_WithInvalidModel_ReturnsViewWithError()
        {
            // Arrange
            var model = new ModificationRoleModel
            {
                RoleName = null, // Invalid model state
                RoleId = "someRoleId",
                AddIds = new string[] { "userId1", "userId2" },
                DeleteIds = new string[] { "userId3", "userId4" }
            };
            controller.ModelState.AddModelError("roleName", "");

            // Act
            var result = await controller.Update(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
