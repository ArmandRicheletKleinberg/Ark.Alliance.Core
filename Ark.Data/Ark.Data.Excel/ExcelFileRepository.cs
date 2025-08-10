using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace Ark.Data.Excel
{
    /// <summary>
    /// This is the base class for all the manipulation on a excel worksheet. 
    /// </summary>
    public class ExcelFileRepository
    {
        /// <summary>
        /// Take a list of Entity and return it as an excel spreadsheet excel in an array of bytes.
        /// Just take the primitive type properties.
        /// It also works with an enumerable of ExpandoObject.
        /// </summary>
        /// <typeparam name="T">Type of the entity of the list</typeparam>
        /// <param name="items">List of data items to be inserted into an excel array.</param>
        /// <returns>
        /// Success : The Excel file has been successfully created.
        /// BadParameters : The items provided are null.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual async Task<Result<byte[]>> GetExcelBytesFromIEnumerable<T>(IEnumerable<T> items)
            => await Task.Run(() => Result<byte[]>.SafeExecute(() =>
            {
                if (items == null)
                    return Result<byte[]>.BadParameters.WithReason("The items should be set and not null.");

                var itemsArray = items as T[] ?? items.ToArray();
                var isExpandoObject = typeof(T) == typeof(ExpandoObject);

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Export");

                if (itemsArray.HasNoElements() && isExpandoObject)
                    return new Result<byte[]>(package.GetAsByteArray());

                var properties = isExpandoObject
                    ? ((ExpandoObject)(object)itemsArray.First()).Select(kvp => kvp.Key).ToArray()
                    : typeof(T).GetProperties().Select(p => p.Name).ToArray();

                var filterProperties = properties.Where(p => !(!isExpandoObject && (typeof(T).GetProperty(p)?.PropertyType.IsComplex() ?? false) && !(typeof(T).GetProperty(p).PropertyType == typeof(DateTime)) && !(typeof(T).GetProperty(p).PropertyType == typeof(DateTime?)))).ToArray();

                filterProperties.Each((propertyName, column) =>
                {
                    worksheet.Cells[1, column + 1].Value = propertyName;
                    itemsArray.Each((item, row) =>
                    {
                        var value = isExpandoObject ? ((ExpandoObject)item).GetValue(propertyName) : item.GetPropertyValue<object>(propertyName);

                        if (item is DateTime?)
                            value = value != null ? ((DateTime?)value).Value.ToString("dd/MM/yyyy hh:mm:ss") : "";
                        if (value is DateTime datetime)
                            value = datetime.ToString("dd/MM/yyyy hh:mm:ss");

                        worksheet.Cells[row + 2, column + 1].Value = value;
                    });
                });

                return new Result<byte[]>(package.GetAsByteArray());
            }));

        /// <summary>
        /// Compare a new version of an Excel file to an older version.
        /// </summary>
        /// <param name="newVersion">The new version of the Excel file.</param>
        /// <param name="oldVersion">The old version of the Excel file.</param>
        /// <returns>Excel file where the cells which have been changed are colored.</returns>
        public virtual async Task<Result<byte[]>> CompareTwoExcelFilesVersions(byte[] newVersion, byte[] oldVersion)
            => await Task.Run(() =>
            {
                try
                {
                    using var newVersionMemStream = new MemoryStream(newVersion);
                    using var oldVersionMemStream = new MemoryStream(oldVersion);
                    var newPackage = new ExcelPackage(newVersionMemStream);
                    var oldPackage = new ExcelPackage(oldVersionMemStream);

                    var wkNew = newPackage.Workbook.Worksheets.First();
                    var wkOld = oldPackage.Workbook.Worksheets.First();

                    for (var rowIndex = 5; rowIndex <= wkNew.Dimension.Rows; rowIndex++)
                    {
                        for (var colIndex = 2; colIndex <= wkNew.Dimension.Columns; colIndex++)
                        {
                            if (wkNew.Cells[rowIndex, colIndex].Value == null)
                            {
                                if (wkOld.Cells[rowIndex, colIndex].Value != null)
                                    wkNew.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(191, 255, 63));
                            }
                            else if (wkOld.Cells[rowIndex, colIndex].Value == null || !wkNew.Cells[rowIndex, colIndex].Value.Equals(wkOld.Cells[rowIndex, colIndex].Value))
                            {
                                wkNew.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(191, 255, 63));
                            }
                        }
                    }

                    wkNew.View.ShowGridLines = false;
                    return new Result<byte[]>(newPackage.GetAsByteArray());
                }
                catch (Exception exception)
                {
                    return new Result<byte[]>(exception);
                }
            });
    }
}
