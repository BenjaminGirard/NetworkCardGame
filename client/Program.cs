using System;

namespace client
{
    class Program
    {
        static void Main(string[] args)
        {
            Core core = new Core();

            try
            {
                core.StartClient();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
