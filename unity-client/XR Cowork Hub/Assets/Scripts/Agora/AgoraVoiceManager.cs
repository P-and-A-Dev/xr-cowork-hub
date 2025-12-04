using UnityEngine;
using Agora.Rtc;
using System.Collections.Generic;

public class AgoraVoiceManager : MonoBehaviour
{
    public string APP_ID = "5f8caf34e3194321b240595670769c32";
    public string roomId = "test-room";
    public string userId;

    private IRtcEngine mRtc;
    public uint agoraUid;

    public ParticipantManager participantManager;
    private int currentVoiceGroup = 0;
    private bool isSelfMuted = false;

    void Start()
    {
        
        userId = PlayerPrefs.GetString("userId", "user-" + Random.Range(0, 99999));
        agoraUid = (uint)Mathf.Abs(userId.GetHashCode());

        InitializeAgora();
        JoinRoom();

        participantManager.OnParticipantsUpdated += OnParticipantsUpdated;
    }

    void InitializeAgora()
    {
        mRtc = RtcEngine.CreateAgoraRtcEngine();

        // SDK 4.2.2 
        RtcEngineContext context = new RtcEngineContext(
            APP_ID,
            0,  // context
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

    void JoinRoom()
    {
        Debug.Log("Join in the voice room: " + roomId);

        ChannelMediaOptions options = new ChannelMediaOptions();

        
        options.autoSubscribeAudio.SetValue(true);

        int result = mRtc.JoinChannel(
            "",
            roomId,
            agoraUid,
            options
        );

        Debug.Log("JoinChannel -> " + result);
    }

    void OnParticipantsUpdated(List<Participant> participants)
    {
        foreach (var p in participants)
        {
            if (p.userId == userId)
                currentVoiceGroup = (int)p.voiceGroupId;
        }

        ApplyVoiceRules(participants);
    }

    void ApplyVoiceRules(List<Participant> participants)
    {
        foreach (var p in participants)
        {
            if (p.userId == userId) continue;

            uint remoteUid = (uint)Mathf.Abs(p.userId.GetHashCode());
            bool sameGroup = (int)p.voiceGroupId == currentVoiceGroup;

            mRtc.MuteRemoteAudioStream(remoteUid, !sameGroup);

            Debug.Log((sameGroup ? "unmuted" : "mutated") + p.userId);
        }
    }

    public void ToggleSelfMute()
    {
        isSelfMuted = !isSelfMuted;
        mRtc.MuteLocalAudioStream(isSelfMuted);

        Debug.Log(isSelfMuted ? "unmuted" : "ON");
    }
}
