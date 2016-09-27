using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Patterns
{
    public class CachingAudioController : IAudioController
    {
        private readonly IAudioController _audioController;
        private readonly ConcurrentDictionary<string, object> _cache =  new ConcurrentDictionary<string, object>();

        public CachingAudioController(IAudioController audioController)
        {
            _audioController = audioController;
        }

        public Task<Song> GetSongById(int id)
        {
            var cacheKey = $"Id:{id}";
            return GetValue(cacheKey, () => _audioController.GetSongById(id));
        }

        public Task<Song[]> FindSongsByName(string name)
        {
            var cacheKey = $"SongName:{name}";
            return GetValue(cacheKey, () => _audioController.FindSongsByName(name));
        }

        public Task<Song[]> FindArtistSongs(string artist)
        {
            var cacheKey = $"ArtistSongs:{artist}";
            return GetValue(cacheKey, () => _audioController.FindArtistSongs(artist));
        }

        public async Task EditSong(Song song, string token)
        {
            await _audioController.EditSong(song, token);
            _cache.Clear();
        }

        private async Task<T> GetValue<T>(string cacheKey, Func<Task<T>> fallback)
        {
            object value;
            var fromCache = _cache.TryGetValue(cacheKey, out value);
            if (fromCache)
            {
                return (T)value;
            }

            value = await fallback();
            _cache.AddOrUpdate(cacheKey, value, (s, o) => value);
            return (T)value;
        }
     }

    public class RemoteAudioController : IAudioController
    {
        private readonly Uri _host;
        public RemoteAudioController(Dictionary<string, string> settings)
        {
            _host = new Uri(settings["Host"]);
        }

        public async Task<Song> GetSongById(int id)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_host, $"songs/{id}"));
                using (var response = await httpClient.SendAsync(request))
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    return Deserialize<Song>(content);
                }
            }
        }

        public async Task<Song[]> FindSongsByName(string name)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_host, $"songsfinder/{name}"));
                using (var response = await httpClient.SendAsync(request))
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    return Deserialize<Song[]>(content);
                }
            }
        }

        public async Task<Song[]> FindArtistSongs(string artist)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_host, $"artist/{artist}/songs"));
                using (var response = await httpClient.SendAsync(request))
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    return Deserialize<Song[]>(content);
                }
            }
        }

        public async Task EditSong(Song song, string token)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Put, new Uri(_host, $"songs/{song.Id}"))
                {
                    Content = new ByteArrayContent(Serialize(song))
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await httpClient.SendAsync(request);
                response.Dispose();
            }
        }

        private static byte[] Serialize<T>(T input)
        {
            throw new NotImplementedException();
        }
        private static T Deserialize<T>(byte[] input)
        {
            throw new NotImplementedException();
        }
    }

    public interface IAudioController
    {
        Task<Song> GetSongById(int id);

        Task<Song[]> FindSongsByName(string name);

        Task<Song[]> FindArtistSongs(string artist);

        Task EditSong(Song song, string token);
    }

    public class Song
    {
        public Song(int id, string name, string artist, TimeSpan duration)
        {
            Id = id;
            Name = name;
            Artist = artist;
            Duration = duration;
        }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public string Artist { get; private set; }

        public TimeSpan Duration { get; private set; }
    }
}