namespace VideoProcessor;

public class LocalStorageOptions
{
    public string RootPath { get; set; } = Path.Combine(AppContext.BaseDirectory, "storage");
    public string BaseUrl { get; set; } = "http://localhost:8080";
}