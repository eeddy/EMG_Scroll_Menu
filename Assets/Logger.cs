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
  public string fileName;

  public Logger()
  {
    var todayDate = DateTime.Now;
		string strToday = todayDate.ToString("[MM-dd-yyyy][HH;mm;ss]");
    // "Assets/Data/Multiple_choice_grammar.txt"
    fileName = "Assets/logs/" + strToday + ".txt";
    if (File.Exists(fileName))
    {
        File.Delete(fileName);
    }
    // Create Empty File
    using (FileStream fs = File.Create(fileName))
    {
    }
    this.writeDebug("Initialization of Logging File Completed");
  }

  public void writeDebug(string txt) {
    var todayDate = DateTime.Now;
    Debug.Log(todayDate.ToString("[HH:mm:ss.ffff]") + "[DEBUG]\t" + txt);
    // Add some text to file
    using (StreamWriter fs = File.AppendText(fileName))
    {
        fs.WriteLine(todayDate.ToString("[HH:mm:ss.ffff]") + "[DEBUG]\t" + txt);
    }
  }
  public void writeError(string txt) {
    var todayDate = DateTime.Now;
    Debug.Log(todayDate.ToString("[HH:mm:ss.ffff]") + "[ERROR]\t" + txt);
    // Add some text to file
    using (StreamWriter fs = File.AppendText(fileName))
    {
        fs.WriteLine(todayDate.ToString("[HH:mm:ss.ffff]") + "[ERROR]\t" + txt);
    }
  }

}
