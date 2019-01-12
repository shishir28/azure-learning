using System;
using System.IO;
using AnimationCreator.Utils;
using AnimationCreator.Utils.Animation;
using AnimationCreator.Utils.Storage;

namespace AnimationCreator.Console
{
    class AnimationCreator
    {
        private static int ErrorFrameRate = 5;
        public int Width { get; set; }
        public int Height { get; set; }
        public int Frames { get; set; }
        public bool GenerateErrors { get; set; }

        public AnimationCreator(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void CreateSceneFiles(AppSettings appSettings)
        {
            var pinAnimation = new PinAnimation(this.Width, this.Height);
            pinAnimation.CameraY = 4;
            pinAnimation.CameraAtX = 0;
            pinAnimation.CameraAtY = 2;
            pinAnimation.CameraAtZ = 0;

            var storageConnectionString = appSettings.StorageConnectionString;
            var frameBlobContainerName = appSettings.FrameBlobContainerName;
            var sceneBlobContainerName = appSettings.SceneBlobContainerName;

            var frameBlobHelper = new BlobHelper(storageConnectionString, frameBlobContainerName, 10);
            var sceneBlobHelper = new BlobHelper(storageConnectionString, sceneBlobContainerName, 10);

            for (int frameIndex = 0; frameIndex < Frames; frameIndex++)
            {
                var depthFileNamePart = (frameIndex % 100).ToString().PadLeft(6, '0');
                var fileNamePart = frameIndex.ToString().PadLeft(6, '0');

                var imageFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"Depth\A{depthFileNamePart}.png");
                pinAnimation.SetPinDepth(imageFile);
                pinAnimation.AdvanceFrame();
                var sceneDescription = pinAnimation.ToString();

                string fileName = $"F{fileNamePart}.pi";

                if (GenerateErrors && frameIndex % ErrorFrameRate == 0)
                {
                    sceneBlobHelper.UploadText(fileName, "SCENE FILE ERROR\r\n" + sceneDescription);
                }
                else
                {
                    sceneBlobHelper.UploadText(fileName, sceneDescription);
                }
            }

        }

    }
}
