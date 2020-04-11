using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SharePointOnlineTasker.Entities;
using SharePointOnlineTasker.Interfaces.DriveTasks;

namespace SharePointOnlineTasker.Services.DriveTasks
{
    public class LocalFileRemover : IDriveFileTask
    {
        private readonly IConfiguration _configuration;

        public LocalFileRemover(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Name => "LocalFileRemover";

        public async Task ExecuteAsync(DriveFile driveFile)
        {
            string localFullName = Path.Combine(_configuration["LocalRoot"], driveFile.FullName);
            FileInfo file = new FileInfo(localFullName);
            if (file.Exists && file.Length == driveFile.SizeInBytes)
            {
                byte[] quickXorHashBytes;
                await using (FileStream inputStream = new FileStream(file.FullName, FileMode.Open))
                {
                    using HashAlgorithm algorithm = new QuickXorHash();
                    quickXorHashBytes = CalculateHash(inputStream, algorithm);
                }

                string quickXorHash = Convert.ToBase64String(quickXorHashBytes);
                if (quickXorHash.Equals(driveFile.QuickXorHash, StringComparison.InvariantCultureIgnoreCase))
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
