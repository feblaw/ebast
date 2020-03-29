using DataTables.AspNet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace App.Services.Utils
{
    public static class Extension
    {
        public static string GetFilter(this IColumn column)
        {
            if (column == null) return string.Empty;
            if (!column.IsSearchable || column.Search == null) return string.Empty;
            if (column.Search.IsRegex) return string.Empty;
            if (string.IsNullOrEmpty(column.Search.Value))
                return string.Empty;

            return string.Format("({0} != null AND {0}.ToString().ToLower().Contains(\"{1}\"))",
               column.Name,
                column.Search.Value.ToLower());
        }

        public static string GetFilter(this IColumn column, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return string.Empty;
            if (!column.Name.Contains("."))
            {
                return string.Format("({0} != null && {0}.ToString().ToLower().Contains(\"{1}\"))",
                    column.Name,
                    search.ToLower());
            }
            else
            {
                //case: ubah "a.b.c != null"  jadi "a != null && a.b != null && a.b.c != null"
                var columnNames = column.Name.Split('.'); //split berdasarkan titik
                var prefix = columnNames[0]; //ambil yang paling pertama, biar gampang urus si &&
                var builder = new StringBuilder(prefix).Append(" != null "); //builder udah terinit nama pertama
                for (int i = 1; i < columnNames.Length; i++)
                {
                    prefix = prefix + '.' + columnNames[i]; //setiap nama baru, append
                    builder.Append(" && ").Append(prefix).Append(" != null ");
                }
                var result = string.Format("({0} && {1}.ToString().ToLower().Contains(\"{2}\"))", //ga perlu lagi != null, karena udah ditangani di for
                    builder,
                    column.Name,
                    search.ToLower());
                return result;
            }
        }

        public static string GetFilter(this IEnumerable<IColumn> columns)
        {
            return columns.GetFilter(false);
        }

        public static string GetFilter(this IEnumerable<IColumn> columns, string search, string opr = "AND")
        {
            return columns.GetFilter(false, search, opr);
        }

        public static string GetFilter(this IEnumerable<IColumn> columns, bool includeWhere)
        {
            if (columns == null || !columns.Any()) return string.Empty;

            var wheres = columns
                .Select(x => x.GetFilter())
                .Where(x => !string.IsNullOrWhiteSpace(x));

            var result = string.Empty;
            if (wheres.Any())
            {
                if (includeWhere) result += "WHERE ";
                result += string.Join(" AND ", wheres);
            }
            return result;
        }

        public static string GetFilter(this IEnumerable<IColumn> columns, bool includeWhere, string search, string opr)
        {
            if (columns == null || !columns.Any()) return string.Empty;

            var wheres = columns
                .Select(x => x.GetFilter(search))
                .Where(x => !string.IsNullOrWhiteSpace(x));

            var result = string.Empty;
            if (wheres.Any())
            {
                if (includeWhere) result += "WHERE ";
                result += string.Join(string.Format(" {0} ", opr), wheres);
            }
            return result;
        }

        public static string GetSort(this IColumn column)
        {
            if (column == null) return string.Empty;
            if (!column.IsSortable || column.Sort == null) return string.Empty;

            return string.Format("{0}{1}",
                column.Name,
                column.Sort.Direction == SortDirection.Ascending
                    ? " asc"
                    : " desc");
        }

        public static string GetSort(this IEnumerable<IColumn> columns)
        {
            if (columns == null || !columns.Any()) return string.Empty;

            var sorts = columns
                .Where(x => x.IsSortable && x.Sort != null)
                .OrderBy(x => x.Sort.Order)
                .Select(x => x.GetSort())
                .Where(x => !string.IsNullOrWhiteSpace(x));

            var result = string.Empty;
            if (sorts.Any())
            {
                result = string.Join(", ", sorts);
            }
            return result;
        }
    }
}
