namespace RhythmCodex.Infrastructure;

internal static class Json
{
    public static string Serialize<T>(T obj) => 
        System.Text.Json.JsonSerializer.Serialize(obj);
        
    public static T Deserialize<T>(string s) => 
        System.Text.Json.JsonSerializer.Deserialize<T>(s);
}