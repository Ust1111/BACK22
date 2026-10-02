using System.ComponentModel;
using System.Text.Json.Serialization;
namespace ArticulationAPI.Models;

public class TranscriptionRequest
{
    public string Text { get; set; } = string.Empty;

    [DefaultValue("ru")]
    [Description("Язык текста: ru, en, de, fr, es, it")]
    public string Language { get; set; } = "ru";
    
    [DefaultValue("ipa")]
    [Description("Система транскрипции: ipa, x-sampa, cyrillic")]
    public string System { get; set; } = "ipa";
}