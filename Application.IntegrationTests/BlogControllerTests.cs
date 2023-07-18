
namespace Application.IntegrationTests
{
    [TestFixture]
    public class BlogControllerTests
    {
        private Mock<IBlogService> mockBlogService;
        private MockControllerContext mockControllerContext;
        private BlogController controller;
        private ClaimsPrincipal user;
        private List<CreateArticleViewModel> articles;
        private string username;

        [SetUp]
        public void Initialize()
        {
            this.mockBlogService = new Mock<IBlogService>();
            this.controller = new BlogController(mockBlogService.Object);

            this.user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "guest"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            mockControllerContext = new MockControllerContext(this.user);

            this.controller.ControllerContext = mockControllerContext;

            this.username = "guest";

            this.articles = new List<CreateArticleViewModel>()
            {
                new CreateArticleViewModel()
                {
                    Id = "1",
                    ApplicationUserId = "1",
                    Title = "First Article",
                    Content = "Mir wurde gesagt von denen, dass die die Ruckmeldkarte Ihnen schiken " +
                        "werden. Wahrscheinlich haben sie das noch nich gemacht. Ich habe mit denen " +
                        "gesprochen und sie werden, dass sie Ihnen so schnell wie möglich zusenden werden.",
                    EditedOn = DateTime.Now,
                    IsDeleted = false
                },
                new CreateArticleViewModel()
                {
                    Id = "1",
                    ApplicationUserId = "2",
                    Title = "First Article",
                    Content = "Mir wurde gesagt von denen, dass die die Ruckmeldkarte Ihnen schiken " +
                        "werden. Wahrscheinlich haben sie das noch nich gemacht. Ich habe mit denen " +
                        "gesprochen und sie werden, dass sie Ihnen so schnell wie möglich zusenden werden.",
                    EditedOn = DateTime.Now,
                    IsDeleted = false
                }
            };
        }

        [Test]
        public void Create_Get_ValidId_ReturnsView()
        {
            var result = controller.Create(this.username) as ActionResult;

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Create_Get_InvalidId_ReturnsNotFound()
        {
            var result = this.controller.Create("gues") as NotFoundResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task Create_Post_ValidModel_RedirectsToIndex()
        {
            var model = new CreateArticleViewModel
            {
                ApplicationUserId = "guest"
            };
            controller.TempData = new TempDataDictionary(mockControllerContext.HttpContext, Mock.Of<ITempDataProvider>());

            var result = await controller.Create(model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("Index"));
                Assert.That(result.ControllerName, Is.EqualTo("Portfolio"));
                Assert.That(controller.TempData["SuccessMessage"], Is.EqualTo("Article was posted successfully!"));
            });
        }

