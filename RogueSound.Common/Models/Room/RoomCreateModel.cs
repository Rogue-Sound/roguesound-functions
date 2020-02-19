﻿using RogueSound.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueSound.Common.Models
{
    public class RoomCreateModel
    {
        public string Creator { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool Private { get; set; }
    }

    public static class RoomCreateModelExtensions
    {
        public static RoomModel ToRoomModel(this RoomCreateModel room)
        {
            return new RoomModel
            {
                id = Guid.NewGuid().ToString(),
                PublicId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Creator = room.Creator,
                Name = room.Name,
                Description = room.Description,
                Private = room.Private,
                Region = RoomConstants.RoomPartitionKey
            };
        }
    }
}
