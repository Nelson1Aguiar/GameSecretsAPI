using GameSecretsAPI.Models;

namespace GameSecretsAPI.Interfaces
{
    public interface IGameTRA
    {
        public void GameStart(string playerName, string password);
        public Game? GetGame();
        public void GameTurn(string playerName, string password);
    }
}
