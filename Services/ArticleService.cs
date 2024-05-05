using auth_playground.Models;
using auth_playground.Entities;
using System.Threading.Tasks;
using auth_playground.Authorization;
using auth_playground.Helpers;
using Microsoft.EntityFrameworkCore;

namespace auth_playground.Services;
    public interface IArticleService
    {
        public Task<AddArticleResponse> AddArticle(AddArticleRequest article);
        public Task<IEnumerable<Article>> GetAllArticles();
        
        public Task<Article> GetArticleById(int id);
        
        public Task<IEnumerable<Article>> GetArticlesByUserId(int userId);
        
        public Task<Article> UpdateArticle(int id, AddArticleRequest article);
        
        public Task DeleteArticle(int id);
    }

    public class ArticleService : IArticleService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtUtils _jwtUtils;

        public ArticleService(DataContext context, IHttpContextAccessor httpContextAccessor, IJwtUtils jwtUtils)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _jwtUtils = jwtUtils;
        }

        public async Task<AddArticleResponse> AddArticle(AddArticleRequest article)
        {
           var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
           var userId = _jwtUtils.GetUserIdFromToken(); 
            if (userId == null)
            {
                throw new ArgumentException("Invalid UserId: No corresponding user found.");
            }

            var user = _context.Users.Find(userId);
            if (user == null)
            {
                throw new ArgumentException("Invalid UserId: No corresponding user found.");
            }

            var newArticle = new Article
            {
                Title = article.Title,
                Content = article.Content,
                UserId = (int)userId,
                User = user
            };

            _context.Articles.Add(newArticle);
            await _context.SaveChangesAsync();

            return new AddArticleResponse(newArticle);
        }
        
        public async Task<IEnumerable<Article>> GetAllArticles()
        {
            return await _context.Articles.ToListAsync();
        }
        
        public async Task<Article> GetArticleById(int id)
        {
            return await _context.Articles.FindAsync(id);
        }
        
        public async Task<IEnumerable<Article>> GetArticlesByUserId(int userId)
        {
            return await _context.Articles.Where(article => article.UserId == userId).ToListAsync();
        }
        
        public async Task<Article> UpdateArticle(int id, AddArticleRequest article)
        {
            var existingArticle = await _context.Articles.FindAsync(id);
            if (existingArticle == null)
            {
                throw new ArgumentException("Invalid ArticleId: No corresponding article found.");
            }

            existingArticle.Title = article.Title;
            existingArticle.Content = article.Content;
            existingArticle.UpdatedAt = DateTime.Now;

            _context.Articles.Update(existingArticle);
            await _context.SaveChangesAsync();

            return existingArticle;
        }
        
        public async Task DeleteArticle(int id)
        {
            var existingArticle = await _context.Articles.FindAsync(id);
            if (existingArticle == null)
            {
                throw new ArgumentException("Invalid ArticleId: No corresponding article found.");
            }

            _context.Articles.Remove(existingArticle);
            await _context.SaveChangesAsync();
        }
    }