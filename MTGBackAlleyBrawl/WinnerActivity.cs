using System;
using System.Net;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;

namespace MTGBackAlleyBrawl
{
    [Activity(Label = "WinnerActivity")]
    public class WinnerActivity : Activity
    {
        TextView textMessage;
        ImageView winnningCreature;
        Button playAgainButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.winner);

            textMessage = FindViewById<TextView>(Resource.Id.textView1);
            winnningCreature = FindViewById<ImageView>(Resource.Id.imageView1);
            playAgainButton = FindViewById<Button>(Resource.Id.button1);

            playAgainButton.Click += delegate {
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };

            string outputText = "Your MTG Back Alley Brawler Champion is:\n" + Intent.GetStringExtra("WinningFighterName") + "\nIllustrated By: " + Intent.GetStringExtra("WinningFighterArtist");
            textMessage.SetText(outputText.ToCharArray(),0, outputText.Length);

            winnningCreature.SetImageBitmap(ScryfallTheCreatureImage(Intent.GetStringExtra("WinningFighterURI")));
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
    }
}