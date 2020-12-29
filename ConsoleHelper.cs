using System;
using System.Collections.Generic;
using System.Text;
/*
Этот блок с понтами и описанием. Хелпер и FAQ.
*/
namespace TaskExcecutorProcess
{
    public class ConsoleHelper
    {
        const ConsoleColor colorYellow = ConsoleColor.Yellow;
        static ConsoleColor colorDefault;

        public static void SetDefaultColor() { colorDefault = Console.ForegroundColor; }

        public static void ShowHelp()
        {
            Console.WriteLine("     Приложение для обработки запросов на сертификат");
            Console.WriteLine("Ответ: в зависимости от выбранного параметра возвращает либо *.csr и хэш сумму(контрольную),");
            Console.WriteLine("либо готовый .p12 сертификат на основании CRT поставщика");
            Console.WriteLine("Использование:");
            Console.WriteLine("\tMyProgram -j [путь до JKS файла] [Пароль] ");
            Console.WriteLine("\t\tСформирует *.csr");
            Console.WriteLine("\tMyProgram -c [путь до JKS файла] [Пароль] [путь до CRT файла ключа] [путь до CSR файла для сверки]");
            Console.WriteLine("\t\tСформирует *.new.p12");  
            Console.WriteLine("\nНЮАНС: Пробелы в названии папок не поддерживаются внешними ресурсами");
            Console.WriteLine("\nСсылка на исходник: https://github.com/Zalmat/TaskExcecutorProcess");          
        }

        public static void ShowError(string text)
        {
            Console.ForegroundColor = colorYellow;
            Console.WriteLine(text);
            Console.ForegroundColor = colorDefault;
        }
    }
}
