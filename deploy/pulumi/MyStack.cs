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

        //SetBucketPolicy(bucket);

        // var bucketObjectIndex = new BucketObject("index.html", new BucketObjectArgs
        // {
        //     Acl = "public-read",
        //     ContentType = "text/html",
        //     Bucket = bucket.BucketName,
        //     Source = new FileAsset("index.html")
        // });

        // var bucketObjectFavicon = new BucketObject("favicon.ico", new BucketObjectArgs
        // {
        //     Acl = "public-read",
        //     ContentType = "image/ico",
        //     Bucket = bucket.BucketName,
        //     Source = new FileAsset("favicon.ico")
        // });

        Regex rx = new Regex(@"^(.*)(wwwroot)(?<filePath>.*)",
                  RegexOptions.Compiled | RegexOptions.IgnoreCase);


        var files = Directory.GetFiles("../frontend/wwwroot", "*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var name = Path.GetFileName(file);
            var contentType = MimeTypes.GetMimeType(name);

            var key = rx.Matches(file).Select(m => m.Groups["filePath"].Value.TrimStart('/')).First();



            // ... create a bucket object
            var bucketObject = new BucketObject(key, new BucketObjectArgs
            {
                Acl = "public-read",
                Bucket = bucket.BucketName,
                ContentType = contentType,
                Key = key,
                Source = new FileAsset(file)
                {

                }
            }, new CustomResourceOptions { Parent = bucket });
        }



        // Export the name of the bucket
        this.BucketName = bucket.Id;
        this.BucketEndpoint = Output.Format($"http://{bucket.WebsiteEndpoint}");

    }



    private void SetBucketPolicy(Bucket bucket)
    {

        var bucketPolicy = new Aws.S3.BucketPolicy("bucketPolicy", new Aws.S3.BucketPolicyArgs
        {
            Bucket = bucket.Id,
            Policy = Output.Tuple(bucket.Arn, bucket.Arn).Apply(values =>
            {
                var bucketArn = values.Item1;
                var bucketArn1 = values.Item2;
                return JsonSerializer.Serialize(new Dictionary<string, object?>
                {
                    { "Version", "2012-10-17" },
                    { "Id", "CocojackalopeAllowGetOnSite" },
                    { "Statement", new[]
                        {
                            new Dictionary<string, object?>
                            {
                                { "Effect", "Allow" },
                                { "Principal", "*" },
                                { "Action", "s3:GetObject" },
                                { "Resource", new[]
                                    {
                                        bucketArn,
                                        $"{bucketArn1}/*",
                                    }
                                 },

                            },
                        }
                     },
                });
            }),
        });





    }

    [Output]
    public Output<string> BucketName { get; set; }

    [Output]
    public Output<string> BucketEndpoint { get; set; }
}
