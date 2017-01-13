using System;

namespace NumberAsPerYouTubePlaylist
{
    public class VideoTitle
    {
        private string _id = null;

        public VideoTitle(string title, int order, Func<string, string> urlMaker, string id)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentNullException(nameof(title));

            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            if (urlMaker == null)
                throw new ArgumentNullException(nameof(urlMaker));

            Title = title;
            Order = order;
            UrlMaker = urlMaker;
            Id = id;
        }

        public string Title { get; set; }
        public int Order { get; set; }

        public string Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;

                OnSetId(value);
            }
        }

        public string Url { get; protected set; }

        public Func<string, string> UrlMaker { get; protected set; }

        protected virtual void OnSetId(string id)
        {
            Url = UrlMaker(id);
        }
    }
}