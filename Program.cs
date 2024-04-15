// See https://aka.ms/new-console-template for more information

using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

const string extensionPath = @"C:\Users\janwe\AppData\Local\Google\Chrome\User Data\Default\Extensions\cjpalhdlnbpafiamejdnhcphjbkeiagm\1.57.0_0";
const string animeUrl = "https://aniworld.to/anime/stream/that-time-i-got-reincarnated-as-a-slime/staffel-3/episode-1";

// Set up the Chrome driver
var options = new ChromeOptions();
options.AddArgument($"load-extension={extensionPath}");
options.AddExcludedArguments("excludeSwitches", "enable-logging");
var driver = new ChromeDriver(options);
driver.Url = animeUrl;


// Sleep to prevent ads
Thread.Sleep(1000);

var seasonLinks = GetSeasonLinks();
var activeSeason = seasonLinks.IndexOf(seasonLinks.FirstOrDefault(x => x.GetAttribute("class").Contains("active")));

for(var season = activeSeason; season < seasonLinks.Count; season++)
{
	var episodeLinks = GetEpisodeLinks();
	var activeEpisode = episodeLinks.IndexOf(episodeLinks.First(e => e.GetAttribute("class").Contains("active")));
	
	for (var episode = activeEpisode; episode < episodeLinks.Count - 1; episode++)
	{
		// run
		// GetVideoForEpisode();
	
		// get links
		episodeLinks = GetEpisodeLinks();
		episodeLinks[episode + 1].Click();
	}
	
	if(season == seasonLinks.Count - 1)
		break;
	seasonLinks = GetSeasonLinks();
	seasonLinks[season + 1].Click();
	episodeLinks = GetEpisodeLinks();
	episodeLinks.First().Click();
}

driver.Quit();

return;

ReadOnlyCollection<IWebElement> GetEpisodeLinks()
{
    return driver.FindElements(By.CssSelector("[data-episode-id]"));
}

ReadOnlyCollection<IWebElement> GetSeasonLinks()
{
    return driver.FindElements(By.CssSelector("#stream > ul:nth-child(1) > li > a"));
}

void GetVideoForEpisode()
{
	var linkElement = driver.FindElements(By.CssSelector("[data-link-id] a"))[3];
	var href = linkElement.GetAttribute("href");

	((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
	driver.SwitchTo().Window(driver.WindowHandles.Last());
	driver.Navigate().GoToUrl(href);

	Thread.Sleep(500);

	driver.FindElement(By.TagName("body")).Click();
	((IJavaScriptExecutor)driver).ExecuteScript("document.getElementsByClassName('play-overlay')[0].click();");
	var control = driver.FindElement(By.ClassName("plyr__control"));
	control.Click();
	var video = driver.FindElement(By.TagName("video")).GetAttribute("src");
	Console.WriteLine(video);

	driver.Close();
	driver.SwitchTo().Window(driver.WindowHandles.First());
}
