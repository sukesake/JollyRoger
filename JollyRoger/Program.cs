using System;

namespace JollyRoger
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var game = new JollyRoger.Game())
            {
                game.Run();
            }
        }
    }
}
