RazorTrasformTool
------

This project help use razor syntax for trasform text like T4

Project support  include  external dll for compiler

Using Custom Tool for visual studio
-------

Build project RazorTransform Tool

Run command in command.bat with Administrator user for register dll 

Run install.reg for include custom tool information

Restart visual studio 

Goto test.cstt(razor syntax like .cshtml file) and set custom tool is RazorTransformTool

Using Console Application
-------

Build project RazorTransformConsole
type command razor template\test.cshtml for transform code

HeaderConfig
------
InputDllFolder : Folder contain dll for compiler code
OutputFile: out put file path for generate code. When using in custom tool It will only set extension in generate code

---

/*config
     OutPutFile=Out\data.html
     InputDllFolder=lib

*/

---


Some Helper Function in razor Syntax
------

---

    @R2();// write @ for generate code cshtml
    @WF(2,content,data)// write string format with 2 \t characters in start of line

---
