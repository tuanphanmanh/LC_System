using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Abp;
using Abp.Dependency;
using Abp.UI;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Hosting;
using MyCompanyName.AbpZeroTemplate.Configuration;

namespace MyCompanyName.AbpZeroTemplate.Web.Configuration
{
    public class AppConfigurationWriter : IAppConfigurationWriter, ISingletonDependency
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ILogger Logger { get; set; }

        public AppConfigurationWriter(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            Logger = NullLogger.Instance;
        }

        public void Write(string key, string value)
        {
            if (!File.Exists("appsettings.json"))
            {
                throw new UserFriendlyException("appsettings.json file does not exist");
            }
            Writenternal("appsettings.json", key, value);

            if (File.Exists($"appsettings.{_webHostEnvironment.EnvironmentName}.json"))
            {
                Writenternal($"appsettings.{_webHostEnvironment.EnvironmentName}.json", key, value);
            }
        }

        protected virtual void Writenternal(string filename, string key, string value)
        {
            Check.NotNullOrWhiteSpace(key, nameof(key));
            Check.NotNull(value, nameof(value));

            var jsonFile = JsonNode.Parse(File.ReadAllText(filename));

            var objectNames = key.Split(":").ToList();
            if (objectNames.Count == 1)
            {
                objectNames.Clear();
            }
            else
            {
                key = objectNames.Last();
                objectNames.RemoveAt(objectNames.Count - 1);
            }

            var jobj = jsonFile;

            foreach (var objectName in objectNames)
            {
                jobj = (JsonObject) jobj[objectName];
                if (jobj == null)
                {
                    Logger.Error($"Key {key} does not exist!");
                    return;
                }
            }


            var jProperty = jobj[key];
            if (jProperty == null)
            {
                Logger.Error($"Key {key} does not exist!");
                return;
            }


            jobj[key] = value;

            using (var file = File.Create(filename))
            {
                using (var writer = new Utf8JsonWriter(file))
                {
                    JsonSerializer.Serialize(writer, jsonFile, new JsonSerializerOptions()
                    {
                        WriteIndented = true
                    });
                }
            }
        }
    }
}
