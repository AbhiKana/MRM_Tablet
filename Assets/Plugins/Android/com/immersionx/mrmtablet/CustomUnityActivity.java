package com.immersionx.mrmtablet;

import android.app.Activity;
import android.os.Bundle;
import com.unity3d.player.UnityPlayerActivity;
import com.unity3d.player.UnityPlayer;

public class CustomUnityActivity extends UnityPlayerActivity {
    private static boolean wasStopped = false;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        wasStopped = false;
    }

    @Override
    protected void onStop() {
        super.onStop();
        wasStopped = true;
        UnityPlayer.UnitySendMessage("AppLifeCycleManager", "OnAppBackgrounded", "");
    }

    @Override
    protected void onDestroy() {
        if (wasStopped) {
            UnityPlayer.UnitySendMessage("AppLifeCycleManager", "OnAppKilledFromBackground", "");
        }
        super.onDestroy();
    }

    public static boolean isAppInBackground() {
        return wasStopped;
    }
}