// <copyright file="ClassesController.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// </copyright>

namespace IcMBot.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Graph.Communications.Common;
    using Microsoft.Graph.Communications.Core.Serialization;
    using Sample.Common.Logging;
    using Sample.IncidentBot;
    using Sample.IncidentBot.Bot;
    using Sample.IncidentBot.Data;

    /// <summary>
    /// The incidents controller class.
    /// </summary>
    [Route("[controller]")]
    public class ClassesController : Controller
    {
        private Bot bot;
        private SampleObserver observer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassesController" /> class.
        /// </summary>
        /// <param name="bot">The bot.</param>
        /// <param name="observer">The log observer.</param>
        public ClassesController(Bot bot, SampleObserver observer)
        {
            this.bot = bot;
            this.observer = observer;
        }

        /// <summary>
        /// Raise a incident.
        /// </summary>
        /// <param name="joinClassData">The incident data.</param>
        /// <returns>The action result.</returns>
        [HttpPost("join")]
        public async Task<IActionResult> JoinClassAsync([FromBody] JoinClassData joinClassData)
        {
            Validator.NotNull(joinClassData, nameof(JoinClassData));

            try
            {
                var call = await this.bot.JoinClassAsync(joinClassData).ConfigureAwait(false);

                var values = new { Status = "OK", ClassID = joinClassData.ClassId, Call = call };

                var serializer = new CommsSerializer(pretty: true);
                var json = serializer.SerializeObject(values);
                return this.Ok(json);
            }
            catch (Exception e)
            {
                return this.Exception(e);
            }
        }
    }
}
