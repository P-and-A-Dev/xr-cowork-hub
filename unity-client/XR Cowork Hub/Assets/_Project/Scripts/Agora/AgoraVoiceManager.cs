using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Manager;
using Agora.Rtc;
using UnityEngine;

namespace _Project.Scripts.Agora
{
    public class AgoraVoiceManager : MonoBehaviour
    {
        private const string k_AppID = "5f8caf34e3194321b240595670769c32";
        public string roomId = "test-room";
        public string userId;

        private IRtcEngine _mRtc;
        public uint agoraUid;

        public ParticipantManager participantManager;
        private int _currentVoiceGroup;
        private bool _isSelfMuted;

        private void Start()
        {
            userId = PlayerPrefs.GetString("userId", "user-" + Random.Range(0, 99999));
            agoraUid = (uint)Mathf.Abs(userId.GetHashCode());

            InitializeAgora();
            JoinRoom();

            participantManager.OnParticipantsUpdated += OnParticipantsUpdated;
        }

        private void InitializeAgora()
        {
            _mRtc = RtcEngine.CreateAgoraRtcEngine();

            var logConfig = new LogConfig("agora.log", 2048, LOG_LEVEL.LOG_LEVEL_INFO);

            var context = new RtcEngineContext(
                k_AppID,
                0UL,
                CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION,
                "",
                AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT,
                AREA_CODE.AREA_CODE_GLOB,
                logConfig,
                default,
                false,
                true,
                true
            );

            _mRtc.Initialize(context);

            _mRtc.EnableAudio();
        }

        private void JoinRoom()
        {
            Debug.Log("Join in the voice room: " + roomId);

            ChannelMediaOptions options = new ChannelMediaOptions();


            options.autoSubscribeAudio.SetValue(true);

            int result = _mRtc.JoinChannel(
                "",
                roomId,
                agoraUid,
                options
            );

            Debug.Log("JoinChannel -> " + result);
        }

        public void OnParticipantsUpdated(List<Participant> participants)
        {
            foreach (var p in participants.Where(p => p.userId == userId))
            {
                _currentVoiceGroup = (int)p.voiceGroupId;
            }

            ApplyVoiceRules(participants);
        }

        private void ApplyVoiceRules(List<Participant> participants)
        {
            foreach (var p in participants)
            {
                if (p.userId == userId) continue;

                uint remoteUid = (uint)Mathf.Abs(p.userId.GetHashCode());
                bool sameGroup = (int)p.voiceGroupId == _currentVoiceGroup;

                _mRtc.MuteRemoteAudioStream(remoteUid, !sameGroup);

                Debug.Log((sameGroup ? "unmuted" : "mutated") + p.userId);
            }
        }

        public void ToggleSelfMute()
        {
            _isSelfMuted = !_isSelfMuted;
            _mRtc.MuteLocalAudioStream(_isSelfMuted);

            Debug.Log(_isSelfMuted ? "unmuted" : "ON");
        }
    }
}