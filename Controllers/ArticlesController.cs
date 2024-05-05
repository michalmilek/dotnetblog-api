using auth_playground.Entities;
using auth_playground.Models;
using auth_playground.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace auth_playground.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ArticlesController: ControllerBase
{
    private readonly IArticleService _articleService;
    
    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }
    
    [HttpPost]
    [Route("add-article")]
    public async Task<AddArticleResponse> AddArticle(AddArticleRequest article)
    {
        return await _articleService.AddArticle(article);
    }
    
    [HttpGet]
    public async Task<IEnumerable<Article>> GetAllArticles()
    {
        return await _articleService.GetAllArticles();
    }
    
    [HttpGet]
    [Route("{id}")]
    public async Task<Article> GetArticleById(int id)
    {
        return await _articleService.GetArticleById(id);
    }
    
    [HttpGet]
    [Route("user/{userId}")]
    public async Task<IEnumerable<Article>> GetArticlesByUserId(int userId)
    {
        return await _articleService.GetArticlesByUserId(userId);
    }
    
    [HttpPut]
    [Route("{id}")]
    public async Task<Article> UpdateArticle(int id, AddArticleRequest article)
    {
        return await _articleService.UpdateArticle(id, article);
    }
    
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        await _articleService.DeleteArticle(id);
        return Ok(new { message = "Article deleted" });
    }
}