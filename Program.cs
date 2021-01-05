using PowerArgs;
using System;



namespace TaskExcecutorProcess
{
    class Program
    {
        static void Main(string[] args)
        {          
            Args.InvokeAction<MyArgs>(args);
            TaskExcecutorProcess.ConsoleHelper.ShowError(CheckingApp.Keytool());
            Console.WriteLine(CheckingApp.GetCheckOpenSslCommandLine());
            TaskExcecutorProcess.ConsoleHelper.ShowError(CheckingApp.GetCheckOpenSslCommandLine());
        }     
    }
}