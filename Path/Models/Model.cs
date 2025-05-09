using LearnPath.ViewModels;
using System.Configuration;
using System.IO;

namespace LearnPath.Models
{

    class Model
    {
        public static void PathUpdate(MainViewModel mainViewMode) {

            mainViewMode.FileCollectionShows = [];

            if (mainViewMode.RootFolder == null)
            {
                return;
            }

            foreach (string suffix in mainViewMode.SubtitleSuffixes)
            {
                foreach (var subtitleFile in mainViewMode.RootFolder.GetFiles("*." + suffix))
                {
                    mainViewMode.FileCollectionShows.Add(
                        new FileCollectionItem(subtitleFile, FileType.Subtitle)
                        );
                }
            }

            foreach (string suffix in mainViewMode.VideoSuffixes)
            {
                foreach (var subtitleFile in mainViewMode.RootFolder.GetFiles("*." + suffix))
                {
                    mainViewMode.FileCollectionShows.Add(
                        new FileCollectionItem(subtitleFile, FileType.Video)
                        );
                }
            }
        }
        public static void FilterGroupUpdate(MainViewModel mainViewModel) {
            
        }
    }
}
