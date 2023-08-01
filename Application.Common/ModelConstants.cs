
namespace Application.Common;

public static class ModelConstants
{
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

        public const string DefaultProfileImageCharacteristic = "defaultProfileImage";
        public const string DefaultProjectImageCharacteristic = "defaultProjectImage";
    }
    public static class PortfolioConstants
    {
        public const string GreetingsMessageDefaultText  = "Hallo ";
        public const string UserDisplayNameDefaultText = "friend";
        public const string DescriptionDefaultText = "Here you can write something to describe you or your business";
        public const string AboutDefaultText = "Here, you can describe your work, competences, or just a short autobiography.";

        public const int GreetingsMessageMaxLength = 50;
        public const int UserDisplayNameMaxLength = 100;
        public const int DescriptionMaxLength = 200;
        public const int AboutMaxLength = 1000;
    }
    public static class ProjectConstants
    {
        public const int NameMaxLength = 100;
        public const int NameMinLength = 1;
        public const int DescriptionMaxLength = 100;
        public const int UrlMaxLength = 2048;
    }
    public static class ContactFormConstants
    {
        public const int SenderNameMaxLength = 100;
        public const int SenderNameMinLength = 2;
        public const int SenderEmailMaxLength = 150;
        public const int TextMaxLength = 5000;
    }
    public static class RoleConstants
    {
        public const int RoleNameMaxLength = 50;
        public const int RoleNameMinLength = 2;
    }
    public static class SocialMediaConstants
    {
        public const string FacebookDisplayName = "Facebook";
        public const string InstagramDisplayName = "Instagram";
        public const string LinkedInDisplayName = "LinkedIn";
        public const string TwitterDisplayName = "Twitter";


        public const int UrlMaxLength = 2048;        
    }
}
