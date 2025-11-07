using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Dan.Main;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> names;
    [SerializeField] private List<TextMeshProUGUI> scores;

    private string publicLeaderboardKey = "7c747d096e509f819e824bd1aa68de949991458336e0a3baab44b1106899d560";

    private void Start()
    {
        GetLeaderBoard();
    }

    /// <summary>
    /// Call this method to retrieve and display the leaderboard
    /// </summary>
    public void GetLeaderBoard()
    {
        // Clear existing names and scores
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, ((msg) =>
        {
            int loopLength = (msg.Length < names.Count) ? msg.Length : names.Count; // Ensure we don't exceed the size of the UI lists

            // Populate names and scores
            for (int i = 0; i < loopLength; i++)
            {
                names[i].text = msg[i].Username;
                scores[i].text = msg[i].Score.ToString();
            }
        }));
    }

    /// <summary>
    /// Call this method to set a new leaderboard entry
    /// </summary>
    /// <param name="username"></param>
    /// <param name="score"></param>
    public void SetLeaderboardEntry(string username, int score)
    {
        // Upload new entry to the leaderboard
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, score, ((msg) =>
        {
            // Use first 10 characters of username for leaderboard display
            username = username.Length >= 10 ? username.Substring(0, 10) : username;
            GetLeaderBoard();
        }));

        // Reset player data after submission
        LeaderboardCreator.ResetPlayer();
    }
}
