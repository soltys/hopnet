using Microsoft.Xna.Framework;


namespace Game
{
    class GameConstants
    {
        //General
        public const int HorizontalGameResolution = 1280;
        public const int VerticalGameResolution = 720;
        public const int DefaultHorizontalResolutionToScaleInto = 1280;
        public const int DefaultVerticalResolutionToScaleInto = 720;
        public const float SpaceBetweenPlatforms = 4f;
        public const float FirstPlatformPosition = -8.0f;
        public const float SpaceBetweenRows = 5f;
        public const float PlatformGroundLevel = 0.0f;
        public const float PlatformRadius = 1.86f;
        public const float EndOfBoardPositionZ = 11.0f;
        public const float BeginningOfBoardPositionZ = 8.0f;
        public const float SpeedOfPlatforms = 0.05f;
        public const float GameUpdatesPerSecond = 60.0f;


        public static Vector3 CameraPosition = new Vector3(0.0f, 5.0f, 10.0f);
        //public static Vector3 CameraPosition = new Vector3(10.0f, 0.0f, 0.0f); //debug camera

        //MainMenu
        public const int DefaultMenuBtnWidth = 420;
        public const int DefaultMenuBtnHeight = 210;
        public const float HandCursorRadius = 64;
        public const int HorizontalSpaceFromLeft = 100;
        public const int VerticalSpaceBetweenButtons = 20;
        public const int MenuTextureNumber = 2;
        public enum TextureType { Normal = 0, WithBorder = 1 };
        public enum Hand { Left = 0, Right = 1 };
        public enum MenuState { InMainMenu = 0, InNewGame = 1, InScores = 2, OnExit = 3, Playing = 4, OnDifficultySelect = 5, ExitConfirmed = 6, AfterGameLoss=7 }
        public enum MenuButton { Scores = 0, Exit = 1, GoBack = 2, None = 3, NewGame = 4, EasyDifficulty = 5, MediumDifficulty = 6, HardDifficulty = 7, ConfirmExit = 8, PlayAgain=9 }
        public enum GameDifficulty { Easy = 1, Medium = 2, Hard = 3 }
        public const int ButtonTimeDelayInSeconds = 2;


        //KinectPlayer
        public const float JumpHeightDivider = 20f;
        public const float PlayerModelHeight = 1.0f;
        public const float PlayerForwardJumpModifier = 0.2f;
        public const float PlayerSideJumpModifier = 0.1f;
        public const int NewGameCountdownTime = 3;
        public enum PlayerStance { Idle = 1, IdleJump = 2, LeftHandUp = 3, RightHandUp = 4, JumpReady = 7, Jump = 8, SideJump = 9, SideJumpReady = 10, GameEnded = 11, GameStartCountDown = 12 }

        //KinectData
        public const float HeightChangeThreshold = 0.005f;
        public const float HeightChangeTimeMiliseconds = 3000f;

        //PlatformCollection
        public const int LanesNumber = 5;
        public enum Direction { Left, Right };

        //PlatformRow
        public const int RowLength = 5;

        //HighScores
        public const int MaxCapacity = 10;
    }
}
