using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MTGBackAlleyBrawl
{
    class CreatureFighter
    {
        private string cardName;
        private string imageUrl;
        private string artist;
        private string set;

        public CreatureFighter(string cardName, string imageUrl, string artist, string set)
        {
            this.cardName = cardName;
            this.imageUrl = imageUrl;
            this.artist = artist;
            this.set = set;
        }
        
        public string getCardName()
        {
            return cardName;
        }

        public string getimageUrl()
        {
            return imageUrl;
        }

        public string getArtist()
        {
            return artist;
        }

        public string getSet()
        {
            return set;
        }

    }
}