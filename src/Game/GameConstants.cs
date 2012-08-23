
namespace Game
{
    class GameConstants
    {
        public const int HorizontalGameResolution = 1280;
        public const int VerticalGameResolution = 720;

        public const int DefaultHorizontalResolutionToScaleInto = 1280;
        public const int DefaultVerticalResolutionToScaleInto = 720;

        public const int DefaultMenuBtnWidth = 420;
        public const int DefaultMenuBtnHeight = 210;

        public const float HandCursorRadius = 64;
        public const float JumpHeightDivider = 20f;
        public const float PlayerModelHeight = 1.0f;

        public const float HeightChangeThreshold = 0.005f;
        public const float HeightChangeTimeMiliseconds = 3000f;


        public const int MenuTextureNumber = 2;
        public const float PlayerForwardJumpModifier = 0.2f;
        public const float PlayerSideJumpModifier = 0.1f;
        public enum TextureType { Normal = 0, WithBorder = 1 };
        public enum Hand { Left = 0, Right = 1 };
        public enum MenuState { InMainMenu = 0, InNewGame = 1, InScores = 2, OnExit = 3, Playing = 4, OnDifficultySelect = 5, ExitConfirmed = 6 }
        public enum MenuButton { Scores = 0, Exit = 1, GoBack = 2, None = 3, NewGame = 4, EasyDifficulty = 5, MediumDifficulty = 6, HardDifficulty = 7, ConfirmExit = 8 }
        public enum GameDifficulty { Easy = 1, Medium = 2, Hard = 3 }
        public enum PlayerStance { Idle = 1, IdleJump = 2, LeftHandUp = 3, RightHandUp = 4, JumpReady = 7, Jump = 8, SideJump = 9, SideJumpReady = 10, GameEnded=11, GameStartCountDown=12 }
        public const int NewGameCountdownTime = 4;
    }
}
