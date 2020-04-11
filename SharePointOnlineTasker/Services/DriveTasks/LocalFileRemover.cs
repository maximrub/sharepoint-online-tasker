using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SharePointOnlineTasker.Entities;
using SharePointOnlineTasker.Interfaces.DriveTasks;

namespace SharePointOnlineTasker.Services.DriveTasks
{
    public class LocalFileRemover : IGoogleDriveFileTask
    {
        private readonly IConfiguration _configuration;

        public LocalFileRemover(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Name => "LocalFileRemover";

        public async Task ExecuteAsync(GoogleDriveFile googleDriveFile)
        {
            string localFullName = Path.Combine(_configuration["LocalRoot"], googleDriveFile.FullName);
            FileInfo file = new FileInfo(localFullName);
            if (file.Exists && file.Length == googleDriveFile.SizeInBytes)
            {
                byte[] md5Hash;
                await using (FileStream inputStream = new FileStream(file.FullName, FileMode.Open))
                {
                    using MD5 algorithm = MD5.Create();
                    md5Hash = CalculateHash(inputStream, algorithm);
                }

                string md5HashHex = string.Join(string.Empty, md5Hash.Select(b => b.ToString("x2")));
                if (md5HashHex.Equals(googleDriveFile.Md5Checksum, StringComparison.InvariantCultureIgnoreCase))
                {
                    file.Delete();
                }
            }
        }

        static byte[] CalculateHash(Stream input, HashAlgorithm algorithm)
        {
            int bufferSize = 4 * 1024 * 1024;
            byte[] buffer = new byte[bufferSize];
            int readCount;

            while ((readCount = input.Read(buffer, 0, bufferSize)) > 0)
            {
                algorithm.TransformBlock(buffer, 0, readCount, buffer, 0);
            }

            algorithm.TransformFinalBlock(buffer, 0, readCount);
            return algorithm.Hash;
        }
    }
}
