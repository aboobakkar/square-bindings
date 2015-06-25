﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Square.OkHttp;

namespace OkHttpSample
{
    [Activity(Label = "OkHttpSample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private static string Endpoint = "https://api.github.com/repos/square/okhttp/contributors";
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Button download = FindViewById<Button>(Resource.Id.download);
            ListView listView = FindViewById<ListView>(Resource.Id.listView);

            download.Click += async delegate
            {
                await Task.Run(async () =>
                {
                    OkHttpClient client = new OkHttpClient();

                    // Create request for remote resource.
                    Request request = new Request.Builder()
                        .Url(Endpoint)
                        .Build();

                    // Execute the request and retrieve the response.
                    Response response = await client.NewCall(request).ExecuteAsync();

                    // Deserialize HTTP response to concrete type.
                    string body = response.Body().String();
                    List<Contributor> contributors = JsonConvert.DeserializeObject<List<Contributor>>(body);

                    // Sort list by the most contributions.
                    List<string> data = contributors
                        .OrderByDescending(c => c.contributions)
                        .Select(c => string.Format("{0} ({1})", c.login, c.contributions))
                        .ToList();

                    // Output list of contributors.
                    IListAdapter adapter = new ArrayAdapter<string>(
                        this, 
                        Android.Resource.Layout.SimpleListItem1,
                        Android.Resource.Id.Text1, 
                        data);
                    RunOnUiThread(() =>
                    {
                        listView.Adapter = adapter;
                    });
                });
            };
        }

        public class Contributor
        {
            public string login { get; set; }

            public int contributions { get; set; }
        }
    }
}

