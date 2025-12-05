/*using UnityEngine;
using Agora.Rtc;

public class AgoraInit : MonoBehaviour
{
    public string appId = "5f8caf34e3194321b240595670769c32";
    public string channelName = "testchannel";

    private IRtcEngine rtcEngine;

    void Start()
    {
        Debug.Log("teste");
        rtcEngine = RtcEngine.CreateAgoraRtcEngine();

        // Setup
        RtcEngineContext context = new RtcEngineContext();
        context.appId = appId;
        context.channelProfile = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION;
        context.audioScenario = AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT;
        context.areaCode = AREA_CODE.AREA_CODE_GLOB;

        rtcEngine.Initialize(context);
        rtcEngine.InitEventHandler(new UserEventHandler());

        // habilita audio
        rtcEngine.EnableAudio();

        // opções de midia necessárias pelo SDK
        ChannelMediaOptions options = new ChannelMediaOptions();
        options.autoSubscribeAudio.SetValue(true);
        options.publishMicrophoneTrack.SetValue(true);

        // entrar no canal
        rtcEngine.JoinChannel("", channelName, 0, options);

        Debug.Log("Agora voice initialized & joined channel");
    }

    void OnApplicationQuit()
    {
        if (rtcEngine != null)
        {
            rtcEngine.LeaveChannel();
            rtcEngine.Dispose();
        }
    }
}*/

