using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Ark.Infrastructure.Info
{
    /// <summary>
    /// Collects drive capacity and sequential I/O performance metrics.
    /// + Measures read/write speed using temporary files.
    /// - Requires write access to each drive and may trigger disk activity.
    /// Ref: <see href="https://learn.microsoft.com/dotnet/api/system.io.driveinfo"/>
    /// </summary>
    internal static class StorageInfoUtils
    {
        /// <summary>
        /// Enumerates storage devices with capacity and simple performance stats.
        /// + Provides read/write throughput in MB/s.
        /// - Results may fluctuate due to filesystem caching.
        /// Example JSON:
        /// <code language="json">
        /// [{ "name": "root", "totalSize": 1000000 }]
        /// </code>
        /// </summary>
        /// <returns>A <see cref="Result{T}"/> containing <see cref="StorageInfoDto"/> items.</returns>
        public static Result<List<StorageInfoDto>> GetStorageInfos()
        {
            var infos = new List<StorageInfoDto>();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                try
                {
                    if (!drive.IsReady)
                        continue;

                    var readWrite = MeasurePerformance(drive.RootDirectory.FullName);
                    infos.Add(new StorageInfoDto
                    {
                        Name = drive.VolumeLabel,
                        LogicalPath = drive.Name,
                        PhysicalPath = drive.RootDirectory.FullName,
                        Alias = drive.Name,
                        Permissions = drive.RootDirectory.Attributes.ToString(),
                        AvailableFreeSpace = drive.AvailableFreeSpace,
                        TotalSize = drive.TotalSize,
                        DriveType = drive.DriveType.ToString(),
                        ReadSpeed = readWrite.read,
                        WriteSpeed = readWrite.write
                    });
                }
                catch
                {
                    // Ignore drives that cannot be read
                }
            }

            return new Result<List<StorageInfoDto>>(infos);
        }

        /// <summary>
        /// Measures sequential read and write speed for a given path.
        /// + Uses a 1&#160;MB buffer to estimate throughput.
        /// - Deletes a temporary file; repeated tests may affect SSD lifespan.
        /// </summary>
        /// <param name="path">Directory on which to benchmark I/O.</param>
        /// <returns>Tuple containing read and write speeds in MB/s.</returns>
        private static (double read, double write) MeasurePerformance(string path)
        {
            string testFile = Path.Combine(path, $"perf_{System.Guid.NewGuid():N}.tmp");
            byte[] data = new byte[1024 * 1024]; // 1 MB
            new Random().NextBytes(data);
            double writeSpeed = 0;
            double readSpeed = 0;
            try
            {
                var sw = Stopwatch.StartNew();
                File.WriteAllBytes(testFile, data);
                sw.Stop();
                writeSpeed = 1 / sw.Elapsed.TotalSeconds;

                sw.Restart();
                _ = File.ReadAllBytes(testFile);
                sw.Stop();
                readSpeed = 1 / sw.Elapsed.TotalSeconds;
            }
            catch
            {
                // ignore errors
            }
            finally
            {
                try { if (File.Exists(testFile)) File.Delete(testFile); } catch { }
            }

            // Convert to MB/s
            return (readSpeed, writeSpeed);
        }
    }
}
