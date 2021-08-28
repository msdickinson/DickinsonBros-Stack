namespace DickinsonBros.Middleware.Function.Runner.Models
{
    public class JWTResponse
    {
        public string AccessToken { get; set; }
        public int AccessTokenExpiresIn { get; set; }
        public string TokenType { get; set; }
    }
}
