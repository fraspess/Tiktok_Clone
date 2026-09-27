using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Domain;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VideoReportReasons
{
    FallBack = 0,
    [Description("Спам")] Spam = 1,
    [Description("Непристоний контент")] InappropriateContent = 2,
    [Description("Мова ворожнечі")] HateSpeech = 3,
    [Description("Цькування або булінг")] Violence = 4,
    [Description("Дезінформація")] MissInformation = 5,
    [Description("Інше")] Other = 6
}