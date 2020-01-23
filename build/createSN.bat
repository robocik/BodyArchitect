ildasm.exe ..\BA40\packages\XAMLMarkupExtensions.1.1.3\lib\net40\XAMLMarkupExtensions.dll /out:..\Assemblies\temp.il
ilasm.exe ..\Assemblies\temp.il /dll /key=..\Cert\key.snk /output=..\Assemblies\XAMLMarkupExtensions.dll
del ..\Assemblies\temp.il
pause
C:\Sources\BA\BA40\packages\XAMLMarkupExtensions.1.1.3\lib\net40