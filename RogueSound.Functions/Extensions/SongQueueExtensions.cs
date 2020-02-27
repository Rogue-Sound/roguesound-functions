﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueSound.Functions
{
    public static class SongQueueExtensions
    {
        public static bool AnyUnplayed(this IEnumerable<SongQueueModel> songQueue, string songId)
        {
            return songQueue.Select(x => x.SongId == songId).Any();
        }

        public static IEnumerable<SongQueueModel> FindUnplayed(this IEnumerable<SongQueueModel> songQueue, string songId)
        {
            return songQueue.Where(x => x.SongId == songId && x.StartTime > DateTime.UtcNow).OrderBy(x => x.StartTime).ToList();
        }

        public static IEnumerable<SongQueueModel> RemoveUnplayed(this IEnumerable<SongQueueModel> songQueue, string songId)
        {
            var theOddOneOut = songQueue.Where(x => x.SongId == songId && x.StartTime > DateTime.UtcNow)
                .OrderBy(x => x.StartTime).FirstOrDefault();

            return songQueue.Except(new List<SongQueueModel>(){theOddOneOut})
                .FixQueueSongGapTimings(theOddOneOut);
        }

        public static IEnumerable<SongQueueModel> RemoveCurrent(this IEnumerable<SongQueueModel> songQueue)
        {
            var playingSong = songQueue.Where(x => x.StartTime < DateTime.Now && x.EndTime > DateTime.UtcNow).FirstOrDefault();

            return songQueue.Except(new List<SongQueueModel>() { playingSong })
                .FixQueueSongGapTimings(playingSong);
        }

        public static IEnumerable<SongQueueModel> FixQueueSongGapTimings(this IEnumerable<SongQueueModel> songQueue, SongQueueModel gappedSong)
        {
            var gapSpan = TimeSpan.FromMilliseconds(gappedSong.Duration);

            return songQueue.Select(x => new SongQueueModel()
            {
                User = x.User,
                AlbumImg = x.AlbumImg,
                AlbumName = x.AlbumName,
                Artist = x.Artist,
                Duration = x.Duration,
                EndTime = x.EndTime.Subtract(gapSpan),
                RequestTime = x.RequestTime,
                RoomId = x.RoomId,
                SongId = x.SongId,
                StartTime = x.StartTime.Subtract(gapSpan),
                Title = x.Title,
                PublicId = x.PublicId
            });
        }

        public static bool IsSongCurrent(this SongQueueModel song)
        {
            return DateTime.UtcNow > song.StartTime && DateTime.UtcNow < song.EndTime;
        }
    }
}
