﻿using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Contains the wrapper methods to access the API of the Gamfication Framework
/// </summary>
public class GamificationFramework : Singleton<GamificationFramework>
{

    // ---------------------------------------------------------------
    // Games
    // ---------------------------------------------------------------

    /// <summary>
    /// Create a new game
    /// </summary>
    /// <param name="game">The game data which are passed on to the Gamification Framework</param>
    public void CreateGame(Game game, Action<Game, long> callback)
    {
        if (game.ID == "")
        {
            Debug.LogWarning("Tried to create a game without an id");
            return;
        }

        List<IMultipartFormSection> body = game.ToMultipartFormData();
        RestManager.Instance.POST(InformationManager.Instance.FullGamificationAddress + "/gamification/games/data", body, reqRes =>
        {
            if (callback != null)
            {
                if (reqRes.responseCode == 200 || reqRes.responseCode == 201)
                {
                    callback(game, reqRes.responseCode);
                }
                else

                {
                    callback(null, reqRes.responseCode);
                }
            }
        });
    }

    /// <summary>
    /// Gets the details about a specific game. The result is passed to the callback method.
    /// </summary>
    /// <param name="gameId">The game id of the game</param>
    /// <param name="callWithResult">This method will be called with the resulting game object and the response code of the request</param>
    public void GetGameDetails(string gameId, Action<Game, long> callWithResult)
    {
        if (callWithResult != null)
        {
            RestManager.Instance.GET(InformationManager.Instance.FullGamificationAddress + "/gamification/games/data/" + gameId, reqRes =>
            {
                Game game = null;
                if (reqRes.responseCode == 200)
                {
                    game = Game.FromJson(reqRes.downloadHandler.text);
                }
                if (callWithResult != null)
                {
                    callWithResult(game, reqRes.responseCode);
                }
            });
        }
        else
        {
            Debug.LogWarning("Getting game details without providing callWithResult makes no sense");
        }
    }

    public void DeleteGame(string gameId)
    {
        RestManager.Instance.DELETE(InformationManager.Instance.FullGamificationAddress + "/gamification/games/data/" + gameId, OperationFinished);
    }

    public void AddUserToGame(string gameId, Action<long> callback)
    {
        RestManager.Instance.POST(InformationManager.Instance.FullGamificationAddress + "/gamification/games/data/" + gameId + "/" + InformationManager.Instance.UserInfo.preferred_username,
            reqRes =>
            {
                if (callback != null)
                {
                    callback(reqRes.responseCode);
                }
            }
            );
    }

    public void RemoveUserFromGame(string gameId, Action<long> callback)
    {
        RestManager.Instance.DELETE(InformationManager.Instance.FullGamificationAddress + "/gamification/games/data" + gameId + "/" + InformationManager.Instance.UserInfo.preferred_username,
            reqRes =>
            {
                if (callback != null)
                {
                    callback(reqRes.responseCode);
                }
            }
            );
    }

    public void GetSeparateGameInfos(Action<JsonGameWithUserInfo[], long> callback)
    {
        RestManager.Instance.GET(InformationManager.Instance.FullGamificationAddress + "/gamification/games/list/separated",
            req =>
            {
                if (callback != null)
                {
                    if (req.responseCode == 200)
                    {
                        string jsonAnswer = req.downloadHandler.text;
                        // the json needs to be modified in order to work with JsonUtility
                        jsonAnswer = "{\"array\":" + jsonAnswer + "}";
                        JsonUserGameArray array = JsonUtility.FromJson<JsonUserGameArray>(jsonAnswer);
                        callback(array.array, req.responseCode);
                    }
                    else
                    {
                        callback(null, req.responseCode);
                    }
                }
            }
            );
    }

    /// <summary>
    /// Called when the GetGameDetails operation is finished. Checks if the request was successful and invokes the secondary callback.
    /// </summary>
    /// <param name="result">The result of the web request</param>
    /// <param name="args">Additional arguments which have been passed through the request. This should contain a secondary callback method.</param>
    private void ConvertGameDetailsToGame(UnityWebRequest result, object[] args)
    {
        Game game = null;
        if (result.responseCode == 200)
        {
            game = Game.FromJson(result.downloadHandler.text);
        }
        Action<Game> secondaryCallback = ((Action<Game>)args[0]);
        if (secondaryCallback != null)
        {
            secondaryCallback(game);
        }
    }

