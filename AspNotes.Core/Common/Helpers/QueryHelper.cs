using SqlKata;
using SqlKata.Compilers;
using System.Diagnostics;

namespace AspNotes.Core.Common.Helpers;

public static class QueryHelper
{
    public static void LogCompiledSql(Query query)
    {
#if DEBUG
        var compiler = new SqliteCompiler();
        var sql = compiler.Compile(query);
        Debug.WriteLine(sql.ToString());
#endif
    }
}
