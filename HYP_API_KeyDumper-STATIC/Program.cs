using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace HYP_API_KeyDumper_STATIC;
internal class Program
{
    public static void Main(string[] args)
    {
        string destFileName = "app.conf.dat";
        string appDir = "C:\\Program Files\\HoYoPlay";
        if (!Directory.Exists(appDir) || args.Length > 0)
        {
            if (args.Length > 0)
            {
                Console.WriteLine("Could not find HoYoPlay folder.");
                Console.Write("Please enter a valid HYP launcher directory path: ");
                appDir = Console.ReadLine()!;
            } else
            {
                appDir = args[0];
            }

            if (string.IsNullOrWhiteSpace(appDir) || !Directory.Exists(appDir))
            {
                Console.WriteLine("Invalid directory path. Exiting...");
                return;
            }
        }
        var path = Path.Combine(FindDirectoryWithPattern(appDir)!, destFileName);

        using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
        {
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            string decrypted = EncryptionHelper.AesDecrypt(buffer).Substring(0x109);
            JsonObject data = JsonNode.Parse(decrypted)!.AsObject();
            string launcherId = data["App"]!["LauncherId"]!.ToString();
            string Region = data["App"]!["Region"]!.ToString();
            string _meta_ver = data["_meta_ver"]!.ToString();
            Console.WriteLine($"HoYoPlay {Region} v{_meta_ver} launcher ID: {launcherId}");
        }
    }
    private static string? FindDirectoryWithPattern(string baseDir)
    {
        // pattern to match directory names like '1.1.4.133', since that's the HYP version and can change
        string pattern = @"^\d+(\.\d+)+$";
        Regex regex = new Regex(pattern);
        try
        {
            string[] directories = Directory.GetDirectories(baseDir);

            foreach (string dir in directories)
            {
                string dirName = Path.GetFileName(dir);
                if (regex.IsMatch(dirName))
                {
                    return dir;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while searching for directories: {ex.Message}");
        }
        return null;
    }

}
