using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RMotownFestival.Api.Common;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RMotownFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        private readonly BlobUtility blobUtility;

        public PicturesController(BlobUtility blobUtility)
        {
            this.blobUtility = blobUtility ?? throw new ArgumentNullException(nameof(blobUtility));
        }
        [HttpGet]
        public string[] GetAllPictureUrls()
        {
            var client = blobUtility.GetThumbsContainer();
            return client.GetBlobs()
                .Select(blob => blobUtility.GetSasUri(client, blob.Name))
                .ToArray(); 
        }

        [HttpPost]
        public async Task PostPicture(IFormFile file)
        {
            BlobContainerClient client = blobUtility.GetPicturesContainer();
            await client.UploadBlobAsync(file.FileName, file.OpenReadStream());
        }
    }
}
