using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Syndication;
using System.Collections.ObjectModel;
using System.Xml;

namespace Srk.BetaseriesApiApp.ViewModels
{
    public class SiteNews : CommonViewModel
    {
        public ObservableCollection<SyndicationItem> Items
        {
            get { return _items; }
        }
        private readonly ObservableCollection<SyndicationItem> _items = new ObservableCollection<SyndicationItem>();


        public SiteNews(Main main) : base(main)
        {

        }

        #region Logic: Load
        


        #endregion

        private List<SyndicationItem> ReadRss()
        {
            var reader = XmlReader.Create("https://www.betaseries.com/blog/feed/");
            var rss = new Rss20FeedFormatter();

            if (rss.CanRead(reader))
            {
                rss.ReadFrom(reader);
                return rss.Feed.Items.ToList();
            }
            return null;
        }
        


    }
}
