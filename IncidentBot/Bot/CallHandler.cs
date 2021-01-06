// <copyright file="CallHandler.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// </copyright>

namespace Sample.IncidentBot.Bot
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Timers;
    using Microsoft.Graph;
    using Microsoft.Graph.Communications;
    using Microsoft.Graph.Communications.Calls;
    using Microsoft.Graph.Communications.Calls.Media;
    using Microsoft.Graph.Communications.Common;
    using Microsoft.Graph.Communications.Common.Telemetry;
    using Microsoft.Graph.Communications.Resources;
    using Microsoft.Skype.Bots.Media;
    using Sample.Common;

    /// <summary>
    /// Base class for call handler for event handling, logging and cleanup.
    /// </summary>
    public class CallHandler : HeartbeatHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallHandler"/> class.
        /// </summary>
        /// <param name="bot">The bot.</param>
        /// <param name="call">The call.</param>
        public CallHandler(Bot bot, ICall call)
            : base(TimeSpan.FromMinutes(10), call?.GraphLogger)
        {
            this.Bot = bot;
            this.Call = call;

            // Eventos recibidos por el bot
            this.Call.OnUpdated += this.CallOnUpdated;
            this.Call.Participants.OnUpdated += this.OnParticipantsUpdated;

            // subscribe to dominant speaker event on the audioSocket
            this.Call.GetLocalMediaSession().AudioSocket.DominantSpeakerChanged += this.OnDominantSpeakerChanged;

            // subscribe to the VideoMediaReceived event on the main video socket
            // this.Call.GetLocalMediaSession().VideoSockets.FirstOrDefault().VideoMediaReceived += this.OnVideoMediaReceived;
            // susbscribe to the participants updates, this will inform the bot if a particpant left/joined the conference
            this.Call.Participants.OnUpdated += this.ParticipantsOnUpdated;

            /*foreach (var videoSocket in this.Call.GetLocalMediaSession().VideoSockets)
            {
                this.availableSocketIds.Add((uint)videoSocket.SocketId);
            }*/
        }

        /// <summary>
        /// Gets the call interface.
        /// </summary>
        public ICall Call { get; }

        /// <summary>
        /// Gets the bot.
        /// </summary>
        protected Bot Bot { get; }

        /// <inheritdoc/>
        protected override Task HeartbeatAsync(ElapsedEventArgs args)
        {
            return this.Call.KeepAliveAsync();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.Call.OnUpdated -= this.CallOnUpdated;
            this.Call.Participants.OnUpdated -= this.OnParticipantsUpdated;

            foreach (var participant in this.Call.Participants)
            {
                participant.OnUpdated -= this.OnParticipantUpdated;
            }
        }

        /// <summary>
        /// The event handler when call is updated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        protected virtual void CallOnUpdated(ICall sender, ResourceEventArgs<Call> args)
        {
            // do nothing in base class.
        }

        /// <summary>
        /// The event handler when participants are updated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        protected virtual void ParticipantsOnUpdated(IParticipantCollection sender, CollectionEventArgs<IParticipant> args)
        {
            // do nothing in base class.
        }

        /// <summary>
        /// Event handler when participan is updated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        protected virtual void ParticipantOnUpdated(IParticipant sender, ResourceEventArgs<Participant> args)
        {
            // do nothing in base class.
        }

        /// <summary>
        /// Event handler when participan is updated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        private void OnParticipantUpdated(IParticipant sender, ResourceEventArgs<Participant> args)
        {
            var request = new { Type = "participant_update", Content = args };
            string jsonString = JsonSerializer.Serialize(request);
            using (var client = new HttpClient())
            {
                var response = client.PostAsync("https://ngage.eastus2.cloudapp.azure.com:2145/teamsbot", new StringContent(jsonString, Encoding.UTF8, "application/json")).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    string responseString = responseContent.ReadAsStringAsync().Result;
                    Console.WriteLine(responseString);
                }
            }

            this.ParticipantOnUpdated(sender, args);
        }

        /// <summary>
        /// The event handler when participants are updated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        private void OnParticipantsUpdated(IParticipantCollection sender, CollectionEventArgs<IParticipant> args)
        {
            var request = new { Type = "participants_update", Content = args };
            string jsonString = JsonSerializer.Serialize(request);
            using (var client = new HttpClient())
            {
                var response = client.PostAsync("https://ngage.eastus2.cloudapp.azure.com:2145/teamsbot", new StringContent(jsonString, Encoding.UTF8, "application/json")).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    string responseString = responseContent.ReadAsStringAsync().Result;
                    Console.WriteLine(responseString);
                }
            }

            foreach (var participant in args.AddedResources)
            {
                participant.OnUpdated += this.OnParticipantUpdated;
            }

            foreach (var participant in args.RemovedResources)
            {
                participant.OnUpdated -= this.OnParticipantUpdated;
            }

            this.ParticipantsOnUpdated(sender, args);
        }

        /// <summary>
        /// Listen for dominant speaker changes in the conference.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The dominant speaker changed event arguments.
        /// </param>
        private void OnDominantSpeakerChanged(object sender, DominantSpeakerChangedEventArgs args)
        {
            this.GraphLogger.Info($"[{this.Call.Id}:OnDominantSpeakerChanged(DominantSpeaker={args.CurrentDominantSpeaker})]");

            var request = new { Type = "dominant_speaker_change", Content = args };
            string jsonString = JsonSerializer.Serialize(request);
            using (var client = new HttpClient())
            {
                var response = client.PostAsync("https://ngage.eastus2.cloudapp.azure.com:2145/teamsbot", new StringContent(jsonString, Encoding.UTF8, "application/json")).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    string responseString = responseContent.ReadAsStringAsync().Result;
                    Console.WriteLine(responseString);
                }
            }
        }
    }
}
