using System;
using System.IO;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Ark.Data
{
    /// <summary>
    /// Base repository for manipulating PDF files.
    /// + Utilizes iTextSharp <see cref="Document"/> and <see cref="PdfCopy"/> types for composition.
    /// - Processes whole documents in memory which may impact large files.
    /// Ref: <see href="https://itextpdf.com/en/resources/api-documentation"/>
    /// </summary>
    public class PdfFileRepository
    {
        #region Methods (Public)

        /// <summary>
        /// Merges one or more PDF files into a single document.
        /// + Maintains original page order using <see cref="PdfCopy"/>.
        /// - Password‑protected PDFs are ignored.
        /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.memorystream"/>
        /// </summary>
        /// <param name="pdfFileContentBytes">PDF file contents to merge.</param>
        /// <returns>
        /// Success : The PDF files have been merged successfully.
        /// BadParameters : No PDF file content has been provided.
        /// Unexpected : An unexpected error occurs.
        /// Example JSON: { "isSuccess": true, "value": "JVBERi0xLjMKJ..." }
        /// </returns>
        public virtual Task<Result<byte[]>> Merge(params byte[][] pdfFileContentBytes) => Task.Run(() =>
        {
            try
            {
                if (pdfFileContentBytes == null || pdfFileContentBytes.Length == 0)
                    return Result<byte[]>.BadParameters.WithReason("At least one file should be given to merge PDF files.");
                if (pdfFileContentBytes.Length == 1)
                    return new Result<byte[]>(pdfFileContentBytes[0]).WithReason("Only one PDF file to merge so return this file.");

                var document = new Document();
                using (var stream = new MemoryStream())
                {
                    var writer = new PdfCopy(document, stream);
                    document.Open();

                    pdfFileContentBytes.ForEach(pdfFileContent =>
                    {
                        var reader = new PdfReader(pdfFileContent);
                        for (var pageCounter = 1; pageCounter <= reader.NumberOfPages; pageCounter++)
                        {
                            var page = writer.GetImportedPage(reader, pageCounter);
                            writer.AddPage(page);
                        }
                        if (reader.AcroForm != null)
                            writer.CopyAcroForm(reader);

                        reader.Close();
                    });
                    writer.Close();
                    document.Close();

                    return new Result<byte[]>(stream.ToArray());
                }
            }
            catch (Exception exception)
            {
                return new Result<byte[]>(exception);
            }
        });

        #endregion Methods (Public)
    }
}
