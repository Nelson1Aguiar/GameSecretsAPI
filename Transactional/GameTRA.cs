using GameSecretsAPI.Interfaces;
using GameSecretsAPI.Models;

namespace GameSecretsAPI.Transactional
{
    public class GameTRA : IGameTRA
    {
        private Game game = new Game();
        public void GameStart(string playerName, string password)
        {
            if (game == null) throw new ArgumentNullException(nameof(game));

            if (string.IsNullOrEmpty(game.Player1))
            {
                game.Player1 = playerName;
                game.PasswordPlayer1 = password;
            }
            else if (string.IsNullOrEmpty(game.Player2))
            {
                game.Player2 = playerName;
                game.PasswordPlayer2 = password;
            }
            else
            {
                throw new ApplicationException("Número máximo de jogadores atingidos");
            }

            game.CurrentPlayerTurn = game.Player1;
        }

        public void GameTurn(string player, string password)
        {
            game.RightNumbersTurn = 0;

            if(string.Equals(player, game.Player1))
            {
                for (int i = 0; i < password.Length; i++)
                {
                    if (password[i] == game.PasswordPlayer2[i])
                        game.RightNumbersTurn += 1;
                }

                game.CurrentPlayerTurn = game.Player2;
            }
            else if (string.Equals(player, game.Player2))
            {
                for (int i = 0; i < password.Length; i++)
                {
                    if (password[i] == game.PasswordPlayer1[i])
                        game.RightNumbersTurn += 1;
                }

                game.CurrentPlayerTurn = game.Player1;
            }
            else
            {
                throw new ArgumentNullException("Jogador não encontrado");
            }
        }

        public Game? GetGame()
        {
            if (game is not null)
            {
                return game;
            }

            return null;
        }

        private void ResetInstanceGame()
        {
            game.CurrentPlayerTurn = string.Empty;
            game.Player1 = string.Empty;
            game.Player2 = string.Empty;
            game.PasswordPlayer1 = string.Empty;
            game.PasswordPlayer2 = string.Empty;
            game.RightNumbersTurn = 0;
        }
    }
}
