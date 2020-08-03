using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Services
{
    public class FileService
    {
        IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task SaveAttachedFile(Stream stream, string directoryName,string fileName)
        {
            var fullpath = $"{_environment.WebRootPath}/Files/AttachedFiles/{directoryName}";

            await Task.Run(() => Directory.CreateDirectory(fullpath));

            using (FileStream fs = new FileStream($"{fullpath}/{fileName}",FileMode.Create))
            {
                await stream.CopyToAsync(fs);
            }
        }

        public async Task SaveAvatar(Stream stream, string fileName)
        {
            using (FileStream fs = new FileStream($"{_environment.WebRootPath}/Files/Avatars/{fileName}", FileMode.Create))
            {
                await stream.CopyToAsync(fs);
            }
        }

        public async Task<(Stream,string)> GetAttachedFile(string directoryName, string fileName)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string contentType);

            return (File.OpenRead($"{_environment.WebRootPath}/Files/AttachedFiles/{directoryName}/{fileName}"), contentType);
        }

        public async Task GetAvatar()
        {

        }

        public async Task DeleteAttachedFile(string directoryName)
        {
            var filesDirectory = $"{_environment.WebRootPath}/Files/AttachedFiles/{directoryName}";
            if (await Task.Run(() => Directory.Exists(filesDirectory)))
                await Task.Run(() => Directory.Delete(filesDirectory, true));
        }

        public async Task SaveGroupAvatar(Stream stream, string fileName)
        {
            using (FileStream fs = new FileStream($"{_environment.WebRootPath}/Files/GroupAvatars/{fileName}", FileMode.Create))
            {
                await stream.CopyToAsync(fs);
            }
        }

        public async Task<(Stream, string)> GetGroupAvatar(string fileName)
        {
            var avatartPatch = Directory.GetFiles(_environment.WebRootPath + "/Files/GroupAvatars/", $"{fileName}.*").FirstOrDefault();

            if (avatartPatch == string.Empty)
                return (null, null);

            new FileExtensionContentTypeProvider().TryGetContentType(avatartPatch, out string contentType);

            return (File.OpenRead(avatartPatch), contentType);
        }

        public async Task DeleteGroupAvatar(string fileName)
        {
            var avatartPatch = Directory.GetFiles(_environment.WebRootPath + "/Files/GroupAvatars/", $"{fileName}.*").FirstOrDefault();

            if (avatartPatch != null)
                await Task.Run(() => File.Delete(avatartPatch));
        }
    }
}
