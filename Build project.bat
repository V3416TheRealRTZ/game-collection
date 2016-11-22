@echo off
path "D:\Program Files\Unity\Editor";"C:\Program Files\Unity\Editor";"D:\Program Files (x86)\Unity\Editor";"C:\Program Files (x86)\Unity\Editor"
unity.exe -quit -batchmode -projectPath %1 -executeMethod Build.BuildWin32
@echo on
