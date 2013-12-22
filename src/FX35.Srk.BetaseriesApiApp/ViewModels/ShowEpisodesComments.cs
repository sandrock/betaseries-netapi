using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Srk.BetaseriesApi;

namespace Srk.BetaseriesApiApp.ViewModels
{
    public class ShowEpisodesComments : CommonViewModel
    {

        #region Property

        #region Show
        /// <summary>
        /// The <see cref="MyShow" /> property's name.
        /// </summary>
        public const string MyPropertyShow = "Show";

        private string _show = "";

        /// <summary>
        /// Gets the MyProperty property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string MyShow
        {
            get
            {
                return _show;
            }

            set
            {
                if (_show == value)
                {
                    return;
                }

                var oldValue = _show;
                _show = value;

                // Update bindings, no broadcast
                RaisePropertyChanged("MyShow");
            }
        }
        #endregion

        #region Season
        /// <summary>
        /// The <see cref="MySeason" /> property's name.
        /// </summary>
        public const string MyPropertySeason = "Season";

        private string _season = "";

        /// <summary>
        /// Gets the MyProperty property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string MySeason
        {
            get
            {
                return _season;
            }

            set
            {
                if (_season == value)
                {
                    return;
                }

                _season = value;

                // Update bindings, no broadcast
                RaisePropertyChanged("MySeason");
            }
        }
        #endregion

        #region Comments
        /// <summary>
        /// The <see cref="MyProperty" /> property's name.
        /// </summary>
        public const string MyPropertyComments = "MyComments";

        private readonly ObservableCollection<Comment> _comments = new ObservableCollection<Comment>();

        /// <summary>
        /// Gets the MyProperty property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public ObservableCollection<Comment> MyComments
        {
            get
            {
                return _comments;
            }
        }
        #endregion

        #region EpisodeNb
        /// <summary>
        /// The <see cref="MyEpisode" /> property's name.
        /// </summary>
        public const string _MyEpisodeNB = "EpisodeNB";

        private string _episodenb = "";

        /// <summary>
        /// Gets the MyEpisode property.
        /// TODO Update documentation:
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string MyEpisodeNB
        {
            get
            {
                return _episodenb;
            }

            set
            {
                if (_episodenb == value)
                {
                    return;
                }

                _episodenb = value;

                // Update bindings, no broadcast
                RaisePropertyChanged("MyEpisodeNB");
            }
        }
        #endregion

        #region Member
        public string MemberLogin
        {
            get { return _memberLogin; }
            set { _memberLogin = value; }
        }
        private string _memberLogin;

        #endregion

        public string Comment
        {
            get { return _comment; }
            set { SetValue(ref _comment, value, "Comment"); }
        }
        private string _comment;

        public string InReplyTo
        {
            get { return _inReplyTo; }
            set { SetValue(ref _inReplyTo, value, "InReplyTo"); }
        }
        private string _inReplyTo;

        public Comment SelectedComment
        {
            get { return _selectedComment; }
            set { SetValue(ref _selectedComment, value, "SelectedComment"); }
        }
        private Comment _selectedComment;

        #endregion

        #region Commands

