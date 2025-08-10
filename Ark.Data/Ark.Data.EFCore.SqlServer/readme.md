## Jvd.Data.EFCore.Sqlite
### Purpose
This library allows to use SQL Server databases with the DbServices of the Ark.Data.EFCore library.

### How to use
Simply by inheriting the DbContext to use with Sqlite by the SqliteDbContext.
```C#
public class AppDbContext : SqlServerDbContext
{
}
```
Then when the database will be setup at startup using UseDatabase in the IHostBuilder, the context will be setup for SqlServer.
```C#
builder.UseDatabase<AppDbContext>();
```
For more details, check the [Ark.Data.EFCore](..\Ark.Data.EFCore\readme.md) documentation.