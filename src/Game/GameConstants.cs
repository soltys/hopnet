using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Game
{
    class GameConstants
    {
        //----- DO NOT CHANGE! -----//
        public const float DefaultSpaceBetweenRows = 5.0f;
        public const float DefaultTimerMultiplier = 0.16767f;
        public const float DefaultJumpGravity = 0.001f;
        public const float DefaultSpeedBetweenPlatforms = 0.1f;
        //----- DO NOT CHANGE! -----//

        public const float MaxiumumJumpHeight = 2.0f;

        //General
        public static int HorizontalGameResolution = 1280;
        public static int VerticalGameResolution = 720;
        public static bool IsGameInFullScreen = false;
        public const float GameUpdatesPerSecond = 100.0f;

        public static Stopwatch zegar= new Stopwatch();


        public const int DefaultHorizontalResolutionToScaleInto = 1280;
        public const int DefaultVerticalResolutionToScaleInto = 720;
        public const float SpaceBetweenPlatforms = 5f;
        public const float SpaceBetweenRows = 5f;
        public const float PlatformGroundLevel = 0.0f;
        public const float PlatformRadius = 1.86f;
        public const float EndOfBoardPositionZ = BeginningOfBoardPositionZ + PlatformRadius;
        public const float BeginningOfBoardPositionZ = 8.0f;
        public const float SpeedOfPlatformsOneUpdate = 0.01f;
        
        public const float FirstPlatformPosition = -(RowLength / 2) * SpaceBetweenPlatforms;

        //public static Vector3 CameraPosition = new Vector3(FirstPlatformPosition+(RowLength/2)*SpaceBetweenPlatforms, 5.0f, 14.0f);
        //public static Vector3 CameraLookAtPoint = new Vector3(FirstPlatformPosition + (RowLength / 2) * SpaceBetweenPlatforms, 0.0f, -30f);

        public static Vector3 CameraPosition = new Vector3(10f,2f,5f);
        public static Vector3 CameraLookAtPoint = new Vector3(0f,2,5f);

        public const float NearPlane = 1.0f;
        public const float FarPlane = 200.0f;


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
        public const float JumpHeightDivider = 200f;
        public const float PlayerModelHeight = 1.0f;
        public const float PlayerForwardJumpModifier = 0.2f;
        public const float PlayerSideJumpModifier = 0.1f;
        public const int NewGameCountdownTime = 1;
        public enum PlayerStance { Idle = 1, IdleJump = 2, LeftHandUp = 3, RightHandUp = 4, JumpReady = 7, Jump = 8, SideJump = 9, SideJumpReady = 10, GameEnded = 11, GameStartCountDown = 12 }

        public const float JumpGravity = 0.1f;



        //KinectData
        public const float HeightChangeThreshold = 0.005f;
        public const float HeightChangeTimeMiliseconds = 3000f;

        //PlatformCollection
        public const int LanesNumber = 10;
        public enum Direction { Left, Right };

        //PlatformRow
        public const int RowLength = 5;

        //HighScores
        public const int MaxCapacity = 10;

        
    }
}
