using Pulumi;
using Pulumi.Aws.S3;
using Pulumi.Aws.S3.Inputs;

class MyStack : Stack
{
    public MyStack()
    {
        var bucket = new Bucket("lovelacegin.com", new BucketArgs
        {
            BucketName = "lovelacegin.com",
            Website = new BucketWebsiteArgs
            {
                IndexDocument = "index.html"
            }
        });

        var bucketObjectIndex = new BucketObject("index.html", new BucketObjectArgs
        {
            Acl = "public-read",
            ContentType = "text/html",
            Bucket = bucket.BucketName,
            Source = new FileAsset("index.html")
        });

        var bucketObjectFavicon = new BucketObject("favicon.ico", new BucketObjectArgs
        {
            Acl = "public-read",
            ContentType = "image/ico",
            Bucket = bucket.BucketName,
            Source = new FileAsset("favicon.ico")
        });


        // Export the name of the bucket
        this.BucketName = bucket.Id;
        this.BucketEndpoint = Output.Format($"http://{bucket.WebsiteEndpoint}");

    }

    [Output]
    public Output<string> BucketName { get; set; }

    [Output]
    public Output<string> BucketEndpoint { get; set; }
}
