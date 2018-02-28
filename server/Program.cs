using System;


namespace sever
{
    class Program
    {
        static void Main(string[] args)
        {
            Core application = new Core(10000);

            application.Run();
        }
    }
}
