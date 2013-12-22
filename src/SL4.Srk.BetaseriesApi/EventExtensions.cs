﻿using System;

namespace Srk.BetaseriesApi {
    static class Extensions {
        public static void Raise(this EventHandler @event, object sender, EventArgs e) {
            if (@event != null)
                @event(sender, e);
        }

        public static void Raise<T>(this EventHandler<T> @event, object sender, T e) where T : EventArgs {
            if (@event != null)
                @event(sender, e);
        }
    }

}
