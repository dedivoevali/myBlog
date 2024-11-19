using Common.Models.Abstract;

namespace Common.Models
{
    public class PostModel : BaseModel
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public int AuthorId { get; set; }

        public string AuthorUsername { get; set; }
        public string AuthorInitials { get; set; }

        public string Topic { get; set; }

        public int AmountOfComments { get; set; }
        public string? Language { get; set; }
    }
}