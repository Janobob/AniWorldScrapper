namespace AniWorldScrapper.Renamer;

public class Renamer
{
	public static void Main(string[] args)
	{
		// Directory containing files
		const string directoryPath = @"\\192.168.1.176\Plex\downloads\kono2";
		const string animeName = "KonoSuba – God’s blessing on this wonderful world!!";
		const string template = animeName + " S{0:D2}E{1:D2}";
		var season = 2;
		var episode = 1;

		try
		{
			// Get all files in the directory
			var files = Directory.GetFiles(directoryPath);

			foreach (var filePath in files)
			{
				// Get file name and extension
				var fileName = Path.GetFileName(filePath);
				var fileExtension = Path.GetExtension(filePath);

				// Rename the file
				var newFileName = string.Format(template, season, episode) + fileExtension;
				var newFilePath = Path.Combine(directoryPath, newFileName);
				File.Move(filePath, newFilePath);

				// Output the old and new file names
				Console.WriteLine($"Renamed: {fileName} -> {newFileName}");
				episode++;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred: {ex.Message}");
		}
	}
}