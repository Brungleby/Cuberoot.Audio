
/** AudioPoolFilter.cs
*
*	Created by LIAM WOFFORD of CUBEROOT SOFTWARE, LLC.
*
*	Free to use or modify, with or without creditation,
*	under the Creative Commons 0 License.
*/

#region Includes

using UnityEngine;

#endregion

namespace Cuberoot
{
	/// <summary>
	/// A component that plays a specific AudioPool.
	///</summary>

	public sealed class AudioPoolFilter : MonoBehaviour
	{
		#region Fields

		#region (field) Source

		/// <summary>
		/// The AudioSource associated with this AudioPoolFilter. If not specified, only one-shot audios will play.
		///</summary>

		[Tooltip("The AudioSource associated with this AudioPoolFilter. If not specified, only one-shot audios will play.")]
		[SerializeField]

		private AudioSource _Source;

		/// <inheritdoc cref="_Source"/>

		public AudioSource Source
		{
			get => _Source;
			set
			{
				_Source = value;

				if (value == null)
					_PlayAction = PlayAtTransform;
				else
					_PlayAction = PlayAtSource;
			}
		}

		#endregion
		#region (field) Pool

		/// <summary>
		/// The selected <see cref="AudioPool"/> from which to draw <see cref="AudioClip"/>s.
		///</summary>

		[Tooltip("The selected pool from which to draw AudioClips.")]
		[SerializeField]

		public AudioPool Pool;

		#endregion

		#endregion
		#region Members

		#region PlayAction

		/// <summary>
		/// This function defines which method to use to play the sound.
		///</summary>

		private System.Action _PlayAction;

		#endregion

		#endregion

		#region Properties
		#endregion
		#region Functions

		#region Awake

		private void Awake()
		{
			Source = _Source;
		}

		#endregion
		#region Play

		/// <summary>
		/// Plays this component's assigned audio pool.
		///</summary>

		public void Play() => _PlayAction();

		/// <summary>
		/// Plays this component's assigned audio pool from <see cref="Source"/>.
		///</summary>

		private void PlayAtSource() => Source.PlayAudioPool(Pool);

		/// <summary>
		/// Plays this component's assigned audio pool from <see cref="transform.position"/>.
		///</summary>

		private void PlayAtTransform() => Pool.PlayOneShotAudio(transform.position);

		#endregion

		#endregion
	}


	public static class AudioPoolExtension
	{
		/// <summary>
		/// Plays an AudioPool from the given <paramref name="source"/>.
		///</summary>

		public static void PlayAudioPool(this AudioSource source, AudioPool pool)
		{
			var __splash = pool.CreateSplash();

			source.clip = __splash.clip;
			source.volume = __splash.volume;
			source.pitch = __splash.pitch;

			source.Play();
		}
	}
}
