using System.ComponentModel.DataAnnotations;

namespace auth_playground.Models;

public class AddArticleRequest
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }
}