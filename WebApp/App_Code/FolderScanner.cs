
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmartPhotoGallery
{
    // Data used by Default.aspx
    public class YearItem
    {
        public string YearName { get; set; }
        public string PreviewUrl { get; set; }
        public int EventCount { get; set; }
    }

    // Data used by Events.aspx
    public class EventItem
    {
        public string YearName { get; set; }
        public string EventName { get; set; }
        public string PreviewUrl { get; set; }
        public int PhotoCount { get; set; }
    }

    public static class FolderScanner
    {
        private static readonly string[] ImageExtensions =
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".gif"
        };

        // Get all year folders and their preview information
        public static List<YearItem> GetYears()
        {
            string root = GalleryConfig.PhotoRoot;

            if (!Directory.Exists(root))
                throw new DirectoryNotFoundException(
                    "Photo folder not found: " + root);

            List<YearItem> result = new List<YearItem>();

            foreach (string yearFolder in Directory.GetDirectories(root))
            {
                string year = Path.GetFileName(yearFolder);

                if (String.IsNullOrEmpty(year) ||
                    !year.All(Char.IsDigit))
                    continue;

                List<string> events = Directory.GetDirectories(yearFolder)
                    .ToList();

                List<string> photos = new List<string>();

                foreach (string eventFolder in events)
                {
                    photos.AddRange(GetPhotos(eventFolder));
                }

                result.Add(new YearItem
                {
                    YearName = year,
                    EventCount = events.Count,
                    PreviewUrl = photos.Count > 0
                        ? GalleryConfig.ToImageUrl(
                            MakeRelative(photos[0]))
                        : ""
                });
            }

            return result.OrderByDescending(x => x.YearName).ToList();
        }

        // Get all events for a selected year
        public static List<EventItem> GetEvents(string year)
        {
            List<EventItem> result = new List<EventItem>();

            string yearFolder = GetSafeFolder(
                GalleryConfig.PhotoRoot, year);

            if (yearFolder == null ||
                !Directory.Exists(yearFolder))
                return result;

            foreach (string eventFolder in Directory.GetDirectories(yearFolder))
            {
                string eventName = Path.GetFileName(eventFolder);

                List<string> photos = GetPhotos(eventFolder);

                result.Add(new EventItem
                {
                    YearName = year,
                    EventName = eventName,
                    PhotoCount = photos.Count,
                    PreviewUrl = photos.Count > 0
                        ? GalleryConfig.ToImageUrl(
                            MakeRelative(photos[0]))
                        : ""
                });
            }

            return result.OrderBy(x => x.EventName).ToList();
        }

        // Get all images recursively from an event folder
        // Supports direct photos and nested images folders
        public static List<string> GetPhotos(string eventPath)
        {
            List<string> result = new List<string>();

            if (String.IsNullOrEmpty(eventPath) ||
                !Directory.Exists(eventPath))
                return result;

            string fullPath = Path.GetFullPath(eventPath);
            string root = Path.GetFullPath(GalleryConfig.PhotoRoot);

            if (!IsInsideRoot(fullPath, root))
                return result;

            foreach (string file in Directory.GetFiles(
                fullPath, "*.*", SearchOption.AllDirectories))
            {
                string extension =
                    Path.GetExtension(file).ToLowerInvariant();

                if (ImageExtensions.Contains(extension))
                    result.Add(file);
            }

            return result.OrderBy(x => x).ToList();
        }

        // Convert physical file path to relative photo path
        public static string MakeRelative(string filePath)
        {
            string root = Path.GetFullPath(GalleryConfig.PhotoRoot)
                .TrimEnd(Path.DirectorySeparatorChar) +
                Path.DirectorySeparatorChar;

            string fullFile = Path.GetFullPath(filePath);

            if (!fullFile.StartsWith(
                root, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    "Photo is outside PhotoRoot.");

            return fullFile.Substring(root.Length);
        }

        // Prevent paths outside the photo archive
        private static string GetSafeFolder(
            string root, string folderName)
        {
            if (String.IsNullOrWhiteSpace(folderName))
                return null;

            string fullRoot = Path.GetFullPath(root)
                .TrimEnd(Path.DirectorySeparatorChar) +
                Path.DirectorySeparatorChar;

            string fullFolder = Path.GetFullPath(
                Path.Combine(root, folderName));

            if (!IsInsideRoot(fullFolder, fullRoot.TrimEnd(
                Path.DirectorySeparatorChar)))
                return null;

            return fullFolder;
        }

        private static bool IsInsideRoot(
            string path, string root)
        {
            string rootWithSeparator = root.TrimEnd(
                Path.DirectorySeparatorChar) +
                Path.DirectorySeparatorChar;

            return path.StartsWith(
                rootWithSeparator,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
