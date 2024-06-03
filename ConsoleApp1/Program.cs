using System;
using System.Net.Http;
using HtmlAgilityPack;
using System.Threading.Tasks;

class Program
{
	static async Task Main(string[] args)
	{
		try
		{
			//URL of the website you want to scrape
			var url = "https://fa.wikipedia.org/wiki/%D9%85%D8%A7%DB%8C%DA%A9%D8%B1%D9%88%D8%B3%D8%A7%D9%81%D8%AA";
			//await TestConnection(url);

			//Initialize HttpClient
				using HttpClient client = new HttpClient();

			// Directory to save images
			var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
			var imagesDirectory = Path.Combine(desktopPath, "NewFolder");
			Directory.CreateDirectory(imagesDirectory);

			// Get the HTML content of the webpage
			var response = await client.GetStringAsync(url);

			// Load the HTML content into HtmlDocument
			HtmlDocument document = new HtmlDocument();
			document.LoadHtml(response);

			// Example: Select all headings (h1 elements) from the page
			var img = document.DocumentNode.SelectNodes("//img");

			if (img != null)
			{
				foreach (var image in img)
				{
					var src = image.Attributes["src"].Value;
					if (!string.IsNullOrEmpty(src))
					{
						// Ensure the URL is absolute
						if (!Uri.IsWellFormedUriString(src, UriKind.Absolute))
						{
							var baseUri = new Uri(url);
							var absoluteUri = new Uri(baseUri, src);
							src = absoluteUri.ToString();
						}

						Console.WriteLine($"Found image URL: {src}");
						await DownloadImgAsync(client, src, imagesDirectory);
					}

				}
			}
			else
			{
				Console.WriteLine("No headings found.");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Erroerr meages: {ex.Message}");
		}

	}
	private static async Task TestConnection(string url)
	{
		using (var client = new HttpClient())
		{
			try
			{
			
				HttpResponseMessage httpResponse = await client.GetAsync(url);

				if (httpResponse.IsSuccessStatusCode)
				{
                    Console.WriteLine($"connection success {url} status code: {httpResponse.StatusCode}");
                }
				else
				{
                    Console.WriteLine($"connection errorr: {url} status code: {httpResponse.StatusCode}");
                }


			}
			catch(HttpRequestException htp)
			{
                Console.WriteLine($"errorr request: {htp.Message}");
            }
			catch (Exception ex)
			{
				Console.WriteLine($"errorr request: {ex.Message}");
			}
		}
	}
	private static async Task DownloadImgAsync(HttpClient client,string url,string imagesDirectory)
	{
		try
		{
			var imgBytes= await client.GetByteArrayAsync(url);

			var fileName=Path.GetFileName(new Uri(url).LocalPath);

			fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));

			var filePath = Path.Combine(imagesDirectory, fileName);

			// Write the image data to a file
			await File.WriteAllBytesAsync(filePath, imgBytes);
		}
		catch (Exception ex) 
		{
			Console.WriteLine($"errorr with download: {ex.Message}");
		}

	}
}

