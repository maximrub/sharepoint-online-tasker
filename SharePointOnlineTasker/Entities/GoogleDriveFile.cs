using System;
using System.Text.Json;

namespace SharePointOnlineTasker.Entities
{
    public class GoogleDriveFile : IEquatable<GoogleDriveFile>
    {
        public GoogleDriveFile(string id, string name, string fullName, string md5Checksum, long sizeInBytes)
        {
            Id = id;
            Name = name;
            FullName = fullName;
            Md5Checksum = md5Checksum;
            SizeInBytes = sizeInBytes;
        }

        public string Id { get; }
        public string Name { get; }
        public string FullName { get; }
        public string Md5Checksum { get; }
        public long SizeInBytes { get; }

        public bool Equals(GoogleDriveFile other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Id.Equals(other.Id, StringComparison.InvariantCultureIgnoreCase) && 
                   Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase) &&
                   FullName.Equals(other.FullName, StringComparison.InvariantCultureIgnoreCase) &&
                   Md5Checksum.Equals(other.Md5Checksum, StringComparison.InvariantCultureIgnoreCase) &&
                   SizeInBytes == other.SizeInBytes;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GoogleDriveFile);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, FullName, Md5Checksum, SizeInBytes);
        }

        public static bool operator ==(GoogleDriveFile left, GoogleDriveFile right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GoogleDriveFile left, GoogleDriveFile right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(this, options);
            return json;
        }
    }
}