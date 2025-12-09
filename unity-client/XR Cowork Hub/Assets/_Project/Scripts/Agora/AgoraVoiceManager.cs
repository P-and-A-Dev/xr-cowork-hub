using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Manager;
using Agora.Rtc;
using UnityEngine;

namespace _Project.Scripts.Agora
{
    public class AgoraVoiceManager : MonoBehaviour
    {
        private const string k_AppID = "2b9859da844a410ca12d0d20ba04260f";
        public string roomId = "test-room";
        public string userId;

        private IRtcEngine _mRtc;
        public uint agoraUid;

        [SerializeField] public ParticipantManager participantManager;
        private int _currentVoiceGroup;
        private bool _isSelfMuted;


        private void Start()
        {
            try
            {
                if (participantManager == null)
                {
                    Debug.LogError(
                        "[AgoraVoiceManager] Critical: ParticipantManager reference is missing! Please assign it in the Inspector.");
                    return;
                }

                userId = PlayerPrefs.GetString("userId");
                if (string.IsNullOrEmpty(userId))
                {
                    userId = Guid.NewGuid().ToString();
                    PlayerPrefs.SetString("userId", userId);
                }

                byte[] guidBytes = new Guid(userId).ToByteArray();
                agoraUid = BitConverter.ToUInt32(guidBytes, 0);

                // AGORA REACTIVATED
                InitializeAgora();
                JoinRoom();

                participantManager.OnParticipantsUpdated += OnParticipantsUpdated;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AgoraVoiceManager] Error in Start: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void InitializeAgora()
        {
            _mRtc = RtcEngine.CreateAgoraRtcEngine();

            // Use property-based initialization to avoid potential nulls in constructor arguments causing serialization errors
            RtcEngineContext context = new RtcEngineContext();
            context.appId = k_AppID;
            context.channelProfile = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION;
            context.audioScenario = AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT;
            context.areaCode = AREA_CODE.AREA_CODE_GLOB;
            context.logConfig = new LogConfig("agora.log", 2048, LOG_LEVEL.LOG_LEVEL_INFO);

            int initResult = _mRtc.Initialize(context);
            if (initResult != 0)
            {
                Debug.LogError($"[AgoraVoiceManager] Initialize failed with error: {initResult}");
                return;
            }

            _mRtc.InitEventHandler(new UserEventHandler());
            _mRtc.EnableAudio();
            
            Debug.Log("[AgoraVoiceManager] Initialized successfully.");
        }

        private void JoinRoom()
        {
            Debug.Log("[AgoraVoiceManager] Joining voice room: " + roomId);

            ChannelMediaOptions options = new ChannelMediaOptions();
            options.autoSubscribeAudio.SetValue(true);
            options.publishMicrophoneTrack.SetValue(true);
            options.clientRoleType.SetValue(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);

            int result = _mRtc.JoinChannel(
                "",
                roomId,
                agoraUid,
                options
            );

            Debug.Log("[AgoraVoiceManager] JoinChannel request sent. Result: " + result);
        }

        public void OnParticipantsUpdated(List<Participant> participants)
        {
            foreach (var p in participants.Where(p => p.userId == userId))
            {
                _currentVoiceGroup = (int)p.voiceGroupId;
            }

            ApplyVoiceRules(participants);
        }

        // Cache mute state to avoid log spam
        private Dictionary<string, bool> _remoteMuteStates = new Dictionary<string, bool>();

        private void ApplyVoiceRules(List<Participant> participants)
        {
            if (_mRtc == null) return;

            foreach (var p in participants)
            {
                if (p.userId == userId) continue;

                uint remoteUid = (uint)Mathf.Abs(p.userId.GetHashCode());
                bool sameGroup = (int)p.voiceGroupId == _currentVoiceGroup;
                bool shouldMute = !sameGroup;

                // Check if state changed before applying/logging
                if (!_remoteMuteStates.ContainsKey(p.userId) || _remoteMuteStates[p.userId] != shouldMute)
                {
                    _mRtc.MuteRemoteAudioStream(remoteUid, shouldMute);
                    _remoteMuteStates[p.userId] = shouldMute;
                    
                    if (shouldMute)
                        Debug.Log($"[Agora] Mute user {p.userId} (Different Group: {p.voiceGroupId} vs {_currentVoiceGroup})");
                    else
                        Debug.Log($"[Agora] Unmute user {p.userId} (Same Group: {p.voiceGroupId})");
                }
            }
        }

        [ContextMenu("Toggle Self Mute")]
        public void ToggleSelfMute()
        {
            if (_mRtc == null) return;

            _isSelfMuted = !_isSelfMuted;
            _mRtc.MuteLocalAudioStream(_isSelfMuted);

            Debug.Log($"[AgoraVoiceManager] Local Mic: {(_isSelfMuted ? "MUTED" : "UNMUTED")}");
        }

        private void OnDestroy()
        {
            if (_mRtc != null)
            {
                _mRtc.LeaveChannel();
                _mRtc.Dispose();
                _mRtc = null;
            }
        }

        internal class UserEventHandler : IRtcEngineEventHandler
        {
            public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
            {
                Debug.Log($"[Agora] Successfully joined channel: {connection.channelId}");
            }

            public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
            {
                Debug.Log($"[Agora] Remote user joined: {uid}");
            }

            public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
            {
                Debug.Log($"[Agora] Remote user offline: {uid} (Reason: {reason})");
            }

            public override void OnError(int err, string msg)
            {
                if (err != 0) Debug.LogError($"[Agora] Error {err}: {msg}");
            }
        }
    }
}