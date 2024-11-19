using Common.Models.Abstract;

namespace Common.Models
{
    public class CommentModel : BaseModel
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; }
        public int AuthorId { get; set; }
        public string AuthorUsername { get; set; }
        public string AuthorInitials { get; set; }
        public string Content { get; set; }
    }
}