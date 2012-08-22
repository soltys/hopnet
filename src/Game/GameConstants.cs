
namespace Game
{
    class GameConstants
    {
        public const int DefaultScreenWidth = 1280;
        public const int DefaultScreenHeight = 720;

        public const int DefaultMenuBtnWidth = 420;
        public const int DefaultMenuBtnHeight = 210;

        public const int MenuTextureNumber = 2;

        public enum Texture : int { Normal = 0, WithBorder = 1 };
        public enum Hand : int { Left = 0, Right = 1 };
        public enum State : int { InMainMenu = 0, InNewGame = 1, InScores = 2, OnExit = 3, Playing = 4, OnDifficultySelect = 5, ExitConfirmed = 6 }
        public enum ButtonSelect : int { Scores = 0, Exit = 1, GoBack = 2, None = 3, NewGame = 4, EasyDifficulty = 5, MediumDifficulty = 6, HardDifficulty = 7, ConfirmExit = 8 }
        public enum GameDifficulty : int { Easy = 1, Medium = 2, Hard = 3 }
    }
}
