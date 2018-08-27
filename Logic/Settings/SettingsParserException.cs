using System;

namespace Logic.Settings
{
    public class SettingsParserException : Exception
    {
        public SettingsParserException(Exception exception) 
            : base("Произошла ошибка при парсинге файла настроек", exception) { }
    }
}