del *.suo /F /A:H
rd MultiMediaCenter\obj /S /Q
copy MultiMediaCenter\bin\Debug\MultiMediaCenter.exe D:\Programs\MultimediaCenter
copy MultiMediaCenter\bin\Debug\MultiMediaCenter.cnf D:\Programs\MultimediaCenter
move MultiMediaCenter\bin\Debug\MultimediaCenter.cnf MultiMediaCenter\bin
del MultiMediaCenter\bin\Debug\*.* /F /Q
move MultiMediaCenter\bin\MultimediaCenter.cnf MultiMediaCenter\bin\Debug
