set Constants=DEBUGINFO
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe msbuild\build.proj /fl1 /flp1:LogFile=Build.log;Verbosity=Normal /m -tv:14.0
pause