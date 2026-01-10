namespace Rise.Domain.Tests.News
{
    public class NewsShould
    {
        [Fact]
        public void Should_Create_News_With_All_Required_Fields()
        {
            var news = new Domain.News.News
            {
                Title = "Campus Update",
                ImageUrl = "https://hogent.be/news-image.png",
                Subject = "Important Announcement",
                Date = DateTime.UtcNow,
                Content = "We have updated our campus schedule.",
                AuthorName = "John Doe",
                AuthorFunction = "Administrator",
                AuthorAvatarUrl = "https://hogent.be/avatar.png"
            };

            news.ShouldNotBeNull();
            news.Title.ShouldBe("Campus Update");
            news.ImageUrl.ShouldBe("https://hogent.be/news-image.png");
            news.Subject.ShouldBe("Important Announcement");
            var now = DateTime.UtcNow;
            (news.Date - now).Duration().ShouldBeLessThan(TimeSpan.FromSeconds(1));
            news.Content.ShouldBe("We have updated our campus schedule.");
            news.AuthorName.ShouldBe("John Doe");
            news.AuthorFunction.ShouldBe("Administrator");
            news.AuthorAvatarUrl.ShouldBe("https://hogent.be/avatar.png");
        }

        [Fact]
        public void Should_Create_News_Without_AuthorAvatar()
        {
            var news = new Domain.News.News
            {
                Title = "No Avatar News",
                ImageUrl = "https://hogent.be/news-image.png",
                Subject = "General Info",
                Date = DateTime.UtcNow,
                Content = "Content without avatar",
                AuthorName = "Jane Doe",
                AuthorFunction = "Editor",
                AuthorAvatarUrl = null
            };

            news.AuthorAvatarUrl.ShouldBeNull();
        }

        [Theory]
        [InlineData("", "https://hogent.be/image.png", "Subject", "Content", "Author", "Function")]
        [InlineData("Title", "", "Subject", "Content", "Author", "Function")]
        [InlineData("Title", "https://hogent.be/image.png", "", "Content", "Author", "Function")]
        [InlineData("Title", "https://hogent.be/image.png", "Subject", "", "Author", "Function")]
        [InlineData("Title", "https://hogent.be/image.png", "Subject", "Content", "", "Function")]
        [InlineData("Title", "https://hogent.be/image.png", "Subject", "Content", "Author", "")]
        public void Creating_News_With_Null_Required_Fields_Should_Throw_Exception(
            string title,
            string imageUrl,
            string subject,
            string content,
            string authorName,
            string authorFunction)
        {
            Exception exception = Record.Exception(() => new Domain.News.News
            {
                Title = title!,
                ImageUrl = imageUrl!,
                Subject = subject!,
                Date = DateTime.UtcNow,
                Content = content!,
                AuthorName = authorName!,
                AuthorFunction = authorFunction!
            });

            exception.ShouldNotBeNull();
        }

        [Fact]
        public void Should_Set_Date_Correctly()
        {
            var expectedDate = new DateTime(2025, 11, 8);
            var news = new Domain.News.News
            {
                Title = "Date Test",
                ImageUrl = "https://hogent.be/image.png",
                Subject = "Subject",
                Date = expectedDate,
                Content = "Content",
                AuthorName = "Author",
                AuthorFunction = "Function"
            };

            news.Date.ShouldBe(expectedDate);
        }
    }
}
