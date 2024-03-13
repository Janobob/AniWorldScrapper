// See https://aka.ms/new-console-template for more information

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;

var extensionPath = @"C:\Users\janwe\AppData\Local\Google\Chrome\User Data\Default\Extensions\cjpalhdlnbpafiamejdnhcphjbkeiagm\1.56.0_0";
var options = new ChromeOptions();
options.AddArgument($"load-extension={extensionPath}");
var driver = new ChromeDriver(options);

driver.Url = "https://aniworld.to/anime/stream/classroom-of-the-elite/staffel-1/episode-1";

var linkElement = driver.FindElements(By.CssSelector("[data-link-id] a"))[3];
var href = linkElement.GetAttribute("href");

Console.WriteLine(href);
//
// PerformClick();
// PerformClick();
//
// var video = driver.FindElement(By.TagName("video")).GetAttribute("src");
//
// Console.WriteLine(video);
//
// return;
//
//
// void PerformClick()
// {
// 	var actions = new Actions(driver);
// 	actions.Click(linkElement).Perform();
//
// 	Thread.Sleep(1000);
//
// 	try
// 	{
// 		driver.SwitchTo().Window(driver.WindowHandles[1]);
// 		driver.Close();
// 		driver.SwitchTo().Window(driver.WindowHandles[0]);
// 	}
// 	catch
// 	{
// 		// ignored
// 	}
// }
//

