using Godot;
using System;
using System.Collections.Generic;

namespace CrazyEaters.Managers
{
    public class AudioStreamManager : Node
    {
        [Export] public int qtdPlayers = 8;
        [Export] public string bus = "master";
        [Export] public Godot.Collections.Dictionary<string, AudioStream> sounds = new Godot.Collections.Dictionary<string, AudioStream>();

        private Queue<AudioStreamPlayer> available;
        private Queue<object> queue;

        public override void _Ready()
        {
            available = new Queue<AudioStreamPlayer>();
            queue = new Queue<object>();

            for (int i = 0; i < qtdPlayers; i++) {
                AudioStreamPlayer stream = new AudioStreamPlayer();
                AddChild(stream);
                available.Enqueue(stream);
                stream.Connect("finished", this, nameof(OnStreamFinished), new Godot.Collections.Array() { stream });
                stream.Bus = bus;
            }
        }

        public void OnStreamFinished(AudioStreamPlayer stream)
        {
            available.Enqueue(stream);
        }

        public void Play(string soundName)
        {
            AudioStream sound;
            sounds.TryGetValue(soundName, out sound);
            queue.Enqueue(sound);
        }

        public void PlayResource(string soundPath)
        {
            queue.Enqueue(soundPath);
        }

        public override void _Process(float delta)
        {
            if (queue.Count > 0 && available.Count > 0) {
                AudioStreamPlayer player = available.Dequeue();
                object stream = queue.Dequeue();
                player.Stream = (stream is string) ? ResourceLoader.Load<AudioStream>((string) stream) : (AudioStream) stream;
                player.Play();
            }
        }

    }
}