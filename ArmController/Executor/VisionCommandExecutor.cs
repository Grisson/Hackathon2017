using ArmController.Models.Command;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArmController.Executor
{
    public class VisionCommandExecutor : IExecutor
    {
        public static readonly VisionCommandExecutor SharedInstance = new VisionCommandExecutor();

        public Action<string> LogHandler => CommandExecutor.SharedInstance.LogHandler;
        public Action<string> TakePhoto => CommandExecutor.SharedInstance.TakePhoto;

        private VisionCommandExecutor()
        {

        }

        public void Execute(BaseCommand command)
        {
            this.Execute(command as VisionCommand);
        }

        public void Execute(VisionCommand command)
        {
            var fileName = $"{Guid.NewGuid().ToString()}.jpg";
            TakePhoto(fileName);

            Thread.Sleep(1000);

            if (File.Exists(fileName))
            {
                SendImageToServer(fileName, command.Data);
            }

            lock (CommandExecutor.SharedInstance)
            {
                CommandStore.SharedInstance.CurrentCommand = null;
                CommandExecutor.SharedInstance.IsWaitingResponse = false;
            }
        }

        byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        async void SendImageToServer(string imageFilePath, string data)
        {
            var client = new HttpClient();
            var registerId = CommandExecutor.SharedInstance.RegisterId;
            string uri = $"{ConfigurationManager.AppSettings["BaseUrl"]}/api/vision/{registerId}/{data}";

            HttpResponseMessage response;

            // Request body. Try this sample with a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
            }

            File.Delete(imageFilePath);
        }
    }
}
