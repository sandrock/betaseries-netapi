using System;

namespace Srk.BetaseriesApi {

    /// <summary>
    /// Represent a user notification.
    /// </summary>
    public class Notification {

        /// <summary>
        /// The unique ID.
        /// </summary>
        public ulong NotificationId { get; set; }

        /// <summary>
        /// A french description of the notification.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Notification date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Boolean to mark the notification as seen.
        /// </summary>
        public bool Seen { get; set; }

    }
}
