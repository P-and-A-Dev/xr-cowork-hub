using UnityEngine;
using Agora.Rtc;
using System.Collections.Generic;

/// <summary>
/// Controls Agora real-time voice, assigns users to voice groups,
/// mutes/unmutes remote users based on bubble groups,
/// and connects the local user to the audio channel.
/// </summary>
public class AgoraVoiceManager : MonoBehaviour
{
    [SerializeField] private string APP_ID = "5f8caf34e3194321b240595670769c32";
    [SerializeField] private string roomId = "main-room";
    public string userId;

    private IRtcEngine mRtc;
    public uint agoraUid;

    public ParticipantManager participantManager;
    private int currentVoiceGroup = 0;
    private bool isSelfMuted = false;

    /// <summary>
    /// Initializes Agora and joins the voice channel.
    /// Also subscribes to participant updates.
    /// </summary>
    void Start()
    {
        userId = PlayerPrefs.GetString("userId", "user-" + Random.Range(0, 99999));
        agoraUid = (uint)Mathf.Abs(userId.GetHashCode());

        InitializeAgora();
        JoinRoom();

        participantManager.OnParticipantsUpdated += OnParticipantsUpdated;
    }

    /// <summary>
    /// Initializes the Agora RTC engine and audio configuration.
    /// </summary>
    void InitializeAgora()
    {
        mRtc = RtcEngine.CreateAgoraRtcEngine();

        RtcEngineContext context = new RtcEngineContext(
            APP_ID,
            0,
            CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION,
            AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT,
            AREA_CODE.AREA_CODE_GLOB,
            new LogConfig("agora.log"),
            "",
            false
        );

        mRtc.Initialize(context);
        mRtc.EnableAudio();
    }

    /// <summary>
    /// Joins the Agora audio room based on roomId.
    /// </summary>
    void JoinRoom()
    {
        Debug.Log("Join in the voice room: " + roomId);

        ChannelMediaOptions options = new ChannelMediaOptions();
        options.autoSubscribeAudio.SetValue(true);

        int result = mRtc.JoinChannel("", roomId, agoraUid, options);

        Debug.Log("JoinChannel -> " + result);
    }

    /// <summary>
    /// Updates the current user's assigned voice group
    /// and triggers audio isolation rules.
    /// </summary>
    void OnParticipantsUpdated(List<Participant> participants)
    {
        foreach (var p in participants)
            if (p.userId == userId)
                currentVoiceGroup = (int)p.voiceGroupId;

        ApplyVoiceRules(participants);
    }

    /// <summary>
    /// Mutes or unmutes remote users depending on whether
    /// they share the same voiceGroupId as the local user.
    /// </summary>
    void ApplyVoiceRules(List<Participant> participants)
    {
        foreach (var p in participants)
        {
            if (p.userId == userId) continue;

            uint remoteUid = (uint)Mathf.Abs(p.userId.GetHashCode());
            bool sameGroup = (int)p.voiceGroupId == currentVoiceGroup;

            mRtc.MuteRemoteAudioStream(remoteUid, !sameGroup);
        }
    }

    /// <summary>
    /// Toggles local microphone mute state.
    /// </summary>
    public void ToggleSelfMute()
    {
        isSelfMuted = !isSelfMuted;
        mRtc.MuteLocalAudioStream(isSelfMuted);
    }
}
