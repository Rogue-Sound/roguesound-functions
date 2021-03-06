﻿using System;
using System.Collections.Generic;

namespace RogueSound.Functions
{
    public class SongQueueResponseModel
    {
        public Guid PublicId { get; set; }

        public string RoomId { get; set; }

        public string User { get; set; }

        public string SongId { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public string AlbumName { get; set; }

        public string AlbumImg { get; set; }

        public double Duration { get; set; }

        public double Position { get; set; }

        public DateTime ResquestTime { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public IEnumerable<RetardInSessionResponseModel> RetardsInSession { get; set; }
    }
}
