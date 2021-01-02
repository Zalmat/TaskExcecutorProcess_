using System;
using PowerArgs;
namespace TaskExcecutorProcess
{

[ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
public class MyArgs
{
    [HelpHook, ArgShortcut("-?"), ArgDescription("Показать помощь\nНЮАНС: Пробелы в названии папок не поддерживаются внешними ресурсами\nСсылка на исходник: https://github.com/Zalmat/TaskExcecutorProcess_")]
    public bool Help { get; set; }
    
     [ArgActionMethod, ArgShortcut("j"), ArgDescription("Сформирует *.csr")]
    public void GetCSR(GetCSR args)
    {
    	CertTool.passwCert = args.passwd;
        CertTool.CreateCsr(args.jksPath);
    }

     [ArgActionMethod, ArgShortcut("c"), ArgDescription("Сформирует *.new.p12")]
    public void GetP12(GetP12 args)
    {
    	CertTool.passwCert = args.passwd;
        CertTool.CreateNewKey(args.jksPath,args.crtPath,args.crtPath);
    }

}

public class GetCSR
{
    [ArgRequired,ArgExistingFile, ArgDescription("путь до JKS файла"), ArgPosition(1)]
    public string jksPath { get; set; }
    [ArgRequired, ArgDescription("Пароль"), ArgPosition(2)]
    public string passwd { get; set; }
}

public class GetP12
{
    [ArgRequired,ArgExistingFile, ArgDescription("путь до JKS файла"), ArgPosition(1)]
    public string jksPath { get; set; }
    [ArgRequired, ArgDescription("Пароль"), ArgPosition(2)]
    public string passwd { get; set; }
    [ArgRequired,ArgExistingFile, ArgDescription("путь до CRT файла ключа"), ArgPosition(3)]
    public string crtPath { get; set; }
    [ArgRequired(PromptIfMissing=false), ArgExistingFile, ArgDescription("путь до CSR файла для сверки"), ArgPosition(4)]
    public string csrPath { get; set; }
}

}