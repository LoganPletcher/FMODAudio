using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


class Program
{
    static void Main(string[] args)
    {
        MusicPlayer.Init();
        MusicPlayer.Instance.Play(MusicPlayer.JAGUAR);
        Console.Beep();
        MusicPlayer.Instance.Unload();
    }
}

    public class MusicPlayer
    {
        public const int NUM_SONGS = 2;

        public const int JAGUAR = 1;

        private FMOD.System FMODSystem;
        private FMOD.Channel Channel;
        private FMOD.Sound[] Songs;

        private static MusicPlayer _instance;

        public static MusicPlayer Instance { get { return _instance; } }

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        public static void Init()
        {
            if (Environment.Is64BitProcess)
                LoadLibrary(System.IO.Path.GetFullPath("FMOD\\64\\fmod.dll"));
            else
                LoadLibrary(System.IO.Path.GetFullPath("FMOD\\32\\fmod.dll"));

            _instance = new MusicPlayer();
        }

        public void Unload()
        {
            FMODSystem.release();
        }

        private MusicPlayer()
        {
            FMOD.Factory.System_Create(out FMODSystem);

            FMODSystem.setDSPBufferSize(1024, 10);
            FMODSystem.init(32, FMOD.INITFLAGS.NORMAL, (IntPtr)0);

            Songs = new FMOD.Sound[NUM_SONGS];

            LoadSong(JAGUAR, "jaguar");
        }

        private void LoadSong(int songId, string name)
        {
        //Figure out directory path.
            FMOD.RESULT r = FMODSystem.createStream("Content/Music/" + name + ".flac", FMOD.MODE.DEFAULT, out Songs[songId]);
            Console.WriteLine("loading " + songId + ", got result " + r);
        }

        private int _current_song_id;

        public bool IsPlaying()
        {
            bool isPlaying = false;

            if (Channel != null)
                Channel.isPlaying(out isPlaying);

            return isPlaying;
        }

        public void Play(int songId)
        {
            Console.WriteLine("Play(" + songId + ")");

            if (_current_song_id != songId)
            {
                Stop();

                if (songId >= 0 && songId < NUM_SONGS && Songs[songId] != null)
                {
                    FMODSystem.playSound(Songs[songId], null, false, out Channel);
                    //UpdateVolume();
                    Channel.setMode(FMOD.MODE.LOOP_NORMAL);
                    Channel.setLoopCount(-1);

                    _current_song_id = songId;
                }
            }
        }

        public void UpdateVolume()
        {
            if (Channel != null) { }
                //Channel.setVolume(Settings.GetInstance().MusicVolume / 100f);
        }

        public void Stop()
        {
            if (IsPlaying())
                Channel.stop();

            _current_song_id = -1;
        }
    }
