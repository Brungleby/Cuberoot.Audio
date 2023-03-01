
/** SoundPool.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#endregion

namespace Cuberoot
{
	/// <summary>
	/// An audio pool defines a list of sounds and acceptable pitch and volume ranges in order to play sounds randomly from the pool.
	///</summary>

	[CreateAssetMenu(fileName = "New Audio Pool", menuName = "Cuberoot/Audio/Audio Pool", order = 50)]
	public class AudioPool : ScriptableObject
	{
		/// <summary>
		/// An instance of a drawing of an Audio Pool.
		///</summary>

		public readonly struct Splash
		{
			public Splash(AudioClip _clip, float _volume = 1f, float _pitch = 1f)
			{
				clip = _clip;
				volume = _volume;
				pitch = _pitch;
			}

			public readonly AudioClip clip;
			public readonly float volume;
			public readonly float pitch;
		}

		#region Fields

		#region AudioClips

		/// <summary>
		/// The map of sounds and associated weights.
		///</summary>
		[Tooltip("The list of sounds which may be played.")]
		[SerializeField]

		private MapField<AudioClip, float> _AudioEntries = new MapField<AudioClip, float>();

		#endregion

		#region AudioClips

		/// <summary>
		/// AudioClips with selected weights associated.
		///</summary>

		private WeightedList<AudioClip> _AudioClips = new WeightedList<AudioClip>();

		/// <inheritdoc cref="_AudioClips"/>

		public WeightedList<AudioClip> AudioClips => _AudioClips;

		#endregion

		#region VolumeMin

		/// <summary>
		/// The minimum volume an <see cref="AudioClip"/> this <see cref="AudioPool"/> can play.
		///</summary>
		[Tooltip("The minimum volume this AudioPool can play.")]
		[Min(0f)]
		[SerializeField]

		private float _VolumeMin = 1f;

		/// <inheritdoc cref="_VolumeMin"/>

		public float VolumeMin
		{
			get => _VolumeMin;
			set
			{
				_VolumeMin = Mathf.Max(value, 0f);
				Math.ReassignMinMax(ref _VolumeMin, ref _VolumeMax);
			}
		}

		#endregion
		#region VolumeMax

		/// <summary>
		/// The maximum volume an <see cref="AudioClip"/> from this <see cref="AudioPool"/> can play.
		///</summary>
		[Tooltip("The maximum volume an AudioClip from this AudioPool can play.")]
		[Min(0f)]
		[SerializeField]

		private float _VolumeMax = 1f;

		/// <inheritdoc cref="_VolumeMax"/>

		public float VolumeMax
		{
			get => _VolumeMax;
			set
			{
				_VolumeMax = Mathf.Max(value, 0f);
				Math.ReassignMinMax(ref _VolumeMin, ref _VolumeMax);
			}
		}

		#endregion

		#region PitchMin

		/// <summary>
		/// The minimum pitch an <see cref="AudioClip"/> from this <see cref="AudioPool"/> can play.
		///</summary>
		[Tooltip("The minimum pitch an AudioClip from this AudioPool can play.")]
		[SerializeField]

		private float _PitchMin = 1f;

		/// <inheritdoc cref="_PitchMin"/>

		public float PitchMin
		{
			get => _PitchMin;
			set
			{
				_PitchMin = Mathf.Max(value, 0f);
				Math.ReassignMinMax(ref _PitchMin, ref _PitchMax);
			}
		}

		#endregion
		#region PitchMax

		/// <summary>
		/// The maximum pitch an <see cref="AudioClip"/> from this <see cref="AudioPool"/> can play.
		///</summary>
		[Tooltip("The maximum pitch an AudioClip from this AudioPool can play.")]
		[SerializeField]

		private float _PitchMax = 1f;

		/// <inheritdoc cref="_PitchMax"/>
		public float PitchMax
		{
			get => _PitchMax;
			set
			{
				_PitchMax = Mathf.Max(value, 0f);
				Math.ReassignMinMax(ref _PitchMin, ref _PitchMax);
			}
		}

		#endregion

		#region SelectionStyle

		/// <summary>
		/// Defines how <see cref="AudioClip"/>s will be drawn from this pool.
		///</summary>
		[Tooltip("Defines how AudioClips will be drawn from this pool.")]
		[SerializeField]

		private ESelectionStyle _SelectionStyle = ESelectionStyle.Smart;

		/// <inheritdoc cref="_SelectionStyle"/>

		public ESelectionStyle SelectionStyle
		{
			get => _SelectionStyle;
			set
			{
				_SelectionStyle = value;

				switch (value)
				{
					case ESelectionStyle.Random:
						_DrawAudioClip = DrawAudioClip_Random;
						break;
					case ESelectionStyle.RandomWeighted:
						_DrawAudioClip = DrawAudioClip_RandomWeighted;
						break;
					case ESelectionStyle.Smart:
						_DrawAudioClip = DrawAudioClip_Smart;
						break;
					case ESelectionStyle.SmartWeighted:
						_DrawAudioClip = DrawAudioClip_SmartWeighted;
						break;
					case ESelectionStyle.Shuffle:
						_DrawAudioClip = DrawAudioClip_Shuffle;
						break;
					case ESelectionStyle.Sequential:
						_DrawAudioClip = DrawAudioClip_Sequential;
						break;
					default:
						_DrawAudioClip = DrawAudioClip_PrimaryOnly;
						break;
				}
			}
		}

		#endregion
		#region RandomSeed

		/// <summary>
		/// Seed used to pick sounds at random. Set to 0 for a random seed.
		///</summary>
		[Tooltip("Seed used to pick sounds at random. Set to 0 for a random seed.")]
		[SerializeField]

		public int RandomSeed = 0;

		#endregion

		#endregion
		#region Members

		#region DrawAudioClip

		/// <summary>
		/// Calling this function will draw a new AudioClip based on the <see cref="SelectionStyle"/>.
		///</summary>

		private System.Func<AudioClip> _DrawAudioClip;

		#endregion
		#region JustDrawnClip

		/// <summary>
		/// This is the most recently drawn clip. It is used when evaluating <see cref="DrawAudioClip_Smart"/>, <see cref="DrawAudioClip_SmartWeighted"/>, and <see cref="DrawAudioClip_Shuffle"/>.
		///</summary>

		private AudioClip _justDrawnClip;

		#endregion
		#region ShuffleQueue

		/// <summary>
		/// This is a list of sounds to be played in order. It is used when evaluating <see cref="DrawAudioClip_Shuffle"/> and <see cref="DrawAudioClip_Sequential"/>.
		///</summary>

		private Queue<AudioClip> _playQueue;

		#endregion

		#endregion
		#region Properties
		#endregion
		#region Functions

		public static implicit operator AudioClip(AudioPool self) =>
			self.DrawAudioClip();

		#region Awake

		private void Awake()
		{
			_AudioClips = new WeightedList<AudioClip>();

			foreach (var entry in _AudioEntries)
				_AudioClips.Add((entry.Key, entry.Value));
		}


		#endregion

		#region PlayOneShotAudio

		/// <summary>
		/// Draws one AudioClip and plays it at the given <paramref name="position"/>. No AudioSource needed.
		///</summary>
		/// <remarks>
		/// Randomized pitch is not available when using this method.
		///</remarks>

		public void PlayOneShotAudio(Vector3 position)
		{
			var splash = CreateSplash();
			AudioSource.PlayClipAtPoint(splash.clip, position, splash.volume);
		}

		#endregion

		#region GetRandomVolume

		/// <returns>
		/// A random value between <see cref="VolumeMin"/> and <see cref="VolumeMax"/>.
		///</returns>

		public float GetRandomVolume() =>
			Math.Random(VolumeMin, VolumeMax);

		#endregion
		#region GetRandomPitch

		/// <returns>
		/// A random value between <see cref="PitchMin"/> and <see cref="PitchMax"/>.
		///</returns>

		public float GetRandomPitch() =>
			Math.Random(PitchMin, PitchMax);

		#endregion

		#region CreateSplash

		/// <summary>
		/// Draws an audio clip and creates a Splash instance.
		///</summary>

		public Splash CreateSplash() =>
			new Splash(DrawAudioClip(), GetRandomVolume(), GetRandomPitch());

		#endregion

		#region DrawAudioClip

		/// <inheritdoc cref="_DrawAudioClip"/>

		public AudioClip DrawAudioClip()
		{
			if (AudioClips.Count == 0)
				return null;
			if (AudioClips.Count == 1)
				return DrawAudioClip_PrimaryOnly();

			return _DrawAudioClip();
		}

		#endregion

		#region DrawAudioClip Submethods

		#region DrawAudioClip_Random

		/// <inheritdoc cref="ESelectionStyle.Random"/>

		private AudioClip DrawAudioClip_Random()
		{
			return AudioClips.Random_Unweighted(RandomSeed == 0 ? null : RandomSeed);
		}

		#endregion
		#region DrawAudioClip_RandomWeighted

		/// <inheritdoc cref="ESelectionStyle.RandomWeighted"/>

		private AudioClip DrawAudioClip_RandomWeighted()
		{
			return AudioClips.Random(RandomSeed == 0 ? null : RandomSeed);
		}

		#endregion
		#region DrawAudioClip_Smart

		/// <inheritdoc cref="ESelectionStyle.Smart"/>

		private AudioClip DrawAudioClip_Smart()
		{
			AudioClip result; do
				result = DrawAudioClip_Random();
			while (result == _justDrawnClip);

			return result;
		}

		#endregion
		#region DrawAudioClip_SmartWeighted

		/// <inheritdoc cref="ESelectionStyle.SmartWeighted"/>

		private AudioClip DrawAudioClip_SmartWeighted()
		{
			AudioClip result; do
				result = DrawAudioClip_RandomWeighted();
			while (result == _justDrawnClip);

			return result;
		}

		#endregion
		#region DrawAudioClip_Shuffle

		/// <inheritdoc cref="ESelectionStyle.Shuffle"/>

		private AudioClip DrawAudioClip_Shuffle()
		{
			AudioClip result = _playQueue.Dequeue();

			if (_playQueue.Count == 0)
				ShufflePlayQueue();

			return result;
		}

		#endregion
		#region DrawAudioClip_Sequential

		/// <inheritdoc cref="ESelectionStyle.Sequential"/>

		private AudioClip DrawAudioClip_Sequential()
		{
			AudioClip result = _playQueue.Dequeue();

			if (_playQueue.Count == 0)
				ResequencePlayQueue();

			return result;
		}

		#endregion
		#region DrawAudioClip_PrimaryOnly

		/// <inheritdoc cref="ESelectionStyle.PrimaryOnly"/>

		private AudioClip DrawAudioClip_PrimaryOnly()
		{
			return AudioClips.items[0];
		}

		#endregion

		#endregion

		#region ShufflePlayQueue

		/// <summary>
		/// Shuffles the play queue into a random order.
		///</summary>

		public void ShufflePlayQueue()
		{
			_playQueue.Clear();

			var roster = new List<AudioClip>();
			roster.AddRange(AudioClips.items);

			for (int i = 0; i < AudioClips.Count; i++)
			{
				AudioClip selection; do
					selection = roster[Random.Range(0, roster.Count - 1)];
				while (selection == _justDrawnClip || i > 0);

				_playQueue.Enqueue(selection);
				roster.Remove(selection);
			}
		}

		#endregion
		#region ReorderPlayQueue

		/// <summary>
		/// Clears and the orders the play queue into the default order.
		///</summary>

		public void ResequencePlayQueue()
		{
			_playQueue.Clear();

			foreach (var clip in AudioClips)
			{
				_playQueue.Enqueue(clip.Item1);
			}
		}

		#endregion

		#endregion
	}

	#region (enum) ESelectionStyle

	/// <summary>
	/// Determines how audio clips will be drawn from an <see cref="AudioPool"/>.
	///</summary>

	public enum ESelectionStyle
	{
		/// <summary>
		/// Selects an <see cref="AudioClip"/> completely at random.
		///</summary>
		[Tooltip("Selects an AudioClip completely at random.")]

		Random,

		/// <summary>
		/// Selects an <see cref="AudioClip"/> at random based on a given weight for each <see cref="AudioClip"/>.
		///</summary>
		[Tooltip("Selects an AudioClip at random based on a given weight for each AudioClip.")]

		RandomWeighted,

		/// <summary>
		/// Selects <see cref="AudioClip"/>s at random, but guarantees that the same <see cref="AudioClip"/> is never played twice in a row.
		///</summary>
		[Tooltip("Selects AudioClips at random, but guarantees that the same AudioClip is never played twice in a row.")]

		Smart,

		/// <summary>
		/// Selects <see cref="AudioClip"/>s at random based on a given weight for each <see cref="AudioClip"/>, and guarantees that the same <see cref="AudioClip"/> is never played twice in a row.
		///</summary>
		[Tooltip("Selects AudioClips at random based on a given weight for each AudioClip, and guarantees that the same AudioClip is never played twice in a row.")]

		SmartWeighted,

		/// <summary>
		/// Shuffles the <see cref="AudioClip"/> list once, then plays them in sequence. Reshuffles when all have been played. Also guarantees that the same <see cref="AudioClip"/> is never played twice in a row.
		///</summary>
		[Tooltip("Shuffles the AudioClip list once, then plays them in sequence. Reshuffles when all have been played. Also guarantees that the same AudioClip is never played twice in a row.")]

		Shuffle,

		/// <summary>
		/// Plays all <see cref="AudioClip"/>s in order from first to last.
		///</summary>
		[Tooltip("Plays all AudioClips in order from first to last.")]

		Sequential,

		/// <summary>
		/// Plays only the 0th <see cref="AudioClip"/> in the pool.
		///</summary>
		[Tooltip("Plays only the 0th AudioClip in the pool.")]

		PrimaryOnly,
	}

	#endregion
}
