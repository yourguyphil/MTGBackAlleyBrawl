using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MTGBackAlleyBrawl
{
    [Activity(Icon = "@drawable/fight", Label = "BAB", Theme = "@style/AppTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView textMessage;
        ImageButton topCreature;
        ImageButton bottomCreature;
        int matchCounter = 0;
        bool isFirstCreate = true;
        bool round1Completed = false;
        bool round2Completed = false;
        List<CreatureFighter> listOfCreatures = new List<CreatureFighter>();
        List<string> listOfMatchResults = new List<String>();
        CreatureMatch[] allMatches = new CreatureMatch[15];
        CreatureMatch matchOn;
        int indexOfNextmatchToFill = 8;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (isFirstCreate)
            {
                isFirstCreate = false;

                Task.Run(
                    async () => await LoadMatchDataAsync()
                ).Wait();
            }

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            textMessage = FindViewById<TextView>(Resource.Id.message);

            updateScreen();

            topCreature.Click += delegate
            {
                advanceAndLoadNextMatch(matchOn.getTopFighter());
                listOfMatchResults.Add(matchOn.getTopFighter().getCardName() + "(WINNER) vs. " + matchOn.getBottomFighter());
            };

            bottomCreature.Click += delegate
            {
                advanceAndLoadNextMatch(matchOn.getBottomFighter());
                listOfMatchResults.Add(matchOn.getTopFighter().getCardName() + " vs. " + matchOn.getBottomFighter() + "(WINNER)");
            };

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
        }

        private void updateScreen()
        {
            matchOn = allMatches[matchCounter];
            matchCounter++;
            string textToSet;

            if (matchCounter == 15)
            {
                //Populate top informational message
                textToSet = "GRAND FINALS!!!";

            }
            else if (matchCounter >= 13)
            {
                textToSet = "QUARTER FINALS!!!";
                round2Completed = true;
            }
            else if (matchCounter >= 9)
            {
                textToSet = "SEMI FINALS!!!";
                round1Completed = true;
            }
            else
            {
                textToSet = "THE 16 BRAWL!!!";
            }

            textToSet += "\nMatch " + matchCounter + " (Out of 15)";
            textToSet += "\n" + matchOn.getTopFighter().getCardName() + " - vs - " + matchOn.getBottomFighter().getCardName();
            textMessage.SetText(textToSet.ToCharArray(), 0, textToSet.Length);
            topCreature = FindViewById<ImageButton>(Resource.Id.imageButton1);
            bottomCreature = FindViewById<ImageButton>(Resource.Id.imageButton2);
            topCreature.SetImageBitmap(ScryfallTheCreatureImage(matchOn.getTopFighter().getimageUrl()));
            bottomCreature.SetImageBitmap(ScryfallTheCreatureImage(matchOn.getBottomFighter().getimageUrl()));
        }

        private async System.Threading.Tasks.Task LoadMatchDataAsync()
        {
            for (int i = 0; i < 16; i++)
            {
                listOfCreatures.Add(await ScryfallARandomCreature());
            }

            int matchCounter = 0;

            //Create Matches Round 1
            for (int i = 0; i < 16; i = i + 2)
            {
                allMatches[matchCounter] = new CreatureMatch(listOfCreatures[i], listOfCreatures[i + 1]);
                matchCounter++;
            }
        }

        private async System.Threading.Tasks.Task<CreatureFighter> ScryfallARandomCreature()
        {
            var httpClient = new System.Net.Http.HttpClient();
            var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, "https://api.scryfall.com/cards/random?q=t:creature");
            HttpResponseMessage response;
            string content;
            MTGCardObject.RootObject card;

            httpClient = new System.Net.Http.HttpClient();
            response = await httpClient.SendAsync(request);
            content = await response.Content.ReadAsStringAsync();
            card = JsonConvert.DeserializeObject<MTGCardObject.RootObject>(content);

            return new CreatureFighter(card.theName, card.image_uris.art_crop, card.artist, card.set);
        }

        private Bitmap ScryfallTheCreatureImage(String URI)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(URI);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }

        private void advanceAndLoadNextMatch(CreatureFighter winner)
        {
            if (matchCounter == allMatches.Length)
            {
                Intent intent = new Intent(this, typeof(WinnerActivity));
                intent.PutExtra("WinningFighterName", winner.getCardName());
                intent.PutExtra("WinningFighterArtist", winner.getArtist());
                intent.PutExtra("WinningFighterURI", winner.getimageUrl());
                StartActivity(intent);
                return;
            }

            if (allMatches[indexOfNextmatchToFill] == null)
            {
                allMatches[indexOfNextmatchToFill] = new CreatureMatch(winner, null);
            }
            else
            {
                allMatches[indexOfNextmatchToFill].setBottomFighter(winner);
                indexOfNextmatchToFill++;
            }

            updateScreen();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_round_1:
                    Android.App.AlertDialog.Builder dialog1 = new Android.App.AlertDialog.Builder(this);
                    Android.App.AlertDialog alert1 = dialog1.Create();
                    alert1.SetTitle("Round 1 Results!");
                    string messageToDisplay= "";

                    for (int i = 0; i < listOfMatchResults.Count; i++)
                    {
                        if(i + 1 <= 4)
                        {
                            messageToDisplay += "[Round " + (i + 1) + "]: " + listOfMatchResults[i] + "\n\n";
                        }
                    }

                    alert1.SetMessage(messageToDisplay);
                    alert1.SetButton("OK", (c, ev) =>
                    {
                        // Ok button click task  
                    });
                    alert1.Show();
                    return true;
                case Resource.Id.navigation_round_2:
                    if (!round1Completed)
                    {
                        Android.App.AlertDialog.Builder dialog2 = new Android.App.AlertDialog.Builder(this);
                        Android.App.AlertDialog alert2 = dialog2.Create();
                        alert2.SetTitle("Hold up!");
                        alert2.SetMessage("Round 1 has not concluded yet.");
                        alert2.SetButton("OK", (c, ev) =>
                        {
                            // Ok button click task  
                        });
                        alert2.Show();
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                case Resource.Id.navigation_round_3:
                    if (!round2Completed)
                    {
                        Android.App.AlertDialog.Builder dialog3 = new Android.App.AlertDialog.Builder(this);
                        Android.App.AlertDialog alert3 = dialog3.Create();
                        alert3.SetTitle("Hold up!");
                        alert3.SetMessage("Round 2 has not concluded yet.");
                        alert3.SetButton("OK", (c, ev) =>
                        {
                            // Ok button click task  
                        });
                        alert3.Show();
                        return false;
                    }
                    else
                    {
                        return true;
                    }
            }
            return false;
        }
    }
}

