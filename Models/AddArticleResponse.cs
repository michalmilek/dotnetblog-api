using auth_playground.Entities;

namespace auth_playground.Models;

public class AddArticleResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int UserId { get; set; }
    
    public AddArticleResponse(Article article)
    {
        Id = article.Id;
        Title = article.Title;
        Content = article.Content;
        CreatedAt = article.CreatedAt;
        UpdatedAt = article.UpdatedAt;
        UserId = article.UserId;
    }
}