using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Server.Services
{
    public interface IEntityFilesHandler
    {
        public Task<(Stream, string)> GetFile(string id);
        public Task SaveFile(IFormFile file, string innerDirectory = null, string newFileName = null);
    }
}
