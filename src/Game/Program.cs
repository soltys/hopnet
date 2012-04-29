using System;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace Game
{
#if WINDOWS || XBOX
    static class Program
    {
        private static WindsorContainer windsorContainer;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (HopnetGame game = new HopnetGame())
            {
                game.Run();
            }
        }
    }
#endif
}

