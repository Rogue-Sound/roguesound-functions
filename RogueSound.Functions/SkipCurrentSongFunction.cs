﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents;
using System.Collections.Generic;

namespace RogueSound.Functions
{
    public static partial class RogueSoundFunctions
    {
        [FunctionName("SkipCurrentSong")]
        public static async Task<IActionResult> SkipCurrentSong(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var data = JsonConvert.DeserializeObject<SkipSongRequestModel>(await req.ReadAsStringAsync());
            log.LogInformation($"HttpTriger, skipping song as requested by {data.User}");

            var queryUri = UriFactory.CreateDocumentCollectionUri("RogueSound", "Sessions");
            var feedOptions = new FeedOptions { PartitionKey = new PartitionKey(0) };

            var todayDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);

            var currentSessionQuery = client.CreateDocumentQuery<RoomSessionModel>(queryUri, feedOptions)
                .Where(x => x.SessionDate == todayDate)
                .OrderBy(x => x.CreatedAt)
                .Take(1)
                .AsDocumentQuery();

            var sessionsReturned = new List<RoomSessionModel>();

            while (currentSessionQuery.HasMoreResults) sessionsReturned.AddRange(await currentSessionQuery.ExecuteNextAsync<RoomSessionModel>());

            var currentSession = sessionsReturned.FirstOrDefault();

            log.LogInformation($"Returned {sessionsReturned.Count} sessions");

            var songList = currentSession.Songs.ToList();

            if (songList.Where(x => x.StartTime < DateTime.Now).Any())
            {
                currentSession.Songs = songList.RemoveCurrent();

                var uri = UriFactory.CreateDocumentUri("RogueSound", "Sessions", currentSession.id);

                var partitionOptions = new RequestOptions { PartitionKey = new PartitionKey(0) };

                await client.ReplaceDocumentAsync(queryUri, currentSession, partitionOptions);
            }

            return new OkResult();
        }
    }
}
