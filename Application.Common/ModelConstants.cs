
namespace Application.Common;

public static class ModelConstants
{
    public const string DefaultProfileImageCharacteristic = "defaultProfileImage";
    public const string DefaultProjectImageCharacteristic = "defaultProjectImage";

    public static class ApplicationUserConstants
    {

    }
    public static class ArticleConstants
    {
        public const int TitleMaxLength = 200;
        public const int TitleMinLength = 2;
        public const int ContentMaxLength = 4000;
        public const int ContentMinLength = 150;
    }
    public static class ImageConstants
    {
        public const int FileExtensionMaxLength = 10;
        public const int CharacteristicMaxLength = 20;
    }
    public static class PortfolioConstants
    {
        public const string GreetingsMessageDefaultText  = "Hallo ";
        public const string UserDisplayNameDefaultText = "friend";
        public const string DescriptionDefaultText = "Hier you can write somethig to describe you...";
        public const string AboutDefaultText = "here you can describe your work, competences or just a short autobiography";

        public const int GreetingsMessageMaxLength = 50;
        public const int UserDisplayNameMaxLength = 100;
        public const int DescriptionMaxLength = 200;
        public const int AboutMaxLength = 1000;
    }
    public static class ProjectConstants
    {
        public const int NameMaxLength = 100;
        public const int DescriptionMaxLength = 500;
        public const int UrlMaxLength = 2048;
    }
}
