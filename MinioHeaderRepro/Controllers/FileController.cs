using Microsoft.AspNetCore.Mvc;
using Minio;

namespace MinioHeaderRepro.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly ILogger<FileController> _logger;
        readonly MinioClient _minioClient;

        public FileController(ILogger<FileController> logger, MinioClient minioClient)
        {
            _minioClient = minioClient;
            _logger = logger;
        }

        [HttpGet(Name = "GetFile")]
        public async Task<IActionResult> GetFileAsync(CancellationToken cancellationToken)
        {
            var bucketName = Environment.GetEnvironmentVariable("minio_bucket");
            var minioObjectName = Environment.GetEnvironmentVariable("minio_object");

            var outFile = new MemoryStream();

            var getObjectArgs = new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(minioObjectName)
            .WithCallbackStream((s) =>
                s.CopyTo(outFile)
            );

            await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken: cancellationToken).ConfigureAwait(false);

            outFile.Seek(0, SeekOrigin.Begin);

            return File(outFile, "image/png", null, true);
        }
    }
}