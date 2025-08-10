using System.Text;

namespace Ark
{
    /// <summary>
    /// Provides a <see cref="StringWriter"/> that always emits UTF-8 encoded text.
    /// <para>+ Simplifies serialization scenarios requiring UTF-8 output.</para>
    /// <para>- Encoding is fixed and cannot be changed.</para>
    /// <para>Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.stringwriter"/>.</para>
    /// </summary>
    public class Utf8StringWriter : StringWriter
    {
        #region Properties (Public)

        /// <summary>
        /// Gets the writer encoding which is forced to UTF-8.
        /// <para>+ Ensures consistent UTF-8 output across platforms.</para>
        /// <para>- Prevents consumers from selecting alternative encodings.</para>
        /// </summary>
        public override Encoding Encoding => Encoding.UTF8;

        #endregion Properties (Public)
    }
}