        [Test]
        public async Task Create_Post_InvalidValidModel_RedirectsToView()
        {
            var model = new CreateArticleViewModel
            {
                ApplicationUserId = "guest",
            };
            controller.ModelState.AddModelError("Title", "1");

            var result = await controller.Create(model) as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ViewName == null || result.ViewName == "Create", Is.True);
                Assert.That(result.ViewData.ModelState.IsValid, Is.False);
            });
        }

        [Test]
        public async Task Create_Post_ReturnsGeneralError()
        {
            var model = new CreateArticleViewModel();

            controller.TempData = new TempDataDictionary(mockControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
            mockBlogService.Setup(service => service.CreatePostAsync(It.IsAny<CreateArticleViewModel>())).ThrowsAsync(new Exception("Some error message"));

            var result = await controller.Create(model) as IActionResult;

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task Edit_Get_ValidId_ReturnsView()
        {
            var result = await controller.Edit("1") as ActionResult;

            Assert.That(result, Is.Not.Null);
        }
        [Test]
        public async Task Edit_Get_ReturnsGeneralError()
        {
            controller.TempData = new TempDataDictionary(mockControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
            mockBlogService.Setup(x => x.IsUserOwnerOfArticle("1", "1")).Throws<Exception>();

            var result = await controller.Edit("1");

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Edit_Get_InvalidId_ReturnsNotFound()
        {
            this.user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "guest"),
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));
            mockControllerContext = new MockControllerContext(this.user);
            this.controller.ControllerContext = mockControllerContext;

            var result = await this.controller.Edit("1") as NotFoundResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task Edit_Post_WhenUserIsOwner_ReturnsView()
        {
            mockBlogService.Setup(x => x.GetUsernameByArticleIdAsync("1")).ReturnsAsync("1");
            CreateArticleViewModel model = this.articles.First(x => x.Id == "1");
            controller.TempData = new TempDataDictionary(mockControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
            var result = await controller.Edit(model) as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.ActionName, Is.EqualTo("All")); ;
                Assert.That(controller.TempData["SuccessMessage"], Is.EqualTo("Article was edited successfuly!"));
            });
        }
        [Test]
        public async Task Edit_Post_WhenUserIsNotOwnerAndNotAdmin_ReturnsNotFound()
        {
            this.user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
             {
                new Claim(ClaimTypes.Name, "guest"),
                new Claim(ClaimTypes.NameIdentifier, "1")
             }, "mock"));
            mockBlogService.Setup(x => x.IsUserOwnerOfArticle("2", "1")).Returns(false);
            CreateArticleViewModel model = this.articles.First(x => x.Id == "1");

            var result = await controller.Edit(model);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Edit_Post_ReturnsGeneralError()
        {
            var model = new CreateArticleViewModel();
            controller.TempData = new TempDataDictionary(mockControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
            mockBlogService.Setup(x => x.GetUsernameByArticleIdAsync(model.Id)).Throws<Exception>();

            var result = await controller.Edit(model);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
                Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("Index"));
            });
        }

        [Test]
        public async Task Edit_Post_InvalidModelState()
        {
            CreateArticleViewModel model = this.articles.First(x => x.Id == "1");
            controller.ModelState.AddModelError("Title", "1");

            var result = await controller.Edit(model);

            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task Details_WhenArticleExists_ReturnsView()
        {
            string articleId = "123";
            ArticleViewModel expectedModel = new();
            mockBlogService.Setup(x => x.GetArticleViewModelByIdAsync(articleId)).ReturnsAsync(expectedModel);

            var result = await controller.Details(articleId);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<ViewResult>());
                Assert.That(((ViewResult)result).Model, Is.EqualTo(expectedModel));
            });
        }

        [Test]
        public async Task Details_WhenArticleDoesNotExist_ReturnsNotFound()
        {
            string articleId = "123";

            var result = await controller.Details(articleId);

            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public async Task Details_ExceptionThrown_ReturnsNotFound()
        {
            string articleId = "123";
            mockBlogService.Setup(x => x.GetArticleViewModelByIdAsync(articleId)).Throws<Exception>();

            var result = await controller.Details(articleId);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task All_WhenArticlesExist_ReturnsView()
        {
            string userName = "testuser";
            List<ArticleViewModel> expectedArticles = new List<ArticleViewModel>()
            {
                new ArticleViewModel { Id = "1", Title = "Article 1" },
                new ArticleViewModel { Id = "2", Title = "Article 2" }
            };
            mockBlogService.Setup(x => x.GetAllArticlesByUserNameAsync(userName)).ReturnsAsync(expectedArticles);

            var result = await controller.All(userName);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.InstanceOf<AllArticlesViewModel>());
            var model = (AllArticlesViewModel)viewResult.Model;
            Assert.Multiple(() =>
            {
                Assert.That(model.UserName, Is.EqualTo(userName));
                Assert.That(model.Articles, Is.EqualTo(expectedArticles));
            });
        }

        [Test]
        public async Task All_WhenNoArticlesExist_ReturnsView()
        {
            string userName = "testuser";
            List<ArticleViewModel> expectedArticles = new List<ArticleViewModel>();
            mockBlogService.Setup(x => x.GetAllArticlesByUserNameAsync(userName)).ReturnsAsync(expectedArticles);

            var result = await controller.All(userName);

            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.InstanceOf<AllArticlesViewModel>());
            var model = (AllArticlesViewModel)viewResult.Model;
            Assert.Multiple(() =>
            {
                Assert.That(model.UserName, Is.EqualTo(userName));
                Assert.That(model.Articles, Is.EqualTo(expectedArticles));
            });
        }

        [Test]
        public async Task All_ExceptionThrown_RedirectsToIndexAction()
        {
            string userName = "testuser";
            mockBlogService.Setup(x => x.GetAllArticlesByUserNameAsync(userName)).Throws<Exception>();

            var result = await controller.All(userName);

            // Assert
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = (RedirectToActionResult)result;
            Assert.Multiple(() =>
            {
                Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
                Assert.That(redirectResult.ControllerName, Is.EqualTo("Home"));
            });
        }

        [Test]
        public async Task Delete_WithValidArticleId()
        {
            mockBlogService.Setup(x => x.IsUserOwnerOfArticle("1", "1")).Returns(true);
            var model = mockBlogService.Setup(x => x.GetArticleViewModelByIdAsync("1"))
                .ReturnsAsync(new ArticleViewModel()
                {
                    Id = "1",
                    Title = "Title",
                    ApplicationUserName = "guest"
                });

            controller.TempData = new TempDataDictionary(mockControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
            var result = await controller.Delete("1");

            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
            var redirectResult = (RedirectToActionResult)result;
            Assert.Multiple(() =>
            {
                Assert.That(redirectResult.ActionName, Is.EqualTo("Details"));
                Assert.That(redirectResult.ControllerName, Is.EqualTo("Portfolio"));
            });
        }

        [Test]
        public async Task Delete_WithInvalidValidArticleIdAndUserIsNotAdmin()
        {
            this.user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "guest"),
                new Claim(ClaimTypes.NameIdentifier, "2")
            }, "mock"));
            mockControllerContext = new MockControllerContext(this.user);
            mockBlogService.Setup(x => x.IsUserOwnerOfArticle("notFound", "2")).Returns(false);
            this.controller.ControllerContext = mockControllerContext;

            var result = await this.controller.Delete("notFound") as NotFoundResult;

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Delete_ReturnsGeneralError()
        {            
            controller.TempData = new TempDataDictionary(mockControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
            mockBlogService.Setup(x => x.IsUserOwnerOfArticle("1", "1")).Throws<Exception>();

            var result = await controller.Delete("1");
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
                Assert.That(((RedirectToActionResult)result).ActionName, Is.EqualTo("Index"));
            });
        }
    }
}
