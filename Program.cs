using System;
using PowerArgs;

namespace TaskExcecutorProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            Args.InvokeAction<MyArgs>(args);
        }     
    }
}