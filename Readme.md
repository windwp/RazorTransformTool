RazorTransformTool
------

This project use razor syntax for transform text like T4

Project support include external dll for compiler

Sample
------

![Code Folder](http://i.imgur.com/3hDEHsX.jpg)

![SourceCode generate](http://i.imgur.com/aQD1qoT.gif)

Using CustomTool for visual studio
-------

Install RazorTransformTool extensions

[Install](https://visualstudiogallery.msdn.microsoft.com/68d81c9e-3e07-47bd-a0ec-27ab4a80ef02)

Restart visual studio 

Choose a template file(.cshtml) then open Properties Panel and set custom tool to RazorTransformTool

![Setting CustomTool](http://i.imgur.com/MoGYq3e.png)


Using Console Application
-------

Start project RazorTransformConsole

type command razor template\test.cshtml for transform code


HeaderConfig
------
InputDllFolder : Folder contain dll for compiler code

OutputFile:  file path for save file using in console application. When using in custom tool It will only set extension in generate code

IsHeader=1 : auto generate header in result file
---

/*config

     OutPutFile=Out\data.html
     InputDllFolder=lib

*/

---


Some Helper Function in razor Syntax
------

---
    @R("Data");// print raw data
    @R2();// write @ for generate code cshtml
    @WF(2,content,data)// write string format with 2 \t characters in start of line

---



---
Project use [RazorEngine](https://github.com/Antaris/RazorEngine) and [Tharga Console](https://github.com/poxet/tharga-console)