        /// <summary>
        /// Search
        /// To be bound in the view.
        /// </summary>
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(
                        OnSearch, CanSearch);
                }
                return _searchCommand;
            }
        }
        private ICommand _searchCommand;
        private void OnSearch()
        {
            LoadCommentsEpisaodeAsync(MyShow, MySeason, MyEpisodeNB);
        }

        private bool CanSearch()
        {
            return IsNotBusy;
        }

        /// <summary>
        /// Command description.
        /// To be bound in the view.
        /// </summary>
        public ICommand SearchShowCommentCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _SearchShowCommentCommand ?? (_SearchShowCommentCommand = new RelayCommand(OnSearchShowcomment, CanSearchShowcomment)); }
        }
        private ICommand _SearchShowCommentCommand;

        private void OnSearchShowcomment()
        {
            LoadCommentsShowAsync(MyShow);
        }

        private bool CanSearchShowcomment()
        {
            return IsNotBusy;
        }

        /// <summary>
        /// Command description.
        /// To be bound in the view.
        /// </summary>
        public ICommand GetMemberCommentsCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _getMemberCommentsCommand ?? (_getMemberCommentsCommand = new RelayCommand(OnGetMemberComments, CanGetMemberComments)); }
        }
        private ICommand _getMemberCommentsCommand;

        private void OnGetMemberComments()
        {
            LoadMemberCommentsAsync(MemberLogin);
        }

        private bool CanGetMemberComments()
        {
            return IsNotBusy;
        }

        #region CommentShow command

        /// <summary>
        /// comment on show
        /// To be bound in the view.
        /// </summary>
        public ICommand CommentShowCommand
        {
            get
            {
                return _commentShowCommand ?? (_commentShowCommand = new RelayCommand(OnCommentShow, CanCommentShow));
            }
        }
        private ICommand _commentShowCommand;

        private void OnCommentShow()
        {
            UpdateStatus("Sending comment...", true);

            uint? inreplyto = 0;
            if (SelectedComment != null)
            {
                //TODO: Comment.InnerId is not a valid interger type.
                inreplyto = (uint)SelectedComment.InnerId;

            }

            try
            {
                client.CommentShowAsync(MyShow, Comment, inreplyto, this.client_CommentEnded);
            }
            catch (Exception ex)
            {
                UpdateStatus("Failed to send comment.", false);
                MessageBox.Show(ex.Message);
            }
        }

        private bool CanCommentShow()
        {
            return IsNotBusy;
        }

        #endregion

        #region CommentEpisode command

        /// <summary>
        /// CommentEpisode
        /// To be bound in the view.
        /// </summary>
        public ICommand CommentEpisodeCommand
        {
            get
            {
                return _commentEpisodeCommand ?? (_commentEpisodeCommand = new RelayCommand(OnCommentEpisode, CanCommentEpisode));
            }
        }
        private ICommand _commentEpisodeCommand;

        private void OnCommentEpisode()
        {
            UpdateStatus("Sending comment...", true);

            uint? inreplyto = 0;
            if (SelectedComment != null)
            {
                inreplyto = (uint)SelectedComment.InnerId;
            }

            try
            {
                client.CommentEpisodeAsync(MyShow, uint.Parse(MySeason), uint.Parse(MyEpisodeNB), Comment, inreplyto, this.client_CommentEnded);
            }
            catch (Exception ex)
            {
                UpdateStatus("Failed to send comment.", false);
                MessageBox.Show(ex.Message);
            }
        }

        private bool CanCommentEpisode()
        {
            return IsNotBusy;
        }

        #endregion

        #region CommentMember command

        /// <summary>
        /// CommentMember
        /// To be bound in the view.
        /// </summary>
        public ICommand CommentMemberCommand
        {
            get
            {
                return _commentMemberCommand ?? (_commentMemberCommand = new RelayCommand(OnCommentMember, CanCommentMember));
            }
        }
        private ICommand _commentMemberCommand;

        private void OnCommentMember()
        {
            UpdateStatus("Sending comment....", true);

            uint? inreplyto = 0;
            if (SelectedComment != null)
            {
                inreplyto = (uint)SelectedComment.InnerId;
            }

            try
            {
                client.CommentMemberAsync(MemberLogin, Comment, inreplyto, this.client_CommentEnded);
            }
            catch (Exception ex)
            {
                UpdateStatus("Failed to send comment.", false);
                MessageBox.Show(ex.Message);
            }
        }

        private bool CanCommentMember()
        {
            return IsNotBusy;
        }

        #endregion

        #region ClearReply command

        /// <summary>
        /// ClearReply
        /// To be bound in the view.
        /// </summary>
        public ICommand ClearReplyCommand
        {
            get
            {
                return _clearReplyCommand ?? (_clearReplyCommand = new RelayCommand(OnClearReply));
            }
        }
        private ICommand _clearReplyCommand;

        private void OnClearReply()
        {
            SelectedComment = null;
        }

        #endregion

        #endregion

        public ShowEpisodesComments(Main main)
            : base(main)
        {
            bool debug = false;
#if DEBUG
            debug = true;
#endif
            #region Design data
            // Design data
            if (IsInDesignMode || debug)
            {
                // Design data
                _show = "a-developers-life";
                // Design data
                _episodenb = "1";
                // Design data
                _season = "1";
                // Design data
                _memberLogin = "srktest";
                // Design data
                _comment = @"comment sent from wpftestapp #
Special: é€$£µ%!?,;èàç
URL: key=value&otherkey=othervalue
HTML: <b>is this <a href=""http://www.betaseries.com/"" ref=""nofollow noindex"">bold</a>?</b>
end";
                // Design data
            }
            if (IsInDesignMode)
            {
                // Design data
                _comments = new ObservableCollection<Comment>() {
                    new Comment {
                        InnerId = 65447874,
                        Text = "nhvedfjk jher hvjerh bjkesrh bjrej bje bjhredh bjrehbjerhdjb hreqwlb re",
                        Username = "suepr username"
                    },
                    new Comment {
                        InnerId = 15472,
                        Text = "nhvedfjk jher hvjerh bjkesrh bjrej bje bjhredh bjrehbjerhdjb hreqwlb re",
                        Username = "suepr username"
                    },
                    new Comment {
                        InnerId = 5674564,
                        Text = "nhvedfjk jher hvjerh bjkesrh bjrej bje bjhredh bjrehbjerhdjb hreqwlb re",
                        Username = "suepr username"
                    },
                };
                // Design data
            }
            #endregion
        }

        private void LoadCommentsEpisaodeAsync(string showurl, string season, string episodenb)
        {
            UpdateStatus("Searching...", true);

            ThreadPool.QueueUserWorkItem(param =>
            {
                try
                {
                    var result = client.GetCommentsForEpisode(showurl, uint.Parse(season), uint.Parse(episodenb));
                    DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
                    {
                        MyComments.Clear();
                        UpdateStatus("Done.", false);
                        MyComments.Clear();
                        foreach (var item in result)
                        {
                            MyComments.Add(item);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    CurrentDispatcher.BeginInvoke(new Action(() =>
                    {
                        UpdateStatus(ex.Message, false);
                    }));
                    return;
                }
            });
        }

        private void LoadCommentsShowAsync(string showurl)
        {
            UpdateStatus("Searching...", true);

            ThreadPool.QueueUserWorkItem(param =>
            {
                try
                {
                    var result = client.GetCommentsForShow(showurl);
                    DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
                    {
                        MyComments.Clear();
                        UpdateStatus("Done.", false);
                        MyComments.Clear();
                        foreach (var item in result)
                        {
                            MyComments.Add(item);
                        }
                    }));
                }
                catch (Exception ex)
                {

                    CurrentDispatcher.BeginInvoke(new Action(() =>
                    {
                        UpdateStatus(ex.Message, false);
                    }));
                    return;
                }

            });
        }

        private void LoadMemberCommentsAsync(string username)
        {
            ThreadPool.QueueUserWorkItem(param =>
            {
                UpdateStatus("Searching...", true);

                try
                {
                    var result = client.GetCommentsForMember(username);
                    DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
                    {
                        MyComments.Clear();
                        UpdateStatus("Done.", false);
                        MyComments.Clear();
                        foreach (var item in result)
                        {
                            MyComments.Add(item);
                        }
                    }));
                }
                catch (Exception ex)
                {

                    CurrentDispatcher.BeginInvoke(new Action(() =>
                    {
                        UpdateStatus(ex.Message, false);
                    }));
                    return;
                }
            });
        }

        void client_CommentEnded(object sender, AsyncResponseArgs e)
        {
            DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
            {
                if (e.Succeed)
                {
                    UpdateStatus("Comment posted.", false);
                }
                else
                {
                    UpdateStatus("Comment not posted: " + e.Message, false);
                    MessageBox.Show(e.Message);
                }
            }));
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}
