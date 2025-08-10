using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ReSharper disable UnusedMember.Global

namespace Ark.Data.EFCore.SqlServer
{
    /// <inheritdoc />
    /// <summary>
    /// This base entity class allows to manage SQL Server file tables.
    /// </summary>
    public abstract class FileTableDbEntity<TContext> : DbEntity<TContext>
        where TContext : DbContextEx, new()
    {
        #region Properties (Public Columns)

        /// <summary>
        /// The unique identifier of the file stream.
        /// It is a auto generated GUID.
        /// </summary>
        [Key]
        [Column("stream_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid StreamId { get; set; }

        /// <summary>
        /// The file stream content in bytes.
        /// </summary>
        [Column("file_stream")]
        [Required]
        public byte[] FileStream { get; set; }

        /// <summary>
        /// The file name.
        /// </summary>
        [Column("name")]
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// The file type.
        /// </summary>
        [Column("file_type")]
        [MaxLength(255)]

        public string FileType { get; set; }

        /// <summary>
        /// The cached file size.
        /// </summary>
        [Column("cached_file_size")]

        public long? CachedFileSize { get; set; }

        /// <summary>
        /// The creation time.
        /// </summary>
        [Column("creation_time")]
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// The last write time.
        /// </summary>
        [Column("last_write_time")]
        public DateTimeOffset LastWriteTime { get; set; }

        /// <summary>
        /// The last access time.
        /// </summary>
        [Column("last_access_time")]
        public DateTimeOffset? LastAccessTime { get; set; }

        /// <summary>
        /// Whether the file is a directory.
        /// </summary>
        [Column("is_directory")]
        public bool IsDirectory { get; set; }

        /// <summary>
        /// Whether the file is offline.
        /// </summary>
        [Column("is_offline")]
        public bool IsOffline { get; set; }

        /// <summary>
        /// Whether the file is hidden.
        /// </summary>
        [Column("is_hidden")]
        public bool IsHidden { get; set; }

        /// <summary>
        /// Whether the file is read-only.
        /// </summary>
        [Column("is_readonly")]
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Whether the file is an archive.
        /// </summary>
        [Column("is_archive")]
        public bool IsArchive { get; set; }

        /// <summary>
        /// Whether the file is a system file.
        /// </summary>
        [Column("is_system")]
        public bool IsSystem { get; set; }

        /// <summary>
        /// Whether the file is a temporary file.
        /// </summary>
        [Column("is_temporary")]
        public bool IsTemporary { get; set; }

        #endregion Properties (Public Columns)
    }
}