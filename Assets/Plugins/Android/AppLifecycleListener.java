package com.immersionx.mrmtablet;

import android.app.Activity;
import android.os.Bundle;
import com.unity3d.player.UnityPlayer;

public class AppLifecycleListener extends Activity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    // @Override
    // protected void onDestroy() {
    //     super.onDestroy();
    //     // Check if the app is being destroyed from recents
    //     if (isFinishing()) {
    //         UnityPlayer.UnitySendMessage("ClientController", "StopClient", "");
    //     }
    // }

    @Override
    protected void onDestroy() {
        // This is called when the activity is being destroyed
        UnityPlayer.UnitySendMessage("ClientController", "StopClient", "");
        super.onDestroy();
    }
}