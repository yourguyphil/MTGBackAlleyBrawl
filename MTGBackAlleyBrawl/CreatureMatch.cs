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
    class CreatureMatch
    {
        private CreatureFighter topFighter;
        private CreatureFighter bottomFighter;

        public CreatureMatch(CreatureFighter topFighter, CreatureFighter bottomFighter)
        {
            this.topFighter = topFighter;
            this.bottomFighter = bottomFighter;
        }

        public CreatureFighter getTopFighter()
        {
            return topFighter;
        }

        public CreatureFighter getBottomFighter()
        {
            return bottomFighter;
        }

        public void setBottomFighter(CreatureFighter bottomFighter)
        {
            this.bottomFighter = bottomFighter;
        }

    }
}