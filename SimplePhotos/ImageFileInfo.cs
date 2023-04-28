﻿using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace SimplePhotos
{
    public class ImageFileInfo : INotifyPropertyChanged
    {
        public ImageFileInfo(ImageProperties properties,
            StorageFile imageFile,
            string name,
            string type)
        {
            ImageProperties = properties;
            ImageName = name;
            ImageFileType = type;
            ImageFile = imageFile;
            var rating = (int)properties.Rating;
            var random = new Random();
            ImageRating = rating == 0 ? random.Next(1, 5) : rating;
        }

        public StorageFile ImageFile { get; }

        public ImageProperties ImageProperties { get; }

        public async Task<BitmapImage> GetImageSourceAsync()
        {
            using IRandomAccessStream fileStream = await ImageFile.OpenReadAsync();

            // Create a bitmap to be the image source.
            BitmapImage bitmapImage = new();
            bitmapImage.SetSource(fileStream);

            return bitmapImage;
        }

        public async Task<BitmapImage> GetImageThumbnailAsync()
        {
            StorageItemThumbnail thumbnail =
                await ImageFile.GetThumbnailAsync(ThumbnailMode.PicturesView);
            // Create a bitmap to be the image source.
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(thumbnail);
            thumbnail.Dispose();

            return bitmapImage;
        }

        public string ImageName { get; }

        public string ImageFileType { get; }

        public string ImageDimensions => $"{ImageProperties.Width} x {ImageProperties.Height}";

        public string ImageTitle
        {
            get => string.IsNullOrEmpty(ImageProperties.Title) ? ImageName : ImageProperties.Title;
            set
            {
                if (ImageProperties.Title != value)
                {
                    ImageProperties.Title = value;
                    _ = ImageProperties.SavePropertiesAsync();
                    OnPropertyChanged();
                }
            }
        }

        public int ImageRating
        {
            get => (int)ImageProperties.Rating;
            set
            {
                if (ImageProperties.Rating != value)
                {
                    ImageProperties.Rating = (uint)value;
                    _ = ImageProperties.SavePropertiesAsync();
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
