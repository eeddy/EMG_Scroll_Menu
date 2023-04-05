using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.IO;
using System;
using System.Text;

public class Logger
{
  public string testFileName;
  public string buildFileName;

  public Logger()
  {
    var todayDate = DateTime.Now;
		string strToday = todayDate.ToString("[MM-dd-yyyy][HH;mm;ss]");
    // "Assets/Data/Multiple_choice_grammar.txt"
    testFileName = "Assets/logs/" + strToday + ".txt";
    Directory.CreateDirectory(Application.streamingAssetsPath + "/logs/");
    buildFileName = Application.streamingAssetsPath + "/logs/" + strToday + ".txt";
    if (File.Exists(testFileName))
    {
        File.Delete(testFileName);
    }
    if (File.Exists(buildFileName))
    {
        File.Delete(buildFileName);
    }
    // Create Empty File
    using (FileStream fs = File.Create(testFileName))
    {
    }
    // Create Empty File
    using (FileStream fs = File.Create(buildFileName))
    {
    }
    this.writeDebug("Initialization of Logging File Completed");
  }

  public void writeDebug(string txt) {
    var todayDate = DateTime.Now;
    Debug.Log(todayDate.ToString("[HH:mm:ss.ffff]") + "[DEBUG]\t" + txt);
    // Add some text to file
    using (StreamWriter fs = File.AppendText(testFileName))
    {
        fs.WriteLine(todayDate.ToString("[HH:mm:ss.ffff]") + "[DEBUG]\t" + txt);
    }
    using (StreamWriter fs = File.AppendText(buildFileName))
    {
        fs.WriteLine(todayDate.ToString("[HH:mm:ss.ffff]") + "[DEBUG]\t" + txt);
    }
  }
  public void writeError(string txt) {
    var todayDate = DateTime.Now;
    Debug.Log(todayDate.ToString("[HH:mm:ss.ffff]") + "[ERROR]\t" + txt);
    // Add some text to file
    using (StreamWriter fs = File.AppendText(testFileName))
    {
        fs.WriteLine(todayDate.ToString("[HH:mm:ss.ffff]") + "[ERROR]\t" + txt);
    }
    using (StreamWriter fs = File.AppendText(buildFileName))
    {
        fs.WriteLine(todayDate.ToString("[HH:mm:ss.ffff]") + "[ERROR]\t" + txt);
    }
  }

}
