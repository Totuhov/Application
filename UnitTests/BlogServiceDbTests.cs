using Application.Data;
using Application.Data.Models;
using Application.Services;
using Application.Services.Interfaces;
using Application.Web.ViewModels.Article;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace UnitTests
{
    [TestFixture]
    public class BlogServiceDbTests
    {
        private IEnumerable<Article> _articles;
        private IEnumerable<ApplicationUser> _users;
        private ApplicationDbContext _context;

        [OneTimeSetUp]
        public void TestInitialize()
        {
            this._users = new List<ApplicationUser>()
            {
                new ApplicationUser()
                {
                    Id = "6960a305-4ea5-4d22-8611-c437f5e5164c",
                    UserName = "guest"
                }
            };

            this._articles = new List<Article>()
            {
                new Article()
                {
                    Id = "193e0179-9a55-4f6e-ac06-ce75d48bd6c2",
                    ApplicationUserId = "c184f1f8-ccd7-4d62-b9df-ab4221cfce01",
                    Title = "First Article",
                    Content = "Mir wurde gesagt von denen, dass die die Ruckmeldkarte Ihnen schiken werden. Wahrscheinlich haben sie das noch nich gemacht. Ich habe mit denen gesprochen und sie werden, dass sie Ihnen so schnell wie möglich zusenden werden.",
                    CreatedOn = DateTime.Now,
                    EditedOn = DateTime.Now,
                    IsDeleted = false,
                    ApplicationUser = new ApplicationUser()
                },
                new Article()
                {
                    Id = "2e63dbec-91bc-4c09-8aa6-7c6e70fd3ad6",
                    ApplicationUserId = "6960a305-4ea5-4d22-8611-c437f5e5164c",
                    Title = "Second Article",
                    Content = "Mir wurde gesagt von denen, dass die die Ruckmeldkarte Ihnen schiken werden. Wahrscheinlich haben sie das noch nich gemacht. Ich habe mit denen gesprochen und sie werden, dass sie Ihnen so schnell wie möglich zusenden werden.",
                    CreatedOn = DateTime.Now,
                    EditedOn = DateTime.Now,
                    IsDeleted = false
                },
                new Article()
                {
                    Id = "293e0179-9a55-4f6e-ac06-ce75d48bd6c3",
                    ApplicationUserId = "c184f1f8-ccd7-4d62-b9df-ab4221cfce01",
                    Title = "Third Article",
                    Content = "Mir wurde gesagt von denen, dass die die Ruckmeldkarte Ihnen schiken werden. Wahrscheinlich haben sie das noch nich gemacht. Ich habe mit denen gesprochen und sie werden, dass sie Ihnen so schnell wie möglich zusenden werden.",
                    CreatedOn = DateTime.Now,
                    EditedOn = DateTime.Now,
                    IsDeleted = false,
                    ApplicationUser = new ApplicationUser()
                    {
                        UserName = "Nikolay",
                        Id = "1960a305-4ea5-4d22-8611-c437f5e5164d"
                    }
                }
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryApplicationDatabase")
                .Options;

            this._context = new ApplicationDbContext(options);
            this._context.Articles.AddRange(this._articles);
            this._context.Users.AddRange(this._users);
            this._context.SaveChanges();
        }
        [Test]
        public async Task Test_GetCreateArticleViewModelByIdAsync()
        {
            string articleId = "193e0179-9a55-4f6e-ac06-ce75d48bd6c2";
            string userId = "c184f1f8-ccd7-4d62-b9df-ab4221cfce01";

            IBlogService service = new BlogService(this._context);
            CreateArticleViewModel? testModel = await service.GetCreateArticleViewModelByIdAsync(articleId, userId);
            Assert.Multiple(() =>
            {
                Assert.That(testModel, Is.Not.EqualTo(null));
                Assert.That(testModel.IsDeleted, Is.EqualTo(false));
                Assert.That(testModel.Id, Is.EqualTo(articleId));
                Assert.That(testModel.ApplicationUserId, Is.EqualTo(userId));
                Assert.That(testModel.Title, Is.EqualTo("First Article"));
                Assert.That(testModel.Content, Is.EqualTo("Mir wurde gesagt von denen, dass die die Ruckmeldkarte Ihnen schiken werden. Wahrscheinlich haben sie das noch nich gemacht. Ich habe mit denen gesprochen und sie werden, dass sie Ihnen so schnell wie möglich zusenden werden."));
            });
        }

        [Test]
        public async Task Test_CreatePostAsync()
        {
            IBlogService service = new BlogService(this._context);

            var article = new CreateArticleViewModel()
            {
                ApplicationUserId = "6960a305-4ea5-4d22-8611-c437f5e5164c",
                Title = "Fourth Article",
                Content = "Mir wurde gesagt von denen, dass die die Ruckmeldkarte Ihnen schiken werden. Wahrscheinlich haben sie das noch nich gemacht. Ich habe mit denen gesprochen und sie werden, dass sie Ihnen so schnell wie möglich zusenden werden.",
                EditedOn = DateTime.Now,
                IsDeleted = false
            };

            await service.CreatePostAsync(article);

            Assert.That(_context.Articles.Count(), Is.EqualTo(4));
        }

        [Test]
        public async Task Test_SavePostAsync()
        {
            IBlogService service = new BlogService(this._context);

            string articleId = "2e63dbec-91bc-4c09-8aa6-7c6e70fd3ad6";
            string userId = "6960a305-4ea5-4d22-8611-c437f5e5164c";

            CreateArticleViewModel? testModel = await service.GetCreateArticleViewModelByIdAsync(articleId, userId);

            testModel.Title = "Edited Second Article";

            await service.SavePostAsync(testModel);

            testModel = await service.GetCreateArticleViewModelByIdAsync(articleId, userId);

            Assert.That(testModel.Title, Is.EqualTo("Edited Second Article"));
        }

        [Test]
        public async Task Test_GetAllArticlesByUserNameAsync()
        {
            string username = "guest";
            IBlogService service = new BlogService(this._context);
            List<ArticleViewModel> testModels = await service.GetAllArticlesByUserNameAsync(username);

            Assert.That(testModels.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Test_GetArticleViewModelByIdAsync_ReturnsArticleViewModel()
        {
            IBlogService service = new BlogService(this._context);
            string articleId = "293e0179-9a55-4f6e-ac06-ce75d48bd6c3";

            ArticleViewModel tesModel = await service.GetArticleViewModelByIdAsync(articleId);

            Assert.That(tesModel, Is.Not.EqualTo(null));
        }
    }
}
