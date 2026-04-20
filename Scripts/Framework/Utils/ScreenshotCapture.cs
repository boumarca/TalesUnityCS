using System.IO;
using UnityEngine;

namespace Framework.Utils
{
    public static class ScreenshotCapture
    {
        #region Public Methods
        public static byte[] CaptureToBytes(string cameraTag, int width, int height)
        {
            Texture2D screenshot = TakeScreenshot(cameraTag, width, height);
            return screenshot.EncodeToPNG();
        }

        public static void CaptureToFile(string path, string cameraTag, int width, int height)
        {
            byte[] bytes = CaptureToBytes(cameraTag, width, height);
            File.WriteAllBytes(path, bytes);
        }
        #endregion

        #region Private Methods
        private static Texture2D TakeScreenshot(string cameraTag, int width, int height)
        {
            Camera captureCam = GameObject.FindGameObjectWithTag(cameraTag).GetComponent<Camera>();
            RenderTexture currentRt = captureCam.targetTexture;
            RenderTexture rt = new(width, height, 32);
            captureCam.targetTexture = rt;
            Texture2D screenshot = new(width, height, TextureFormat.ARGB32, false);
            captureCam.Render();
            RenderTexture.active = rt;
            screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            captureCam.targetTexture = currentRt;
            RenderTexture.active = null;
            Object.Destroy(rt);
            return screenshot;
        }
        #endregion
    }
}
