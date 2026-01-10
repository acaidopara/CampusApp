using System.ComponentModel.DataAnnotations.Schema;
using Ardalis.GuardClauses;

namespace Rise.Domain.News
{
    public class News : Entity
    {
        private string _title = default!;
        public required string Title
        {
            get => _title;
            set => _title = Guard.Against.NullOrWhiteSpace(value, nameof(Title));
        }

        private string _imageUrl = "image1.jpg";

        public string ImageUrl
        {
            get => _imageUrl;
            set => _imageUrl = Guard.Against.NullOrWhiteSpace(value, nameof(ImageUrl)) ?? _imageUrl;
        }


        private string _subject = default!;
        public required string Subject
        {
            get => _subject;
            set => _subject = Guard.Against.NullOrWhiteSpace(value, nameof(Subject));
        }

        private DateTime _date = default!;
        public required DateTime Date
        {
            get => _date;
            set => _date = Guard.Against.Default(value, nameof(Date));
        }

        [Column(TypeName = "TEXT")]
        private string _content = default!;
        public required string Content
        {
            get => _content;
            set => _content = Guard.Against.NullOrWhiteSpace(value, nameof(Content));
        }

        private string _authorName = default!;
        public required string AuthorName
        {
            get => _authorName;
            set => _authorName = Guard.Against.NullOrWhiteSpace(value, nameof(AuthorName));
        }

        private string _authorFunction = default!;
        public required string AuthorFunction
        {
            get => _authorFunction;
            set => _authorFunction = Guard.Against.NullOrWhiteSpace(value, nameof(AuthorFunction));
        }

        public string? AuthorAvatarUrl { get; set; }
    }
}