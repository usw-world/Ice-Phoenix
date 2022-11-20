using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IntroScreen : MonoBehaviour {
    void Update() {
        if(Input.anyKeyDown) {
            try {
                string productDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "\\Ice Phoenix";
                if(!Directory.Exists(productDir)) {
                    print("dir is not exist. will make dir that named Ice Phoenix.");
                    Directory.CreateDirectory(productDir);
                }
                string FILE_NAME = "\\userinfo.csv";
                FileInfo userinfo = new FileInfo(productDir + FILE_NAME);
                var fs = userinfo.Create();
                fs.Close();

                StreamWriter writer = new StreamWriter(productDir + FILE_NAME);
                System.DateTime now = System.DateTime.Now;
                writer.Write("user_key,create_date\n" + $"usoock,{now.ToShortDateString()}");
                writer.Close();
                
            } catch(System.Exception e) {
                print(e.Message);
                Debug.LogError(e.StackTrace);
            }
        }
    }
}
