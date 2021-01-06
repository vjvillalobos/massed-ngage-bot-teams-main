// <copyright file="ResponderCallHandler.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// </copyright>

namespace Sample.IncidentBot.Bot
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Graph;
    using Microsoft.Graph.Communications.Calls;
    using Microsoft.Graph.Communications.Common.Telemetry;
    using Microsoft.Graph.Communications.Resources;
    using Sample.IncidentBot.Data;
    using Sample.IncidentBot.IncidentStatus;

    /// <summary>
    /// The responder call handler class.
    /// </summary>
    public class ResponderCallHandler : CallHandler
    {
        private string responderId;

        private string classID;

        private int promptTimes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponderCallHandler"/> class.
        /// </summary>
        /// <param name="bot">The bot.</param>
        /// <param name="call">The call.</param>
        /// <param name="responderId">The responder id.</param>
        /// <param name="classID">The incident status data.</param>
        public ResponderCallHandler(Bot bot, ICall call, string responderId, string classID)
            : base(bot, call)
        {
            this.responderId = responderId;
            this.classID = classID;
        }

        /// <inheritdoc/>
        protected override void CallOnUpdated(ICall sender, ResourceEventArgs<Call> args)
        {
            if (sender.Resource.State == CallState.Established)
            {
                var currentPromptTimes = Interlocked.Increment(ref this.promptTimes);

                if (currentPromptTimes == 1)
                {
                    this.SubscribeToTone();
                    this.PlayNotificationPrompt();
                }

                if (sender.Resource.ToneInfo?.Tone != null)
                {
                    Tone tone = sender.Resource.ToneInfo.Tone.Value;

                    this.GraphLogger.Info($"Tone {tone} received.");

                    // handle different tones from responder
                    switch (tone)
                    {
                        case Tone.Tone1:
                            this.PlayTransferingPrompt();

                            // this.TransferToIncidentMeeting();
                            break;
                        case Tone.Tone0:
                        default:
                            this.PlayNotificationPrompt();
                            break;
                    }

                    sender.Resource.ToneInfo.Tone = null;
                }
            }
        }

        /// <summary>
        /// Subscribe to tone.
        /// </summary>
        private void SubscribeToTone()
        {
            Task.Run(async () =>
            {
                try
                {
                    await this.Call.SubscribeToToneAsync().ConfigureAwait(false);
                    this.GraphLogger.Info("Started subscribing to tone.");
                }
                catch (Exception ex)
                {
                    this.GraphLogger.Error(ex, $"Failed to subscribe to tone. ");
                    throw;
                }
            });
        }

        /// <summary>
        /// Play the transfering prompt.
        /// </summary>
        private void PlayTransferingPrompt()
        {
            Task.Run(async () =>
            {
                try
                {
                    await this.Call.PlayPromptAsync(new List<MediaPrompt> { this.Bot.MediaMap[Bot.TransferingPromptName] }).ConfigureAwait(false);
                    this.GraphLogger.Info("Started playing transfering prompt");
                }
                catch (Exception ex)
                {
                    this.GraphLogger.Error(ex, $"Failed to play transfering prompt.");
                    throw;
                }
            });
        }

        /// <summary>
        /// Play the notification prompt.
        /// </summary>
        private void PlayNotificationPrompt()
        {
            Task.Run(async () =>
            {
                try
                {
                    await this.Call.PlayPromptAsync(new List<MediaPrompt> { this.Bot.MediaMap[Bot.NotificationPromptName] }).ConfigureAwait(false);
                    this.GraphLogger.Info("Started playing notification prompt");
                }
                catch (Exception ex)
                {
                    this.GraphLogger.Error(ex, $"Failed to play notification prompt.");
                    throw;
                }
            });
        }
    }
}
