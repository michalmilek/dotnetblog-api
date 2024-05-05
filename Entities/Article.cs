using System;
using System.ComponentModel.DataAnnotations;

namespace auth_playground.Entities;

    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public int UserId { get; set; }
        public User User { get; set; }
    }