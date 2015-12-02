using System;
using System.Data.SqlClient;
using System.Reflection;

namespace Web.Utilities
{
    public static class SqlBulkCopyExtension
    {
        const String _rowsCopiedFieldName = "_rowsCopied";
        static FieldInfo _rowsCopiedField = null;

        public static int RowsCopiedCount(this SqlBulkCopy bulkCopy)
        {
            if (_rowsCopiedField == null) _rowsCopiedField = typeof(SqlBulkCopy).GetField(_rowsCopiedFieldName, BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            return (int)_rowsCopiedField.GetValue(bulkCopy);
        }
    }
}