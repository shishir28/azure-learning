using Microsoft.Azure.Batch;

namespace AnimationCreator.Utils.Bacth
{
    public struct SkuAndImage
    {

        public NodeAgentSku SKU { get; set; }
        public ImageReference Image { get; set; }

        public SkuAndImage(NodeAgentSku sku, ImageReference image)
        {
            this.SKU = sku;
            this.Image = image;
        }
    }
}