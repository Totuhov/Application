namespace Application.UnitTests
{
    [TestFixture]
    public class BlogServiceTests
    {
        private IEnumerable<Article> _articles;
        private IEnumerable<ApplicationUser> _users;
        private IBlogService _blogService;

        private ApplicationDbContext _context;

        [OneTimeSetUp]
        public void TestInitialize()
        {
            this._users = new List<ApplicationUser>()
            {
                new ApplicationUser()
                {
                    Id = "1",
                    UserName = "guest"
                }
            };

            this._articles = new List<Article>()
            {
                new Article()
                {
                    Id = "1",
                    ApplicationUserId = "2",
                    Title = "First Article",
                    Content = "Mir wurde gesagt von denen, dass die die Ruckmeldkarte Ihnen schiken " +
                    "werden. Wahrscheinlich haben sie das noch nich gemacht. Ich habe mit denen " +
                    "gesprochen und sie werden, dass sie Ihnen so schnell wie möglich zusenden werden.",
                    CreatedOn = DateTime.Now,
                    EditedOn = DateTime.Now,
                    IsDeleted = false,
                    ApplicationUser = new ApplicationUser()
                },
                new Article()
                {
                    Id = "2",
                    ApplicationUserId = "1",
                    Title = "Second Article",
                    Content = "Mir wurde gesagt von denen, dass die die Ruckmeldkarte Ihnen schiken " +
                    "werden. Wahrscheinlich haben sie das noch nich gemacht. Ich habe mit denen " +
                    "gesprochen und sie werden, dass sie Ihnen so schnell wie möglich zusenden werden.",
                    CreatedOn = DateTime.Now,
                    EditedOn = DateTime.Now,
                    IsDeleted = false
                },
                new Article()
                {
                    Id = "3",
                    ApplicationUserId = "2",
                    Title = "Third Article",
                    Content = "Mir wurde gesagt von denen, dass die die Ruckmeldkarte Ihnen schiken" +
                    " werden. Wahrscheinlich haben sie das noch nich gemacht. Ich habe mit denen " +
                    "gesprochen und sie werden, dass sie Ihnen so schnell wie möglich zusenden werden.",
                    CreatedOn = DateTime.Now,
                    EditedOn = DateTime.Now,
                    IsDeleted = false,
                    ApplicationUser = new ApplicationUser()
                    {
                        UserName = "Nikolay",
                        Id = "2"
                    }
                }
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryBlogServiceDatabase")
                .Options;

            this._context = new ApplicationDbContext(options);
            this._context.Articles.AddRange(this._articles);
            this._context.Users.AddRange(this._users);
            this._context.SaveChanges();


            this._blogService = new BlogService(this._context);
        }

        [Test]
        public async Task Test_GetCreateArticleViewModelByIdAsync()
        {
            string articleId = "1";
            string userId = "2";

            CreateArticleViewModel? testModel = await this._blogService.GetCreateArticleViewModelByIdAsync(articleId, userId);
            Assert.Multiple(() =>
            {
                Assert.That(testModel, Is.Not.EqualTo(null));
                Assert.That(testModel.IsDeleted, Is.EqualTo(false));
                Assert.That(testModel.Id, Is.EqualTo(articleId));
                Assert.That(testModel.ApplicationUserId, Is.EqualTo(userId));
                Assert.That(testModel.Title, Is.EqualTo("First Article"));
                Assert.That(testModel.Content, Is.EqualTo("Mir wurde gesagt von denen, dass die die " +
                    "Ruckmeldkarte Ihnen schiken werden. Wahrscheinlich haben sie das noch nich gemacht. Ich " +
                    "habe mit denen gesprochen und sie werden, dass sie Ihnen so schnell wie möglich zusenden werden."));
            });
        }

        [Test]
        public async Task Test_CreatePostAsync()
        {

            var article = new CreateArticleViewModel()
            {
                ApplicationUserId = "1",
                Title = "Fourth Article",
                Content = "Mir wurde gesagt von denen, dass die die Ruckmeldkarte Ihnen schiken werden. " +
                "Wahrscheinlich haben sie das noch nich gemacht. Ich habe mit denen gesprochen und " +
                "sie werden, dass sie Ihnen so schnell wie möglich zusenden werden.",
                EditedOn = DateTime.Now,
                IsDeleted = false
            };

            await this._blogService.CreatePostAsync(article);

            Assert.That(_context.Articles.Count(), Is.EqualTo(4));
        }

        [Test]
        public async Task Test_SavePostAsync()
        {

            string articleId = "2";
            string userId = "1";

            CreateArticleViewModel? testModel = await this._blogService.GetCreateArticleViewModelByIdAsync(articleId, userId);

            testModel.Title = "Edited Second Article";

            await this._blogService.SavePostAsync(testModel);

            testModel = await this._blogService.GetCreateArticleViewModelByIdAsync(articleId, userId);

            Assert.That(testModel.Title, Is.EqualTo("Edited Second Article"));
        }

        [Test]
        public async Task Test_GetAllArticlesByUserNameAsync()
        {
            string username = "guest";

            List<ArticleViewModel> testModels = await this._blogService.GetAllArticlesByUserNameAsync(username);

            Assert.That(testModels, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task Test_GetArticleViewModelByIdAsync_ReturnsArticleViewModel()
        {
            string articleId = "3";

            ArticleViewModel tesModel = await this._blogService.GetArticleViewModelByIdAsync(articleId);

            Assert.That(tesModel, Is.Not.EqualTo(null));
        }

        [Test]
        public async Task Test_DeleteArteicleAsync()
        {
            string articleId = "3";

            ArticleViewModel testModel = await this._blogService.GetArticleViewModelByIdAsync(articleId);

            await this._blogService.DeleteArticleAsync(testModel);

            testModel = await this._blogService.GetArticleViewModelByIdAsync(articleId);

            Assert.That(testModel.IsDeleted, Is.EqualTo(true));
        }

        [Test]
        public void Test_IsUserOwnerOfArticle_True()
        {
            string articleId = "2";
            string applicationUserId = "1";

            var result = this._blogService.IsUserOwnerOfArticle(articleId, applicationUserId);            

            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void Test_IsUserOwnerOfArticle_False()
        {
            string articleId = "1";
            string applicationUserId = "1";

            var result = this._blogService.IsUserOwnerOfArticle(articleId, applicationUserId);

            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public async Task Test_GetUsernameByArticleIdAsync()
        {
            string articleId = "2";

            var result = await this._blogService.GetUsernameByArticleIdAsync(articleId);

            Assert.That(result, Is.EqualTo("guest"));
        }
    }
}