    /// <summary>
    /// Validates the login credentials of the user
    /// </summary>
    /// <param name="callback">Method which gets called when the request is finished. The parameter of the method will contain the request result.</param>
    public void ValidateLogin(Action<UnityWebRequest> callback)
    {
        RestManager.Instance.POST(InformationManager.Instance.FullGamificationAddress + "/gamification/games/validation", callback);
    }

    // ---------------------------------------------------------------
    // Quests
    // ---------------------------------------------------------------

    /// <summary>
    /// Creates a new quest
    /// </summary>
    /// <param name="gameId">The game id of the game that should contain the quest</param>
    /// <param name="quest">The quest data to pass to the Gamification Framework</param>
    public void CreateQuest(string gameId, Quest quest, Action<Quest, long> callback)
    {
        RestManager.Instance.POST(InformationManager.Instance.FullGamificationAddress + "/gamification/quests/" + gameId, quest.ToJson(),
            reqRes =>
            {
                if (reqRes.responseCode == 201)
                {
                    Debug.Log("Created Quest " + quest.ID + " (" + gameId + ")");
                }

                if (callback != null)
                {
                    callback(quest, reqRes.responseCode);
                }
            }
            );
    }

    public void UpdateQuest(string gameId, Quest quest)
    {
        RestManager.Instance.PUT(InformationManager.Instance.FullGamificationAddress + "/gamification/quests/" + gameId + "/" + quest.ID, quest.ToJson(), OperationFinished);
    }

    public void GetOrCreateQuest(string gameId, Quest quest, Action<Quest> callback)
    {
        GetQuestWithId(gameId, quest.ID,
            (resQuest, resCode) =>
            {
                if (resCode == 200)
                {
                    if (callback != null)
                    {
                        callback(resQuest);
                    }
                }
                else
                {
                    CreateQuest(gameId, quest,
                        (createdQuest, createCode) =>
                        {
                            if (createCode == 200 || createCode == 201)
                            {
                                if (callback != null)
                                {
                                    callback(createdQuest);
                                }
                            }
                            else
                            {
                                callback(null);
                            }
                        }
                        );
                }
            }
            );
    }

    public void GetQuestWithId(string gameId, string questId, Action<Quest, long> callback)
    {
        RestManager.Instance.GET(InformationManager.Instance.FullGamificationAddress + "/gamification/quests/" + gameId + "/" + questId,
            reqRes =>
            {
                if (callback != null)
                {
                    if (reqRes.responseCode == 200)
                    {
                        Quest quest = Quest.FromJson(reqRes.downloadHandler.text);
                        if (quest != null)
                        {
                            Debug.Log("Sucessfully loaded quest " + quest.ID);
                        }
                        callback(quest, reqRes.responseCode);
                    }
                    else
                    {
                        callback(null, reqRes.responseCode);
                    }
                }
            }
            );
    }

    // ---------------------------------------------------------------
    // Achievements
    // ---------------------------------------------------------------

    public void CreateAchievement(string gameId, Achievement achievement, Action<Achievement, long> callback)
    {
        List<IMultipartFormSection> body = achievement.ToMultipartFormData();
        RestManager.Instance.POST(InformationManager.Instance.FullGamificationAddress + "/gamification/achievements/" + gameId, body,
            reqRes =>
            {
                if (callback != null)
                {
                    callback(achievement, reqRes.responseCode);
                }
            }
            );
    }

    public void UpdateAchievement(string gameId, Achievement achievement, Action<Achievement, long> callback)
    {
        List<IMultipartFormSection> body = achievement.ToMultipartFormData();
        RestManager.Instance.PUT(InformationManager.Instance.FullGamificationAddress + "/gamification/achievements/" + gameId + "/" + achievement.ID, body,
            reqRes =>
            {
                if (callback != null)
                {
                    callback(achievement, reqRes.responseCode);
                }
            }
            );
    }

    public void GetAchievementWithId(string gameId, string achievementId, Action<Achievement, long> callback)
    {
        RestManager.Instance.GET(InformationManager.Instance.FullGamificationAddress + "/gamification/achievements/" + gameId + "/" + achievementId,
            reqRes =>
            {
                if (callback != null)
                {
                    if (reqRes.responseCode == 200)
                    {
                        Achievement resAchievement = Achievement.FromJson(reqRes.downloadHandler.text);
                        callback(resAchievement, reqRes.responseCode);
                    }
                    else
                    {
                        callback(null, reqRes.responseCode);
                    }
                }
            }
            );
    }

