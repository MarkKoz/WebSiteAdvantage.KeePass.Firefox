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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using HtmlAgilityPack;

using NLog;

namespace WebSiteAdvantage.KeePass.Firefox.Utilities
{
    /// <summary>
    /// Utilities for scraping information from the internet.
    /// </summary>
    public static class WebScraper
    {
        private static readonly HttpClient _HttpClient = new HttpClient();
        private static readonly Logger _Logger = LogManager.GetCurrentClassLogger();

        static WebScraper()
        {
            // ServicePointManager.MaxServicePointIdleTime =
            // ServicePointManager.MaxServicePoints =
            // ServicePointManager.DefaultConnectionLimit = ;
            _HttpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Scrapes a title and/or icon from a website.
        /// </summary>
        /// <param name="url">The URL to the website to scrape.</param>
        /// <param name="scrapeTitle"><c>true</c> if the title should be scraped; <c>false</c> otherwise.</param>
        /// <param name="scrapeIcon"><c>true</c> if the icon should be scraped; <c>false</c> otherwise.</param>
        /// <returns>The scraped title and the icon as a byte array. Either may be <c>null</c> if scraping fails.</returns>
        public static async Task<(string title, byte[] icon)> ScrapeAsync(string url, bool scrapeTitle = true, bool scrapeIcon = true)
        {
            string title = null;
            byte[] icon = null;
            var iconUrls = new List<string>(); // URLs from which to try to retrieve the icon.
            var triedGetIcon = false; // To distinguish between icon == null due to initial value or GetIcon's return value.
            Uri uri = null;

            try
            {
                uri = new Uri(url);

                if (scrapeIcon)
                    iconUrls.Add(new UriBuilder(uri) {Path = "/favicon.ico"}.Uri.AbsoluteUri);

                using (HttpResponseMessage response = await _HttpClient.GetAsync(uri).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException($"A request for {url} failed: {(int)response.StatusCode} {response.ReasonPhrase}.");

                    using (Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        var document = new HtmlDocument();
                        document.Load(stream);

                        if (scrapeTitle)
                            title = document.DocumentNode.SelectSingleNode("html/head/title")?.InnerText;

                        if (scrapeIcon)
                        {
                            // These are prepended because explicit icon links have a higher priority than the server root icon.
                            iconUrls.InsertRange(
                                0,
                                document.DocumentNode
                                    .SelectNodes("/html/head/link[@rel='shortcut icon' or @rel='icon']")
                                    .Select(n => n.GetAttributeValue("href", null)).Where(v => v != null));

                            if (iconUrls.Count == 1)
                                _Logger.Warn(
                                    $"Did not find any icon link tags for {url}; relying on favicon at server root.");

                            icon = await GetIcon(iconUrls, uri);
                            triedGetIcon = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case ArgumentException _: // TODO: Too broad. Intended to catch invalid URI schemes.
                    case UriFormatException _:
                        _Logger.Error($"The URI {url} is invalid.");

                        break;
                    case TaskCanceledException _: // TODO: Too broad.
                        _Logger.Error($"The request for {url} timed out ({_HttpClient.Timeout}).");

                        break;
                    default:
                        _Logger.Error(e, $"Error scraping {url}.");

                        break;
                }
            }

            if (scrapeTitle)
            {
                if (title == null)
                    _Logger.Error($"Error scraping title for {url}.");
                else
                    _Logger.Info($"Successfully scraped title for {url}.");
            }

            // Tries to at least get the server root favicon if an exception occurs.
            // TODO: May not want to do this if request timed out.
            if (scrapeIcon && uri != null && !triedGetIcon)
                icon = await GetIcon(iconUrls, uri);

            return (title, icon);
        }

        /// <summary>
        /// Retrieves the first successfully found icon from a list of URLs to icons.
        /// </summary>
        /// <remarks>
        /// Supports relative and absolute URLs. Relative URLs should be relative to <paramref name="site"/>.
        /// </remarks>
        /// <param name="urls">A list of potential URLs to icons of a single host.</param>
        /// <param name="site">The site for which to retrieve an icon.</param>
        /// <returns>The icon as a byte array, or <c>null</c> if no icon could be retrieved.</returns>
        private static async Task<byte[]> GetIcon(IEnumerable<string> urls, Uri site)
        {
            byte[] icon = null;
            string iconUrl = null;

            foreach (string url in urls)
            {
                try
                {
                    var uri = new Uri(url, UriKind.RelativeOrAbsolute);

                    if (!uri.IsAbsoluteUri && !Uri.TryCreate(site, url.TrimStart('/'), out uri)) continue;

                    using (HttpResponseMessage response = await _HttpClient.GetAsync(uri).ConfigureAwait(false))
                    {
                        if (!response.IsSuccessStatusCode)
                            throw new HttpRequestException($"A request for {url} failed: {(int)response.StatusCode} {response.ReasonPhrase}.");

                        icon = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                        iconUrl = url;

                        break;
                    }
                }
                catch (Exception e)
                {
                    switch (e)
                    {
                        case ArgumentException _: // TODO: Too broad. Intended to catch invalid URI schemes.
                        case UriFormatException _:
                            _Logger.Error($"The URI {url} is invalid.");

                            break;
                        case TaskCanceledException _: // TODO: Too broad.
                            _Logger.Error($"The request for {url} timed out ({_HttpClient.Timeout}).");

                            break;
                        default:
                            _Logger.Error(e, $"Error retrieving icon from {url}.");

                            break;
                    }
                }
            }

            if (icon == null)
                _Logger.Error($"Failed to scrape any icon for {site.AbsoluteUri}.");
            else
                _Logger.Info($"Successfully scraped icon from {iconUrl} for {site.AbsoluteUri}.");

            return icon;
        }
    }
}
