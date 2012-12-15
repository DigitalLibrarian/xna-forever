using System;

namespace ForeverDriverGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ForeverDriverGame game = new ForeverDriverGame())
            {
                game.Run();
            }
        }
    }
#endif
}