    public void GetOrCreateAchievement(string gameId, Achievement achievement, Action<Achievement> callback)
    {
        GetAchievementWithId(gameId, achievement.ID,
            (resAchievement, resCode) =>
            {
                if (resCode == 200)
                {
                    if (callback != null)
                    {
                        callback(resAchievement);
                    }
                }
                else
                {
                    CreateAchievement(gameId, achievement,
                        (createdAchievement, createCode) =>
                        {
                            if (createCode == 200 || createCode == 201)
                            {
                                if (callback != null)
                                {
                                    callback(createdAchievement);
                                }
                            }
                            else
                            {
                                callback(null);
                            }
                        }
                        );
                }
            }
            );
    }

    // ---------------------------------------------------------------
    // Actions
    // ---------------------------------------------------------------

    public void CreateAction(string gameId, GameAction action, Action<long> callback)
    {
        List<IMultipartFormSection> body = action.ToMultipartFormData();
        RestManager.Instance.POST(InformationManager.Instance.FullGamificationAddress + "/gamification/actions/" + gameId, body,
            reqRes =>
            {
                if (reqRes.responseCode != 201)
                {
                    Debug.Log("Action creation error (" + reqRes.responseCode + ") " + reqRes.downloadHandler.text);
                }
                if (callback != null)
                {
                    callback(reqRes.responseCode);
                }
            }
            );
    }

    public void UpdateAction(string gameId, GameAction action, Action<long> callback)
    {
        List<IMultipartFormSection> body = action.ToMultipartFormData();
        RestManager.Instance.PUT(InformationManager.Instance.FullGamificationAddress + "/gamification/actions/" + gameId + "/" + action.ID, body,
            reqRes =>
            {
                if (callback != null)
                {
                    callback(reqRes.responseCode);
                }
            }
            );
    }

    public void DeleteAction(string gameId, string actionId, Action<long> callback)
    {
        RestManager.Instance.DELETE(InformationManager.Instance.FullGamificationAddress + "/gamification/actions/" + gameId + "/" + actionId,
            reqRes =>
            {
                if (callback != null)
                {
                    callback(reqRes.responseCode);
                }
            }
            );
    }

    public void TriggerAction(string gameId, string actionId)
    {
        RestManager.Instance.POST(InformationManager.Instance.FullGamificationAddress
            + "/visualization/actions/" + gameId + "/" + actionId + "/" + InformationManager.Instance.UserInfo.preferred_username,
            OperationFinished);
    }

    // ---------------------------------------------------------------
    // Badges
    // ---------------------------------------------------------------

    public void CreateBadge(string gameId, Badge badge, Action<long> callback)
    {
        List<IMultipartFormSection> body = badge.ToMultipartForm();

        RestManager.Instance.POST(InformationManager.Instance.FullGamificationAddress + "/gamification/badges/" + gameId, body,
            reqRes =>
            {
                if (callback != null)
                {
                    callback(reqRes.responseCode);
                }
            }
            );
    }

    public void GetBadgeWithId(string gameId, string badgeId, Action<Badge, long> callback)
    {
        RestManager.Instance.GET(InformationManager.Instance.FullGamificationAddress + "/gamification/badges/" + gameId + "/" + badgeId,
            reqRes =>
            {
                if (callback != null)
                {
                    if (reqRes.responseCode == 200)
                    {
                        Badge res = Badge.FromJson(reqRes.downloadHandler.text);
                        callback(res, reqRes.responseCode);
                    }
                    else
                    {
                        callback(null, reqRes.responseCode);
                    }
                }
            }
            );
    }

    public void GetBadgeImage(string gameId, string badgeId, Action<Texture, long> callback)
    {
        RestManager.Instance.GetTexture(InformationManager.Instance.FullGamificationAddress + "/gamification/badges/" + gameId + "/" + badgeId + "/img",
            (reqRes, texture) =>
            {
                if (callback != null)
                {
                    callback(texture, reqRes.responseCode);
                }
            }
            );
    }

