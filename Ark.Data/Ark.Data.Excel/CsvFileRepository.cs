using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.Data.Excel
{
    /// <summary>
    /// This is the base class for all the manipulation on a CSV
    /// </summary>
    public class CsvFileRepository
    {
        /// <summary>
        /// Take a list of an object T and return a CSV file with the non complex properties of the object. 
        /// </summary>
        /// <typeparam name="T">Type of the entity of the list</typeparam>
        /// <param name="items">List of data items to be inserted into an CSV array.</param>
        /// <param name="delimiter">The delimiter for each column.</param>
        /// <returns>
        /// Success : The Csv file has been successfully created.
        /// BadParameters : The items provided are null.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual async Task<Result<byte[]>> GetCsvBytesFromIEnumerable<T>(IEnumerable<T> items, char delimiter)
            => await Task.Run(() => Result<byte[]>.SafeExecute(() =>
            {
                if (items == null)
                    return Result<byte[]>.BadParameters.WithReason("The items should be set and not null.");

                var itemsArray = items as T[] ?? items.ToArray();
                if (itemsArray.HasNoElements())
                    return new Result<byte[]>(new byte[0]);

                var propertiesNames = typeof(T).GetProperties().Where(p => !p.PropertyType.IsComplex()).Select(p => p.Name).ToArray();

                var csvContent = string.Join(delimiter, propertiesNames) + Environment.NewLine;

                foreach (var item in itemsArray)
                {
                    foreach (var propertyName in propertiesNames)
                    {
                        var value = item.GetPropertyValue<object>(propertyName);
                        if (value is DateTime datetime)
                            value = datetime.ToString("dd/MM/yyyy hh:mm:ss");
                        csvContent += value.ToString() + delimiter;
                    }

                    csvContent += Environment.NewLine;
                }

                return new Result<byte[]>(Encoding.UTF8.GetBytes(csvContent));
            }));
    }
}
