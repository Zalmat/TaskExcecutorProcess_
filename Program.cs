using System;

namespace TaskExcecutorProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            //сделать обработку аргументов следующим обращом:
            //MyProgram.exe 
            //      -j [pathJKS]
            //      -c [pathCRT]
            // ПОИЩИ ОБРАБОТКУ АРГУМЕНТОВ в C# КАК ДЕЛАЕТСЯ            
            ConsoleHelper.SetDefaultColor();


            if (args.Length == 0)            
                ConsoleHelper.ShowHelp();            
            else
            {   
                if(args[0] == "-j")
                {
                    string pathFile = args[1];
                    CertTool.passwCert = args[2]; //пароль               
                    if (!System.IO.File.Exists(pathFile))
                    {
                        //если файл не существует то покажем ошибку
                        ConsoleHelper.ShowError("Указаного файла не существует!");
                        return;
                    }                
                    CertTool.CreateCsr(pathFile);
                }
                else if (args[0] == "-c")
                    {
                        string jksPathFile = args[1];
                        CertTool.passwCert = args[2]; //пароль                        
                        string newCrtPathFile = args[3]; //Присланный CRT для сверки и формирования ключа
                        string oldCsrPathFile = args[4]; //Старый CSR Для сверки
                            if (!System.IO.File.Exists(jksPathFile)||(!System.IO.File.Exists(oldCsrPathFile))||(!System.IO.File.Exists(newCrtPathFile)))
                            {
                                //если файл не существует то покажем ошибку
                                ConsoleHelper.ShowError("Один или несколько файлов не найдены");
                                return;
                            } 
                            CertTool.CreateNewKey(pathJksFile:jksPathFile, crtPathFile:newCrtPathFile, csrPathFile:oldCsrPathFile);                                      
                    }

                //Для теста
                else if (args[0] == "-crc")
                    {
                        string oldCrtPathFile = args[1];
                        string newCrtPathFile = args[2];
                        Console.WriteLine(CertTool.GetCRC(oldCrtPathFile));
                        Console.WriteLine(CertTool.GetCRC(newCrtPathFile));
                        Console.WriteLine(CertTool.GetCRC(oldCrtPathFile,newCrtPathFile));
                    } 
                else ConsoleHelper.ShowHelp();
            }
        }
    }
}
