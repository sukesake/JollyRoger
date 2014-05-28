namespace JollyRoger
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new JollyRoger.Game())
            {
                game.Run();
            }
        }
    }
}
