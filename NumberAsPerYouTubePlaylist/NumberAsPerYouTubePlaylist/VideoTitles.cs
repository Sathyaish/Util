using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NumberAsPerYouTubePlaylist
{
    public class VideoTitles : IEnumerable<VideoTitle>
    {
        private List<VideoTitle> _titles = null;
        private Dictionary<string, IList<VideoTitle>> _dictionary = null;

        public VideoTitles()
        {
            _titles = new List<VideoTitle>();
            _dictionary = new Dictionary<string, IList<VideoTitle>>(StringComparer.InvariantCultureIgnoreCase);
        }

        public IEnumerator<VideoTitle> GetEnumerator()
        {
            return _titles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IList<VideoTitle> this[string title]
        {
            get
            {
                return _dictionary[title];
            }
        }

        public VideoTitle this[int order]
        {
            get
            {
                var contender = _titles[order - 1];

                Debug.Assert(contender.Order == order);

                return contender;
            }
        }

        public IList<VideoTitle> GetVideoTitlesOrDefault(string title)
        {
            try
            {
                var value = _dictionary[title];

                return value;
            }
            catch(KeyNotFoundException)
            {
                return default(IList<VideoTitle>);
            }
        }

        public VideoTitle Add(string title, int order, string id, Func<string, string> urlMaker)
        {
            var videoTitle = new VideoTitle(title, order, urlMaker, id);

            this.Add(videoTitle);
            
            return videoTitle;
        }

        public VideoTitle Add(VideoTitle videoTitle)
        {
            _titles.Add(videoTitle);

            if (_dictionary.ContainsKey(videoTitle.Title))
                _dictionary[videoTitle.Title].Add(videoTitle);
            else
                _dictionary.Add(videoTitle.Title, new List<VideoTitle> { videoTitle });

            return videoTitle;
        }

        public int Count
        {
            get
            {
                var valuesInDictionary = _dictionary.SelectMany(col => col.Value).Count();

                Debug.Assert(valuesInDictionary == _titles.Count);

                return _titles.Count;
            }
        }
    }
}