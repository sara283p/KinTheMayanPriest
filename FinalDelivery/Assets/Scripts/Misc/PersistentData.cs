using System;

namespace Misc
{
    [Serializable]
    public class PersistentData
    {
        public bool[] completedLevels;
        public bool[] collectedCollectibles;
        public int linkableStars;

        public PersistentData(bool[] completedLevels, bool[] collectedCollectibles, int linkableStars)
        {
            this.completedLevels = completedLevels;
            this.collectedCollectibles = collectedCollectibles;
            this.linkableStars = linkableStars;
        }
    }
}