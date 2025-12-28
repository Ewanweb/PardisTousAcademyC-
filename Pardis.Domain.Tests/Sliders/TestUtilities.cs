using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Pardis.Domain.Tests.Sliders
{
    // Shared test implementation of IFormFile
    public class SharedTestFormFile : IFormFile
    {
        private readonly Stream _stream;
        
        public SharedTestFormFile(Stream stream, string fileName, string contentType)
        {
            _stream = stream;
            FileName = fileName;
            Name = "ImageFile";
            ContentType = contentType;
            Length = stream.Length;
            Headers = new SharedTestHeaderDictionary();
        }

        public string ContentType { get; set; }
        public string ContentDisposition { get; set; } = "";
        public IHeaderDictionary Headers { get; set; }
        public long Length { get; }
        public string Name { get; set; }
        public string FileName { get; set; }

        public Stream OpenReadStream() => _stream;

        public void CopyTo(Stream target)
        {
            _stream.Position = 0;
            _stream.CopyTo(target);
        }

        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            _stream.Position = 0;
            await _stream.CopyToAsync(target, cancellationToken);
        }
    }

    // Shared test implementation of IHeaderDictionary
    public class SharedTestHeaderDictionary : Dictionary<string, StringValues>, IHeaderDictionary
    {
        public long? ContentLength { get; set; }
    }

    // Utility methods for creating test files
    public static class TestFileUtilities
    {
        public static IFormFile CreateTestFile(string fileName, int sizeBytes, string contentType)
        {
            var content = new byte[sizeBytes];
            // Fill with some dummy data
            for (int i = 0; i < sizeBytes; i++)
            {
                content[i] = (byte)(i % 256);
            }
            
            var stream = new MemoryStream(content);
            return new SharedTestFormFile(stream, fileName, contentType);
        }
    }
}