using UnityEngine;
using GJW;

namespace naichilab {
	public class TweetSample : MonoBehaviour {
		public void Tweet() {
			UnityRoomTweet.Tweet( "AAA", "BBB" );
		}

		public void TweetWithHashtag() {
			UnityRoomTweet.Tweet( "unityroom-tweet-sample", "ツイートサンプルです。", "unityroom" );
		}

		public void TweetWithHashtags() {
			int score = GameController.instance.GetScore();
			UnityRoomTweet.Tweet( "select10", $"{score}点取りました!!", "unityroom", "unity1week" );
		}

		public void TweetWithImage() {
			UnityRoomTweet.TweetWithImage( "unityroom-tweet-sample", "ツイートサンプルです。" );
		}

		public void TweetWithHashtagAndImage() {
			UnityRoomTweet.TweetWithImage( "unityroom-tweet-sample", "ツイートサンプルです。", "unityroom" );
		}

		public void TweetWithHashtagsAndImage() {
			UnityRoomTweet.TweetWithImage( "unityroom-tweet-sample", "ツイートサンプルです。", "unityroom", "unity1week" );
		}
	}
}