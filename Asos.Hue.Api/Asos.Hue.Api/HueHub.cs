﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Asos.Hue.Api.Interfaces;
using Asos.Hue.Api.Models;
using Asos.Hue.Api.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Asos.Hue.Api
{
    public class HueHub : IHue
    {
        private readonly HueHubOptions _options;
        private readonly HttpClient _httpClient;

        public HueHub(HueHubOptions options)
        {
            _options = options;
            _httpClient = ConfigureHttpClient(options);
        }

        //public async Task<List<Bulb>> GetAllBulbs()
        //{
        //    List<Bulb> bulbs = new List<Bulb>();
        //    var uri = $"api/{_options.UserKey}/lights";
        //    HttpResponseMessage response = await _httpClient.GetAsync(uri);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var response = await response.Content.ReadAsStringAsync();
        //        return 
        //    }
        //    return bulbs;
        //}

        public async Task TurnOn(Bulb bulb)
        {
            HttpResponseMessage response = await _httpClient.PutAsync(bulb.OnOffEndpoint(_options.UserKey), new { on = true }, new JsonMediaTypeFormatter());
            response.EnsureSuccessStatusCode();
        }

        public async Task TurnOff(Bulb bulb)
        {
            HttpResponseMessage response = await _httpClient.PutAsync(bulb.OnOffEndpoint(_options.UserKey), new { on = false }, new JsonMediaTypeFormatter());
            response.EnsureSuccessStatusCode();
        }
        public async Task Toggle(Bulb bulb)
        {
            HttpResponseMessage response = await _httpClient.PutAsync(bulb.OnOffEndpoint(_options.UserKey), new { on = !bulb.isOn }, new JsonMediaTypeFormatter());
            response.EnsureSuccessStatusCode();
        }
        public async Task Flash(Bulb bulb, int durationInSeconds = 10)
        {
            while (durationInSeconds > 0)
            {
                await TurnOn(bulb);
                Thread.Sleep(1000);
                await TurnOff(bulb);
                Thread.Sleep(1000);
                durationInSeconds--;
            }
        }
        public static HueHub Create(HueHubOptions options)
        {
            return new HueHub(options);
        }
        private HttpClient ConfigureHttpClient(HueHubOptions options)
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(options.Uri)
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/JSON"));
            return _httpClient;
        }

    }
}
