# TaskExcecutorProcess

- [x] генерации CSR из JKS
- [x] вывод контрольной суммы CSR
- [x] Формирование контрольного P12

- [x] Подключение пакета Nuget по работе аргументами командной строки
- [ ] Исправить стилистические ошибки
- [x] Исправить логические ошибки
- [x] Проверка установлен ли OpenSSL и KeyTool
- [ ] Исправить совместимость с Windows

- [ ] Исправить инструкцию
- [ ] Полноценное описание логики
- [ ] Отдельно, как проверяется OpenSSL и KeyTool для windows
- [ ] Как установить и добавить OpenSSL и KeyTool
- [ ] ДОбавить логику для особенностей работы с PowerShell Windows

***
**Использование:**

Формируем *.csr
 ```bash */
 MyProgram GetCSR <jksPath> <passwd>
 ```
| Option | Description |
| ------ | ------ |
| jksPath* (-j) | -путь до JKS файла |
| passwd* (-p) | -Пароль |
 
Формируем из CRT и нашего исходника *.new.p12
```bash */
MyProgram GetP12 <jksPath> <passwd> <crtPath> <csrPath> 
```
| Option | Description |
| ------ | ------ |
| jksPath* (-j) | -путь до JKS файла |
| passwd* (-p) | -Пароль |
| crtPath* (-c) | -путь до CRT файла ключа |
| csrPath* (-cs) | путь до CSR файла для сверки |



**НЮАНС:** *Пробелы в названии папок не поддерживаются внешними ресурсами*


***
**Для unix быть включен powerSHELL** 
```bash */
dotnet tool install --global PowerShell --version 7.0.3
```
***
**Особенности для Windows**

Для работы приложения требуется:
1. [OpenSSL](https://slproweb.com/products/Win32OpenSSL.html)
>FULL Version.
>Рекомендуется устанавливать OpenSSL вне вашей системной директории Windows.
2. [Java Keytool](https://www.java.com/ru/download/)
>Исполняемый файл утилиты распространяется вместе с Java SDK (или JRE), поэтому, если у вас установлен SDK, значит она у вас также будет предустановлена.
Исполняемый файл называется keytool. Чтобы выполнить его, откройте командную строку (cmd, console, shell и т.д.). и измените текущий каталог на каталог bin в каталоге установки Java SDK. Введите keytool, а затем нажмите клавишу Enter. 
>Пример: C:\Program Files\Java\jdk1.8.0_111\bin>keytool
3. Установленный PowerShell

