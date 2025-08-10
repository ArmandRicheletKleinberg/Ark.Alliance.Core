using System.Threading.Tasks;
using Ark.Net.CrossCutting.Models;

namespace Ark.Net.CrossCutting
{
    /// <summary>
    /// This class is used to access the cross cutting archive services.
    /// Mainly used to store a new archive
    /// </summary>
    public class CrossCuttingArchiveService
    {
        #region Fields

        /// <summary>
        /// The cross cutting HTTP repository is needed.
        /// </summary>
        internal CrossCuttingHttpRepository CrossCuttingHttpRepository = new CrossCuttingHttpRepository();

        #endregion Fields

        #region Properties (Public)

        /// <summary>
        /// Stores a document archive in the database along with its metadata.
        /// </summary>
        /// <param name="archive">The document archive to upload.</param>
        /// <returns>
        /// Success : The document has been saved successfully in database.
        /// BadParameters : The file has not been provided.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        public virtual Task<Result> StoreDocument(ArchiveToCreateDto archive)
            => CrossCuttingHttpRepository.PostStoreDocument(archive);

        #endregion Properties (Public)
    }
}