    public void GetBadgesOfUser(string gameId, Action<Badge[], long> callback)
    {
        RestManager.Instance.GET(InformationManager.Instance.FullGamificationAddress + "/visualization/badges/" + gameId + "/" + InformationManager.Instance.UserInfo.preferred_username,
            reqRes =>
            {
                if (callback != null)
                {
                    if (reqRes.responseCode == 200)
                    {
                        string jsonResponse = reqRes.downloadHandler.text;
                        jsonResponse = "{\"array\":" + jsonResponse + "}"; // json array needs to be packed into an array object in order to work with JsonUtility
                        JsonBadgeArray array = JsonUtility.FromJson<JsonBadgeArray>(jsonResponse);
                        Badge[] badgeArray = new Badge[array.array.Length];
                        for (int i = 0; i < array.array.Length; i++)
                        {
                            badgeArray[i] = Badge.FromJsonBadge(array.array[i]);
                        }
                        callback(badgeArray, reqRes.responseCode);
                    }
                    else
                    {
                        callback(null, reqRes.responseCode);
                    }
                }
            }
            );
    }

    public void GetAllBadgesOfUser(Action<List<KeyValuePair<string, Badge>>, long> callback)
    {
        List<KeyValuePair<string, Badge>> badgesFromAllGames = new List<KeyValuePair<string,Badge>>();
        int countLoaded = 0;
        int countToLoad;
        GetSeparateGameInfos(
            (games, resCode) =>
            {
                if (resCode == 200)
                {
                    Debug.Log("Game length: " + games.Length);
                    countToLoad = games.Length;

                    for (int i = 0; i < games.Length; i++)
                    {
                        int indexForLambda = i;
                        // only look at this game if the member is actually added to the game
                        if (games[i].memberHas)
                        {
                            // get the badges of the game
                            GetBadgesOfUser(games[i].game_id,
                                (badgeArray, badgesCode) =>
                                {
                                    if (badgesCode == 200)
                                    {
                                        foreach (Badge b in badgeArray)
                                        {
                                            badgesFromAllGames.Add(new KeyValuePair<string, Badge>(games[indexForLambda].game_id, b));
                                        }

                                        countLoaded++;
                                        Debug.Log("Loaded: " + countLoaded);

                                        if (countLoaded >= countToLoad)
                                        {
                                            if (callback != null)
                                            {
                                                callback(badgesFromAllGames, resCode);
                                            }
                                        }

                                        Debug.Log(games[indexForLambda].game_id + ":" + indexForLambda);
                                        Debug.Log(badgesFromAllGames.Count);
                                    }
                                    else
                                    {
                                        if (callback != null)
                                        {
                                            callback(null, badgesCode);
                                        }
                                    }
                                }
                                );
                        }
                        else
                        {
                            countToLoad--;
                        }
                    }
                }
                else
                {
                    callback(null, resCode);
                }
            }
            );
    }

    // ---------------------------------------------------------------
    // Points
    // ---------------------------------------------------------------

    public void ChangePointUnitName(string gameId, string newUnitName)
    {
        RestManager.Instance.PUT(InformationManager.Instance.FullGamificationAddress + "/gamification/points/" + gameId + "/name/" + newUnitName, OperationFinished);
    }

    public void GetPointUnitName(string gameId, Action<string> callWithResult)
    {
        object[] passOnArgs = { callWithResult };
        RestManager.Instance.GET(InformationManager.Instance.FullGamificationAddress + "/gamification/points/" + gameId + "/name", EvaluateGetPointUnitName, passOnArgs);
    }

    private void EvaluateGetPointUnitName(UnityWebRequest req, object[] passOnArgs)
    {
        if (req.responseCode != 200)
        {
            Debug.Log("Error code for request: " + req.responseCode + " " + req.error);
            Debug.Log("Error: " + req.downloadHandler.text);
        }
        else
        {
            if (passOnArgs != null && passOnArgs.Length > 0)
            {
                JsonPointUnit unit = JsonUtility.FromJson<JsonPointUnit>(req.downloadHandler.text);
                ((Action<string>)passOnArgs[0])(unit.pointUnitName);
            }
        }
    }

    // -------------------------------------------------------------------------------
    // Helper methods

    private void OperationFinished(UnityWebRequest req)
    {
        if (req.responseCode != 200 && req.responseCode != 201)
        {
            Debug.Log("Error code for request: " + req.responseCode + " " + req.error);
            Debug.Log("Error: " + req.downloadHandler.text);
        }
        else
        {
            Debug.Log("Success");
            Debug.Log(req.downloadHandler.text);
        }
    }

    private void Start()
    {
        // for testing:
    }

    private void Result(string obj)
    {
        Debug.Log(obj);
    }
}
