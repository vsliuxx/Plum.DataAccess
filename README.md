一、Oracle访问
1、注册GAC
查看GAC：c:\windows\system32\gacutil.exe /l Oracle.DataAccess
注册GAC：c:\windows\system32\gacutil.exe /i Oracle.DataAccess.dll
2、配置程序集
  <system.data>
    <DbProviderFactories>
    <add name="Oracle Data Provider for .NET" invariant="Oracle.DataAccess.Client" description="Oracle Data Provider for .NET" type="Oracle.DataAccess.Client.OracleClientFactory, Oracle.DataAccess, Version=2.111.6.20, Culture=neutral, PublicKeyToken=89b483f429c47342"/>
    </DbProviderFactories>
  </system.data>
3、调用方法
Vic.Data.DataAccess dbAccess = new Vic.Data.DataAccess("Data Source=orcl;User ID=test;Password=test;", Vic.Data.DbProviderType.OracleClient);
dbAccess.Query("select * from users ");
