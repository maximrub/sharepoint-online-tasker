using System;
using System.Text.Json;

namespace SharePointOnlineTasker.Entities
{
    public class DriveFile : IEquatable<DriveFile>
    {
        public DriveFile(string id, string name, string fullName, string quickXorHash, long sizeInBytes)
        {
            Id = id;
            Name = name;
            FullName = fullName;
            QuickXorHash = quickXorHash;
            SizeInBytes = sizeInBytes;
        }

        public string Id { get; }
        public string Name { get; }
        public string FullName { get; }
        public string QuickXorHash { get; }
        public long SizeInBytes { get; }

        public bool Equals(DriveFile other)
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
                   QuickXorHash.Equals(other.QuickXorHash, StringComparison.InvariantCultureIgnoreCase) &&
                   SizeInBytes == other.SizeInBytes;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DriveFile);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, FullName, QuickXorHash, SizeInBytes);
        }

        public static bool operator ==(DriveFile left, DriveFile right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DriveFile left, DriveFile right)
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