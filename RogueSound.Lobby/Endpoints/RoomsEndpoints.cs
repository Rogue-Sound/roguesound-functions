﻿using EzyPaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RogueSound.Common.Models;
using RogueSound.Common.Sorting;
using RogueSound.Lobby.Actions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RogueSound.Lobby.Endpoints
{
    public class RoomsEndpoints
    {
        private readonly GetRoomsAction getRoomsAction;
        private readonly AddRoomAction addRoomAction;

        public RoomsEndpoints(GetRoomsAction getRoomsAction, AddRoomAction addRoomAction)
        {
            this.getRoomsAction = getRoomsAction;
            this.addRoomAction = addRoomAction;
        }

        [FunctionName(nameof(GetRooms))]
        public async Task<IActionResult> GetRooms(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Rooms")] HttpRequest req, ILogger log)
        {
            var paging = req.ExtractPaging();
            var sorting = req.ExtractSorting();

            log.LogInformation("Serving request: GetRooms");

            return await this.getRoomsAction.ExecuteAsync(paging, sorting);
        }

        [FunctionName(nameof(GetRoomDetails))]
        public async Task<IActionResult> GetRoomDetails(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Rooms/{roomId}")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Serving request: GetRoomDetails");

            return new OkResult();
        }

        [FunctionName(nameof(AddRoom))]
        public async Task<IActionResult> AddRoom(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Rooms")] HttpRequest req, ILogger log)
        {
            log.LogInformation("Serving request: add a new room");

            RoomCreateModel reqBody;

            try
            {
                reqBody = JsonConvert.DeserializeObject<RoomCreateModel>(await req.ReadAsStringAsync());
                if (reqBody == null) return new BadRequestObjectResult("Malformed request body");
            }
            catch(Exception e)
            {
                return new BadRequestObjectResult("Malformed request body");
            }

            return await this.addRoomAction.ExecuteAsync(reqBody);
        }
    }
}
