namespace GameSecretsAPI.Models
{
    public class Game
    {
        //public long GameId { get; set; }
        public string Player1 { get; set; } = string.Empty;
        public string Player2 { get; set; } = string.Empty;
        public string PasswordPlayer1 { get; set; } = string.Empty;
        public string PasswordPlayer2 { get; set; } = string.Empty;
        public string PasswordTryPlayer1 { get; set; } = string.Empty;
        public string PasswordTryPlayer2 { get; set; } = string.Empty;
        public string CurrentPlayerTurn { get; set;} = string.Empty;
        public int RightNumbersTurn { get; set; } = 0;
    }
}
