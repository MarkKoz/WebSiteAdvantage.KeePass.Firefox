/* WebSiteAdvantage KeePass to Firefox
 * Copyright (C) 2008 - 2012  Anthony James McCreath
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using HtmlAgilityPack;

using NLog;

namespace WebSiteAdvantage.KeePass.Firefox
{
    /// <summary>
    /// Utilities for scraping information from the internet.
    /// </summary>
    public static class InternetAccessor
    {
        private static readonly HttpClient _HttpClient = new HttpClient();
        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

        static InternetAccessor()
        {
            // ServicePointManager.MaxServicePointIdleTime =
            // ServicePointManager.MaxServicePoints =
            // ServicePointManager.DefaultConnectionLimit = ;
            _HttpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Scrapes a title from a website.
        /// </summary>
        /// <param name="url">The URL to the website to scrape.</param>
        /// <returns>The scraped title or <c>null</c> if scraping failed.</returns>
        public static async Task<string> ScrapeTitleAsync(string url)
        {
            string errorMessage = $"Error scraping title for {url}.";

            try
            {
                using (HttpResponseMessage response = await _HttpClient.GetAsync(new Uri(url)).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException($"A request for {url} failed: {(int)response.StatusCode} {response.ReasonPhrase}.");

                    using (Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        var document = new HtmlDocument();
                        document.Load(stream);
                        string title = document.DocumentNode.SelectSingleNode("html/head/title")?.InnerText;

                        try
                        {
                            return title;
                        }
                        finally
                        {
                            if (title == null)
                                _Logger.Error(errorMessage);
                            else
                                _Logger.Info($"Successfully scraped title for {url}.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                switch (e) {
                    case ArgumentException _: // TODO: Too broad. Intended to catch invalid URI schemes.
                    case UriFormatException _:
                        _Logger.Error($"The URI {url} is invalid.");

                        break;
                    case TaskCanceledException _: // TODO: Too broad.
                        _Logger.Error($"The request for {url} timed out ({_HttpClient.Timeout}).");

                        break;
                    default:
                        _Logger.Error(e, errorMessage);

                        break;
                }

                return null;
            }
        }
    }
}
