using DickinsonBros.Infrastructure.Cosmos.Abstractions.Models;

namespace DickinsonBros.IntegrationTests.Tests.Infrastructure.Cosmos.Models
{
    public class Todo
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; }
    }
}
