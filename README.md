# TaskExcecutorProcess

- [x] генерации CSR из JKS
- [x] вывод контрольной суммы CSR
- [x] Формирование контрольного P12

- [x] Подключение пакета Nuget по работе аргументами командной строки
- [ ] Исправить стилистические ошибки
- [x] Исправить логические ошибки
- [ ] Проверка установлен ли OpenSSL и KeyTool
- [x] Исправить инструкцию

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
