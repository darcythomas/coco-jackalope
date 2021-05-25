using System.IO;
using Pulumi;
using Pulumi.Aws.S3;
using Pulumi.Aws.S3.Inputs;
using Coco;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Aws = Pulumi.Aws;

class MyStack : Stack
{
    public MyStack()
    {
        var bucket = new Bucket("lovelacegin.com", new BucketArgs
        {
            BucketName = "lovelacegin.com",
            Website = new BucketWebsiteArgs
            {
                IndexDocument = "index.html",
                ErrorDocument = "index.html"

            }
        });

        Regex rx = new Regex(@"^(.*)(wwwroot/)(?<filePath>.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        var files = Directory.GetFiles("../frontend/wwwroot", "*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var name = Path.GetFileName(file);
            
            var trimedName = Regex.Replace(name, @"\.br|\.gz$", string.Empty);
            var contentType = MimeTypes.GetMimeType(trimedName);

            var key = rx.Matches(file).Select(m => m.Groups["filePath"].Value).First();

            var bucketArgs = new BucketObjectArgs
            {
                Acl = "public-read",
                Bucket = bucket.BucketName,
                ContentType = contentType,
                Key = key,
                Source = new FileAsset(file)
                {

                }
            };
            
            if (key.EndsWith(".br"))
            {
                bucketArgs.ContentEncoding = "br";
            }
            if (key.EndsWith(".gz"))
            {
                bucketArgs.ContentEncoding = "gzip";
            }

            // ... create a bucket object
            var bucketObject = new BucketObject(key, bucketArgs, new CustomResourceOptions { Parent = bucket });
        }



        // Export the name of the bucket
        this.BucketName = bucket.Id;
        this.BucketEndpoint = Output.Format($"http://{bucket.WebsiteEndpoint}");







    }

    [Output]
    public Output<string> BucketName { get; set; }

    [Output]
    public Output<string> BucketEndpoint { get; set; }
}
