﻿using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;
using My.JDownloader.Api;
using My.JDownloader.Api.Models.LinkgrabberV2.Request;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AniWorldScrapper;

public class Scrapper
{
	public static void Main(string[] args)
	{
		// Setup Console Application
		var builder = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", false)
			.AddUserSecrets<Scrapper>()
			.AddCommandLine(args);
		var config = builder.Build();

		var email = config.GetValue<string>("JDownloaderSettings:Email");
		var password = config.GetValue<string>("JDownloaderSettings:Password");
		var jdownloader = new JDownloaderHandler(email, password, "crawler");
		var device = jdownloader.GetDeviceHandler(jdownloader.GetDevices().FirstOrDefault());
		var grabber = device.LinkgrabberV2;

		var extensionPath = config.GetValue<string>("SeleniumSettings:ExtensionPath");
		var animeUrl = config.GetValue<string>("AnimeSettings:Url");
		var switchSeason = config.GetValue<bool>("AnimeSettings:SwitchSeason");

		Console.WriteLine($"Extension Path: {extensionPath}");
		Console.WriteLine($"Anime URL: {animeUrl}");

		// Set up the Chrome driver
		var options = new ChromeOptions();
		options.AddArgument($"load-extension={extensionPath}");
		options.AddExcludedArguments("excludeSwitches", "enable-logging");
		var driver = new ChromeDriver(options);
		driver.Url = animeUrl;

		// Sleep to prevent ads
		Thread.Sleep(1000);

		var seasonLinks = GetSeasonLinks();
		var activeSeason =
			seasonLinks.IndexOf(seasonLinks.FirstOrDefault(x => x.GetAttribute("class").Contains("active")));

		for (var season = activeSeason; season < (switchSeason ? seasonLinks.Count : activeSeason + 1); season++)
		{
			var episodeLinks = GetEpisodeLinks();
			var activeEpisode =
				episodeLinks.IndexOf(episodeLinks.First(e => e.GetAttribute("class").Contains("active")));

			for (var episode = activeEpisode; episode < episodeLinks.Count - 1; episode++)
			{
				// Run
				GetVideoForEpisode();

				// Get links
				episodeLinks = GetEpisodeLinks();
				episodeLinks[episode + 1].Click();
			}

			if (season == seasonLinks.Count - 1)
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
			grabber.AddLinks(
				new AddLinkRequest
				{
					AutoExtract = true,
					PackageName = config.GetValue<string>("AnimeSettings:AnimeName"),
					Links = video
				});

			driver.Close();
			driver.SwitchTo().Window(driver.WindowHandles.First());
		}
	}
}