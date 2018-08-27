using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Logic.Settings
{
    public class SettingsParser
    {
        private readonly string _settingsFileContent;
        private readonly IGlobalInfo _globalInfo;

        private List<WebsiteNodeData> _parsedResult;

        public SettingsParser(string settingsFileContent, IGlobalInfo globalInfo)
        {
            _settingsFileContent = settingsFileContent 
                                   ?? throw new ArgumentNullException(paramName: nameof(settingsFileContent));
            _globalInfo = globalInfo
                          ?? throw new ArgumentNullException(paramName: nameof(globalInfo));
        }

        public List<WebsiteNodeData> GetSettings()
        {
            ParseContent();

            ValidateSettings();

            return _parsedResult;
        }

        private void ValidateSettings()
        {
            if (_parsedResult == null)
                throw new InvalidOperationException(@"Сначала нужно разпарсить файл");

            if (_parsedResult.Count == 0)
                throw new InvalidOperationException(@"Файл настроек нод не содержит полезных данных");

            for (var index = 0; index < _parsedResult.Count; index++)
            {
                WebsiteNodeData websiteNodeData = _parsedResult[index];
                int indexForMessages = index + 1;

                ThrowErrorIfEmptyValue(websiteNodeData.DisplayableName, indexForMessages, "Не указано имя ноды");

                ThrowErrorIfEmptyValue(websiteNodeData.DeployDirectoryPath, indexForMessages, 
                    "Не указана папка деплоя, откуда будут копироваться файлы для сайта");

                ThrowErrorIfEmptyValue(websiteNodeData.WebsiteRootPath, indexForMessages, 
                    $"Не указан путь до корневой папки сайта, где лежат подпапки {_globalInfo.SiteArchiveFolderName()} и {_globalInfo.SiteFolderName()}");

                if (websiteNodeData.WebsiteUrls == null || websiteNodeData.WebsiteUrls.Count == 0)
                    ThrowException(indexForMessages, "Не указаны урлы ноды");
            }

            // Проверим, что ввели уникальные значения. Чтоб два раза какой-нибудь сайт не обновили

            CheckUniqueness(x => x.DisplayableName, $"Есть повторы по полю {nameof(WebsiteNodeData.DisplayableName)}");

            CheckUniqueness(x => x.WebsiteRootPath, $"Есть повторы по полю {nameof(WebsiteNodeData.WebsiteRootPath)}");
        }

        private static void ThrowErrorIfEmptyValue(string valueToCheck, int nodeIndex, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(valueToCheck))
                ThrowException(nodeIndex, errorMessage);
        }

        private static void ThrowException(int nodeIndex, string errorMessage)
            => throw new InvalidOperationException($"Нода #{nodeIndex}. " + errorMessage);

        private void CheckUniqueness<TResult>(Func<WebsiteNodeData, TResult> selector, string errorMessage)
        {
            if(_parsedResult.Select(selector).Distinct().Count() > 1)
                throw new ArgumentException(errorMessage);
        }

        private void ParseContent()
        {
            try
            {
                _parsedResult = JsonConvert.DeserializeObject<List<WebsiteNodeData>>(_settingsFileContent);
            }
            catch (JsonException jsonException)
            {
                throw new SettingsParserException(jsonException);
            }
        }
    }
}